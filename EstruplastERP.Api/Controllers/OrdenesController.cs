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
                .Include(o => o.Empleado)
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
                    EmpleadoId = o.EmpleadoId,
                    Turno = o.Turno,
                    Observacion = o.Observacion,

                    o.Largo,
                    o.Ancho,
                    o.Espesor,

                    o.Cantidad,
                    Kilos = o.KilosEstimados,
                    Operario = o.Empleado != null ? o.Empleado.NombreCompleto : "-",
                    Estado = o.Estado.ToString(),
                    EsFinalizada = o.Estado == EstadoOrden.Finalizada,
                    
                    Consumos = o.Consumos.Select(c => new {
                        MateriaPrimaId = c.MateriaPrimaId,
                        NombreMateriaPrima = c.MateriaPrima.Nombre,
                        CantidadKilos = c.CantidadKilos // Kilos reales consumidos
                    }).ToList()
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


        [HttpPost("cancelar/{id}")]
        public async Task<IActionResult> CancelarOrden(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Buscar la orden con sus consumos (LA FOTO EXACTA DE LO QUE SE GASTÓ)
                var orden = await _context.Ordenes
                    .Include(o => o.Consumos)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound("Orden no encontrada.");

                // Solo permitimos cancelar si está Pendiente o EnProceso
                if (orden.Estado == EstadoOrden.Finalizada || orden.Estado == EstadoOrden.Cancelada)
                    return BadRequest("No se puede cancelar una orden finalizada o ya cancelada.");

                // 2. Devolver Materias Primas al Inventario
                foreach (var consumo in orden.Consumos)
                {
                    var materiaPrima = await _context.Productos.FindAsync(consumo.MateriaPrimaId);

                    if (materiaPrima != null)
                    {
                        // A. Devolvemos el stock
                        materiaPrima.StockActual += consumo.CantidadKilos;

                        // B. Dejamos registro en el Kardex (Entrada por cancelación)
                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = materiaPrima.Id,
                            Cantidad = consumo.CantidadKilos, // Positivo = Entrada
                            TipoMovimiento = "DEVOLUCION", // O "CANCELACION_ORDEN"
                            Observacion = $"Cancelación Orden #{orden.Id}",
                            EmpleadoId = orden.EmpleadoId,
                            Turno = orden.Turno,
                            ClienteId = orden.ClienteId
                        });
                    }
                }

                // 3. Marcar orden como cancelada
                orden.Estado = EstadoOrden.Cancelada;
                orden.FechaFin = DateTime.Now; // Fecha de cancelación

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Orden cancelada y stock devuelto al inventario." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al cancelar: {ex.Message}");
            }
        }
    }
}