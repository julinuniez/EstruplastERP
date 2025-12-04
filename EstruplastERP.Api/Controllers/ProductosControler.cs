using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/productos (Trae TODO)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            return await _context.Productos.ToListAsync();
        }

        // 2. GET: api/productos/materias-primas (Trae solo insumos para las recetas)
        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<Producto>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima == true)
                .ToListAsync();
        }

        // 3. POST: api/productos (Crea un nuevo producto)
        [HttpPost]
        public async Task<ActionResult<Producto>> PostProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductos", new { id = producto.Id }, producto);
        }
    }
}