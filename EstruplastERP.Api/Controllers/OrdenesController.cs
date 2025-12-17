using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos; // Asegúrate de que este namespace exista

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OrdenesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdenesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Ordenes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenProduccion>>> GetOrdenes()
        {
            return await _context.Ordenes
                .Include(o => o.Producto) // Para mostrar el nombre del producto
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();
        }

        [HttpGet("rango")]
        public async Task<ActionResult> GetProduccionPorRango(DateTime desde, DateTime hasta)
        {
            DateTime hastaFinDia = hasta.Date.AddDays(1).AddTicks(-1);

            var lista = await _context.Ordenes
                .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hastaFinDia)
                .OrderByDescending(o => o.FechaCreacion)
                .Select(o => new
                {
                    o.Id,
                    Fecha = o.FechaCreacion,
                    o.Turno,
                    Operario = o.Empleado != null ? o.Empleado.NombreCompleto : "Sin Asignar",
                    Producto = o.Producto.Nombre,
                    Lote = "L-" + o.Id.ToString(),
                    o.Cantidad,
                    Kilos = o.KilosEstimados
                })
                .ToListAsync();

            return Ok(lista);
        }

        // GET: api/Ordenes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenProduccion>> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Consumos) // Incluimos la receta usada
                    .ThenInclude(c => c.MateriaPrima) // Y los nombres de las materias primas
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null) return NotFound();

            return orden;
        }

        // POST: api/Ordenes (Crear Planificación Compleja)
        [HttpPost]
        public async Task<ActionResult<OrdenProduccion>> PostOrden(CrearOrdenDto dto)
        {
            // 1. Validaciones
            if (dto.Kilos <= 0) return BadRequest("Los kilos totales deben ser mayores a 0.");

            // 2. Mapeo DTO -> Entidad
            var nuevaOrden = new OrdenProduccion
            {
                ProductoId = dto.ProductoTerminadoId, // Mapeamos el ID que viene del front
                ClienteId = dto.ClienteId,
                Cantidad = dto.Cantidad,

                // Datos nuevos del formulario avanzado
                EmpleadoId = dto.EmpleadoId,
                Turno = dto.Turno,
                Observacion = dto.Observacion,
                KilosEstimados = dto.Kilos,

                // Configuración automática
                Estado = EstadoOrden.Pendiente,
                FechaCreacion = DateTime.Now,
                FechaFin = null
            };

            // 3. Guardar los Consumos (La Receta calculada)
            if (dto.Consumos != null && dto.Consumos.Count > 0)
            {
                foreach (var consumoDto in dto.Consumos)
                {
                    nuevaOrden.Consumos.Add(new ConsumoOrden
                    {
                        MateriaPrimaId = consumoDto.MateriaPrimaId,
                        CantidadKilos = consumoDto.CantidadKilos
                    });
                }
            }

            // 4. Guardar en Base de Datos (EF guarda Orden + Consumos en una sola transacción)
            _context.Ordenes.Add(nuevaOrden);
            await _context.SaveChangesAsync();
            var resultadoSeguro = new
            {
                nuevaOrden.Id,
                nuevaOrden.FechaCreacion,
                nuevaOrden.Cantidad,
                mensaje = "Orden creada exitosamente"
            };

            // Devolvemos 'resultadoSeguro' en vez de 'nuevaOrden'
            return CreatedAtAction("GetOrden", new { id = nuevaOrden.Id }, resultadoSeguro);
        }

        // POST: api/Ordenes/finalizar/5
        [HttpPost("finalizar/{id}")]
        public async Task<IActionResult> FinalizarOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Producto)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null) return NotFound("Orden no encontrada.");

            // Validaciones de Estado (Usando el Enum)
            if (orden.Estado == EstadoOrden.Finalizada)
                return BadRequest("Esta orden ya fue finalizada anteriormente.");

            if (orden.Estado == EstadoOrden.Cancelada)
                return BadRequest("No puedes finalizar una orden cancelada.");

            // Actualizar Estado
            orden.Estado = EstadoOrden.Finalizada;
            orden.FechaFin = DateTime.Now;

            // IMPACTO EN STOCK PRODUCTO TERMINADO
            if (orden.Producto != null)
            {
                orden.Producto.StockActual += orden.Cantidad;
            }

            // OJO: Aquí deberíamos descontar el stock de Materia Prima?
            // Si quieres hacerlo automático, avísame y agregamos esa lógica aquí.
            // Por ahora solo suma el producto terminado.

            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Orden finalizada y stock actualizado." });
        }

        // POST: api/Ordenes/cancelar/5
        [HttpPost("cancelar/{id}")]
        public async Task<IActionResult> CancelarOrden(int id)
        {
            var orden = await _context.Ordenes.FindAsync(id);
            if (orden == null) return NotFound();

            if (orden.Estado == EstadoOrden.Finalizada)
                return BadRequest("No puedes cancelar una orden que ya se fabricó.");

            orden.Estado = EstadoOrden.Cancelada;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Orden cancelada." });
        }
    }
}