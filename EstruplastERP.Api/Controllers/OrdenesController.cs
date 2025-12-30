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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenProduccion>>> GetOrdenes()
        {
            return await _context.Ordenes
                .Include(o => o.Producto).Include(o => o.Cliente).Include(o => o.Empleado)
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

        [HttpGet("rango")]
        public async Task<ActionResult> GetProduccionPorRango(DateTime desde, DateTime hasta)
        {
            DateTime hastaFinDia = hasta.Date.AddDays(1).AddTicks(-1);
            var lista = await _context.Ordenes
                .Include(o => o.Empleado).Include(o => o.Producto)
                .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hastaFinDia)
                .OrderByDescending(o => o.FechaCreacion)
                .Select(o => new
                {
                    o.Id,
                    Fecha = o.FechaCreacion,
                    o.Turno,
                    Operario = o.Empleado != null ? o.Empleado.NombreCompleto : "Sin Asignar",
                    Producto = o.Producto != null ? o.Producto.Nombre : "Producto Eliminado",
                    Lote = "L-" + o.Id.ToString(),
                    o.Cantidad,
                    Kilos = o.KilosEstimados,
                    estado = (int)o.Estado
                }).ToListAsync();
            return Ok(lista);
        }

        [HttpPost]
        public async Task<ActionResult<OrdenProduccion>> PostOrden([FromBody] NuevaOrdenDto dto)
        {
            if (dto.Kilos <= 0) return BadRequest("Los kilos deben ser mayores a 0.");
            if (dto.Consumos != null && dto.Consumos.Any(c => c.MateriaPrimaId == 22))
                return BadRequest("⛔ ERROR: Debe especificar el color real (No use Masterbatch Genérico ID 22).");

            try
            {
                // 1. Verificar Stock (Usando dynamic para leer respuesta anónima del servicio)
                dynamic check = await _produccionService.VerificarStock(dto);

                // Serialización rápida para leer propiedades
                var jsonCheck = System.Text.Json.JsonSerializer.Serialize(check);
                using var doc = System.Text.Json.JsonDocument.Parse(jsonCheck);

                if (!doc.RootElement.GetProperty("posible").GetBoolean())
                {
                    return BadRequest(doc.RootElement.GetProperty("mensaje").GetString());
                }

                // 2. Crear Orden
                var orden = await _produccionService.RegistrarOrden(dto);

                return CreatedAtAction("GetOrden", new { id = orden.Id }, new { mensaje = "Orden creada", id = orden.Id });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        [HttpPost("finalizar/{id}")]
        public async Task<IActionResult> FinalizarOrden(int id, [FromBody] FinalizarOrdenDto request)
        {
            var orden = await _context.Ordenes.Include(o => o.Producto).FirstOrDefaultAsync(o => o.Id == id);
            if (orden == null) return NotFound("Orden no encontrada.");
            if (orden.Estado == EstadoOrden.Finalizada) return BadRequest("Ya finalizada.");

            // Procesar Adiciones/Ajustes de MP al final
            if (request.Adiciones != null)
            {
                foreach (var item in request.Adiciones)
                {
                    var insumo = await _context.Productos.FindAsync(item.MateriaPrimaId);
                    if (insumo != null)
                    {
                        if (insumo.StockActual < item.Cantidad)
                            return BadRequest($"Falta stock de {insumo.Nombre} para el ajuste final.");

                        insumo.StockActual -= item.Cantidad;
                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = insumo.Id,
                            Cantidad = -item.Cantidad,
                            TipoMovimiento = "AJUSTE_FIN",
                            Observacion = $"Ajuste OP#{id}: {item.Motivo}",
                            Turno = orden.Turno,
                            EmpleadoId = orden.EmpleadoId,
                            ClienteId = orden.ClienteId
                        });
                    }
                }
            }

            // Actualizar Orden
            if (request.Desperdicio > 0) orden.Observacion += $" | Scrap: {request.Desperdicio}kg";
            if (!string.IsNullOrEmpty(request.Observacion)) orden.Observacion += $" | Fin: {request.Observacion}";

            orden.Estado = EstadoOrden.Finalizada;
            orden.FechaFin = DateTime.Now;

            // Ingreso de Producto Terminado (Solo al finalizar)
            if (orden.Producto != null)
            {
                orden.Producto.StockActual += orden.Cantidad;
                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = orden.ProductoId,
                    Cantidad = orden.Cantidad,
                    TipoMovimiento = "ENTRADA_PROD",
                    Observacion = $"Ingreso OP#{id}",
                    Turno = orden.Turno,
                    EmpleadoId = orden.EmpleadoId,
                    ClienteId = orden.ClienteId
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Orden finalizada correctamente." });
        }
    }
}