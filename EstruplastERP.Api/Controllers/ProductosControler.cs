using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;
using CsvHelper.Configuration.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

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
                    .Include(p => p.Proveedor)
                    .Where(p => p.Activo)
                    .OrderBy(p => p.Nombre)
                    .Select(p => new
                    {
                        p.Id,
                        p.Nombre,
                        p.CodigoSku,
                        StockActual = p.StockActual,
                        StockMinimo = p.StockMinimo,
                        PesoEspecifico = p.PesoEspecifico,
                        EsMateriaPrima = p.EsMateriaPrima,
                        EsProductoTerminado = p.EsProductoTerminado,
                        EsFazon = p.EsFazon,
                        PrecioCosto = p.PrecioCosto,
                        ClienteId = p.ClienteId,
                        EsScrap = p.EsScrap,
                        ProveedorId = p.ProveedorId,
                        ProveedorNombre = p.Proveedor != null ? p.Proveedor.RazonSocial : null
                    })
                    .ToListAsync();

                return Ok(productos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ERROR_CRITICO = "Hubo un fallo al leer la base de datos.",
                    MENSAJE = ex.Message,
                    CAUSA_INTERNA = ex.InnerException?.Message ?? "N/A"
                });
            }
        }

        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Include(p => p.Proveedor)
                .Include(p => p.Cliente) // 🚀 Faltaba y lo mantuvimos
                .Where(p => p.EsMateriaPrima && p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.PesoEspecifico,
                    p.StockActual,
                    ClienteId = p.ClienteId,
                    ClienteNombre = p.Cliente != null ? p.Cliente.RazonSocial : "",
                    ProveedorNombre = p.Proveedor != null ? p.Proveedor.RazonSocial : null,
                    p.ProveedorId
                })
                .ToListAsync();
        }

        [HttpGet("insumos-disponibles/{clienteId}")]
        public async Task<IActionResult> GetInsumosParaOrden(int clienteId)
        {
            try
            {
                var insumos = await _context.Productos
                    .Where(p => p.EsMateriaPrima && p.Activo &&
                               (p.ClienteId == clienteId || p.ClienteId == 0 || p.ClienteId == null))
                    .OrderBy(p => p.Nombre)
                    .Select(p => new {
                        p.Id,
                        p.Nombre,
                        p.CodigoSku,
                        p.StockActual,
                        EsDeEstruplast = (p.ClienteId == 0 || p.ClienteId == null)
                    })
                    .ToListAsync();

                return Ok(insumos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener insumos: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductos([FromQuery] int? clienteId = null)
        {
            var query = _context.Productos
                .Include(p => p.Proveedor)
                .Include(p => p.Cliente)
                .Where(p => p.Activo)
                .AsQueryable();

            // Lógica de filtro fusionada
            if (clienteId.HasValue && clienteId.Value > 0)
            {
                query = query.Where(p => p.ClienteId == null || p.ClienteId == clienteId);
            }

            var productos = await query
                .Select(p => new
                {
                    p.Id,
                    p.CodigoSku,
                    p.Nombre,
                    p.Rubro,
                    p.TipoMaterial,
                    p.EsMateriaPrima,
                    p.EsProductoTerminado,
                    EsFazonCalculado = p.ClienteId != null,
                    p.EsFazon,
                    p.EsScrap,
                    p.EsGenerico,
                    p.ClienteId,
                    p.PrecioCosto,
                    p.StockMinimo,
                    p.StockActual,
                    p.PesoEspecifico,
                    ProveedorId = p.ProveedorId,
                    ProveedorNombre = p.Proveedor != null ? p.Proveedor.RazonSocial : null,

                    StockFisico = p.StockActual,
                    StockReservado = _context.ConsumosOrdenes
                        .Where(c => c.MateriaPrimaId == p.Id &&
                                    (c.OrdenProduccion.Estado == EstadoOrden.Pendiente ||
                                     c.OrdenProduccion.Estado == EstadoOrden.EnProceso))
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
                p.EsFazonCalculado,
                p.EsFazon,
                p.EsScrap,
                p.ClienteId,
                p.PrecioCosto,
                p.StockMinimo,
                p.PesoEspecifico,
                p.StockFisico,
                p.StockReservado,
                p.EsGenerico,
                p.ProveedorId,
                p.ProveedorNombre,

                StockDisponible = p.StockFisico - p.StockReservado
            });

            return Ok(resultado);
        }

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
                EspesorMinimo = producto.EspesorMinimo ?? 0,
                EspesorMaximo = producto.EspesorMaximo ?? 0,
                EsGenerico = producto.EsGenerico,
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
                    EsGenerico = false,
                    Rubro = !esProductoTerminado ? "MATERIA PRIMA PLASTICA" : "PRODUCTO TERMINADO",
                    ProveedorId = data.ProveedorId,
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

        [HttpGet("movimientos/{productoId}")]
        public async Task<IActionResult> GetMovimientos(int productoId, [FromQuery] int mes, [FromQuery] int anio)
        {
            try
            {
                var movimientos = await _context.Movimientos
                    .Where(m => m.ProductoId == productoId
                             && m.Fecha.Month == mes
                             && m.Fecha.Year == anio)
                    .OrderByDescending(m => m.Fecha)
                    .Select(m => new {
                        Id = m.Id,
                        Fecha = m.Fecha,
                        TipoMovimiento = m.TipoMovimiento,
                        Cantidad = m.Cantidad,
                        Observacion = m.Observacion
                    })
                    .ToListAsync();

                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al obtener el historial.", detalle = ex.Message });
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
                    EsGenerico = true,
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

        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoEditarDto data)
        {
            if (id <= 0) return BadRequest("ID inválido.");

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound("❌ Producto no encontrado.");

            producto.Nombre = data.Nombre.Trim();
            producto.CodigoSku = data.CodigoSku.Trim().ToUpper();
            producto.StockMinimo = data.StockMinimo;

            // Si ProductoEditarDto lo incluye, actualizamos Color y StockActual fusionando la rama Master
            // (Si no usas estas propiedades en el DTO, puedes eliminarlas)
            // producto.Color = data.Color; 
            // producto.StockActual = data.StockActual;

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "✅ Producto actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar: " + ex.Message);
            }
        }

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

        [HttpPut("configurar/{id}")]
        public async Task<IActionResult> ConfigurarProducto(int id, [FromBody] ProductoConfigDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound("Producto no encontrado");

            producto.StockMinimo = dto.StockMinimo;
            producto.PesoEspecifico = dto.PesoEspecifico;
            producto.EsMateriaPrima = dto.EsMateriaPrima;
            producto.EsProductoTerminado = dto.EsProductoTerminado;
            producto.EsFazon = dto.EsFazon;
            producto.PrecioCosto = dto.PrecioCosto;
            producto.Rubro = dto.Rubro;
            producto.StockActual = dto.StockActual;

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
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Configuración y Stock guardados." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al guardar: " + ex.Message);
            }
        }

        public class CrearMasterbatchDto
        {
            public string NombreColor { get; set; }
            public string CodigoPersonalizado { get; set; }
            public decimal StockInicial { get; set; }
            public int? ProveedorId { get; set; }
        }

        [HttpPost("crear-masterbatch")]
        public async Task<IActionResult> CrearMasterbatchRapido([FromBody] CrearMasterbatchDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NombreColor))
                return BadRequest(new { mensaje = "El nombre del color es obligatorio." });

            string sku = string.IsNullOrWhiteSpace(dto.CodigoPersonalizado)
                ? $"MB-{dto.NombreColor.Substring(0, Math.Min(3, dto.NombreColor.Length)).ToUpper()}-{DateTime.Now.Millisecond}"
                : dto.CodigoPersonalizado;

            var nuevoMasterbatch = new Producto
            {
                Nombre = $"MB {dto.NombreColor}",
                CodigoSku = sku,
                EsMateriaPrima = true,
                EsProductoTerminado = false,
                EsGenerico = false,
                EsFazon = false,
                Rubro = "MASTERBATCH",
                PesoEspecifico = 1.1m,
                StockActual = dto.StockInicial,
                StockMinimo = 0,
                Activo = true,
                FechaCreacion = DateTime.Now,
                ProveedorId = dto.ProveedorId
            };

            _context.Productos.Add(nuevoMasterbatch);

            if (dto.StockInicial > 0)
            {
                await _context.SaveChangesAsync();

                _context.Movimientos.Add(new Movimiento
                {
                    ProductoId = nuevoMasterbatch.Id,
                    Cantidad = dto.StockInicial,
                    Fecha = DateTime.Now,
                    TipoMovimiento = "INGRESO_INICIAL",
                    Observacion = "Carga rápida de nuevo Masterbatch",
                });
            }

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "✅ Color creado exitosamente.", producto = nuevoMasterbatch });
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
                            (c.OrdenProduccion.Estado == EstadoOrden.Pendiente ||
                             c.OrdenProduccion.Estado == EstadoOrden.EnProceso))
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

        public class NuevaMateriaPrimaDto
        {
            public string Nombre { get; set; }
            public string CodigoSku { get; set; }
            public int? ProveedorId { get; set; }
        }

        [HttpPost("crear-materia-prima")]
        public async Task<IActionResult> CrearMateriaPrimaManual([FromBody] NuevaMateriaPrimaDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.CodigoSku))
                return BadRequest(new { mensaje = "Nombre y SKU son obligatorios." });

            if (await _context.Productos.AnyAsync(p => p.CodigoSku == dto.CodigoSku))
                return BadRequest(new { mensaje = "❌ El Código SKU ya existe en la base de datos." });

            var nuevaMp = new Producto
            {
                Nombre = dto.Nombre.Trim().ToUpper(),
                CodigoSku = dto.CodigoSku.Trim().ToUpper(),
                EsMateriaPrima = true,
                EsProductoTerminado = false,
                EsGenerico = false,
                EsFazon = false,
                Rubro = "MATERIA PRIMA PLASTICA",
                Activo = true,
                StockActual = 0,
                StockMinimo = 0,
                PrecioCosto = 0,
                PesoEspecifico = 1.0m,
                FechaCreacion = DateTime.Now,
                ProveedorId = dto.ProveedorId
            };

            try
            {
                _context.Productos.Add(nuevaMp);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "✅ Materia prima creada correctamente.", producto = nuevaMp });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = "Error al guardar en BD: " + ex.Message });
            }
        }
    }
}