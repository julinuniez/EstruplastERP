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

        // 🔥 CORRECCIÓN CLAVE: SEPARAMOS LOS CÓDIGOS
        private (string Codigo, string Nombre) DeterminarTipoMaterial(int productoTerminadoId)
        {
            switch (productoTerminadoId)
            {
                // --- A.I. FINO ---
                case 900: // Fazón AI Fino
                case 902: // Fazón AI Fino Color
                case 906: // Fazón AI Tutti Fino
                    return ("AI-FIN", "A.I. FINO (FAZÓN)");

                // --- A.I. GRUESO ---
                case 901: // Fazón AI Grueso
                case 903: // Fazón AI Grueso Color
                case 907: // Fazón AI Tutti Grueso
                    return ("AI-GRU", "A.I. GRUESO (FAZÓN)");

                // --- A.I. ESPECIALES ---
                case 904: return ("AI-BIC", "A.I. BICAPA (FAZÓN)");
                case 905: return ("AI-TRI", "A.I. TRICAPA (FAZÓN)");

                // --- ABS ---
                case 908: return ("ABS-GRU", "ABS GRUESO (FAZÓN)");

                // --- POLI / PP ---
                case 909: return ("POLI-FIN", "PEAD/PP/BIO FINO (FAZÓN)");
                case 910: return ("POLI-GRU", "PEAD/PP/BIO GRUESO (FAZÓN)");

                // --- PEAD ---
                case 911: return ("PEAD-BIC", "PEAD BICAPA (FAZÓN)");

                default: return ("GEN", "GENÉRICO");
            }
        }

        private async Task<int> ObtenerMaterialClienteAsync(int clienteId, string nombreCliente, int productoTerminadoId)
        {
            var tipo = DeterminarTipoMaterial(productoTerminadoId);

            // SKU Único: MP-CLI-{ID}-{CODIGO} (Ej: MP-CLI-10-AI-BIC)
            string sku = $"MP-CLI-{clienteId}-{tipo.Codigo}";

            var prod = await _context.Productos.FirstOrDefaultAsync(p => p.CodigoSku == sku);

            if (prod != null) return prod.Id;

            // Creación automática si no existe
            var nuevo = new Producto
            {
                Nombre = $"MP {tipo.Nombre} - PROPIEDAD DE {nombreCliente.ToUpper()}",
                CodigoSku = sku,
                EsMateriaPrima = true,
                EsFazon = false,
                EsProductoTerminado = false,
                EsGenerico = false,
                PesoEspecifico = 1.05m,
                StockActual = 0,
                StockMinimo = 0,
                PrecioCosto = 0,
                Activo = true,
                FechaCreacion = DateTime.Now,
                ClienteId = clienteId
            };
            _context.Productos.Add(nuevo);
            await _context.SaveChangesAsync();
            return nuevo.Id;
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
            // Ajustamos 'hasta' para que incluya todo el día (23:59:59)
            DateTime hastaFinDia = hasta.Date.AddDays(1).AddTicks(-1);

            var lista = await _context.Ordenes
                .Include(o => o.Empleado)
                .Include(o => o.Producto)
                .Where(o => o.FechaCreacion >= desde && o.FechaCreacion <= hastaFinDia)
                .OrderByDescending(o => o.FechaCreacion)
                // 🔥 PROYECCIÓN ANÓNIMA (DTO) PARA EVITAR CICLOS JSON
                .Select(o => new
                {
                    o.Id,
                    Fecha = o.FechaCreacion,
                    o.Turno,
                    // Validamos nulos para que no rompa
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
                FechaCreacion = DateTime.Now
            };

            Cliente? cliente = null;
            if (dto.ClienteId.HasValue)
                cliente = await _context.Clientes.FindAsync(dto.ClienteId.Value);

            if (dto.Consumos != null)
            {
                foreach (var c in dto.Consumos)
                {
                    int idReal = c.MateriaPrimaId;

                    // Lógica Fazón
                    if (c.MateriaPrimaId == 999 && cliente != null)
                    {
                        idReal = await ObtenerMaterialClienteAsync(cliente.Id, cliente.RazonSocial, dto.ProductoTerminadoId);
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

            // 🔥 CORRECCIÓN DEL ERROR DE CICLO 🔥
            // En lugar de devolver 'orden' (que tiene ciclos), creamos una respuesta limpia y segura.
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

            if (request.Adiciones != null)
            {
                foreach (var item in request.Adiciones)
                {
                    int idReal = item.MateriaPrimaId;
                    if (item.MateriaPrimaId == 999 && orden.Cliente != null)
                        idReal = await ObtenerMaterialClienteAsync(orden.Cliente.Id, orden.Cliente.RazonSocial, orden.ProductoId);

                    var insumo = await _context.Productos.FindAsync(idReal);
                    if (insumo != null)
                    {
                        if (insumo.StockActual < item.Cantidad)
                            return BadRequest($"No hay stock suficiente de '{insumo.Nombre}' para el ajuste.");

                        insumo.StockActual -= item.Cantidad;
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