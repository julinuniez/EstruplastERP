using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemitosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RemitosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Remitos (Historial ordenado por fecha)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorial()
        {
            var remitos = await _context.Remitos
                .Include(r => r.Detalles) // Traemos los items
                .ThenInclude(d => d.Producto) // Traemos nombres de productos
                .OrderByDescending(r => r.Fecha) // Los más nuevos primero
                .Select(r => new
                {
                    r.Id,
                    r.NumeroRemito,
                    Fecha = r.Fecha.ToShortDateString(),
                    r.ClienteNombre,
                    TotalItems = r.Detalles.Sum(d => d.Cantidad),
                    Items = r.Detalles.Select(d => new {
                        Producto = d.Producto.Nombre,
                        Sku = d.Producto.CodigoSku,
                        Cantidad = d.Cantidad
                    }).ToList()
                })
                .ToListAsync();

            return Ok(remitos);
        }
    }
}