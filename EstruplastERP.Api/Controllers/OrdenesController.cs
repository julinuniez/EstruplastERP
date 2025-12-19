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

        private (string Codigo, string Nombre) DeterminarTipoMaterial(int productoTerminadoId)
        {
            switch (productoTerminadoId)
            {
                // --- A.I. (ALTO IMPACTO) ---
                case 900: // Fino
                case 901: // Grueso
                case 902: // Fino Color
                case 903: // Grueso Color
                case 904: // Bicapa
                case 905: // Tricapa
                case 906: // Tutti Fino
                case 907: // Tutti Grueso
                    return ("AI", "ALTO IMPACTO (A.I.)");

                // --- ABS ---
                case 908: // ABS Grueso
                case 200:
                case 201:
                case 202: // Por si usas IDs viejos de ABS
                    return ("ABS", "ABS");

                // --- POLIPROPILENO / POLIETILENO ---
                case 909: // Poli Fino
                case 910: // Poli Grueso
                case 300:
                case 301: // IDs viejos PP
                    return ("POLI", "PEAD/PP/BIO");

                // --- PEAD BICAPA ---
                case 911:
                case 402:
                    return ("PEAD", "PEAD");

                // --- DEFAULT (ERROR: Cae aquí si el ID es nuevo) ---
                default:
                    return ("GEN-ERROR", "MATERIAL S/D (REVISAR ID)");
            }
        }

        private async Task<int> ObtenerMaterialClienteAsync(int clienteId, string nombreCliente, int productoTerminadoId)
        {
            var tipoMaterial = DeterminarTipoMaterial(productoTerminadoId);
            string codigoSku = $"MP-CLI-{clienteId}-{tipoMaterial.Codigo}";

            var productoCliente = await _context.Productos
                .FirstOrDefaultAsync(p => p.CodigoSku == codigoSku);

            if (productoCliente != null)
            {
                return productoCliente.Id;
            }

            var nuevoMaterial = new Producto
            {
                Nombre = $"MP {tipoMaterial.Nombre} - PROPIEDAD DE {nombreCliente.ToUpper()}",
                CodigoSku = codigoSku,
                EsMateriaPrima = true,
                EsProductoTerminado = false,
                EsGenerico = false,
                EsFazon = false,
                PesoEspecifico = 1.05m,
                StockActual = 0,
                StockMinimo = 0,
                PrecioCosto = 0,
                Activo = true,
                FechaCreacion = DateTime.Now,
                ClienteId = clienteId
            };

            _context.Productos.Add(nuevoMaterial);
            await _context.SaveChangesAsync();

            return nuevoMaterial.Id;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenProduccion>>> GetOrdenes()
        {
            return await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Cliente)
                .Include(o => o.Empleado)
                .OrderByDescending(o => o.FechaCreacion)
                .ToListAsync();
        }

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
                    Producto = o.Producto.Nombre,
                    Lote = "L-" + o.Id.ToString(),
                    o.Cantidad,
                    Kilos = o.KilosEstimados,
                    estado = (int)o.Estado
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenProduccion>> GetOrden(int id)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Consumos)
                    .ThenInclude(c => c.MateriaPrima)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null) return NotFound();

            return orden;
        }

        [HttpPost]
        public async Task<ActionResult<OrdenProduccion>> PostOrden(CrearOrdenDto dto)
        {
            if (dto.Kilos <= 0) return BadRequest("Los kilos totales deben ser mayores a 0.");

            var nuevaOrden = new OrdenProduccion
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
                FechaFin = null
            };

            Cliente? clienteOrden = null;
            if (dto.ClienteId.HasValue)
            {
                clienteOrden = await _context.Clientes.FindAsync(dto.ClienteId.Value);
            }

            if (dto.Consumos != null && dto.Consumos.Count > 0)
            {
                foreach (var consumoDto in dto.Consumos)
                {
                    int idFinalMateriaPrima = consumoDto.MateriaPrimaId;

                    if (consumoDto.MateriaPrimaId == 999 && clienteOrden != null)
                    {
                        idFinalMateriaPrima = await ObtenerMaterialClienteAsync(
                            clienteOrden.Id,
                            clienteOrden.RazonSocial,
                            dto.ProductoTerminadoId
                        );
                    }

                    nuevaOrden.Consumos.Add(new ConsumoOrden
                    {
                        MateriaPrimaId = idFinalMateriaPrima,
                        CantidadKilos = consumoDto.CantidadKilos
                    });

                    var insumoDb = await _context.Productos.FindAsync(idFinalMateriaPrima);
                    if (insumoDb != null)
                    {
                        insumoDb.StockActual -= consumoDto.CantidadKilos;
                    }
                }
            }

            _context.Ordenes.Add(nuevaOrden);
            await _context.SaveChangesAsync();

            var resultadoSeguro = new
            {
                nuevaOrden.Id,
                nuevaOrden.FechaCreacion,
                nuevaOrden.Cantidad,
                mensaje = "Orden creada exitosamente y stock descontado."
            };

            return CreatedAtAction("GetOrden", new { id = nuevaOrden.Id }, resultadoSeguro);
        }

        [HttpPost("finalizar/{id}")]
        public async Task<IActionResult> FinalizarOrden(int id, [FromBody] FinalizarOrdenDto request)
        {
            var orden = await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null) return NotFound("Orden no encontrada.");
            if (orden.Estado == EstadoOrden.Finalizada) return BadRequest("Orden ya finalizada.");

            if (request.Adiciones != null && request.Adiciones.Count > 0)
            {
                foreach (var item in request.Adiciones)
                {
                    int idInsumoReal = item.MateriaPrimaId;

                    if (item.MateriaPrimaId == 999 && orden.Cliente != null)
                    {
                        idInsumoReal = await ObtenerMaterialClienteAsync(
                            orden.Cliente.Id,
                            orden.Cliente.RazonSocial,
                            orden.ProductoId
                        );
                    }

                    var insumo = await _context.Productos.FindAsync(idInsumoReal);

                    if (insumo != null)
                    {
                        if (insumo.StockActual < item.Cantidad)
                        {
                            return BadRequest($"Stock insuficiente: '{insumo.Nombre}'. Tienes {insumo.StockActual}, necesitas {item.Cantidad}.");
                        }

                        insumo.StockActual -= item.Cantidad;

                        var movAjuste = new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = idInsumoReal,
                            Cantidad = -item.Cantidad,
                            TipoMovimiento = "Ajuste Producción",
                            Turno = orden.Turno,
                            EmpleadoId = orden.EmpleadoId,
                            ClienteId = orden.ClienteId,
                            Observacion = $"Ajuste Cierre OP#{id}: {item.Motivo}",
                            PrecioUnitario = 0,
                            PrecioTotal = 0
                        };
                        _context.Movimientos.Add(movAjuste);
                    }
                }
            }

            if (request.Desperdicio > 0)
            {
                string textoDesperdicio = $" | ⚠️ Scrap: {request.Desperdicio} Kg";
                if (string.IsNullOrEmpty(orden.Observacion)) orden.Observacion = textoDesperdicio;
                else orden.Observacion += textoDesperdicio;
            }

            orden.Estado = EstadoOrden.Finalizada;
            orden.FechaFin = DateTime.Now;

            if (!string.IsNullOrEmpty(request.Observacion))
            {
                orden.Observacion += $" | Cierre: {request.Observacion}";
            }

            if (orden.Producto != null)
            {
                orden.Producto.StockActual += orden.Cantidad;

                var movIngreso = new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = orden.ProductoId,
                    Cantidad = orden.Cantidad,
                    TipoMovimiento = "Producción",
                    Turno = orden.Turno,
                    EmpleadoId = orden.EmpleadoId,
                    ClienteId = orden.ClienteId,
                    Observacion = $"Ingreso OP #{id}",
                    PrecioUnitario = 0,
                    PrecioTotal = 0
                };
                _context.Movimientos.Add(movIngreso);
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Orden finalizada correctamente." });
        }
    }
}