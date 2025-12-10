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

        // ==========================================
        // 1. GET: Historial (Lectura)
        // ==========================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorial()
        {
            var historial = await _context.Movimientos
                .Include(m => m.Producto)
                .OrderByDescending(m => m.Fecha)
                .Take(100)
                .Select(m => new
                {
                    m.Id,
                    Fecha = m.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    Producto = m.Producto != null ? m.Producto.Nombre : "Producto eliminado",
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
            var producto = await _context.Productos.FindAsync(request.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");            
            producto.StockActual += request.Cantidad;

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
        
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarMovimiento(int id)
        {
            // A. Buscamos el movimiento a eliminar
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null) return NotFound(new { mensaje = "El movimiento no existe." });

            // B. Buscamos el producto asociado para devolverle el stock
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
    }
}