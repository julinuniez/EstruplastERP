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
        // 1. MÉTODOS DE LECTURA (GET)
        // ==========================================

        // GET: api/Productos/inventario-completo
        [HttpGet("inventario-completo")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetInventarioCompleto()
        {
            return await _context.Productos
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }

        // GET: api/productos/materias-primas (Solo insumos)
        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima && p.Activo)
                .Select(p => new {
                    p.Id,
                    p.Nombre,
                    p.StockActual,
                    p.CodigoSku
                })
                .ToListAsync();
        }

        // GET: api/productos/5 (Para Edición)
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            // 🚨 IMPORTANTE: Aquí incluimos las fórmulas y la materia prima interna
            // para que al editar aparezca la lista de ingredientes.
            var producto = await _context.Productos
                .Include(p => p.Formulas)
                .ThenInclude(f => f.MateriaPrima)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null)
            {
                return NotFound("❌ Producto no encontrado.");
            }

            return producto;
        }

        // ==========================================
        // 2. CREACIÓN (POST) - ¡LÓGICA CORREGIDA!
        // ==========================================

        [HttpPost("crear")]
        public async Task<IActionResult> CrearProductoConReceta([FromBody] NuevoProductoDto data)
        {
            // A. Validaciones
            if (string.IsNullOrWhiteSpace(data.Nombre) || string.IsNullOrWhiteSpace(data.CodigoSku))
                return BadRequest("❌ Nombre y SKU son obligatorios.");

            if (await _context.Productos.AnyAsync(p => p.CodigoSku == data.CodigoSku))
                return BadRequest("❌ El Código SKU ya existe.");

            // B. Detectar si es Producto Terminado (PT)
            // Si trae receta con elementos, es PT.
            bool esProductoTerminado = data.Receta != null && data.Receta.Count > 0;

            // Validaciones específicas de PT
            if (esProductoTerminado)
            {
                if (data.PesoEspecifico <= 0) return BadRequest("❌ Falta la Densidad (Peso Específico).");
                if (data.Ancho <= 0 || data.Espesor <= 0) return BadRequest("❌ Falta Ancho o Espesor para la Lámina.");
            }

            // C. Transacción para guardar Producto + Receta
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoProducto = new Producto
                {
                    Nombre = data.Nombre.Trim(),
                    CodigoSku = data.CodigoSku.Trim().ToUpper(),
                    EsProductoTerminado = esProductoTerminado,
                    EsMateriaPrima = !esProductoTerminado,

                    // Dimensiones (Si es materia prima, vendrán en 0 y está bien)
                    Largo = data.Largo,
                    Ancho = data.Ancho,
                    Espesor = data.Espesor,
                    PesoEspecifico = data.PesoEspecifico,

                    StockMinimo = data.StockMinimo,
                    PrecioCosto = data.PrecioCosto,
                    StockActual = 0, // Nace en 0
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                // 1. Guardar Producto (Genera ID)
                _context.Productos.Add(nuevoProducto);
                await _context.SaveChangesAsync();

                // 2. Guardar Receta (Solo si es PT)
                if (esProductoTerminado && data.Receta != null)
                {
                    foreach (var item in data.Receta)
                    {
                        var formula = new Formula
                        {
                            ProductoTerminadoId = nuevoProducto.Id, // ID recién generado
                            MateriaPrimaId = item.MateriaPrimaId,
                            Cantidad = item.Cantidad
                        };
                        _context.Formulas.Add(formula);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Ítem creado correctamente.", id = nuevoProducto.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error en servidor: " + ex.Message);
            }
        }

        // ==========================================
        // 3. ACTUALIZACIÓN (PUT)
        // ==========================================

        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] NuevoProductoDto data)
        {
            if (id <= 0) return BadRequest("ID inválido.");

            var producto = await _context.Productos
                .Include(p => p.Formulas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound("❌ Producto no encontrado.");

            // Actualizar datos básicos
            producto.Nombre = data.Nombre.Trim();
            producto.CodigoSku = data.CodigoSku.Trim().ToUpper();
            producto.StockMinimo = data.StockMinimo;
            producto.PrecioCosto = data.PrecioCosto;

            // Actualizar dimensiones
            producto.Largo = data.Largo;
            producto.Ancho = data.Ancho;
            producto.Espesor = data.Espesor;
            producto.PesoEspecifico = data.PesoEspecifico;

            // Recalcular tipo
            bool esPT = data.Receta != null && data.Receta.Count > 0;
            producto.EsProductoTerminado = esPT;
            producto.EsMateriaPrima = !esPT;

            // Actualizar Fórmulas
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Borrar viejas
                if (producto.Formulas != null && producto.Formulas.Any())
                {
                    _context.Formulas.RemoveRange(producto.Formulas);
                    await _context.SaveChangesAsync();
                }

                // 2. Insertar nuevas
                if (esPT && data.Receta != null)
                {
                    foreach (var item in data.Receta)
                    {
                        _context.Formulas.Add(new Formula
                        {
                            ProductoTerminadoId = producto.Id,
                            MateriaPrimaId = item.MateriaPrimaId,
                            Cantidad = item.Cantidad
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { mensaje = "✅ Ítem actualizado correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al actualizar: " + ex.Message);
            }
        }

        // ==========================================
        // 4. ELIMINACIÓN (DELETE)
        // ==========================================

        // DELETE: api/Productos/eliminar/5
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            // 1. Verificar si el producto existe
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound("Producto no encontrado.");

            // Usamos una transacción para asegurar que se borre TODO o NADA
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // A. VALIDACIÓN DE SEGURIDAD
                // Verificar si este producto es MATERIA PRIMA de otra receta.
                // Si borras un ingrediente que usan otros, rompes esas recetas.
                bool esIngredienteDeOtros = await _context.Formulas.AnyAsync(f => f.MateriaPrimaId == id);
                if (esIngredienteDeOtros)
                {
                    return BadRequest(new { mensaje = "❌ No se puede eliminar: Este ítem es ingrediente en la receta de otro producto. Quítalo de esas recetas primero." });
                }

                // B. BORRAR HISTORIAL DE MOVIMIENTOS (Stock)
                // Buscamos explícitamente todos los movimientos de este producto y los borramos.
                var movimientos = await _context.Movimientos.Where(m => m.ProductoId == id).ToListAsync();
                if (movimientos.Any())
                {
                    _context.Movimientos.RemoveRange(movimientos);
                }

                // C. BORRAR SU FÓRMULA (Aquí estaba el error)
                // Borramos explícitamente las filas en Formulas donde este producto es el "Producto Terminado"
                var formulasDelProducto = await _context.Formulas
                                            .Where(f => f.ProductoTerminadoId == id)
                                            .ToListAsync();

                if (formulasDelProducto.Any())
                {
                    _context.Formulas.RemoveRange(formulasDelProducto);
                }

                // D. AHORA SÍ, BORRAR EL PRODUCTO
                _context.Productos.Remove(producto);

                // Guardar todos los cambios
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Producto eliminado correctamente (se borró su historial y receta)." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Mostramos el error interno para saber qué pasa si vuelve a fallar
                var mensajeError = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { mensaje = "Error crítico al eliminar: " + mensajeError });
            }
        }
    }

    // ==========================================
    // 5. DTOs (Coinciden con el Frontend)
    // ==========================================

    public class NuevoProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;

        // Medidas
        public decimal Largo { get; set; }
        public decimal Ancho { get; set; }
        public decimal Espesor { get; set; }
        public decimal PesoEspecifico { get; set; }

        // Datos Económicos
        public decimal PrecioCosto { get; set; }
        public decimal StockMinimo { get; set; }

        // La Receta (Nullable para evitar error si viene vacío)
        public List<IngredienteDto>? Receta { get; set; }
    }

    public class IngredienteDto
    {
        public int MateriaPrimaId { get; set; }
        public decimal Cantidad { get; set; }
    }
}