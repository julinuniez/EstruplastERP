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
        public async Task<IActionResult> GetInventarioCompleto()
        {
            try
            {
                var productos = await _context.Productos
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nombre,
                        p.CodigoSku,
                        p.StockActual,
                        p.StockMinimo,
                        p.PesoEspecifico,
                        p.EsMateriaPrima,
                        p.EsProductoTerminado,
                        p.EsFazon,
                        p.PrecioCosto,
                        ClienteId = p.ClienteId,
                        EsScrap = p.EsScrap
                    })
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                string errorReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return StatusCode(500, new { mensaje = $"Error de BD: {errorReal}" });
            }
        }

        // ==========================================
        // 2. GET: MATERIAS PRIMAS
        // ==========================================
        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima && p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.PesoEspecifico,
                    p.StockActual
                })
                .ToListAsync();
        }

        // ==========================================
        // 3. GET: TODOS LOS PRODUCTOS
        // ==========================================
        [HttpGet]
        public async Task<IActionResult> GetProductos()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo)
                .Select(p => new
                {
                    p.Id,
                    p.CodigoSku,
                    p.Nombre,
                    p.Rubro,
                    p.TipoMaterial,
                    p.EsMateriaPrima,
                    p.EsProductoTerminado,
                    p.EsFazon,
                    p.EsScrap,

                    // 🚨 MATAMOS LA LÓGICA VIEJA: Forzamos a true para que todo sea editable
                    EsGenerico = true,

                    p.ClienteId,
                    p.PrecioCosto,
                    p.StockMinimo,
                    StockFisico = p.StockActual,

                    StockReservado = _context.ConsumosOrdenes
                        .Where(c => c.MateriaPrimaId == p.Id &&
                                    c.OrdenProduccion.Estado != EstadoOrden.Finalizada &&
                                    c.OrdenProduccion.Estado != EstadoOrden.Cancelada)
                        .Sum(c => (decimal?)c.CantidadKilos) ?? 0,
                })
                .ToListAsync();

            var resultado = productos.Select(p => new
            {
                p.Id,
                p.CodigoSku,
                p.Nombre,
                p.Rubro,
                p.TipoMaterial,
                p.EsMateriaPrima,
                p.EsProductoTerminado,
                p.EsFazon,
                p.EsScrap,
                p.ClienteId,
                p.PrecioCosto,
                p.StockMinimo,
                p.StockFisico,
                p.StockReservado,
                p.EsGenerico, // Ya viaja como true
                StockDisponible = p.StockFisico - p.StockReservado
            });

            return Ok(resultado);
        }

        // ==========================================
        // 4. GET: UN PRODUCTO (Para Edición)
        // ==========================================
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDetalleDto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Formulas)
                .ThenInclude(f => f.MateriaPrima)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound("❌ Producto no encontrado.");

            List<Formula> formulasFinales;

            if (producto.Formulas != null && producto.Formulas.Any())
            {
                formulasFinales = producto.Formulas.ToList();
            }
            else
            {
                formulasFinales = new List<Formula>();
            }

            var dto = new ProductoDetalleDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                CodigoSku = producto.CodigoSku,
                StockActual = producto.StockActual,
                PrecioCosto = producto.PrecioCosto,
                StockMinimo = producto.StockMinimo,
                PesoEspecifico = producto.PesoEspecifico,
                EsProductoTerminado = producto.EsProductoTerminado,
                EsMateriaPrima = producto.EsMateriaPrima,

                // 🚨 MATAMOS LA LÓGICA VIEJA: Forzamos a true
                EsGenerico = true,

                Rubro = producto.Rubro,

                Receta = formulasFinales.Select(f => new IngredienteDto
                {
                    MateriaPrimaId = f.MateriaPrimaId,
                    NombreInsumo = f.MateriaPrima?.Nombre ?? "(MP No Encontrada)",
                    Cantidad = f.Cantidad
                }).ToList()
            };

            return dto;
        }

        // ==========================================
        // 5. POST: CREAR
        // ==========================================
        [HttpPost("crear")]
        public async Task<IActionResult> CrearProductoConReceta([FromBody] NuevoProductoDto data)
        {
            if (string.IsNullOrWhiteSpace(data.Nombre) || string.IsNullOrWhiteSpace(data.CodigoSku))
                return BadRequest("❌ Nombre y SKU son obligatorios.");

            if (await _context.Productos.AnyAsync(p => p.CodigoSku == data.CodigoSku))
                return BadRequest("❌ El Código SKU ya existe.");

            bool esProductoTerminado = data.Receta != null && data.Receta.Count > 0;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoProducto = new Producto
                {
                    Nombre = data.Nombre.Trim(),
                    CodigoSku = data.CodigoSku.Trim().ToUpper(),
                    EsProductoTerminado = esProductoTerminado,
                    EsMateriaPrima = !esProductoTerminado,
                    StockMinimo = data.StockMinimo,
                    PrecioCosto = data.PrecioCosto,
                    StockActual = 0,
                    Activo = true,
                    EsGenerico = true, // 🚨 Creados por defecto en true
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


        [HttpPost("habilitar-fazon/{clienteId}")]
        public async Task<IActionResult> HabilitarFazonCliente(int clienteId)
        {
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return NotFound("❌ El cliente no existe.");

            var existeMaterial = await _context.Productos
                .AnyAsync(p => p.ClienteId == clienteId && p.EsMateriaPrima);

            if (existeMaterial)
                return BadRequest("⚠️ Este cliente ya tiene habilitado el servicio de Fazon (Ya tiene stock asignado).");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoMaterial = new Producto
                {
                    Nombre = $"Material Recuperado: {cliente.RazonSocial}",
                    CodigoSku = $"MP-CLI-{clienteId.ToString("D3")}",
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    EsGenerico = true, // 🚨 Forzado a true
                    ClienteId = clienteId,

                    StockActual = 0,
                    StockMinimo = 0,
                    PrecioCosto = 0,
                    PesoEspecifico = 1.1m,

                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Productos.Add(nuevoMaterial);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = $"✅ Fazon habilitado para {cliente.RazonSocial}.",
                    productoId = nuevoMaterial.Id,
                    nombreMaterial = nuevoMaterial.Nombre
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al crear material: " + ex.Message);
            }
        }

        // ==========================================
        // 6. PUT: ACTUALIZAR
        // ==========================================
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoEditarDto data)
        {
            if (id <= 0) return BadRequest("ID inválido.");

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
            if (producto == null) return NotFound("❌ Producto no encontrado.");

            producto.Nombre = data.Nombre.Trim();
            producto.CodigoSku = data.CodigoSku.Trim().ToUpper();
            producto.StockMinimo = data.StockMinimo;
            // producto.EsGenerico = data.EsGenerico; // 🚨 Esto se queda comentado para que no lo pise

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "✅ Ítem actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar: " + ex.Message);
            }
        }

        // ==========================================
        // 7. DELETE: ELIMINAR
        // ==========================================
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound("Producto no encontrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                bool esIngredienteDeOtros = await _context.Formulas.AnyAsync(f => f.MateriaPrimaId == id);
                if (esIngredienteDeOtros)
                    return BadRequest(new { mensaje = "❌ No se puede eliminar: Es ingrediente de otro producto." });

                var movimientos = await _context.Movimientos.Where(m => m.ProductoId == id).ToListAsync();
                if (movimientos.Any()) _context.Movimientos.RemoveRange(movimientos);

                var formulasDelProducto = await _context.Formulas.Where(f => f.ProductoTerminadoId == id).ToListAsync();
                if (formulasDelProducto.Any()) _context.Formulas.RemoveRange(formulasDelProducto);

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

        // ==========================================
        // 8. PUT: CONFIGURACIÓN TÉCNICA (Peso, Tipos)
        // ==========================================
        [HttpPut("configurar/{id}")]
        public async Task<IActionResult> ConfigurarProducto(int id, [FromBody] ProductoConfigDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound("Producto no encontrado");

            // 1. Actualizar Datos Técnicos
            producto.StockMinimo = dto.StockMinimo;
            producto.PesoEspecifico = dto.PesoEspecifico;
            producto.EsMateriaPrima = dto.EsMateriaPrima;
            producto.EsProductoTerminado = dto.EsProductoTerminado;
            producto.EsFazon = dto.EsFazon;
            producto.PrecioCosto = dto.PrecioCosto;
            producto.Rubro = dto.Rubro;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (dto.Receta != null)
                {
                    var formulasViejas = await _context.Formulas
                        .Where(f => f.ProductoTerminadoId == id)
                        .ToListAsync();

                    _context.Formulas.RemoveRange(formulasViejas);

                    foreach (var item in dto.Receta)
                    {
                        _context.Formulas.Add(new Formula
                        {
                            ProductoTerminadoId = id,
                            MateriaPrimaId = item.MateriaPrimaId,
                            Cantidad = item.Cantidad
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Confirmar todo

                return Ok(new { mensaje = "✅ Configuración y Receta guardadas." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al guardar: " + ex.Message);
            }
        }

        [HttpPost("reparar-familias-v2")]
        public async Task<IActionResult> RepararFamiliasV2()
        {
            var materialesCliente = await _context.Productos
                .Where(p => p.EsMateriaPrima && p.ClienteId != null)
                .ToListAsync();

            int cambios = 0;

            foreach (var mat in materialesCliente)
            {
                int? nuevoId = null;

                if (mat.CodigoSku.Contains("AI-FIN")) nuevoId = 11;
                else if (mat.CodigoSku.Contains("AI-GRU")) nuevoId = 12;
                else if (mat.CodigoSku.Contains("AI-BIC")) nuevoId = 13;
                else if (mat.CodigoSku.Contains("AI-TRI")) nuevoId = 14;

                else if (mat.CodigoSku.Contains("ABS-GRU")) nuevoId = 21;

                else if (mat.CodigoSku.Contains("POLI-FIN")) nuevoId = 31;
                else if (mat.CodigoSku.Contains("POLI-GRU")) nuevoId = 32;

                else if (mat.CodigoSku.Contains("PEAD-BIC")) nuevoId = 41;

                if (nuevoId.HasValue && mat.FamiliaId != nuevoId)
                {
                    mat.FamiliaId = nuevoId;
                    _context.Entry(mat).State = EntityState.Modified;
                    cambios++;
                }
            }

            await _context.SaveChangesAsync();
            return Ok($"Se especificaron las familias de {cambios} materiales de clientes.");
        }

        [HttpGet("{id}/reservas")]
        public async Task<IActionResult> GetReservasProducto(int id)
        {
            var reservas = await _context.ConsumosOrdenes
                .Where(c => c.MateriaPrimaId == id &&
                            c.OrdenProduccion.Estado != EstadoOrden.Finalizada &&
                            c.OrdenProduccion.Estado != EstadoOrden.Cancelada)
                .Select(c => new
                {
                    Id = c.OrdenProduccion.Id,
                    NotaPedido = c.OrdenProduccion.NotaPedido ?? c.OrdenProduccion.Id.ToString(),
                    Cliente = c.OrdenProduccion.Cliente != null ? c.OrdenProduccion.Cliente.RazonSocial : "Interno / Stock",
                    Cantidad = Math.Round(c.CantidadKilos, 2)
                })
                .ToListAsync();

            return Ok(reservas);
        }
    }
}