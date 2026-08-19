using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;
using EstruplastERP.Api.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class OrdenesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ProduccionService _produccionService;

        public OrdenesController(ApplicationDbContext context, ProduccionService produccionService)
        {
            _context = context;
            _produccionService = produccionService;
        }

        [HttpGet("recientes")]
        public async Task<ActionResult> GetOrdenesRecientes([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var query = _context.Ordenes
                .Include(o => o.Producto)
                    .ThenInclude(p => p.Formulas)
                .Include(o => o.Cliente)
                .Include(o => o.Pallets)
                .Include(o => o.Consumos)
                    .ThenInclude(c => c.MateriaPrima)
                        .ThenInclude(mp => mp.Cliente)
                .AsQueryable()
                .AsSplitQuery();

            int targetMes = mes ?? DateTime.Now.Month;
            int targetAnio = anio ?? DateTime.Now.Year;

            query = query.Where(o =>
                (o.Estado != EstadoOrden.Finalizada && o.Estado != EstadoOrden.Cancelada) ||
                (o.FechaCreacion.Month == targetMes && o.FechaCreacion.Year == targetAnio)
            );

            var lista = await query
                .OrderByDescending(o => o.FechaCreacion)
                .Select(o => new
                {
                    o.Id,
                    Fecha = o.FechaCreacion.ToString("dd/MM HH:mm"),
                    Producto = o.Producto != null ? o.Producto.Nombre : "Desconocido",
                    ProductoId = o.ProductoId,
                    NotaPedido = o.NotaPedido,
                    NumeroPedidoCliente = o.NumeroPedidoCliente,
                    ClienteId = o.ClienteId,
                    ClienteNombre = o.Cliente != null ? o.Cliente.RazonSocial : "STOCK / INTERNO",

                    // 🚀 ACÁ ESTÁ EL DATO VITAL PARA QUE VUE SEPA A CUÁNTOS KILOS CORTAR
                    LimiteKilosPallet = o.Cliente != null && o.Cliente.LimiteKilosPallet > 0 ? o.Cliente.LimiteKilosPallet : 1000m,

                    o.Observacion,
                    o.Largo,
                    o.Ancho,
                    o.Espesor,
                    o.Color,
                    o.Cantidad,
                    Kilos = o.KilosEstimados,
                    Desperdicio = o.Desperdicio,
                    EsBobina = o.EsBobina,
                    ConBrillo = o.ConBrillo,
                    LlevaFilm = o.LlevaFilm,
                    EsGofrado = o.EsGofrado,
                    TipoCorona = o.TipoCorona,
                    AditivoUV = o.AditivoUV,
                    EsImpreso = o.EsImpreso,
                    Estado = o.Estado.ToString(),
                    EsFinalizada = o.Estado == EstadoOrden.Finalizada,
                    HojaCargaId = o.HojaCargaId,
                    Pallets = o.Pallets.OrderBy(p => p.NumeroPallet).Select(p => new {
                        p.Id,
                        p.NumeroPallet,
                        p.Kilos,
                        p.Estado
                    }).ToList(),
                    Consumos = o.Consumos.Select(c => new {
                        c.MateriaPrimaId,
                        NombreMateriaPrima = c.MateriaPrima != null ? c.MateriaPrima.Nombre : "Insumo",
                        c.CantidadKilos,
                        ClienteId = c.MateriaPrima != null ? (c.MateriaPrima.ClienteId ?? 0) : 0,
                        ClienteNombre = (c.MateriaPrima != null && c.MateriaPrima.Cliente != null) ? c.MateriaPrima.Cliente.RazonSocial : "",
                        ExtrusoraDestino = o.Producto != null && o.Producto.Formulas != null
                            ? (o.Producto.Formulas.FirstOrDefault(f => f.MateriaPrimaId == c.MateriaPrimaId) != null
                                ? o.Producto.Formulas.FirstOrDefault(f => f.MateriaPrimaId == c.MateriaPrimaId).ExtrusoraDestino
                                : "UNICA")
                            : "UNICA"
                    }).ToList()
                })
                .ToListAsync();

            return Ok(lista);
        }

        [HttpPost]
        public async Task<ActionResult<OrdenProduccion>> PostOrden([FromBody] NuevaOrdenDto dto)
        {
            if (dto.Kilos <= 0)
                return BadRequest(new { mensaje = "Los kilos deben ser mayores a 0." });

            if (dto.Consumos != null && dto.Consumos.Any(c => c.MateriaPrimaId == 22))
                return BadRequest(new { mensaje = "Debe reemplazar el Masterbatch Genérico (ID 22) por un color real." });

            try
            {
                var orden = await _produccionService.RegistrarOrden(dto, true);
                return CreatedAtAction("GetOrden", new { id = orden.Id }, new { mensaje = "Orden registrada correctamente en Producción.", id = orden.Id });
            }
            catch (Exception ex)
            {
                string errorReal = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                return BadRequest(new { mensaje = $"Error al registrar: {errorReal}" });
            }
        }

        [HttpPut("modificar/{id}")]
        public async Task<IActionResult> ModificarOrdenRapida(int id, [FromBody] ModificarOrdenDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orden = await _context.Ordenes
                    .Include(o => o.Consumos)
                    .Include(o => o.Pallets) // 🚀 AHORA LEEMOS LOS PALLETS
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound("Orden no encontrada.");
                if (orden.Estado == EstadoOrden.Finalizada || orden.Estado == EstadoOrden.Cancelada)
                    return BadRequest("No se puede modificar una orden Finalizada o Cancelada.");
                if (orden.Estado == EstadoOrden.MaterialPreparado)
                    return BadRequest("No se puede modificar la receta de una orden que ya tiene su material descontado en una Hoja de Carga.");

                // 🚀 REGLAS DE SEGURIDAD PARA LOS PALLETS
                if (orden.Pallets != null && orden.Pallets.Any())
                {
                    // Si ya hay un pallet cerrado, PROHIBIDO editar (rompería el stock matemático)
                    if (orden.Pallets.Any(p => p.Estado == "Finalizada"))
                        return BadRequest("No se puede editar los kilos ni la receta porque ya hay pallets fabricados y descontados del stock. Si hubo un error, cancele o revierta la orden.");

                    // Si están todos pendientes, los borramos para que el operario tenga que volver a armar las cajas (📦) con los kilos nuevos
                    _context.PalletsProduccion.RemoveRange(orden.Pallets);
                }

                if (!dto.IgnorarStock)
                {
                    var mpIds = dto.RecetaNueva.Select(x => x.MateriaPrimaId).Distinct().ToList();

                    var productosDict = await _context.Productos
                        .Where(p => mpIds.Contains(p.Id))
                        .ToDictionaryAsync(p => p.Id);

                    var retenidosDict = await _context.Ordenes
                        .Where(o => o.Id != id && o.Estado != EstadoOrden.Finalizada && o.Estado != EstadoOrden.Cancelada)
                        .SelectMany(o => o.Consumos)
                        .Where(c => c.MateriaPrimaId.HasValue && mpIds.Contains(c.MateriaPrimaId.Value))
                        .GroupBy(c => c.MateriaPrimaId.Value)
                        .Select(g => new { MateriaPrimaId = g.Key, Total = g.Sum(c => (decimal?)c.CantidadKilos) ?? 0 })
                        .ToDictionaryAsync(x => x.MateriaPrimaId, x => x.Total);

                    foreach (var nuevoItem in dto.RecetaNueva)
                    {
                        if (!productosDict.TryGetValue(nuevoItem.MateriaPrimaId, out var mp)) continue;

                        decimal kilosRetenidosOtrasOrdenes = retenidosDict.TryGetValue(mp.Id, out var retenido) ? retenido : 0m;
                        decimal stockLibreParaEstaOrden = mp.StockActual - kilosRetenidosOtrasOrdenes;

                        if (nuevoItem.CantidadKilos > stockLibreParaEstaOrden)
                        {
                            return BadRequest($"Stock insuficiente de '{mp.Nombre}'. Requiere {nuevoItem.CantidadKilos}kg pero solo quedan {stockLibreParaEstaOrden}kg libres en planta.");
                        }
                    }
                }

                _context.RemoveRange(orden.Consumos);

                var nuevosConsumos = dto.RecetaNueva.Select(item => new ConsumoOrden
                {
                    MateriaPrimaId = item.MateriaPrimaId,
                    CantidadKilos = item.CantidadKilos
                }).ToList();

                orden.Largo = dto.Largo;
                orden.Ancho = dto.Ancho;
                orden.Espesor = dto.Espesor;
                orden.Cantidad = dto.Cantidad;
                orden.KilosEstimados = dto.KilosTotales;
                orden.Desperdicio = dto.Desperdicio;

                orden.ConBrillo = dto.ConBrillo;
                orden.LlevaFilm = dto.LlevaFilm;
                orden.EsGofrado = dto.EsGofrado;
                orden.AditivoUV = dto.AditivoUV;
                orden.TipoCorona = dto.TipoCorona;
                orden.Color = dto.Color;
                orden.NotaPedido = dto.NotaPedido;
                orden.NumeroPedidoCliente = dto.NumeroPedidoCliente;

                orden.Consumos = nuevosConsumos;

                orden.EsImpreso = false;
                orden.Estado = EstadoOrden.Pendiente;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Orden modificada con éxito. Los pallets pendientes se eliminaron para recalcularse." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPost("activar/{id}")]
        public async Task<IActionResult> ActivarOrden(int id)
        {
            try
            {
                var orden = await _context.Ordenes.Include(o => o.Consumos).FirstOrDefaultAsync(o => o.Id == id);
                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });

                var faltantes = new List<string>();
                var mpIds = orden.Consumos
                    .Where(c => c.MateriaPrimaId.HasValue)
                    .Select(c => c.MateriaPrimaId.Value)
                    .Distinct()
                    .ToList();

                var productosDict = await _context.Productos
                    .Where(p => mpIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                var retenidosDict = await _context.Ordenes
                    .Where(o => o.Id != id && o.Estado != EstadoOrden.Finalizada && o.Estado != EstadoOrden.Cancelada)
                    .SelectMany(o => o.Consumos)
                    .Where(c => c.MateriaPrimaId.HasValue && mpIds.Contains(c.MateriaPrimaId.Value))
                    .GroupBy(c => c.MateriaPrimaId.Value)
                    .Select(g => new { MateriaPrimaId = g.Key, Total = g.Sum(c => (decimal?)c.CantidadKilos) ?? 0 })
                    .ToDictionaryAsync(x => x.MateriaPrimaId, x => x.Total);

                foreach (var consumo in orden.Consumos)
                {
                    if (consumo.MateriaPrimaId.HasValue && productosDict.TryGetValue(consumo.MateriaPrimaId.Value, out var mp) && !(mp.Id >= 990 && mp.Id <= 999))
                    {
                        decimal retenidoPorOtras = retenidosDict.TryGetValue(mp.Id, out var retenido) ? retenido : 0m;
                        decimal libre = mp.StockActual - retenidoPorOtras;

                        if (libre < consumo.CantidadKilos)
                            faltantes.Add($"- {mp.Nombre}: Faltan {(consumo.CantidadKilos - libre):N2} kg (libres)");
                    }
                }

                if (faltantes.Any()) return BadRequest(new { mensaje = "Faltan materiales libres:\n" + string.Join("\n", faltantes) });

                orden.Estado = EstadoOrden.Pendiente;
                await _context.SaveChangesAsync();

                return Ok(new { mensaje = "Orden activada y enviada a Máquina." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al activar: {ex.Message}" });
            }
        }

        [HttpPost("confirmar/{id}")]
        public async Task<IActionResult> ConfirmarOrden(int id, [FromBody] ConfirmacionCierreDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orden = await _context.Ordenes
                    .Include(o => o.Producto)
                    .Include(o => o.Consumos)
                    .Include(o => o.Cliente) 
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
                if (orden.Estado == EstadoOrden.Finalizada) return BadRequest(new { mensaje = "Esta orden ya fue finalizada." });

                string nombreCliente = orden.Cliente != null ? orden.Cliente.RazonSocial : "FÁBRICA (PROPIO)";
                string numeroNP = string.IsNullOrWhiteSpace(orden.NotaPedido) ? "S/NP" : orden.NotaPedido;
                string etiquetaKardex = $"[NP: {numeroNP} | Cli: {nombreCliente}]";

                bool materialYaDescontado = orden.Estado == EstadoOrden.MaterialPreparado;
                DateTime fechaCierreReal = dto.FechaCierre ?? DateTime.Now;

                var consumosAgrupados = dto.ConsumosReales
                    .GroupBy(c => c.MateriaPrimaId)
                    .Select(g => new { MateriaPrimaId = g.Key, TotalKilos = g.Sum(c => c.CantidadKilosReales) })
                    .ToList();

                var mpIds = consumosAgrupados.Select(c => c.MateriaPrimaId).Distinct().ToList();
                var productosDict = await _context.Productos
                    .Where(p => mpIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id);

                var faltantes = new List<string>();

                foreach (var consumo in consumosAgrupados)
                {
                    if (productosDict.TryGetValue(consumo.MateriaPrimaId, out var mp) && !(mp.Id >= 990 && mp.Id <= 999))
                    {
                        if (mp.StockActual < consumo.TotalKilos)
                        {
                            faltantes.Add($"- {mp.Nombre}: Faltan {(consumo.TotalKilos - mp.StockActual):N2} Kg físicos en el sistema.");
                        }
                    }
                }

                if (faltantes.Any())
                {
                    return BadRequest(new { mensaje = "STOCK NEGATIVO DETECTADO.\nCargue los ingresos/remitos de estos materiales antes de cerrar la orden:\n\n" + string.Join("\n", faltantes) });
                }

                orden.KilosEstimados = dto.KilosProducidosReales;
                orden.Desperdicio = dto.DesperdicioReal;

                _context.RemoveRange(orden.Consumos);
                var consumosDefinitivos = new List<ConsumoOrden>();

                foreach (var consumoUsuario in dto.ConsumosReales)
                {
                    if (productosDict.TryGetValue(consumoUsuario.MateriaPrimaId, out var mp))
                    {
                        consumosDefinitivos.Add(new ConsumoOrden
                        {
                            MateriaPrimaId = mp.Id,
                            CantidadKilos = consumoUsuario.CantidadKilosReales
                        });

                        if (!(mp.Id >= 990 && mp.Id <= 999))
                        {
                            mp.StockActual -= consumoUsuario.CantidadKilosReales;

                            _context.Movimientos.Add(new Movimiento
                            {
                                Fecha = fechaCierreReal,
                                ProductoId = mp.Id,
                                Cantidad = -consumoUsuario.CantidadKilosReales,
                                TipoMovimiento = "CONSUMO_PRODUCCION",
                                // 🚀 NUEVO: Inyectamos la etiqueta en el consumo
                                Observacion = $"Cierre OP #{id} {etiquetaKardex}{(materialYaDescontado ? " (Adición Extra)" : "")}: {dto.Observacion}",
                                OrdenProduccionId = id
                            });
                        }
                    }
                }
                orden.Consumos = consumosDefinitivos;

                if (orden.Producto != null)
                {
                    orden.Producto.StockActual += dto.KilosProducidosReales;

                    _context.Movimientos.Add(new Movimiento
                    {
                        Fecha = fechaCierreReal,
                        ProductoId = orden.ProductoId,
                        Cantidad = dto.KilosProducidosReales,
                        TipoMovimiento = "PRODUCCION_TERMINADA",
                        Observacion = $"Cierre OP #{id} {etiquetaKardex} {(materialYaDescontado ? "(Desde Mezcla)" : "")}",
                        OrdenProduccionId = id
                    });
                }

                orden.Estado = EstadoOrden.Finalizada;
                orden.FechaFin = fechaCierreReal;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Producción finalizada correctamente. Inventario actualizado." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = $"Error al confirmar: {ex.Message}" });
            }
        }

        [HttpPost("cancelar/{id}")]
        public async Task<IActionResult> CancelarOrden(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orden = await _context.Ordenes
                    .Include(o => o.Pallets)
                    .Include(o => o.Consumos)
                    .Include(o => o.Producto)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
                if (orden.Estado == EstadoOrden.Finalizada) return BadRequest(new { mensaje = "No se puede cancelar una orden ya terminada. Use 'Revertir'." });

                // 🚀 SI TIENE PALLETS: Revertimos los finalizados y borramos todos
                if (orden.Pallets != null && orden.Pallets.Any())
                {
                    var palletsFinalizados = orden.Pallets.Where(p => p.Estado == "Finalizada").ToList();

                    if (palletsFinalizados.Any())
                    {
                        // 1. Devolver Producto Terminado al restarlo del stock
                        decimal totalKilosARestar = palletsFinalizados.Sum(p => p.Kilos);
                        if (orden.Producto != null)
                        {
                            orden.Producto.StockActual -= totalKilosARestar;
                            _context.Movimientos.Add(new Movimiento
                            {
                                Fecha = DateTime.Now,
                                ProductoId = orden.ProductoId,
                                Cantidad = -totalKilosARestar,
                                TipoMovimiento = "CANCELACION_PRODUCCION",
                                Observacion = $"Restauración por cancelación de OP #{id}",
                                OrdenProduccionId = id
                            });
                        }

                        // 2. Devolver Materias Primas (usando la receta original de la OP)
                        // Calculamos la proporción total que representaban esos pallets sobre la orden
                        decimal proporcionTotal = totalKilosARestar / orden.KilosEstimados;

                        var mpIds = orden.Consumos.Select(c => c.MateriaPrimaId).Where(id => id.HasValue).Cast<int>().ToList();
                        var productosDict = await _context.Productos.Where(p => mpIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

                        foreach (var consumo in orden.Consumos)
                        {
                            if (consumo.MateriaPrimaId.HasValue && productosDict.TryGetValue(consumo.MateriaPrimaId.Value, out var mp))
                            {
                                if (!(mp.Id >= 990 && mp.Id <= 999))
                                {
                                    // Calculamos cuánto material se había "gastado" teóricamente
                                    // Ojo: Usamos la receta original (CantidadKilos + lo que ya se descontó si achicamos reserva)
                                    // Para simplificar, en cancelaciones totales devolvemos la parte proporcional del total estimado
                                    decimal kilosADevolver = (orden.KilosEstimados * (consumo.CantidadKilos / orden.KilosEstimados)) * proporcionTotal;

                                    mp.StockActual += kilosADevolver;
                                    _context.Movimientos.Add(new Movimiento
                                    {
                                        Fecha = DateTime.Now,
                                        ProductoId = mp.Id,
                                        Cantidad = kilosADevolver,
                                        TipoMovimiento = "CANCELACION_CONSUMO",
                                        Observacion = $"Devolución material por cancelación OP #{id}",
                                        OrdenProduccionId = id
                                    });
                                }
                            }
                        }
                    }

                    // 3. Borrar todos los pallets asociados (pendientes y finalizados)
                    _context.PalletsProduccion.RemoveRange(orden.Pallets);
                }

                orden.Estado = EstadoOrden.Cancelada;
                orden.FechaFin = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Orden cancelada. Se han revertido los pallets finalizados y restaurado el stock." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = $"Error al cancelar: {ex.Message}" });
            }
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

        [HttpPost("revertir/{id}")]
        public async Task<IActionResult> RevertirOrden(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orden = await _context.Ordenes
                    .Include(o => o.Producto)
                    .Include(o => o.Consumos)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });

                bool vinoDeHojaCarga = orden.HojaCargaId.HasValue;

                if (orden.Estado == EstadoOrden.Finalizada)
                {
                    if (orden.Producto != null)
                    {
                        orden.Producto.StockActual -= orden.KilosEstimados;

                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = orden.ProductoId,
                            Cantidad = -orden.KilosEstimados,
                            TipoMovimiento = "REVERSION_PRODUCCION",
                            Observacion = $"Reversión de cierre por error humano - OP #{id}",
                            OrdenProduccionId = id
                        });
                    }

                    if (!vinoDeHojaCarga)
                    {
                        var mpIds = orden.Consumos
                            .Where(c => c.MateriaPrimaId.HasValue)
                            .Select(c => c.MateriaPrimaId.Value)
                            .Distinct()
                            .ToList();

                        var productosDict = await _context.Productos
                            .Where(p => mpIds.Contains(p.Id))
                            .ToDictionaryAsync(p => p.Id);

                        foreach (var consumo in orden.Consumos)
                        {
                            if (consumo.MateriaPrimaId.HasValue && productosDict.TryGetValue(consumo.MateriaPrimaId.Value, out var mp) && !(mp.Id >= 990 && mp.Id <= 999))
                            {
                                mp.StockActual += consumo.CantidadKilos;

                                _context.Movimientos.Add(new Movimiento
                                {
                                    Fecha = DateTime.Now,
                                    ProductoId = mp.Id,
                                    Cantidad = consumo.CantidadKilos,
                                    TipoMovimiento = "REVERSION_CONSUMO",
                                    Observacion = $"Reversión de consumo por error humano - OP #{id}",
                                    OrdenProduccionId = id
                                });
                            }
                        }
                    }

                    orden.Estado = vinoDeHojaCarga ? EstadoOrden.MaterialPreparado : EstadoOrden.Pendiente;
                    orden.FechaFin = null;
                }

                orden.EsImpreso = false;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Orden revertida exitosamente. El inventario fue restaurado según su origen." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = $"Error al revertir la orden: {ex.Message}" });
            }
        }

        [HttpPost("registrar-hoja-carga")]
        public async Task<IActionResult> RegistrarHojaCarga([FromBody] List<int> ordenesIds)
        {
            if (ordenesIds == null || !ordenesIds.Any())
                return BadRequest(new { mensaje = "No se enviaron órdenes." });

            string sufijo = Guid.NewGuid().ToString().Substring(0, 3).ToUpper();
            string codigoHC = $"HC-{DateTime.Now:ddMM-HHmm}-{sufijo}";
            string etiqueta = $"[Grupo: {codigoHC}]";

            var nuevaHoja = new HojaCarga
            {
                CodigoLote = codigoHC,
                FechaCreacion = DateTime.Now,
                Estado = EstadoHojaCarga.Pendiente
            };

            _context.HojasCarga.Add(nuevaHoja);
            await _context.SaveChangesAsync();

            var ordenes = await _context.Ordenes
                .Where(o => ordenesIds.Contains(o.Id))
                .ToListAsync();

            foreach (var o in ordenes)
            {
                o.HojaCargaId = nuevaHoja.Id;

                if (!string.IsNullOrWhiteSpace(o.Observacion))
                {
                    o.Observacion = System.Text.RegularExpressions.Regex.Replace(o.Observacion, @"\[Grupo: HC-[^\]]+\]", "").Trim();
                }

                o.Observacion = string.IsNullOrWhiteSpace(o.Observacion) ? etiqueta : o.Observacion + " " + etiqueta;
            }

            await _context.SaveChangesAsync();
            return Ok(new { codigo = codigoHC, mensaje = "Hoja de carga registrada y guardada en base de datos." });
        }

        [HttpPost("marcar-impresa/{id}")]
        public async Task<IActionResult> MarcarComoImpresa(int id)
        {
            var orden = await _context.Ordenes.FindAsync(id);
            if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });

            orden.EsImpreso = true;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Orden marcada como impresa." });
        }

        public class PalletDesgloseDto
        {
            public int Numero { get; set; }
            public decimal Kilos { get; set; }
        }

        [HttpPost("{id}/desglose")]
        public async Task<IActionResult> GuardarDesglose(int id, [FromBody] List<PalletDesgloseDto> palletsRecibidos)
        {
            // Usamos una transacción para blindar la operación
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orden = await _context.Ordenes
                    .Include(o => o.Pallets)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
                if (orden.Estado == EstadoOrden.Finalizada || orden.Estado == EstadoOrden.Cancelada)
                    return BadRequest(new { mensaje = "No se puede desglosar una orden terminada o cancelada." });

                // Si ya hay pallets físicos descontados, bloqueamos
                if (orden.Pallets.Any(p => p.Estado == "Finalizada"))
                    return BadRequest(new { mensaje = "Ya hay pallets cerrados. No se puede modificar el desglose base." });

                // 🚀 BORRADO DE SEGURIDAD: Limpiamos los anteriores y guardamos ANTES de insertar los nuevos
                _context.PalletsProduccion.RemoveRange(orden.Pallets);
                await _context.SaveChangesAsync();

                // Insertamos los nuevos pallets limpios
                foreach (var p in palletsRecibidos)
                {
                    _context.PalletsProduccion.Add(new PalletProduccion
                    {
                        OrdenProduccionId = id,
                        NumeroPallet = p.Numero,
                        Kilos = p.Kilos,
                        Estado = "Pendiente"
                    });
                }

                // Guardado final y confirmación
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Desglose guardado correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = $"Error al guardar el desglose: {ex.Message}" });
            }
        }

        [HttpPost("finalizar-pallet/{idPallet}")]
        public async Task<IActionResult> FinalizarPallet(int idPallet)
        {
            try
            {
                var pallet = await _context.PalletsProduccion
                    .Include(p => p.OrdenProduccion).ThenInclude(o => o.Consumos)
                    .Include(p => p.OrdenProduccion).ThenInclude(o => o.Producto)
                    .Include(p => p.OrdenProduccion).ThenInclude(o => o.Pallets)
                    .FirstOrDefaultAsync(p => p.Id == idPallet);

                if (pallet == null) return NotFound(new { mensaje = "Pallet no encontrado." });
                if (pallet.Estado == "Finalizada") return BadRequest(new { mensaje = "Este pallet ya fue descontado del stock." });

                var orden = pallet.OrdenProduccion;
                if (orden.KilosEstimados <= 0) return BadRequest(new { mensaje = "La orden no tiene kilos válidos para calcular la proporción." });

                decimal kilosYaFabricados = orden.Pallets.Where(p => p.Estado == "Finalizada").Sum(p => p.Kilos);
                decimal kilosPendientes = orden.KilosEstimados - kilosYaFabricados;

                if (kilosPendientes <= 0) return BadRequest(new { mensaje = "Error matemático: La orden ya no tiene kilos pendientes para procesar." });

                decimal proporcion = pallet.Kilos / kilosPendientes;
                DateTime fechaCierre = DateTime.Now;

                var mpIds = orden.Consumos.Select(c => c.MateriaPrimaId).Where(id => id.HasValue).Cast<int>().ToList();
                var productosDict = await _context.Productos.Where(p => mpIds.Contains(p.Id)).ToDictionaryAsync(p => p.Id);

                // ==========================================================
                // 🚀 1. VALIDACIÓN ESTRICTA DE STOCK (ANTI-NEGATIVOS)
                // ==========================================================
                var faltantes = new List<string>();

                foreach (var consumo in orden.Consumos)
                {
                    if (consumo.MateriaPrimaId.HasValue && productosDict.TryGetValue(consumo.MateriaPrimaId.Value, out var mp))
                    {
                        if (!(mp.Id >= 990 && mp.Id <= 999)) // Ignorar los genéricos
                        {
                            decimal kilosRequeridos = consumo.CantidadKilos * proporcion;

                            if (mp.StockActual < kilosRequeridos)
                            {
                                faltantes.Add($"- {mp.Nombre}: Faltan {(kilosRequeridos - mp.StockActual):N2} Kg físicos en sistema.");
                            }
                        }
                    }
                }

                if (faltantes.Any())
                {
                    return BadRequest(new { mensaje = "STOCK NEGATIVO DETECTADO.\nSi el pallet existe físicamente, registre primero el ingreso del material:\n\n" + string.Join("\n", faltantes) });
                }

                // ==========================================================
                // 🚀 2. SI HAY STOCK, PROCEDEMOS A DESCONTAR
                // ==========================================================
                foreach (var consumo in orden.Consumos)
                {
                    if (consumo.MateriaPrimaId.HasValue && productosDict.TryGetValue(consumo.MateriaPrimaId.Value, out var mp))
                    {
                        if (!(mp.Id >= 990 && mp.Id <= 999))
                        {
                            decimal kilosADescontar = consumo.CantidadKilos * proporcion;

                            mp.StockActual -= kilosADescontar;
                            consumo.CantidadKilos -= kilosADescontar; // Achicar la reserva

                            _context.Movimientos.Add(new Movimiento
                            {
                                Fecha = fechaCierre,
                                ProductoId = mp.Id,
                                Cantidad = -kilosADescontar,
                                TipoMovimiento = "CONSUMO_PARCIAL",
                                Observacion = $"Consumo parcial por Pallet N° {pallet.NumeroPallet} (OP #{orden.Id})",
                                OrdenProduccionId = orden.Id
                            });
                        }
                    }
                }

                // Sumamos el Producto Terminado
                if (orden.Producto != null)
                {
                    orden.Producto.StockActual += pallet.Kilos;
                    _context.Movimientos.Add(new Movimiento
                    {
                        Fecha = fechaCierre,
                        ProductoId = orden.ProductoId,
                        Cantidad = pallet.Kilos,
                        TipoMovimiento = "PRODUCCION_PARCIAL",
                        Observacion = $"Ingreso Pallet N° {pallet.NumeroPallet} (OP #{orden.Id})",
                        OrdenProduccionId = orden.Id
                    });
                }

                pallet.Estado = "Finalizada";
                pallet.FechaCierre = fechaCierre;

                if (orden.Pallets.All(p => p.Estado == "Finalizada"))
                {
                    orden.Estado = EstadoOrden.Finalizada;
                    orden.FechaFin = fechaCierre;
                }

                await _context.SaveChangesAsync();

                return Ok(new { mensaje = $"Pallet {pallet.NumeroPallet} finalizado. Inventario ajustado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = $"Error al procesar el pallet: {ex.Message}" });
            }
        }
    }
}