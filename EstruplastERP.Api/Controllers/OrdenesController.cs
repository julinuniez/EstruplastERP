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

        // --- 1. NUEVO: LISTA RÁPIDA PARA EL FRONTEND ---
        [HttpGet("recientes")]
        public async Task<ActionResult> GetOrdenesRecientes()
        {
            var lista = await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Empleado)
                .OrderByDescending(o => o.FechaCreacion)
                .Take(50) // Traemos las últimas 50 para no saturar
                .Select(o => new
                {
                    o.Id,
                    Fecha = o.FechaCreacion.ToString("dd/MM HH:mm"), // Formato corto
                    Producto = o.Producto != null ? o.Producto.Nombre : "Desconocido",
                    o.Cantidad,
                    Kilos = o.KilosEstimados,
                    Operario = o.Empleado != null ? o.Empleado.NombreCompleto : "-",
                    Estado = o.Estado.ToString(),
                    EsFinalizada = o.Estado == EstadoOrden.Finalizada
                })
                .ToListAsync();

            return Ok(lista);
        }

        // --- 2. NUEVO: CONFIRMACIÓN SIMPLE (SUMAR STOCK) ---
        [HttpPost("confirmar/{id}")]
        public async Task<IActionResult> ConfirmarOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Producto)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null) return NotFound("Orden no encontrada.");
            if (orden.Estado == EstadoOrden.Finalizada) return BadRequest("Esta orden ya fue finalizada.");

            // A. Cambiar Estado
            orden.Estado = EstadoOrden.Finalizada;
            orden.FechaFin = DateTime.Now;

            // B. Sumar Stock de Producto Terminado
            if (orden.Producto != null)
            {
                // Sumamos KILOS (o Cantidad según tu unidad de medida)
                orden.Producto.StockActual += orden.KilosEstimados;

                // C. Registrar Movimiento en Kardex
                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = orden.ProductoId,
                    Cantidad = orden.KilosEstimados, // Positivo = Entrada
                    TipoMovimiento = "PRODUCCION_TERMINADA",
                    Observacion = $"Cierre Orden #{id} - {orden.Turno}",
                    Turno = orden.Turno,
                    EmpleadoId = orden.EmpleadoId,
                    ClienteId = orden.ClienteId
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Producción confirmada. Stock actualizado." });
        }

        // --- MÉTODOS EXISTENTES ---

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

        [HttpPost]
        public async Task<ActionResult<OrdenProduccion>> PostOrden([FromBody] NuevaOrdenDto dto)
        {
            if (dto.Kilos <= 0) return BadRequest("Los kilos deben ser mayores a 0.");
            if (dto.Consumos != null && dto.Consumos.Any(c => c.MateriaPrimaId == 22))
                return BadRequest("⛔ ERROR: Debe reemplazar el Masterbatch Genérico (ID 22) por un color real.");

            try
            {
                dynamic check = await _produccionService.VerificarStock(dto);
                var jsonCheck = System.Text.Json.JsonSerializer.Serialize(check);
                using var doc = System.Text.Json.JsonDocument.Parse(jsonCheck);

                if (!doc.RootElement.GetProperty("posible").GetBoolean())
                {
                    return BadRequest(doc.RootElement.GetProperty("mensaje").GetString());
                }

                var orden = await _produccionService.RegistrarOrden(dto);
                return CreatedAtAction("GetOrden", new { id = orden.Id }, new { mensaje = "Orden creada", id = orden.Id });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }
    }
}