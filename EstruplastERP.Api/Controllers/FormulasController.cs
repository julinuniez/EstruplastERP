using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormulasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FormulasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/formulas/5 (Trae la receta completa de un producto)
        [HttpGet("{productoId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetReceta(int productoId)
        {
            // Buscamos todas las filas donde el producto terminado sea el ID que pedimos
            var receta = await _context.Formulas
                .Where(f => f.ProductoTerminadoId == productoId)
                .Include(f => f.MateriaPrima) 
                .Select(f => new
                {
                    f.Id,
                    Ingrediente = f.MateriaPrima != null ?  f.MateriaPrima.Nombre:"Producto eliminado", 
                    f.MateriaPrimaId,
                    f.Cantidad
                })
                .ToListAsync();

            if (receta == null || !receta.Any())
            {
                return NotFound("Este producto no tiene receta cargada.");
            }

            return Ok(receta);
        }

        // 2. POST: api/formulas (Agrega un ingrediente a la receta)
        [HttpPost]
        public async Task<ActionResult<Formula>> PostIngrediente(Formula formulaItem)
        {
            if (formulaItem.Cantidad <= 0)
            {
                return BadRequest("La cantidad debe ser mayor a 0.");
            }

            _context.Formulas.Add(formulaItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReceta", new { productoId = formulaItem.ProductoTerminadoId }, formulaItem);
        }

        // 3. DELETE: api/formulas/5 (Borra un ingrediente de la receta)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIngrediente(int id)
        {
            var formulaItem = await _context.Formulas.FindAsync(id);
            if (formulaItem == null)
            {
                return NotFound();
            }

            _context.Formulas.Remove(formulaItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}