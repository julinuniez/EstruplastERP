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

        [HttpPost("verificar")]
        public async Task<IActionResult> VerificarStock([FromBody] NuevaOrdenDto request)
        {
            // 1. Buscamos la receta
            var receta = await _context.Formulas
                .Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId)
                .ToListAsync();

            if (!receta.Any())
            {
                // Si no hay receta, asumimos que no consume stock (o bloqueamos, según prefieras)
                return Ok(new { posible = true, mensaje = "⚠️ Sin receta definida (No descuenta stock)" });
            }

            // 2. Verificamos cada ingrediente
            foreach (var ingrediente in receta)
            {
                decimal consumoTotal = request.Cantidad * ingrediente.Cantidad;
                var materiaPrima = await _context.Productos.FindAsync(ingrediente.MateriaPrimaId);

                if (materiaPrima == null)
                    return Ok(new { posible = false, mensaje = $"❌ Error: Insumo {ingrediente.MateriaPrimaId} no existe" });

                if (materiaPrima.StockActual < consumoTotal)
                {
                    return Ok(new { posible = false, mensaje = $"❌ Falta {materiaPrima.Nombre} (Hay: {materiaPrima.StockActual})" });
                }
            }

            // 3. Si llega acá, hay stock suficiente
            return Ok(new { posible = true, mensaje = "✅ Stock Disponible para producción." });
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarProduccion([FromBody] NuevaOrdenDto request)
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
                    return BadRequest($"El producto '{productoTerminado.Nombre}' no tiene receta definida.");
                }

                // 3. Validar Stock de Insumos
                foreach (var ingrediente in receta)
                {
                    decimal consumoTotal = request.Cantidad * ingrediente.Cantidad;
                    var materiaPrima = await _context.Productos.FindAsync(ingrediente.MateriaPrimaId);

                    if (materiaPrima == null) return BadRequest($"Insumo ID {ingrediente.MateriaPrimaId} no existe.");

                    if (materiaPrima.StockActual < consumoTotal)
                    {
                        return BadRequest($"FALTA STOCK DE {materiaPrima.Nombre}. Necesitas: {consumoTotal} - Hay: {materiaPrima.StockActual}");
                    }
                }

                // 4. GUARDAR EN HISTORIAL (TABLA PRODUCCIONES)
                // Creamos la Producción manualmente con los datos del DTO
                var nuevaProduccion = new Produccion
                {
                    FechaRegistro = DateTime.Now,
                    ProductoTerminadoId = request.ProductoTerminadoId,
                    ClienteId = request.ClienteId,
                    EmpleadoId = request.EmpleadoId,
                    Cantidad = request.Cantidad,
                    Kilos = request.Kilos,
                    Turno = request.Turno,
                    Observacion = request.Observacion,
                    Lote = DateTime.Now.ToString("yyyyMMdd-HHmm")
                };

                _context.Producciones.Add(nuevaProduccion);
                await _context.SaveChangesAsync();

                // 5. MOVIMIENTOS DE STOCK
                // A. Sumar Stock Producto Terminado
                productoTerminado.StockActual += request.Cantidad;

                var movEntrada = new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = request.ProductoTerminadoId,
                    Cantidad = request.Cantidad,
                    TipoMovimiento = "PRODUCCION_ENTRADA",
                    Observacion = $"Orden #{nuevaProduccion.Id} - Turno {request.Turno}",
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
                            Observacion = $"Consumo Orden #{nuevaProduccion.Id}",
                            EmpleadoId = request.EmpleadoId,
                            Turno = request.Turno
                        };
                        _context.Movimientos.Add(movSalida);
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Producción registrada correctamente", id = nuevaProduccion.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Loguear el error interno real para que lo veas en la consola de Visual Studio
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Error interno: " + ex.Message);
            }
        }

        // GET: api/Produccion/hoy
        [HttpGet("hoy")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionDelDia()
        {
            // Buscamos lo producido HOY (Desde las 00:00 hasta ahora)
            var hoy = DateTime.Today;

            var lista = await _context.Producciones
                .Include(p => p.Producto)  // Traemos datos del producto
                .Include(p => p.Empleado)  // Traemos nombre del empleado
                .Where(p => p.FechaRegistro >= hoy)
                .OrderByDescending(p => p.FechaRegistro) // Lo más nuevo arriba
                .Select(p => new
                {
                    p.Id,
                    Fecha = p.FechaRegistro,
                    Producto = p.Producto.Nombre,
                    Cantidad = p.Cantidad,
                    // Calculamos kilos si no están guardados, o devolvemos 0
                    Kilos = p.Kilos, // Si tienes columna 'Kilos' en Produccion, pon: p.Kilos
                    Lote = p.Lote ?? "Sin lote", // Ajustaremos esto cuando hagamos la lógica de lotes
                    Operario = p.Empleado.NombreCompleto
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/Produccion/rango?desde=2023-01-01&hasta=2023-01-31
        [HttpGet("rango")]
        public async Task<ActionResult<IEnumerable<object>>> GetProduccionPorRango(DateTime desde, DateTime hasta)
        {
            // Ajustamos la fecha 'hasta' para que incluya todo el día (hasta las 23:59:59)
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

    public class NuevaOrdenDto
    {
        public int ProductoTerminadoId { get; set; }
        public int? ClienteId { get; set; }
        public int EmpleadoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Kilos { get; set; }
        public string? Turno { get; set; }
        public string? Observacion { get; set; }
    }
}