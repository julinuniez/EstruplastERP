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

        // DTO ligero para recibir la lista de materiales desde Vue
        public class DeclararConsumoDto
        {
            public int MateriaPrimaId { get; set; }
            public decimal CantidadRealKg { get; set; }
        }

        [HttpPost("{id}/declarar-consumos")]
        public async Task<IActionResult> DeclararConsumos(int id, [FromBody] List<DeclararConsumoDto> consumosReales)
        {
            // 1. Buscamos la Hoja de Carga y traemos sus Órdenes hijas
            var hojaCarga = await _context.HojasCarga
                .Include(h => h.Ordenes)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (hojaCarga == null)
                return NotFound(new { mensaje = "Hoja de carga no encontrada." });

            if (hojaCarga.Estado == EstadoHojaCarga.ConsumosDeclarados)
                return BadRequest(new { mensaje = "⛔ Esta hoja de carga ya tiene los consumos descontados del inventario." });

            // Abrimos una transacción: O se guarda todo, o no se guarda nada (Anti-fallos)
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var consumo in consumosReales)
                {
                    // 2. Restar el material real del inventario general
                    var material = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (material != null)
                    {
                        // Restamos el stock matemático
                        material.StockActual -= consumo.CantidadRealKg;

                        // 🚀 LA SOLUCIÓN: Dejamos el rastro en el Historial general (Kardex)
                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = consumo.MateriaPrimaId,
                            Cantidad = -consumo.CantidadRealKg, // En negativo porque es un consumo
                            TipoMovimiento = "CONSUMO_MEZCLA",
                            Observacion = $"Descarga por Hoja de Carga #{id}"
                        });
                    }

                    // 3. Dejar el comprobante en el historial de qué se gastó en este pastón
                    _context.ConsumosHojasCarga.Add(new ConsumoHojaCarga
                    {
                        HojaCargaId = id,
                        MateriaPrimaId = consumo.MateriaPrimaId,
                        CantidadRealKg = consumo.CantidadRealKg
                    });
                }

                // 4. Sellamos la Hoja de Carga como "Declarada"
                hojaCarga.Estado = EstadoHojaCarga.ConsumosDeclarados;
                hojaCarga.FechaDeclaracion = DateTime.Now;

                // 5. LA MAGIA: Movemos todas las OPs atadas a esta hoja al nuevo estado
                foreach (var orden in hojaCarga.Ordenes)
                {
                    if (orden.Estado == EstadoOrden.Pendiente)
                    {
                        // "MaterialPreparado" significa que ya no hay que restarle material al cerrarla
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