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

        // 1. GET: api/movimientos (Ver el historial)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorial()
        {
            // Traemos los últimos 100 movimientos
            var historial = await _context.Movimientos
                .Include(m => m.Producto)
                .OrderByDescending(m => m.Fecha) // Los más nuevos primero
                .Take(100)
                .Select(m => new
                {
                    m.Id,
                    Fecha = m.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    Producto = m.Producto != null ? m.Producto.Nombre : "El producto no existe",
                    m.Cantidad,
                    m.TipoMovimiento,
                    m.Observacion
                })
                .ToListAsync();

            return Ok(historial);
        }

        // 2. POST: api/movimientos/ajuste (Ingreso manual o compra)
        [HttpPost("ajuste")]
        public async Task<IActionResult> RegistrarAjuste([FromBody] MovimientoStockRequest request)
        {
            var producto = await _context.Productos.FindAsync(request.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");

            // Actualizamos el Stock real
            producto.StockActual += request.Cantidad;

            // Creamos el registro en el historial
            var movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                ProductoId = request.ProductoId,
                Cantidad = request.Cantidad,
                TipoMovimiento = request.Cantidad > 0 ? "ENTRADA_AJUSTE" : "SALIDA_AJUSTE",
                Observacion = request.Observacion
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Stock actualizado", nuevoStock = producto.StockActual });
        }
    }
}