using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. MÉTODOS DE LECTURA (Tus GET originales)
        // ==========================================

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                // Opcional: Si quieres ver si está activo o no
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        // GET: api/productos/materias-primas (Solo insumos)
        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima && p.Activo) // Agregué el Activo por seguridad
                .Select(p => new {
                    p.Id,
                    p.Nombre,
                    p.StockActual,
                    p.CodigoSku
                })
                .ToListAsync();
        }

        // ==========================================
        // 2. MÉTODO DE CREACIÓN POTENCIADO (Nuevo)
        // ==========================================

        // POST: api/productos/crear
        // Este reemplaza a tu PostProducto simple.
        // Recibe el producto Y la receta juntos.
        [HttpPost("crear")]
        public async Task<IActionResult> CrearProductoConReceta([FromBody] NuevoProductoDto data)
        {
            // 1. Validar Nombre y SKU
            if (string.IsNullOrWhiteSpace(data.Nombre))
                return BadRequest("❌ El Nombre del producto es obligatorio.");

            if (string.IsNullOrWhiteSpace(data.CodigoSku))
                return BadRequest("❌ El Código SKU es obligatorio.");

            // 2. Validar Datos Físicos (Clave para Láminas)
            if (data.PesoEspecifico <= 0)
                return BadRequest("❌ Debes indicar la Densidad (Peso Específico) para calcular los kilos.");

            if (data.Ancho <= 0 || data.Espesor <= 0)
                return BadRequest("❌ El Ancho y el Espesor deben ser mayores a 0.");

            // 3. Validar Receta
            if (data.Receta == null || data.Receta.Count == 0)
            {
                return BadRequest("❌ Es obligatorio definir al menos 1 ingrediente para la receta.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. Crear el Producto (Cabecera)
                var nuevoProducto = new Producto
                {
                    Nombre = data.Nombre,
                    CodigoSku = data.CodigoSku,
                    Largo = data.Largo,
                    Ancho = data.Ancho,
                    Espesor = data.Espesor,
                    PesoEspecifico = data.PesoEspecifico,
                    
                    // Configuraciones por defecto
                    StockActual = 0,
                    StockMinimo = data.StockMinimo,
                    PrecioCosto = data.PrecioCosto,

                    // Lógica de negocio
                    EsProductoTerminado = true,
                    EsMateriaPrima = false,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Productos.Add(nuevoProducto);
                await _context.SaveChangesAsync(); // Guardamos ya para obtener el ID nuevo

                // B. Crear la Receta (Ingredientes)
                foreach (var item in data.Receta)
                {
                    var formula = new Formula
                    {
                        ProductoTerminadoId = nuevoProducto.Id, // Usamos el ID generado arriba
                        MateriaPrimaId = item.MateriaPrimaId,
                        Cantidad = item.Cantidad
                    };
                    _context.Formulas.Add(formula);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); 

                return Ok(new { mensaje = "✅ Producto y Receta creados con éxito!", id = nuevoProducto.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(); // Si falla algo,lo deshacemos
                return StatusCode(500, "Error al guardar: " + ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            // Opcional: Validación para no borrar cosas con stock
            if (producto.StockActual > 0)
            {
                return BadRequest("No puedes eliminar un producto con Stock. Haz un ajuste de salida primero.");
            }

            // Borrado lógico (Recomendado)
            producto.Activo = false;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }


    public class NuevoProductoDto
    {
        public string Nombre { get; set; }
        public string CodigoSku { get; set; }

        // Medidas
        public decimal Largo { get; set; }
        public decimal Ancho { get; set; }
        public decimal Espesor { get; set; }
        public decimal PesoEspecifico { get; set; }

        // Datos Económicos
        public decimal PrecioCosto { get; set; }
        public decimal StockMinimo { get; set; }

        // La Receta
        public List<IngredienteDto> Receta { get; set; }
    }

    public class IngredienteDto
    {
        public int MateriaPrimaId { get; set; }
        public decimal Cantidad { get; set; }
    }
}