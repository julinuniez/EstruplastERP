using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public MovimientosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- MÉTODOS DE CONSULTA Y AJUSTE MANUAL ---

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorial()
        {
            var historial = await _context.Movimientos
                .Include(m => m.Producto)
                .Include(m => m.Proveedor)
                .Include(m => m.Cliente)
                .OrderByDescending(m => m.Fecha)
                .Take(100)
                .Select(m => new
                {
                    m.Id,
                    // 🚨 CORRECCIÓN CLAVE: Formato ISO para que Vue / JavaScript lo entienda perfecto
                    Fecha = m.Fecha.ToString("yyyy-MM-ddTHH:mm:ss"),
                    Producto = m.Producto != null ? m.Producto.Nombre : "Producto eliminado",
                    Proveedor = m.Proveedor != null ? m.Proveedor.RazonSocial : "-",
                    Cliente = m.Cliente != null ? m.Cliente.RazonSocial : "-",
                    m.Cantidad,
                    m.TipoMovimiento,
                    m.Observacion
                })
                .ToListAsync();

            return Ok(historial);
        }

        [HttpPost("ajuste")]
        public async Task<IActionResult> RegistrarAjuste([FromBody] MovimientoStockRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var producto = await _context.Productos.FindAsync(request.ProductoId);
                if (producto == null) return NotFound("Producto no encontrado");

                // Calculamos la cantidad real a impactar (resta si es egreso, suma si es ingreso)
                decimal cantidadReal = request.TipoMovimiento == "EGRESO"
                    ? -Math.Abs(request.Cantidad)
                    : Math.Abs(request.Cantidad);

                producto.StockActual += cantidadReal;

                var movimiento = new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = request.ProductoId,
                    Cantidad = Math.Abs(request.Cantidad), // El historial guarda en positivo
                    TipoMovimiento = request.TipoMovimiento == "EGRESO" ? "SALIDA_AJUSTE" : "ENTRADA_AJUSTE",
                    Observacion = $"[AJUSTE MANUAL] {request.Observacion}"
                };

                _context.Movimientos.Add(movimiento);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Stock ajustado correctamente", nuevoStock = producto.StockActual });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al realizar ajuste: {ex.Message}");
            }
        }

        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarMovimiento(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null) return NotFound(new { mensaje = "El movimiento no existe." });

            var producto = await _context.Productos.FindAsync(movimiento.ProductoId);

            if (producto != null)
            {
                producto.StockActual -= movimiento.Cantidad;
            }
            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensaje = "✅ Movimiento eliminado y stock revertido correctamente.",
                stockRestaurado = producto?.StockActual
            });
        }

        [HttpPost("ingreso-inteligente")]
        public async Task<IActionResult> RegistrarIngresoInteligente([FromBody] IngresoScrapDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                string nombreBusqueda = dto.NombreProducto.Trim().ToUpper();

                var producto = await _context.Productos
                    .FirstOrDefaultAsync(p => p.Nombre.ToUpper() == nombreBusqueda && p.ClienteId == dto.ClienteId);

                if (producto == null)
                {
                    producto = new Producto
                    {
                        Nombre = dto.NombreProducto.ToUpper(),
                        ClienteId = dto.ClienteId,
                        StockActual = 0,
                        EsMateriaPrima = true,
                        EsFazon = (dto.ClienteId != null && dto.ClienteId > 0),
                        Rubro = "RECUPERADO",
                        TipoMaterial = "RECUPERADO",
                        FechaCreacion = DateTime.Now,
                        EspesorMinimo = 0,
                        EspesorMaximo = 0
                    };
                    _context.Productos.Add(producto);
                    await _context.SaveChangesAsync();
                }

                producto.StockActual += dto.Cantidad;

                _context.Movimientos.Add(new Movimiento
                {
                    ProductoId = producto.Id,
                    Fecha = DateTime.Now,
                    Cantidad = dto.Cantidad,
                    TipoMovimiento = "INGRESO_RECUPERADO",
                    Observacion = "Ingreso Manual (Scrap/Fazón)",
                    ClienteId = dto.ClienteId
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = $"✅ Ingresados {dto.Cantidad}kg a {producto.Nombre}" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("ingreso-variante")]
        public async Task<IActionResult> IngresarConVariante([FromBody] IngresoVarianteDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var productoBase = await _context.Productos.FindAsync(dto.ProductoBaseId);
                if (productoBase == null) return BadRequest("El producto base no existe.");

                string nombreColor = dto.Color?.Trim().ToUpper();
                string nombreFinal = productoBase.Nombre.ToUpper();

                if (!string.IsNullOrEmpty(nombreColor))
                {
                    if (!nombreFinal.Contains(nombreColor))
                    {
                        nombreFinal = $"{nombreFinal} {nombreColor}";
                    }
                }

                var productoDestino = await _context.Productos
                    .FirstOrDefaultAsync(p => p.Nombre == nombreFinal && p.ClienteId == dto.ClienteId);

                if (productoDestino == null)
                {
                    productoDestino = new Producto
                    {
                        Nombre = nombreFinal,
                        ClienteId = dto.ClienteId,
                        StockActual = 0,
                        Rubro = productoBase.Rubro,
                        TipoMaterial = productoBase.TipoMaterial,
                        EsMateriaPrima = true,
                        EsFazon = (dto.ClienteId != null && dto.ClienteId > 0),
                        PesoEspecifico = productoBase.PesoEspecifico,
                        FechaCreacion = DateTime.Now,
                        EspesorMinimo = 0,
                        EspesorMaximo = 0
                    };
                    _context.Productos.Add(productoDestino);
                    await _context.SaveChangesAsync();
                }

                productoDestino.StockActual += dto.Cantidad;

                _context.Movimientos.Add(new Movimiento
                {
                    ProductoId = productoDestino.Id,
                    Fecha = DateTime.Now,
                    Cantidad = dto.Cantidad,
                    TipoMovimiento = "INGRESO_RECUPERADO",
                    Observacion = string.IsNullOrEmpty(nombreColor) ? "Ingreso Base" : $"Ingreso Variante {nombreColor}",
                    ClienteId = dto.ClienteId
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = $"✅ Ingresados {dto.Cantidad}kg a {productoDestino.Nombre}" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error variante: {ex.Message}");
            }
        }
        


        [HttpPost("ingresar-molido")]
        public async Task<IActionResult> IngresarMolido([FromBody] IngresoMolidoRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                Producto productoScrap = null;

                if (request.ProductoExistenteId.HasValue && request.ProductoExistenteId.Value > 0)
                {
                    productoScrap = await _context.Productos.FindAsync(request.ProductoExistenteId.Value);
                    if (productoScrap == null) return NotFound("El producto seleccionado no existe.");
                }
                else
                {
                    var materialBase = await _context.Productos.FindAsync(request.MaterialBaseId);
                    if (materialBase == null) return BadRequest("Material base no encontrado.");

                    string variedadLimpia = string.IsNullOrWhiteSpace(request.Variedad) ? "GRAL" : request.Variedad.Trim().ToUpper();

                    // Nombre seguro homologado al Excel
                    string nombreMaterial = materialBase.Nombre?.ToUpper() ?? "MATERIAL";
                    string nombreScrap = $"[MOLIDO] {variedadLimpia} ({nombreMaterial})";

                    // Tipo de material seguro para el SKU
                    string materialSeguro = materialBase.TipoMaterial ?? materialBase.Nombre ?? "BASE";
                    string tipoMatLimpio = new string(materialSeguro.Where(char.IsLetterOrDigit).ToArray()).ToUpper();
                    if (tipoMatLimpio.Length > 4) tipoMatLimpio = tipoMatLimpio.Substring(0, 4);

                    string clienteSufijo = request.ClienteId.HasValue ? request.ClienteId.Value.ToString() : "0";
                    string prefixBusqueda = $"MOL-{tipoMatLimpio}";
                    string suffixBusqueda = $"-C{clienteSufijo}";

                    var skusExistentes = await _context.Productos
                        .Where(p => p.CodigoSku != null &&
                                    p.CodigoSku.StartsWith(prefixBusqueda) &&
                                    p.CodigoSku.EndsWith(suffixBusqueda))
                        .Select(p => p.CodigoSku)
                        .ToListAsync();

                    int maxNumero = 0;

                    foreach (var skuExistente in skusExistentes)
                    {
                        string numStr = skuExistente.Replace(prefixBusqueda, "").Replace(suffixBusqueda, "");
                        if (int.TryParse(numStr, out int num))
                        {
                            if (num > maxNumero) maxNumero = num;
                        }
                    }

                    // Generador Autonumérico (si maxNumero es 0, arranca en "001")
                    string codigoAutonumerico = (maxNumero + 1).ToString("D3");
                    string sku = $"{prefixBusqueda}{codigoAutonumerico}{suffixBusqueda}";

                    productoScrap = await _context.Productos
                        .FirstOrDefaultAsync(p => p.CodigoSku == sku && p.ClienteId == request.ClienteId);

                    if (productoScrap == null)
                    {
                        productoScrap = new Producto
                        {
                            Nombre = nombreScrap,
                            CodigoSku = sku,
                            ClienteId = request.ClienteId,
                            StockActual = 0,
                            Rubro = request.ClienteId.HasValue ? "MOLIDO CLIENTE" : "MOLIDO",
                            TipoMaterial = materialBase.TipoMaterial,
                            EsScrap = true,
                            EsMateriaPrima = true,
                            EsFazon = false,
                            EsProductoTerminado = false,
                            Activo = true,
                            FechaCreacion = DateTime.Now,
                            PesoEspecifico = materialBase.PesoEspecifico > 0 ? materialBase.PesoEspecifico : 1,
                            EspesorMinimo = 0,
                            EspesorMaximo = 0
                        };
                        _context.Productos.Add(productoScrap);
                        await _context.SaveChangesAsync();
                    }
                }

                productoScrap.StockActual += request.Kilos;

                _context.Movimientos.Add(new Movimiento
                {
                    ProductoId = productoScrap.Id,
                    ClienteId = request.ClienteId,
                    Fecha = DateTime.Now,
                    Cantidad = request.Kilos,
                    TipoMovimiento = "INGRESO_MOLIDO",
                    Observacion = $"Ingreso Molienda: {productoScrap.Nombre}"
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Molienda ingresada correctamente",
                    producto = productoScrap.Nombre,
                    stock = productoScrap.StockActual
                });
            }
            catch (Exception ex)
            {
                try { await transaction.RollbackAsync(); } catch { }
                Console.WriteLine($"\n\n❌ ERROR FATAL EN INGRESAR MOLIDO: {ex.Message}\n{ex.StackTrace}\n\n");
                return StatusCode(500, new { mensaje = ex.Message, detalle = ex.InnerException?.Message });
            }
        }
    }
}