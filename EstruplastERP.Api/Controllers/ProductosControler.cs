using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

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

        [HttpGet("inventario-completo")]
        public async Task<ActionResult<IEnumerable<object>>> GetInventarioCompleto()
        {
            var productos = await _context.Productos
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.StockActual,
                    p.StockMinimo,
                    p.PrecioCosto,
                    p.EsProductoTerminado,
                    p.Color // <--- NUEVO
                })
                .ToListAsync();

            return Ok(productos);
        }

        // GET: api/productos/materias-primas
        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<ProductoListaDto>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima && p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new ProductoListaDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    StockActual = p.StockActual,
                    CodigoSku = p.CodigoSku
                })
                .ToListAsync();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .ToListAsync();
        }

        // GET: api/productos/5 (Para Edición)
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDetalleDto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Formulas)
                .ThenInclude(f => f.MateriaPrima)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound("❌ Producto no encontrado.");

            var dto = new ProductoDetalleDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                CodigoSku = producto.CodigoSku,
                StockActual = producto.StockActual,
                PrecioCosto = producto.PrecioCosto,
                StockMinimo = producto.StockMinimo,
                Largo = producto.Largo,
                Ancho = producto.Ancho,
                Espesor = producto.Espesor,
                PesoEspecifico = producto.PesoEspecifico,
                Color = producto.Color, // <--- NUEVO (Asegúrate de agregarlo en ProductoDetalleDto)
                EsProductoTerminado = producto.EsProductoTerminado,
                EsMateriaPrima = producto.EsMateriaPrima,

                Receta = producto.Formulas.Select(f => new IngredienteDto
                {
                    MateriaPrimaId = f.MateriaPrimaId,
                    NombreInsumo = f.MateriaPrima.Nombre,
                    Cantidad = f.Cantidad
                }).ToList()
            };

            return dto;
        }

        [HttpPost("crear")]
        public async Task<IActionResult> CrearProductoConReceta([FromBody] NuevoProductoDto data)
        {
            if (string.IsNullOrWhiteSpace(data.Nombre) || string.IsNullOrWhiteSpace(data.CodigoSku))
                return BadRequest("❌ Nombre y SKU son obligatorios.");

            if (await _context.Productos.AnyAsync(p => p.CodigoSku == data.CodigoSku))
                return BadRequest("❌ El Código SKU ya existe.");

            bool esProductoTerminado = data.Receta != null && data.Receta.Count > 0;

            if (esProductoTerminado)
            {
                if (data.PesoEspecifico <= 0) return BadRequest("❌ Falta la Densidad (Peso Específico).");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoProducto = new Producto
                {
                    Nombre = data.Nombre.Trim(),
                    CodigoSku = data.CodigoSku.Trim().ToUpper(),
                    EsProductoTerminado = esProductoTerminado,
                    EsMateriaPrima = !esProductoTerminado,
                    Largo = data.Largo,
                    Ancho = data.Ancho,
                    Espesor = data.Espesor,
                    PesoEspecifico = data.PesoEspecifico,
                    StockMinimo = data.StockMinimo,
                    PrecioCosto = data.PrecioCosto,
                    Color = data.Color, // <--- NUEVO: Guardamos el color
                    StockActual = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Productos.Add(nuevoProducto);
                await _context.SaveChangesAsync();

                if (esProductoTerminado && data.Receta != null)
                {
                    foreach (var item in data.Receta)
                    {
                        _context.Formulas.Add(new Formula
                        {
                            ProductoTerminadoId = nuevoProducto.Id,
                            MateriaPrimaId = item.MateriaPrimaId,
                            Cantidad = item.Cantidad
                        });
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

        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] NuevoProductoDto data)
        {
            if (id <= 0) return BadRequest("ID inválido.");

            var producto = await _context.Productos
                .Include(p => p.Formulas)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound("❌ Producto no encontrado.");

            // Actualizar propiedades
            producto.Nombre = data.Nombre.Trim();
            producto.CodigoSku = data.CodigoSku.Trim().ToUpper();
            producto.StockMinimo = data.StockMinimo;
            producto.PrecioCosto = data.PrecioCosto;
            producto.Largo = data.Largo;
            producto.Ancho = data.Ancho;
            producto.Espesor = data.Espesor;
            producto.PesoEspecifico = data.PesoEspecifico;
            producto.Color = data.Color; // <--- NUEVO: Actualizamos el color

            bool esPT = data.Receta != null && data.Receta.Count > 0;
            producto.EsProductoTerminado = esPT;
            producto.EsMateriaPrima = !esPT;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (producto.Formulas != null && producto.Formulas.Any())
                {
                    _context.Formulas.RemoveRange(producto.Formulas);
                    await _context.SaveChangesAsync();
                }

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

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound("Producto no encontrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Validación: No borrar si es ingrediente de otro
                bool esIngredienteDeOtros = await _context.Formulas.AnyAsync(f => f.MateriaPrimaId == id);
                if (esIngredienteDeOtros)
                {
                    return BadRequest(new { mensaje = "❌ No se puede eliminar: Es ingrediente de otro producto." });
                }

                // Borrar Movimientos
                var movimientos = await _context.Movimientos.Where(m => m.ProductoId == id).ToListAsync();
                if (movimientos.Any()) _context.Movimientos.RemoveRange(movimientos);

                // Borrar Fórmulas donde este es el producto terminado
                var formulasDelProducto = await _context.Formulas.Where(f => f.ProductoTerminadoId == id).ToListAsync();
                if (formulasDelProducto.Any()) _context.Formulas.RemoveRange(formulasDelProducto);

                // Borrar Producto
                _context.Productos.Remove(producto);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Producto eliminado correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error crítico al eliminar." });
            }
        }
    }
}