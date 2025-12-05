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
                // 1. Validar producto terminado
                var productoTerminado = await _context.Productos.FindAsync(request.ProductoTerminadoId);
                if (productoTerminado == null) return NotFound("Producto no encontrado");

                // 2. Buscar la receta
                var receta = await _context.Formulas
                    .Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId)
                    .ToListAsync();

                if (!receta.Any())
                {
                    // Opcional: ¿Permitimos fabricar cosas sin receta? 
                    // Para tu seguridad, mejor decimos que NO.
                    return BadRequest($"El producto '{productoTerminado.Nombre}' no tiene receta definida. No se puede calcular el consumo.");
                }

                // 3. VALIDACIÓN PREVIA (Simulación)
                // Antes de descontar nada, chequeamos si alcanza la materia prima
                foreach (var ingrediente in receta)
                {
                    decimal consumoTotal = request.Cantidad * ingrediente.Cantidad;
                    var materiaPrima = await _context.Productos.FindAsync(ingrediente.MateriaPrimaId);

                    if (materiaPrima == null)
                        return BadRequest($"Error en receta: Insumo ID {ingrediente.MateriaPrimaId} no existe.");

                    if (materiaPrima.StockActual < consumoTotal)
                    {
                        return BadRequest($"NO HAY STOCK SUFICIENTE de {materiaPrima.Nombre}. Requerido: {consumoTotal} - Disponible: {materiaPrima.StockActual}");
                    }
                }

                // 4. SI LLEGAMOS ACÁ, ALCANZA TODO. AHORA SÍ EJECUTAMOS.

                // A. Sumar Stock Producto Terminado
                productoTerminado.StockActual += request.Cantidad;

                var movEntrada = new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = request.ProductoTerminadoId,
                    Cantidad = request.Cantidad,
                    TipoMovimiento = "PRODUCCION_ENTRADA",
                    Observacion = $"Fabricación de {request.Cantidad} u. (Turno {request.Turno})",
                    EmpleadoId = request.EmpleadoId,
                    ClienteId = request.ClienteId,
                    Turno = request.Turno
                };
                _context.Movimientos.Add(movEntrada);

                // B. Restar Materia Prima
                foreach (var ingrediente in receta)
                {
                    decimal consumoTotal = request.Cantidad * ingrediente.Cantidad;
                    var materiaPrima = await _context.Productos.FindAsync(ingrediente.MateriaPrimaId);

                    if (materiaPrima != null)
                    {
                        materiaPrima.StockActual -= consumoTotal;

                        var movSalida = new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = ingrediente.MateriaPrimaId,
                            Cantidad = -consumoTotal,
                            TipoMovimiento = "PRODUCCION_CONSUMO",
                            Observacion = $"Consumo para {productoTerminado.Nombre}",
                            EmpleadoId = request.EmpleadoId,
                            Turno = request.Turno
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
                return StatusCode(500, "Error: " + ex.Message);
            }
        }

        // POST: api/Produccion/verificar
        [HttpPost("verificar")]
        public async Task<IActionResult> VerificarStock([FromBody] OrdenProduccion request)
        {
            // 1. Buscamos la receta
            var receta = await _context.Formulas
                .Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId)
                .ToListAsync();

            if (!receta.Any())
            {
                return Ok(new { posible = false, mensaje = "⚠️ No hay receta cargada para este producto." });
            }

            // 2. Simulamos el consumo ingrediente por ingrediente
            foreach (var item in receta)
            {
                var insumo = await _context.Productos.FindAsync(item.MateriaPrimaId);

                if (insumo == null) continue;

                decimal necesario = request.Cantidad * item.Cantidad;

                // Si falta de ESTE ingrediente, cortamos y avisamos
                if (insumo.StockActual < necesario)
                {
                    decimal faltante = necesario - insumo.StockActual;
                    return Ok(new
                    {
                        posible = false,
                        mensaje = $"🔴 Falta {insumo.Nombre}. Tienes {insumo.StockActual:N2}, necesitas {necesario:N2}."
                    });
                }
            }

            // 3. Si pasó todos los ingredientes sin error
            return Ok(new { posible = true, mensaje = "🟢 Stock Disponible para producción." });
        }
    }
}