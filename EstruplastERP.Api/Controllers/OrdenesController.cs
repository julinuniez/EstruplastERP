using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

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

        // Método auxiliar simplificado para buscar el material del cliente
        private async Task<int> ObtenerMaterialClienteAsync(int clienteId, int productoTerminadoId)
        {
            // Lógica: Buscar cualquier materia prima que pertenezca a este cliente.
            // Asumimos que cada cliente de fazón tiene UN material asignado (su "bolsa").
            // Si en el futuro tienen varios, aquí habría que filtrar por tipo de material también.
            var material = await _context.Productos
                .Where(p => p.EsMateriaPrima && p.ClienteId == clienteId)
                .FirstOrDefaultAsync();

            if (material != null) return material.Id;
            return 999;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenProduccion>>> GetOrdenes()
        {
            return await _context.Ordenes
                .Include(o => o.Producto).Include(o => o.Cliente).Include(o => o.Empleado)
                .OrderByDescending(o => o.FechaCreacion).ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenProduccion>> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Consumos).ThenInclude(c => c.MateriaPrima)
                .FirstOrDefaultAsync(o => o.Id == id);
            if (orden == null) return NotFound();
            return orden;
        }

        // GET: api/Ordenes/rango?desde=...&hasta=...
        [HttpGet("rango")]
        public async Task<ActionResult> GetProduccionPorRango(DateTime desde, DateTime hasta)
        {
            DateTime hastaFinDia = hasta.Date.AddDays(1).AddTicks(-1);

            var lista = await _context.Ordenes
                .Include(o => o.Empleado)
                .Include(o => o.Producto)
                .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hastaFinDia)
                .OrderByDescending(o => o.FechaCreacion)
                .Select(o => new
                {
                    o.Id,
                    Fecha = o.FechaCreacion,
                    o.Turno,
                    Operario = o.Empleado != null ? o.Empleado.NombreCompleto : "Sin Asignar",
                    Producto = o.Producto != null ? o.Producto.Nombre : "Producto Eliminado",
                    Lote = "L-" + o.Id.ToString(),
                    o.Cantidad,
                    Kilos = o.KilosEstimados,
                    estado = (int)o.Estado
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost]
        public async Task<ActionResult<OrdenProduccion>> PostOrden(CrearOrdenDto dto)
        {
            if (dto.Kilos <= 0) return BadRequest("Los kilos deben ser mayores a 0.");

            // 🔥 VALIDACIÓN DE SEGURIDAD (MASTERBATCH GENÉRICO ID 22)
            if (dto.Consumos != null && dto.Consumos.Any(c => c.MateriaPrimaId == 22))
            {
                return BadRequest("⛔ ERROR DE CALIDAD: La orden contiene 'Masterbatch Color Genérico' (ID 22). Debe especificar el color real (Ej: MB Rojo, MB Azul) antes de confirmar.");
            }

            var orden = new OrdenProduccion
            {
                ProductoId = dto.ProductoTerminadoId,
                ClienteId = dto.ClienteId,
                Cantidad = dto.Cantidad,
                EmpleadoId = dto.EmpleadoId,
                Turno = dto.Turno,
                Observacion = dto.Observacion,
                KilosEstimados = dto.Kilos,
                Estado = EstadoOrden.Pendiente,
                FechaCreacion = DateTime.Now,
                Consumos = new List<ConsumoOrden>()
            };

            Cliente? cliente = null;
            if (dto.ClienteId.HasValue)
                cliente = await _context.Clientes.FindAsync(dto.ClienteId.Value);

            if (dto.Consumos != null)
            {
                foreach (var c in dto.Consumos)
                {
                    int idReal = c.MateriaPrimaId;

                    // 🔥 Lógica Fazón: Si es el genérico (999) y hay cliente, buscamos su material
                    if (c.MateriaPrimaId == 999 && cliente != null)
                    {
                        idReal = await ObtenerMaterialClienteAsync(cliente.Id, dto.ProductoTerminadoId);
                    }

                    // Validación de Stock
                    var insumoDb = await _context.Productos.FindAsync(idReal);
                    if (insumoDb == null) return BadRequest($"Insumo ID {idReal} no encontrado.");

                    if (insumoDb.StockActual < c.CantidadKilos)
                    {
                        return BadRequest($"STOCK INSUFICIENTE: '{insumoDb.Nombre}'. Disp: {insumoDb.StockActual:N2} Kg. Req: {c.CantidadKilos:N2} Kg.");
                    }

                    // Descuento
                    insumoDb.StockActual -= c.CantidadKilos;

                    // Agregar consumo a la orden
                    orden.Consumos.Add(new ConsumoOrden { MateriaPrimaId = idReal, CantidadKilos = c.CantidadKilos });
                }
            }

            _context.Ordenes.Add(orden);
            await _context.SaveChangesAsync();

            var respuestaSegura = new
            {
                orden.Id,
                orden.FechaCreacion,
                orden.Cantidad,
                orden.KilosEstimados,
                orden.Estado,
                Mensaje = "Orden creada exitosamente."
            };

            return CreatedAtAction("GetOrden", new { id = orden.Id }, respuestaSegura);
        }

        [HttpPost("finalizar/{id}")]
        public async Task<IActionResult> FinalizarOrden(int id, [FromBody] FinalizarOrdenDto request)
        {
            var orden = await _context.Ordenes.Include(o => o.Producto).Include(o => o.Cliente).FirstOrDefaultAsync(o => o.Id == id);
            if (orden == null) return NotFound("No existe la orden.");
            if (orden.Estado == EstadoOrden.Finalizada) return BadRequest("Ya finalizada.");

            // Procesar Adiciones (Ajustes de Stock al finalizar)
            if (request.Adiciones != null)
            {
                foreach (var item in request.Adiciones)
                {
                    int idReal = item.MateriaPrimaId;

                    // 🔥 Lógica Fazón también para adiciones
                    if (item.MateriaPrimaId == 999 && orden.Cliente != null)
                        idReal = await ObtenerMaterialClienteAsync(orden.Cliente.Id, orden.ProductoId);

                    var insumo = await _context.Productos.FindAsync(idReal);
                    if (insumo != null)
                    {
                        if (insumo.StockActual < item.Cantidad)
                            return BadRequest($"No hay stock suficiente de '{insumo.Nombre}' para el ajuste final.");

                        insumo.StockActual -= item.Cantidad;

                        // Registramos el movimiento de ajuste
                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = idReal,
                            Cantidad = -item.Cantidad,
                            TipoMovimiento = "Ajuste Producción",
                            Turno = orden.Turno,
                            EmpleadoId = orden.EmpleadoId,
                            ClienteId = orden.ClienteId,
                            Observacion = $"Ajuste OP#{id}: {item.Motivo}"
                        });
                    }
                }
            }

            if (request.Desperdicio > 0) orden.Observacion += $" | ⚠️ Scrap: {request.Desperdicio} Kg";
            if (!string.IsNullOrEmpty(request.Observacion)) orden.Observacion += $" | Cierre: {request.Observacion}";

            orden.Estado = EstadoOrden.Finalizada;
            orden.FechaFin = DateTime.Now;

            // Ingreso del Producto Terminado
            if (orden.Producto != null)
            {
                orden.Producto.StockActual += orden.Cantidad;
                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = orden.ProductoId,
                    Cantidad = orden.Cantidad,
                    TipoMovimiento = "Producción",
                    Turno = orden.Turno,
                    EmpleadoId = orden.EmpleadoId,
                    ClienteId = orden.ClienteId,
                    Observacion = $"Ingreso OP #{id}"
                });
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Orden finalizada." });
        }
    }
}