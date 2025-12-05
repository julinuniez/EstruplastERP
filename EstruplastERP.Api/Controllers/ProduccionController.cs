using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.DTO_s; // Importante para usar el DTO

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProduccionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProduccionController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarProduccion([FromBody] OrdenProduccion request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // --- PASO A: SUMAR EL PRODUCTO TERMINADO ---
                var productoTerminado = await _context.Productos.FindAsync(request.ProductoTerminadoId);
                if (productoTerminado == null) return NotFound("Producto no encontrado");

                // Actualizamos Stock
                productoTerminado.StockActual += request.Cantidad;

                // Guardamos el Movimiento (Historial)
                var movEntrada = new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = request.ProductoTerminadoId,
                    Cantidad = request.Cantidad, // Positivo porque entra
                    TipoMovimiento = "PRODUCCION_ENTRADA",
                    Observacion = $"Fabricación de {request.Cantidad} unidades"
                };
                _context.Movimientos.Add(movEntrada);

                // --- PASO B: RESTAR LA MATERIA PRIMA (SEGÚN RECETA) ---

                // Buscamos la receta
                var receta = await _context.Formulas
                    .Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId)
                    .ToListAsync();

                foreach (var ingrediente in receta)
                {
                    // Calculamos cuánto consumir: (Ej: 50 bolsas * 0.08kg = 4kg)
                    decimal consumoTotal = request.Cantidad * ingrediente.Cantidad;

                    // Buscamos la materia prima real en la base de datos
                    var materiaPrima = await _context.Productos.FindAsync(ingrediente.MateriaPrimaId);

                    if (materiaPrima != null)
                    {
                        // Restamos Stock
                        materiaPrima.StockActual -= consumoTotal;

                        // Guardamos el Movimiento de Consumo
                        var movSalida = new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = ingrediente.MateriaPrimaId,
                            Cantidad = -consumoTotal, // Negativo porque sale
                            TipoMovimiento = "PRODUCCION_CONSUMO",
                            Observacion = $"Consumo para orden de {productoTerminado.Nombre}"
                        };
                        _context.Movimientos.Add(movSalida);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); 

                return Ok(new { mensaje = "Producción registrada con éxito", nuevoStock = productoTerminado.StockActual });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al procesar producción: " + ex.Message);
            }
        }
    }
}