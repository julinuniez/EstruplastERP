using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace EstruplastERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HojasCargaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public HojasCargaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class DeclararConsumoDto
        {
            public int MateriaPrimaId { get; set; }
            public decimal CantidadRealKg { get; set; }
        }

        [HttpPost("{id}/declarar-consumos")]
        public async Task<IActionResult> DeclararConsumos(int id, [FromBody] List<DeclararConsumoDto> consumosReales)
        {
            var hojaCarga = await _context.HojasCarga
                .Include(h => h.Ordenes)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hojaCarga == null)
                return NotFound(new { mensaje = "Hoja de carga no encontrada." });

            if (hojaCarga.Estado == EstadoHojaCarga.ConsumosDeclarados)
                return BadRequest(new { mensaje = "⛔ Esta hoja de carga ya tiene los consumos descontados del inventario." });

            List<string> erroresFaltantes = new List<string>();

            foreach (var consumo in consumosReales)
            {
                var material = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                if (material == null)
                {
                    erroresFaltantes.Add($"❌ El insumo con ID {consumo.MateriaPrimaId} no existe.");
                    continue;
                }

                if (material.StockActual < consumo.CantidadRealKg)
                {
                    decimal faltante = consumo.CantidadRealKg - material.StockActual;
                    erroresFaltantes.Add($"🔸 {material.Nombre}: Faltan {faltante} Kg (Intento: {consumo.CantidadRealKg} Kg | Stock: {material.StockActual} Kg)");
                }
            }

            if (erroresFaltantes.Any())
            {
                string mensajeFallo = "⛔ Stock insuficiente para procesar la mezcla. Faltan registrar los siguientes materiales:\n\n" +
                                      string.Join("\n", erroresFaltantes);

                return BadRequest(new { mensaje = mensajeFallo });
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var consumo in consumosReales)
                {
                    var material = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (material != null)
                    {
                        material.StockActual -= consumo.CantidadRealKg;

                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = consumo.MateriaPrimaId,
                            Cantidad = -consumo.CantidadRealKg,
                            TipoMovimiento = "CONSUMO_MEZCLA",
                            Observacion = $"Descarga por Hoja de Carga #{id}"
                        });
                    }

                    _context.Add(new ConsumoHojaCarga
                    {
                        HojaCargaId = id,
                        MateriaPrimaId = consumo.MateriaPrimaId,
                        CantidadRealKg = consumo.CantidadRealKg
                    });
                }

                hojaCarga.Estado = EstadoHojaCarga.ConsumosDeclarados;
                hojaCarga.FechaDeclaracion = DateTime.Now;

                foreach (var orden in hojaCarga.Ordenes)
                {
                    if (orden.Estado == EstadoOrden.Pendiente)
                    {
                        orden.Estado = EstadoOrden.MaterialPreparado;
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Consumos declarados e inventario actualizado correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error crítico al procesar los consumos.", detalle = ex.Message });
            }
        }

        // NUEVO ENDPOINT: Revertir la declaración de la Hoja de Carga completa
        [HttpPost("{id}/revertir")]
        public async Task<IActionResult> RevertirDeclaracion(int id)
        {
            var hojaCarga = await _context.HojasCarga
                .Include(h => h.Ordenes)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hojaCarga == null)
                return NotFound(new { mensaje = "Hoja de carga no encontrada." });

            if (hojaCarga.Estado != EstadoHojaCarga.ConsumosDeclarados)
                return BadRequest(new { mensaje = "⛔ Esta hoja de carga no está declarada, no hay nada que revertir." });

            // Verificar si alguna orden ya fue finalizada. Si es así, no se puede revertir.
            if (hojaCarga.Ordenes.Any(o => o.Estado == EstadoOrden.Finalizada))
                return BadRequest(new { mensaje = "⛔ No se puede revertir la hoja de carga porque ya hay órdenes finalizadas." });

            // Buscar los consumos que se habían descontado
            var consumos = await _context.ConsumosHojasCarga
                .Where(c => c.HojaCargaId == id)
                .ToListAsync();

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Devolver el stock y registrar movimientos de anulación
                foreach (var consumo in consumos)
                {
                    var material = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (material != null)
                    {
                        material.StockActual += consumo.CantidadRealKg; // Devolvemos el material

                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = consumo.MateriaPrimaId,
                            Cantidad = consumo.CantidadRealKg,
                            TipoMovimiento = "ANULACION_CONSUMO",
                            Observacion = $"Reversión de consumo por Hoja de Carga #{id}"
                        });
                    }
                }

                // Borrar los registros de consumo
                _context.ConsumosHojasCarga.RemoveRange(consumos);

                // Volver el estado de las órdenes a Pendiente
                foreach (var orden in hojaCarga.Ordenes)
                {
                    if (orden.Estado == EstadoOrden.MaterialPreparado)
                    {
                        orden.Estado = EstadoOrden.Pendiente;
                    }
                }

                // Limpiar el estado de la hoja de carga
                hojaCarga.Estado = EstadoHojaCarga.Pendiente;
                hojaCarga.FechaDeclaracion = null;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Declaración revertida y stock devuelto correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error crítico al revertir los consumos.", detalle = ex.Message });
            }
        }

        [HttpGet("consumos-detallados")]
        public async Task<IActionResult> GetConsumosDetallados([FromQuery] int mes, [FromQuery] int anio)
        {
            var consumos = await _context.ConsumosHojasCarga
                .Include(c => c.HojaCarga)
                .Include(c => c.MateriaPrima)
                    .ThenInclude(mp => mp.Cliente)
                .Where(c => c.HojaCarga != null &&
                            c.HojaCarga.FechaDeclaracion != null &&
                            c.HojaCarga.FechaDeclaracion.Value.Month == mes &&
                            c.HojaCarga.FechaDeclaracion.Value.Year == anio)
                .Select(c => new {
                    hojaCargaId = c.HojaCargaId,
                    fecha = c.HojaCarga.FechaDeclaracion,
                    cantidadRealKg = c.CantidadRealKg,
                    nombreMateriaPrima = c.MateriaPrima != null ? c.MateriaPrima.Nombre : "Insumo",
                    clienteNombre = (c.MateriaPrima != null && c.MateriaPrima.Cliente != null) ? c.MateriaPrima.Cliente.RazonSocial : null
                })
                .ToListAsync();

            return Ok(consumos);
        }

        [HttpGet("{id}/consumos")]
        public async Task<IActionResult> GetConsumosHoja(int id)
        {
            // 🚀 MAGIA: Traemos la hoja junto con el producto base para leer su receta maestra
            var hoja = await _context.HojasCarga
                .Include(h => h.Ordenes)
                    .ThenInclude(o => o.Producto)
                        .ThenInclude(p => p.Formulas)
                .FirstOrDefaultAsync(h => h.Id == id);

            var formulasMaestras = hoja?.Ordenes.FirstOrDefault()?.Producto?.Formulas;

            var consumos = await _context.ConsumosHojasCarga
                .Include(c => c.MateriaPrima)
                .Where(c => c.HojaCargaId == id)
                .Select(c => new {
                    materiaPrimaId = c.MateriaPrimaId,
                    nombre = c.MateriaPrima != null ? c.MateriaPrima.Nombre : "Insumo",
                    real = c.CantidadRealKg,
                    // 🚀 RESCATE MAESTRO: Si la receta original decía "A", se lo inyectamos al vuelo
                    extrusoraDestino = formulasMaestras != null
                        ? (formulasMaestras.FirstOrDefault(f => f.MateriaPrimaId == c.MateriaPrimaId) != null
                            ? formulasMaestras.FirstOrDefault(f => f.MateriaPrimaId == c.MateriaPrimaId).ExtrusoraDestino
                            : "UNICA")
                        : "UNICA"
                })
                .ToListAsync();

            return Ok(consumos);
        }

        [HttpGet("consumos-consolidados")]
        public async Task<IActionResult> GetConsumosConsolidados([FromQuery] int[] ordenIds)
        {
            if (ordenIds == null || ordenIds.Length == 0)
                return BadRequest(new { mensaje = "Debe enviar al menos una orden." });

            var ordenes = await _context.Ordenes
                .Include(o => o.Producto)
                    .ThenInclude(p => p.Formulas)
                .Where(o => ordenIds.Contains(o.Id))
                .ToListAsync();

            var hojaCargaId = ordenes.FirstOrDefault(o => o.HojaCargaId != null)?.HojaCargaId;
            var formulasMaestras = ordenes.FirstOrDefault()?.Producto?.Formulas;

            if (hojaCargaId.HasValue)
            {
                var hojaCarga = await _context.HojasCarga.FindAsync(hojaCargaId.Value);

                if (hojaCarga != null && hojaCarga.Estado == EstadoHojaCarga.ConsumosDeclarados)
                {
                    // PLAN A: La hoja ya está declarada
                    var consumosReales = await _context.ConsumosHojasCarga
                        .Include(c => c.MateriaPrima)
                        .Where(c => c.HojaCargaId == hojaCargaId.Value)
                        .Select(c => new
                        {
                            materiaPrimaId = c.MateriaPrimaId,
                            nombre = c.MateriaPrima != null ? c.MateriaPrima.Nombre : "Insumo",
                            teorico = c.CantidadRealKg,
                            real = c.CantidadRealKg,
                            // 🚀 RESCATE MAESTRO ACÁ TAMBIÉN
                            extrusoraDestino = formulasMaestras != null
                                ? (formulasMaestras.FirstOrDefault(f => f.MateriaPrimaId == c.MateriaPrimaId) != null
                                    ? formulasMaestras.FirstOrDefault(f => f.MateriaPrimaId == c.MateriaPrimaId).ExtrusoraDestino
                                    : "UNICA")
                                : "UNICA"
                        })
                        .ToListAsync();

                    return Ok(consumosReales);
                }
            }

            // PLAN B: Agrupamos las órdenes al vuelo
            var ordenesConConsumos = await _context.Ordenes
                .Include(o => o.Consumos)
                    .ThenInclude(c => c.MateriaPrima)
                .Include(o => o.Producto)
                    .ThenInclude(p => p.Formulas)
                .Where(o => ordenIds.Contains(o.Id))
                .ToListAsync();

            var consumosHistoricos = ordenesConConsumos
                .SelectMany(o => o.Consumos.Select(c => new
                {
                    Consumo = c,
                    Destino = o.Producto?.Formulas?.FirstOrDefault(f => f.MateriaPrimaId == c.MateriaPrimaId)?.ExtrusoraDestino ?? "UNICA"
                }))
                // 🚀 AGRUPAMOS POR INSUMO Y POR TOLVA (Para que no mezcle el color de la A con la B)
                .GroupBy(x => new { x.Consumo.MateriaPrimaId, Nombre = x.Consumo.MateriaPrima?.Nombre, x.Destino })
                .Select(g => new
                {
                    materiaPrimaId = g.Key.MateriaPrimaId,
                    nombre = g.Key.Nombre ?? "Insumo",
                    teorico = g.Sum(x => x.Consumo.CantidadKilos),
                    real = g.Sum(x => x.Consumo.CantidadKilos),
                    extrusoraDestino = g.Key.Destino
                })
                .ToList();

            return Ok(consumosHistoricos);
        }
    }
}