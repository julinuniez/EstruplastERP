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
                    HojaCargaId = o.HojaCargaId, // 🚀 NUEVO: Enviamos si pertenece a un grupo
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
                if (orden.Estado == EstadoOrden.MaterialPreparado)
                    return BadRequest("No se puede modificar la receta de una orden que ya tiene su material descontado en una Hoja de Carga.");

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
                orden.AditivoUV = dto.AditivoUV;
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

                var faltantes = new List<string>();
                foreach (var consumo in orden.Consumos)
                {
                    var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (mp != null && !(mp.Id >= 990 && mp.Id <= 999))
                    {
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

                bool materialYaDescontado = orden.Estado == EstadoOrden.MaterialPreparado;
                DateTime fechaCierreReal = dto.FechaCierre ?? DateTime.Now;

                // 🚀 SIEMPRE validamos el stock de lo que envíe el Front (sea la receta entera, o solo las adiciones extra)
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

                // 🚀 SIEMPRE procesamos los consumos que lleguen en el DTO
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

                        // Descontamos el stock físico
                        if (!(mp.Id >= 990 && mp.Id <= 999))
                        {
                            mp.StockActual -= consumoUsuario.CantidadKilosReales;

                            _context.Movimientos.Add(new Movimiento
                            {
                                Fecha = fechaCierreReal,
                                ProductoId = mp.Id,
                                Cantidad = -consumoUsuario.CantidadKilosReales,
                                TipoMovimiento = "CONSUMO_PRODUCCION",
                                Observacion = $"Cierre OP #{id}{(materialYaDescontado ? " (Adición Extra)" : "")}: {dto.Observacion}",
                                OrdenProduccionId = id
                            });
                        }
                    }
                }
                orden.Consumos = consumosDefinitivos;

                // SIEMPRE ingresamos el Producto Terminado
                if (orden.Producto != null)
                {
                    orden.Producto.StockActual += dto.KilosProducidosReales;

                    _context.Movimientos.Add(new Movimiento
                    {
                        Fecha = fechaCierreReal,
                        ProductoId = orden.ProductoId,
                        Cantidad = dto.KilosProducidosReales,
                        TipoMovimiento = "PRODUCCION_TERMINADA",
                        Observacion = $"Cierre OP #{id} {(materialYaDescontado ? "(Desde Mezcla)" : "")}",
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
            try
            {
                var orden = await _context.Ordenes.FindAsync(id);
                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
                if (orden.Estado == EstadoOrden.Finalizada) return BadRequest(new { mensaje = "No se puede cancelar una orden ya terminada." });
                if (orden.Estado == EstadoOrden.MaterialPreparado) return BadRequest(new { mensaje = "No se puede cancelar una orden cuyo material ya fue mezclado. Comuníquese con la fábrica." });

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

                // 🚀 NUEVO: Detectar si la orden venía de una Hoja de Carga
                bool vinoDeHojaCarga = orden.HojaCargaId.HasValue;

                if (orden.Estado == EstadoOrden.Finalizada)
                {
                    // 1. Siempre restamos el producto terminado que habíamos sumado
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

                    // 2. Solo devolvemos MP al inventario SI NO fue descontada por un pastón global
                    if (!vinoDeHojaCarga)
                    {
                        foreach (var consumo in orden.Consumos)
                        {
                            var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                            if (mp != null && !(mp.Id >= 990 && mp.Id <= 999))
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

                    // Si venía de una hoja de carga, retrocede solo un paso (A "En Máquina"). Si no, vuelve a Pendiente.
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

            // 🚀 NUEVO: Crear la entidad real Hoja de Carga en la Base de Datos
            var nuevaHoja = new HojaCarga
            {
                CodigoLote = codigoHC,
                FechaCreacion = DateTime.Now,
                Estado = EstadoHojaCarga.Pendiente
            };

            _context.HojasCarga.Add(nuevaHoja);
            await _context.SaveChangesAsync(); // Guardamos para que genere el ID

            var ordenes = await _context.Ordenes
                .Where(o => ordenesIds.Contains(o.Id))
                .ToListAsync();

            foreach (var o in ordenes)
            {
                o.HojaCargaId = nuevaHoja.Id; // Vinculación real en SQL

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
    }
}