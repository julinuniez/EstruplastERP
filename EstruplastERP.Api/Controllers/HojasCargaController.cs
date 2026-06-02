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

                    // 🚀 CORRECCIÓN: Usamos _context.Add directo para evitar problemas de pluralización en el DbSet
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

        [HttpGet("consumos-detallados")]
        public async Task<IActionResult> GetConsumosDetallados([FromQuery] int mes, [FromQuery] int anio)
        {
            // 🚀 Este usa el DbSet exacto que ya sabemos que te compila (ConsumoHojaCarga)
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
    }
}