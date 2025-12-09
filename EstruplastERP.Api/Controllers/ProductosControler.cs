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

        // GET: api/Productos/inventario-completo
        [HttpGet("inventario-completo")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetInventarioCompleto()
        {
            // Usamos el mismo query que GetProductos, pero con el nombre que el frontend espera.
            return await _context.Productos
                .OrderByDescending(p => p.Id)
                .ToListAsync();
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
        [HttpPost("crear")]
        public async Task<IActionResult> CrearProductoConReceta([FromBody] NuevoProductoDto data)
        {
            // ==========================================
            // 🛡️ VALIDACIONES DE SEGURIDAD
            // ==========================================

            if (string.IsNullOrWhiteSpace(data.Nombre))
                return BadRequest("❌ El Nombre del ítem es obligatorio.");

            if (string.IsNullOrWhiteSpace(data.CodigoSku))
                return BadRequest("❌ El Código SKU es obligatorio.");

            // Validación de Densidad: SÓLO si es un Producto Terminado (con receta)
            var esProductoTerminado = data.Receta != null && data.Receta.Count > 0;

            if (esProductoTerminado && data.PesoEspecifico <= 0)
                return BadRequest("❌ Debes indicar la Densidad (Peso Específico) para Productos Terminados.");

            // Si es PT, exigimos al menos Ancho/Espesor para el cálculo de peso (Largo puede ser 0)
            if (esProductoTerminado && (data.Ancho <= 0 || data.Espesor <= 0))
                return BadRequest("❌ El Ancho y el Espesor deben ser mayores a 0 para Láminas.");


            // ==========================================
            // 🚀 ASIGNACIÓN DE TIPO Y GUARDADO EN DB
            // ==========================================

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoProducto = new Producto
                {
                    Nombre = data.Nombre.Trim(),
                    CodigoSku = data.CodigoSku.Trim().ToUpper(),

                    // Si esProductoTerminado es TRUE, es PT. Si es FALSE, es MP.
                    EsProductoTerminado = esProductoTerminado,
                    EsMateriaPrima = !esProductoTerminado, // Lo invertimos para Materia Prima

                    // Datos Físicos: Se guardan 0/0 si es Materia Prima.
                    Largo = data.Largo,
                    Ancho = data.Ancho,
                    Espesor = data.Espesor,
                    PesoEspecifico = data.PesoEspecifico,

                    StockActual = 0,
                    StockMinimo = data.StockMinimo,
                    PrecioCosto = data.PrecioCosto,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Productos.Add(nuevoProducto);
                await _context.SaveChangesAsync();

                // ⚠️ Solo creamos recetas si realmente se enviaron ingredientes
                if (esProductoTerminado)
                {
                    foreach (var item in data.Receta!)
                    {
                        var formula = new Formula
                        {
                            ProductoTerminadoId = nuevoProducto.Id,
                            MateriaPrimaId = item.MateriaPrimaId,
                            Cantidad = item.Cantidad
                        };
                        _context.Formulas.Add(formula);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Ítem creado correctamente.", id = nuevoProducto.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al guardar en el servidor: " + ex.Message);
            }
        }


        // DELETE: api/Productos/eliminar/5
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos
                                         .Include(p => p.Formulas) // Incluimos las recetas
                                         .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound("❌ Producto no encontrado.");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Si es un Producto Terminado, eliminamos sus recetas asociadas
                if (producto.EsProductoTerminado && producto.Formulas.Any())
                {
                    _context.Formulas.RemoveRange(producto.Formulas);
                }

                // 2. Ahora sí, eliminamos el producto (MP o PT)
                _context.Productos.Remove(producto);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = $"✅ El ítem '{producto.Nombre}' (SKU: {producto.CodigoSku}) ha sido eliminado." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al eliminar el producto: " + ex.Message);
            }
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