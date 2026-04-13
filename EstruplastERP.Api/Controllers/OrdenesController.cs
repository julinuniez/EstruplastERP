using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;
using EstruplastERP.Api.Services;
using EstruplastERP.Api.DTOs;
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
                .Include(o => o.Cliente)
                .Include(o => o.Consumos).ThenInclude(c => c.MateriaPrima)
                .AsQueryable();

            // Si no mandan filtros, por defecto mostramos el mes actual
            int targetMes = mes ?? DateTime.Now.Month;
            int targetAnio = anio ?? DateTime.Now.Year;

            // Filtro ESTRICTO mes a mes
            query = query.Where(o => o.FechaCreacion.Month == targetMes && o.FechaCreacion.Year == targetAnio);

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
                    EsImpreso = o.EsImpreso,
                    Estado = o.Estado.ToString(),
                    EsFinalizada = o.Estado == EstadoOrden.Finalizada,
                    Consumos = o.Consumos.Select(c => new {
                        c.MateriaPrimaId,
                        NombreMateriaPrima = c.MateriaPrima.Nombre,
                        c.CantidadKilos
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
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound("Orden no encontrada.");
                if (orden.Estado == EstadoOrden.Finalizada || orden.Estado == EstadoOrden.Cancelada)
                    return BadRequest("No se puede modificar una orden Finalizada o Cancelada.");

                // VALIDACIÓN DE STOCK DINÁMICA CON LINQ
                foreach (var nuevoItem in dto.RecetaNueva)
                {
                    var mp = await _context.Productos.FindAsync(nuevoItem.MateriaPrimaId);
                    if (mp == null) continue;

                    // Retenido por OTRAS órdenes (excluyendo la que estamos editando)
                    var kilosRetenidosOtrasOrdenes = await _context.Ordenes
                        .Where(o => o.Id != id &&
                                    o.Estado != EstadoOrden.Finalizada &&
                                    o.Estado != EstadoOrden.Cancelada)
                        .SelectMany(o => o.Consumos)
                        .Where(c => c.MateriaPrimaId == mp.Id)
                        .SumAsync(c => (decimal?)c.CantidadKilos) ?? 0;

                    var stockLibreParaEstaOrden = mp.StockActual - kilosRetenidosOtrasOrdenes;

                    if (nuevoItem.CantidadKilos > stockLibreParaEstaOrden)
                    {
                        return BadRequest($"Stock insuficiente de '{mp.Nombre}'. Requiere {nuevoItem.CantidadKilos}kg pero solo quedan {stockLibreParaEstaOrden}kg libres en planta.");
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
                orden.TipoCorona = dto.TipoCorona;
                orden.Color = dto.Color;

                orden.Consumos = nuevosConsumos;

                orden.EsImpreso = false;
                orden.Estado = EstadoOrden.Pendiente;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Orden modificada con éxito. Por favor, vuelva a imprimir la hoja de ruta." });
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

                // 🚨 1. Solo validamos si hay stock. NO lo descontamos.
                var faltantes = new List<string>();
                foreach (var consumo in orden.Consumos)
                {
                    var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (mp != null && !(mp.Id >= 990 && mp.Id <= 999))
                    {
                        // Calculamos retenciones de otras órdenes activas
                        var retenidoPorOtras = await _context.Ordenes
                            .Where(o => o.Id != id && o.Estado != EstadoOrden.Finalizada && o.Estado != EstadoOrden.Cancelada)
                            .SelectMany(o => o.Consumos)
                            .Where(c => c.MateriaPrimaId == mp.Id)
                            .SumAsync(c => (decimal?)c.CantidadKilos) ?? 0;

                        var libre = mp.StockActual - retenidoPorOtras;

                        if (libre < consumo.CantidadKilos)
                            faltantes.Add($"- {mp.Nombre}: Faltan {(consumo.CantidadKilos - libre):N2} kg (libres)");
                    }
                }

                if (faltantes.Any()) return BadRequest(new { mensaje = "Faltan materiales libres:\n" + string.Join("\n", faltantes) });

                // 🚨 2. Cambiamos de estado sin tocar el stock.
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
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
                if (orden.Estado == EstadoOrden.Finalizada) return BadRequest(new { mensaje = "Esta orden ya fue finalizada." });

                var consumosAgrupados = dto.ConsumosReales
                    .GroupBy(c => c.MateriaPrimaId)
                    .Select(g => new { MateriaPrimaId = g.Key, TotalKilos = g.Sum(c => c.CantidadKilosReales) })
                    .ToList();

                var faltantes = new List<string>();

                foreach (var consumo in consumosAgrupados)
                {
                    var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (mp != null && !(mp.Id >= 990 && mp.Id <= 999))
                    {
                        if (mp.StockActual < consumo.TotalKilos)
                        {
                            faltantes.Add($"- {mp.Nombre}: Faltan {(consumo.TotalKilos - mp.StockActual):N2} Kg físicos en el sistema.");
                        }
                    }
                }

                if (faltantes.Any())
                {
                    return BadRequest(new { mensaje = "⛔ STOCK NEGATIVO DETECTADO.\nCargue los ingresos/remitos de estos materiales antes de cerrar la orden:\n\n" + string.Join("\n", faltantes) });
                }

                orden.KilosEstimados = dto.KilosProducidosReales;
                orden.Desperdicio = dto.DesperdicioReal;

                string etiquetaGrupo = "";
                if (!string.IsNullOrWhiteSpace(orden.Observacion))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(orden.Observacion, @"\[Grupo: HC-[^\]]+\]");
                    if (match.Success)
                    {
                        etiquetaGrupo = match.Value;
                    }
                }

                orden.Observacion = string.IsNullOrWhiteSpace(dto.Observacion)
                    ? etiquetaGrupo
                    : dto.Observacion + " " + etiquetaGrupo;

                orden.Observacion = orden.Observacion.Trim();

                _context.RemoveRange(orden.Consumos);
                var consumosDefinitivos = new List<ConsumoOrden>();

                foreach (var consumoUsuario in dto.ConsumosReales)
                {
                    var mp = await _context.Productos.FindAsync(consumoUsuario.MateriaPrimaId);
                    if (mp != null)
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
                                Fecha = DateTime.Now,
                                ProductoId = mp.Id,
                                Cantidad = -consumoUsuario.CantidadKilosReales,
                                TipoMovimiento = "CONSUMO_PRODUCCION",
                                Observacion = $"Cierre OP #{id}: {dto.Observacion}",
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
                        Fecha = DateTime.Now,
                        ProductoId = orden.ProductoId,
                        Cantidad = dto.KilosProducidosReales,
                        TipoMovimiento = "PRODUCCION_TERMINADA",
                        Observacion = $"Cierre OP #{id}",
                        OrdenProduccionId = id
                    });
                }

                orden.Estado = EstadoOrden.Finalizada;
                orden.FechaFin = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Producción confirmada con valores reales. Inventario actualizado." });
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
            try
            {
                var orden = await _context.Ordenes.FindAsync(id);
                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
                if (orden.Estado == EstadoOrden.Finalizada) return BadRequest(new { mensaje = "No se puede cancelar una orden ya terminada." });

                // 🚨 1. Como nunca habíamos descontado el stock físico, simplemente la cancelamos. 
                // Al cambiar el estado, LINQ la va a ignorar al calcular retenciones.
                orden.Estado = EstadoOrden.Cancelada;
                orden.FechaFin = DateTime.Now;

                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "Orden cancelada correctamente. El material queda libre para otras órdenes." });
            }
            catch (Exception ex)
            {
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

                // 1. REVERSIÓN DE ORDEN FINALIZADA (Devolver stock físico)
                if (orden.Estado == EstadoOrden.Finalizada)
                {
                    // Restamos el producto terminado que habíamos sumado al inventario
                    if (orden.Producto != null)
                    {
                        orden.Producto.StockActual -= orden.KilosEstimados; // O los kilos reales que guardes

                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = orden.ProductoId,
                            Cantidad = -orden.KilosEstimados, // Movimiento negativo de compensación
                            TipoMovimiento = "REVERSION_PRODUCCION",
                            Observacion = $"Reversión de cierre por error humano - OP #{id}",
                            OrdenProduccionId = id
                        });
                    }

                    // Devolvemos la materia prima que habíamos descontado
                    foreach (var consumo in orden.Consumos)
                    {
                        var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                        if (mp != null && !(mp.Id >= 990 && mp.Id <= 999)) // Ignorando genéricos
                        {
                            mp.StockActual += consumo.CantidadKilos; // Devolvemos al físico

                            _context.Movimientos.Add(new Movimiento
                            {
                                Fecha = DateTime.Now,
                                ProductoId = mp.Id,
                                Cantidad = consumo.CantidadKilos, // Movimiento positivo de compensación
                                TipoMovimiento = "REVERSION_CONSUMO",
                                Observacion = $"Reversión de consumo por error humano - OP #{id}",
                                OrdenProduccionId = id
                            });
                        }
                    }

                    // Reseteamos las fechas y el estado
                    orden.Estado = EstadoOrden.Pendiente;
                    orden.FechaFin = null;
                }

                // 2. REVERSIÓN DE IMPRESIÓN (Aplica tanto si estaba finalizada como si solo estaba pendiente)
                orden.EsImpreso = false;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Orden revertida exitosamente. El inventario fue restaurado." });
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

            var ordenes = await _context.Ordenes
                .Where(o => ordenesIds.Contains(o.Id))
                .ToListAsync();

            foreach (var o in ordenes)
            {
                if (!string.IsNullOrWhiteSpace(o.Observacion))
                {
                    o.Observacion = System.Text.RegularExpressions.Regex.Replace(o.Observacion, @"\[Grupo: HC-[^\]]+\]", "").Trim();
                }

                o.Observacion = string.IsNullOrWhiteSpace(o.Observacion) ? etiqueta : o.Observacion + " " + etiqueta;
            }

            await _context.SaveChangesAsync();
            return Ok(new { codigo = codigoHC, mensaje = "Hoja de carga registrada con éxito." });
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
    }
}