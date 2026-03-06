using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;
using EstruplastERP.Api.Services;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OrdenesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ProduccionService _produccionService;

        public OrdenesController(ApplicationDbContext context, ProduccionService produccionService)
        {
            _context = context;
            _produccionService = produccionService;
        }

        [HttpGet("recientes")]
        public async Task<ActionResult> GetOrdenesRecientes()
        {
            var lista = await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Consumos).ThenInclude(c => c.MateriaPrima)
                .OrderByDescending(o => o.FechaCreacion)
                .Take(50)
                .Select(o => new
                {
                    o.Id,
                    Fecha = o.FechaCreacion.ToString("dd/MM HH:mm"),
                    Producto = o.Producto != null ? o.Producto.Nombre : "Desconocido",
                    ProductoId = o.ProductoId,
                    NotaPedido = o.NotaPedido,
                    ClienteId = o.ClienteId,
                    Observacion = o.Observacion,
                    o.Largo,
                    o.Ancho,
                    o.Espesor,
                    o.Cantidad,
                    Kilos = o.KilosEstimados,
                    Estado = o.Estado.ToString(),
                    EsFinalizada = o.Estado == EstadoOrden.Finalizada,
                    Consumos = o.Consumos.Select(c => new {
                        MateriaPrimaId = c.MateriaPrimaId,
                        NombreMateriaPrima = c.MateriaPrima.Nombre,
                        CantidadKilos = c.CantidadKilos
                    }).ToList()
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost("activar/{id}")]
        public async Task<IActionResult> ActivarOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Consumos)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null)
                return NotFound(new { mensaje = "Orden no encontrada." });

            if (orden.Estado != EstadoOrden.EnCola)
                return BadRequest(new { mensaje = "Solo las órdenes 'En Cola' pueden ser enviadas a producción." });

            var faltantes = new List<string>();
            foreach (var consumo in orden.Consumos)
            {
                var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                if (mp != null && !(mp.Id >= 990 && mp.Id <= 999))
                {
                    if (mp.StockActual < consumo.CantidadKilos)
                    {
                        faltantes.Add($"- {mp.Nombre}: Faltan {(consumo.CantidadKilos - mp.StockActual):N2} kg");
                    }
                }
            }

            if (faltantes.Any())
            {
                return BadRequest(new { mensaje = "Faltan materiales:\n" + string.Join("\n", faltantes) });
            }

            orden.Estado = EstadoOrden.Pendiente;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Hay stock. Orden enviada a Máquina." });
        }

        [HttpPost]
        public async Task<ActionResult<OrdenProduccion>> PostOrden([FromBody] NuevaOrdenDto dto)
        {
            if (dto.Kilos <= 0)
                return BadRequest(new { mensaje = "Los kilos deben ser mayores a 0." });

            if (dto.Consumos != null && dto.Consumos.Any(c => c.MateriaPrimaId == 22))
                return BadRequest(new { mensaje = "Debe reemplazar el Masterbatch Genérico (ID 22) por un color real." });

            try
            {
                dynamic check = await _produccionService.VerificarStock(dto);
                var jsonCheck = System.Text.Json.JsonSerializer.Serialize(check);
                using var doc = System.Text.Json.JsonDocument.Parse(jsonCheck);

                bool hayStock = doc.RootElement.GetProperty("posible").GetBoolean();

                var orden = await _produccionService.RegistrarOrden(dto, hayStock);

                string msg = hayStock
                    ? "Hay stock. Orden enviada directo a Máquina."
                    : "Material insuficiente. Orden guardada en Cola.";

                return CreatedAtAction("GetOrden", new { id = orden.Id }, new { mensaje = msg, id = orden.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = $"Error al crear orden: {ex.Message}" });
            }
        }

        [HttpPost("confirmar/{id}")]
        public async Task<IActionResult> ConfirmarOrden(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orden = await _context.Ordenes
                    .Include(o => o.Producto)
                    .Include(o => o.Consumos)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null)
                    return NotFound(new { mensaje = "Orden no encontrada." });

                if (orden.Estado == EstadoOrden.Finalizada)
                    return BadRequest(new { mensaje = "Esta orden ya fue finalizada." });

                foreach (var consumo in orden.Consumos)
                {
                    var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (mp != null)
                    {
                        mp.StockActual -= consumo.CantidadKilos;

                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = mp.Id,
                            Cantidad = -consumo.CantidadKilos,
                            TipoMovimiento = "CONSUMO",
                            Observacion = $"Fabricación Orden #{id}",
                            ClienteId = orden.ClienteId
                        });
                    }
                }

                if (orden.Producto != null)
                {
                    orden.Producto.StockActual += orden.KilosEstimados;

                    _context.Movimientos.Add(new Movimiento
                    {
                        Fecha = DateTime.Now,
                        ProductoId = orden.ProductoId,
                        Cantidad = orden.KilosEstimados,
                        TipoMovimiento = "PRODUCCION_TERMINADA",
                        Observacion = $"Cierre Orden #{id}",
                        ClienteId = orden.ClienteId
                    });
                }

                orden.Estado = EstadoOrden.Finalizada;
                orden.FechaFin = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Producción confirmada. Inventario actualizado." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = $"Error al confirmar la orden: {ex.Message}" });
            }
        }

        [HttpPost("cancelar/{id}")]
        public async Task<IActionResult> CancelarOrden(int id)
        {
            var orden = await _context.Ordenes.FindAsync(id);
            if (orden == null)
                return NotFound(new { mensaje = "Orden no encontrada." });

            if (orden.Estado == EstadoOrden.Finalizada)
                return BadRequest(new { mensaje = "No se puede cancelar una orden que ya fue finalizada." });

            orden.Estado = EstadoOrden.Cancelada;
            orden.FechaFin = DateTime.Now;

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Orden cancelada correctamente." });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenProduccion>>> GetOrdenes()
        {
            return await _context.Ordenes
                .Include(o => o.Producto).Include(o => o.Cliente)
                .OrderByDescending(o => o.FechaCreacion).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenProduccion>> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Consumos).ThenInclude(c => c.MateriaPrima)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null) return NotFound();
            return orden;
        }
    }
}