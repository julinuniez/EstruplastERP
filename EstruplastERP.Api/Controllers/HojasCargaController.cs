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

            // 🚀 NUEVA LÓGICA: Acumular todos los errores de stock
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

            // Si la lista tiene al menos un error, frenamos todo y mostramos el reporte completo
            if (erroresFaltantes.Any())
            {
                string mensajeFallo = "⛔ Stock insuficiente para procesar la mezcla. Faltan registrar los siguientes materiales:\n\n" +
                                      string.Join("\n", erroresFaltantes);

                return BadRequest(new { mensaje = mensajeFallo });
            }

            // Si pasamos la validación (erroresFaltantes está vacía), abrimos transacción y guardamos
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

                    _context.ConsumosHojasCarga.Add(new ConsumoHojaCarga
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
    }
}