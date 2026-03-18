using EstruplastERP.Api.Dtos;
using EstruplastERP.Api.Services;
using EstruplastERP.Core;
using EstruplastERP.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduccionController : ControllerBase
    {
        private readonly ProduccionService _produccionService;
        private readonly ApplicationDbContext _context;

        public ProduccionController(ProduccionService produccionService, ApplicationDbContext context)
        {
            _produccionService = produccionService;
            _context = context;
        }

        [HttpPost("verificar")]
        public async Task<IActionResult> VerificarStock([FromBody] NuevaOrdenDto request)
        {
            var resultado = await _produccionService.VerificarStock(request);
            return Ok(resultado);
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarProduccion([FromBody] NuevaOrdenDto request)
        {
            try
            {
                dynamic check = await _produccionService.VerificarStock(request);
                var jsonCheck = System.Text.Json.JsonSerializer.Serialize(check);
                using var doc = System.Text.Json.JsonDocument.Parse(jsonCheck);
                bool hayStock = doc.RootElement.GetProperty("posible").GetBoolean();

                var produccion = await _produccionService.RegistrarOrden(request, hayStock);
                return Ok(new { mensaje = "Producción registrada correctamente", id = produccion.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("hoy")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionDelDia()
        {
            var hoy = DateTime.Today;

            var lista = await _context.Producciones
                .Include(p => p.Producto)
                .Where(p => p.FechaRegistro >= hoy)
                .OrderByDescending(p => p.FechaRegistro)
                .Select(p => new
                {
                    p.Id,
                    Fecha = p.FechaRegistro,
                    Producto = p.Producto.Nombre,
                    Cantidad = p.Cantidad,
                    Kilos = p.Kilos,
                    Lote = p.Lote ?? "Sin lote"
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("rango")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionPorRango(DateTime desde, DateTime hasta)
        {
            var finDia = new DateTime(hasta.Year, hasta.Month, hasta.Day, 23, 59, 59);

            var lista = await _context.Producciones
                .Include(p => p.Producto)
                .Where(p => p.FechaRegistro >= desde && p.FechaRegistro <= finDia)
                .OrderByDescending(p => p.FechaRegistro)
                .Select(p => new
                {
                    p.Id,
                    Fecha = p.FechaRegistro,
                    Producto = p.Producto.Nombre,
                    Cantidad = p.Cantidad,
                    Kilos = p.Kilos,
                    Lote = p.Lote ?? "SIN LOTE"
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("receta-proyectada")]
        public async Task<IActionResult> GetRecetaProyectada([FromQuery] int productoId, [FromQuery] int clienteId, [FromQuery] decimal kilos)
        {
            try
            {
                var receta = await _produccionService.ObtenerRecetaProyectada(productoId, clienteId, kilos);
                return Ok(receta);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("registrar-fazon-auto")]
        public async Task<IActionResult> RegistrarProduccionFazonAuto([FromBody] NuevaOrdenDto request)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                await DescontarStockScrapAutomatico(request);
                var produccion = await _produccionService.RegistrarOrden(request, true);
                await transaction.CommitAsync();
                return Ok(new { mensaje = "Producción Fazón registrada y stock descontado automáticamente.", id = produccion.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        private async Task DescontarStockScrapAutomatico(NuevaOrdenDto request)
        {
            var productoTerminado = await _context.Productos.FindAsync(request.ProductoTerminadoId);
            if (productoTerminado == null) throw new Exception("Producto terminado no encontrado");

            string materialNecesario = productoTerminado.TipoMaterial;
            int clienteId = request.ClienteId ?? productoTerminado.ClienteId ?? 0;
            if (clienteId == 0) throw new Exception("Error: Se requiere un Cliente para procesar Fazón.");

            var stocksDisponibles = await _context.Productos
                .Where(p => p.ClienteId == clienteId
                            && p.EsScrap == true
                            && p.TipoMaterial == materialNecesario
                            && p.StockActual > 0)
                .OrderByDescending(p => p.StockActual)
                .ToListAsync();

            if (!stocksDisponibles.Any())
            {
                throw new Exception($"No hay stock disponible de '{materialNecesario}' para el cliente seleccionado.");
            }

            decimal restantePorDescontar = request.Kilos;

            foreach (var lote in stocksDisponibles)
            {
                if (restantePorDescontar <= 0) break;

                decimal aDescontar = Math.Min(restantePorDescontar, lote.StockActual);

                lote.StockActual -= aDescontar;
                restantePorDescontar -= aDescontar;

                _context.Entry(lote).State = EntityState.Modified;
            }

            if (restantePorDescontar > 0)
            {
                throw new Exception($"Stock insuficiente. Faltan {restantePorDescontar} kg de {materialNecesario} en el inventario del cliente.");
            }

            await _context.SaveChangesAsync();
        }

        [HttpPost("transformar-scrap")]
        public async Task<IActionResult> TransformarScrap([FromBody] ScrapDto request)
        {
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (request.KilosObtenidos > request.KilosEntrada)
                    return BadRequest("Error físico: No puedes crear materia de la nada (Salida > Entrada).");

                var productoScrap = await _context.Productos.FindAsync(request.ProductoScrapId);
                if (productoScrap == null) return BadRequest("El producto Scrap no existe.");

                if (productoScrap.StockActual < request.KilosEntrada)
                    return BadRequest($"Stock insuficiente. Tienes {productoScrap.StockActual} kg, intentas usar {request.KilosEntrada} kg.");

                string skuRecuperado = productoScrap.CodigoSku.Replace("SCRAP", "REC").Replace("scrap", "rec");

                if (!skuRecuperado.ToUpper().Contains("REC"))
                    skuRecuperado = $"REC-{skuRecuperado}";

                var productoRecuperado = await _context.Productos
                    .FirstOrDefaultAsync(p => p.CodigoSku == skuRecuperado && p.ClienteId == request.ClienteId);

                if (productoRecuperado == null)
                {
                    string nombreBase = productoScrap.Nombre
                        .Replace("[SCRAP]", "")
                        .Replace("SCRAP", "")
                        .Replace("Scrap", "")
                        .Trim();

                    string nombreFinal = $"[RECUPERADO] {nombreBase}";

                    if (!string.IsNullOrEmpty(productoScrap.Color))
                    {
                        if (!nombreFinal.ToUpper().Contains(productoScrap.Color.ToUpper()))
                        {
                            nombreFinal += $" {productoScrap.Color.ToUpper()}";
                        }
                    }

                    productoRecuperado = new Producto
                    {
                        CodigoSku = skuRecuperado,
                        Nombre = nombreFinal,
                        Rubro = "MATERIA PRIMA RECUPERADA",
                        TipoMaterial = productoScrap.TipoMaterial,
                        Color = productoScrap.Color,
                        ClienteId = request.ClienteId,
                        EsScrap = false,
                        EsMateriaPrima = true,
                        EsProductoTerminado = false,
                        StockActual = 0,
                        StockMinimo = 0,
                        Activo = true,
                        FechaCreacion = DateTime.Now,
                        PesoEspecifico = productoScrap.PesoEspecifico > 0 ? productoScrap.PesoEspecifico : 1
                    };

                    _context.Productos.Add(productoRecuperado);
                    await _context.SaveChangesAsync();
                }

                productoScrap.StockActual -= request.KilosEntrada;
                productoRecuperado.StockActual += request.KilosObtenidos;

                _context.Entry(productoScrap).State = EntityState.Modified;
                _context.Entry(productoRecuperado).State = EntityState.Modified;

                decimal merma = request.KilosEntrada - request.KilosObtenidos;
                decimal porcentaje = (request.KilosEntrada > 0) ? (merma / request.KilosEntrada) * 100 : 0;

                var historial = new Produccion
                {
                    FechaRegistro = DateTime.Now,
                    ProductoTerminadoId = productoRecuperado.Id,
                    ClienteId = request.ClienteId,
                    Cantidad = 1,
                    Kilos = request.KilosObtenidos,
                    Observacion = $"TRANSFORMACION: {request.KilosEntrada}kg de '{productoScrap.Nombre}' -> {request.KilosObtenidos}kg de '{productoRecuperado.Nombre}'. Merma: {merma}kg ({porcentaje:N1}%)"
                };
                _context.Producciones.Add(historial);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Transformación registrada con éxito.",
                    producto = productoRecuperado.Nombre,
                    stockNuevo = productoRecuperado.StockActual,
                    merma = merma
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest($"Error en transformación: {ex.Message}");
            }
        }

        [HttpGet("historial-transformaciones")]
        public async Task<ActionResult> GetHistorialTransformaciones()
        {
            var lista = await _context.Producciones
                .Include(p => p.Producto)
                .Include(p => p.Cliente)
                .Where(p => p.Observacion.StartsWith("TRANSFORMACION"))
                .OrderByDescending(p => p.FechaRegistro)
                .Take(20)
                .Select(p => new {
                    p.Id,
                    Fecha = p.FechaRegistro,
                    Cliente = p.Cliente.RazonSocial,
                    Producto = p.Producto.Nombre,
                    Detalle = p.Observacion
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("tablero-pedidos")]
        public async Task<ActionResult> GetTableroPedidos([FromQuery] int? clienteId)
        {
            var query = _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Cliente)
                // 🚨 MAGIA 1: Filtramos las canceladas desde SQL. ¡No viajan más al frontend!
                .Where(o => o.Estado != EstadoOrden.Cancelada)
                .AsQueryable();

            if (clienteId.HasValue && clienteId > 0)
            {
                query = query.Where(o => o.ClienteId == clienteId);
            }

            var ordenes = await query.ToListAsync();

            var pedidosAgrupados = ordenes
                .GroupBy(o =>
                    !string.IsNullOrWhiteSpace(o.NotaPedido) ? o.NotaPedido.Trim().ToUpper() :
                    !string.IsNullOrWhiteSpace(o.NumeroPedidoCliente) ? o.NumeroPedidoCliente.Trim().ToUpper() :
                    "OP_AISLADA_" + o.Id.ToString()
                )
                .Select(g => new
                {
                    NotaPedido = g.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.NotaPedido))?.NotaPedido ?? "",
                    Pedido = g.FirstOrDefault(o => !string.IsNullOrWhiteSpace(o.NumeroPedidoCliente))?.NumeroPedidoCliente ?? "",
                    Cliente = g.FirstOrDefault(o => o.Cliente != null)?.Cliente?.RazonSocial ?? "Stock Propio",
                    Avance = g.All(o => o.Estado == EstadoOrden.Finalizada) ? 100 :
                             g.Any(o => o.Estado == EstadoOrden.EnProceso) ? 50 : 0,
                    Ordenes = g.Select(o => new
                    {
                        o.Id,
                        Producto = o.Producto != null ? o.Producto.Nombre : "Desconocido",
                        Medidas = $"{o.Ancho}mm x {o.Espesor} micrones", // Queda por las dudas

                        // 🚨 MAGIA 2: Enviamos las variables reales separadas
                        Largo = o.Largo,
                        Ancho = o.Ancho,
                        Espesor = o.Espesor,

                        // 🚨 MAGIA 3: Detectamos si es bobina en el backend
                        EsBobina = o.Largo == 0 || (o.Producto != null && o.Producto.Nombre.ToUpper().Contains("BOBINA")),
                        KilosPorBobina = (o.Largo == 0 && o.Cantidad > 0) ? Math.Round(o.KilosEstimados / o.Cantidad, 2) : 0,

                        Estado = o.Estado.ToString(),
                        o.Cantidad
                    }).ToList()
                })
                .OrderByDescending(p => string.IsNullOrWhiteSpace(p.NotaPedido) ? 0 : 1)
                .ThenByDescending(p => p.NotaPedido)
                .ToList();

            return Ok(pedidosAgrupados);
        }
    }
}