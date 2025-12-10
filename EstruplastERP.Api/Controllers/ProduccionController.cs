using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; 
using EstruplastERP.Data;            
using EstruplastERP.Api.Dtos;        
using EstruplastERP.Api.Services;    

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
                var produccion = await _produccionService.RegistrarOrden(request);
                return Ok(new { mensaje = "Producción registrada correctamente", id = produccion.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // GET: api/Produccion/hoy
        [HttpGet("hoy")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionDelDia()
        {
            var hoy = DateTime.Today;

            var lista = await _context.Producciones
                .Include(p => p.Producto)
                .Include(p => p.Empleado)
                .Where(p => p.FechaRegistro >= hoy)
                .OrderByDescending(p => p.FechaRegistro)
                .Select(p => new
                {
                    p.Id,
                    Fecha = p.FechaRegistro,
                    Producto = p.Producto.Nombre,
                    Cantidad = p.Cantidad,
                    Kilos = p.Kilos,
                    Lote = p.Lote ?? "Sin lote",
                    Operario = p.Empleado.NombreCompleto
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/Produccion/rango
        [HttpGet("rango")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionPorRango(DateTime desde, DateTime hasta)
        {
            var finDia = new DateTime(hasta.Year, hasta.Month, hasta.Day, 23, 59, 59);

            var lista = await _context.Producciones
                .Include(p => p.Producto)
                .Include(p => p.Empleado)
                .Where(p => p.FechaRegistro >= desde && p.FechaRegistro <= finDia)
                .OrderByDescending(p => p.FechaRegistro)
                .Select(p => new
                {
                    p.Id,
                    Fecha = p.FechaRegistro,
                    Producto = p.Producto.Nombre,
                    Cantidad = p.Cantidad,
                    Kilos = p.Kilos,
                    Lote = p.Lote ?? "SIN LOTE",
                    Operario = p.Empleado.NombreCompleto,
                    Turno = p.Turno
                })
                .ToListAsync();

            return Ok(lista);
        }
    }
}