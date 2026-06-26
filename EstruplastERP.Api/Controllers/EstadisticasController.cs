using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using System.Globalization;

namespace EstruplastERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadisticasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstadisticasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("resumen-mensual")]
        public async Task<IActionResult> GetResumenMensual()
        {
            var fechaLimite = DateTime.Now.AddMonths(-12);

            var ordenesRaw = await _context.Ordenes
                .Where(o => o.Estado == EstadoOrden.Finalizada && o.FechaFin != null && o.FechaFin >= fechaLimite)
                .Select(o => new { FechaFin = o.FechaFin.Value, o.KilosEstimados })
                .ToListAsync();

            var datos = ordenesRaw
                .GroupBy(o => new { o.FechaFin.Year, o.FechaFin.Month })
                .Select(g => new {
                    Periodo = $"{g.Key.Month:00}/{g.Key.Year}",
                    Kilos = Math.Round(g.Sum(x => x.KilosEstimados), 0),
                    CantidadOrdenes = g.Count()
                })
                .OrderBy(x => x.Periodo.Substring(3) + x.Periodo.Substring(0, 2))
                .ToList();

            return Ok(datos);
        }

        [HttpGet("produccion-semanal")]
        public async Task<IActionResult> GetProduccionSemanal()
        {
            var fechaLimite = DateTime.Today.AddDays(-56);

            var ordenesRaw = await _context.Ordenes
                .Where(o => o.Estado == EstadoOrden.Finalizada && o.FechaFin != null && o.FechaFin >= fechaLimite)
                .Select(o => new { FechaFin = o.FechaFin.Value, o.KilosEstimados })
                .ToListAsync();

            var culture = CultureInfo.CurrentCulture;

            var datos = ordenesRaw
                .GroupBy(o => new {
                    Year = o.FechaFin.Year,
                    Week = culture.Calendar.GetWeekOfYear(o.FechaFin, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday)
                })
                .Select(g => new {
                    Periodo = $"Sem {g.Key.Week}",
                    Anio = g.Key.Year,
                    NumSemana = g.Key.Week,
                    Kilos = Math.Round(g.Sum(x => x.KilosEstimados), 0)
                })
                .OrderBy(x => x.Anio).ThenBy(x => x.NumSemana)
                .ToList();

            return Ok(datos);
        }

        [HttpGet("top-productos")]
        public async Task<IActionResult> GetTopProductos([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var query = _context.Ordenes.Where(o => o.Estado == EstadoOrden.Finalizada && o.FechaFin != null).AsQueryable();
            if (mes.HasValue && anio.HasValue)
                query = query.Where(o => o.FechaFin.Value.Month == mes && o.FechaFin.Value.Year == anio);

            var ordenes = await query
                .Select(o => new {
                    Producto = o.Producto != null ? o.Producto.Nombre : "Desconocido",
                    Kilos = o.KilosEstimados
                })
                .ToListAsync();

            var todosLosProductos = ordenes
                .GroupBy(x => x.Producto)
                .Select(g => new {
                    Producto = g.Key,
                    TotalKilos = Math.Round(g.Sum(x => x.Kilos), 0)
                })
                .OrderByDescending(x => x.TotalKilos)
                .ToList();

            var top5 = todosLosProductos.Take(5).ToList();
            var kilosSobrantes = todosLosProductos.Skip(5).Sum(x => x.TotalKilos);

            if (kilosSobrantes > 0)
            {
                top5.Add(new { Producto = "OTROS PRODUCTOS", TotalKilos = kilosSobrantes });
            }

            return Ok(top5);
        }

        [HttpGet("top-materiales")]
        public async Task<IActionResult> GetTopMateriales([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var targetMes = mes ?? DateTime.Today.Month;
            var targetAnio = anio ?? DateTime.Today.Year;

            var consumosKardex = await _context.Movimientos
                .Include(m => m.Producto)
                    .ThenInclude(p => p.Cliente)
                .Where(m => m.Fecha.Month == targetMes &&
                            m.Fecha.Year == targetAnio &&
                            m.TipoMovimiento != null &&
                            m.TipoMovimiento.StartsWith("CONSUMO") &&
                            m.Producto != null &&
                            (m.Producto.Id < 990 || m.Producto.Id > 999))
                .GroupBy(m => new {
                    NombreMaterial = m.Producto.Nombre,
                    NombreCliente = m.Producto.Cliente != null ? m.Producto.Cliente.RazonSocial : null
                })
                .Select(g => new
                {
                    Material = g.Key.NombreCliente != null
                        ? $"{g.Key.NombreMaterial} ({g.Key.NombreCliente})"
                        : g.Key.NombreMaterial,

                    TotalKilos = Math.Round(g.Sum(m => Math.Abs(m.Cantidad)), 0)
                })
                .OrderByDescending(x => x.TotalKilos)
                .ToListAsync();

            var top7 = consumosKardex.Take(7).ToList();
            var kilosSobrantes = consumosKardex.Skip(7).Sum(x => x.TotalKilos);

            if (kilosSobrantes > 0)
            {
                top7.Add(new { Material = "OTROS MATERIALES", TotalKilos = kilosSobrantes });
            }

            return Ok(top7);
        }

        [HttpGet("top-clientes")]
        public async Task<IActionResult> GetTopClientes([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var query = _context.Ordenes.Where(o => o.Estado == EstadoOrden.Finalizada && o.ClienteId != null && o.FechaFin != null).AsQueryable();
            if (mes.HasValue && anio.HasValue)
                query = query.Where(o => o.FechaFin.Value.Month == mes && o.FechaFin.Value.Year == anio);

            var ordenes = await query
                .Select(o => new {
                    Cliente = o.Cliente != null ? o.Cliente.RazonSocial : "Sin Cliente",
                    Kilos = o.KilosEstimados
                })
                .ToListAsync();

            var datos = ordenes
                .GroupBy(x => x.Cliente)
                .Select(g => new {
                    Cliente = g.Key,
                    TotalKilos = Math.Round(g.Sum(x => x.Kilos), 0)
                })
                .OrderByDescending(x => x.TotalKilos)
                .Take(7)
                .ToList();

            return Ok(datos);
        }

        [HttpGet("resumen-kpis")]
        public async Task<IActionResult> GetKPIs([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var targetAnio = anio ?? DateTime.Today.Year;
            var targetMes = mes ?? DateTime.Today.Month;

            var fechaInicio = new DateTime(targetAnio, targetMes, 1);
            var fechaFin = fechaInicio.AddMonths(1).AddDays(-1);

            var fechaInicioAnt = fechaInicio.AddMonths(-1);
            var fechaFinAnt = fechaInicio.AddDays(-1);

            var kilosMes = await _context.Ordenes
                .Where(o => o.Estado == EstadoOrden.Finalizada && o.FechaFin != null && o.FechaFin >= fechaInicio && o.FechaFin <= fechaFin)
                .SumAsync(o => (decimal?)o.KilosEstimados) ?? 0;

            var kilosMesAnt = await _context.Ordenes
                .Where(o => o.Estado == EstadoOrden.Finalizada && o.FechaFin != null && o.FechaFin >= fechaInicioAnt && o.FechaFin <= fechaFinAnt)
                .SumAsync(o => (decimal?)o.KilosEstimados) ?? 0;

            var kilosPendientes = await _context.Ordenes
                .Where(o => o.Estado == EstadoOrden.Pendiente || o.Estado == EstadoOrden.EnProceso)
                .SumAsync(o => (decimal?)o.KilosEstimados) ?? 0;

            return Ok(new
            {
                ProduccionMes = Math.Round(kilosMes, 0),
                ProduccionMesAnterior = Math.Round(kilosMesAnt, 0),
                KilosPendientes = Math.Round(kilosPendientes, 0)
            });
        }

        // 🚀 NUEVO: Análisis de Materiales para Rentabilidad (Fazon vs Propio)
        [HttpGet("analisis-materiales")]
        public async Task<IActionResult> GetAnalisisMateriales([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var targetMes = mes ?? DateTime.Today.Month;
            var targetAnio = anio ?? DateTime.Today.Year;

            var consumos = await _context.Movimientos
                .Include(m => m.Producto)
                .Where(m => m.Fecha.Month == targetMes &&
                            m.Fecha.Year == targetAnio &&
                            m.TipoMovimiento != null &&
                            m.TipoMovimiento.StartsWith("CONSUMO") &&
                            m.Producto != null &&
                            (m.Producto.Id < 990 || m.Producto.Id > 999))
                .ToListAsync();

            var kilosFazon = consumos.Where(m => m.Producto.ClienteId != null && m.Producto.ClienteId > 1).Sum(m => Math.Abs(m.Cantidad));
            var kilosPropio = consumos.Where(m => m.Producto.ClienteId == null || m.Producto.ClienteId <= 1).Sum(m => Math.Abs(m.Cantidad));

            return Ok(new
            {
                KilosFazon = Math.Round(kilosFazon, 2),
                KilosPropio = Math.Round(kilosPropio, 2),
                TotalKilos = Math.Round(kilosFazon + kilosPropio, 2)
            });
        }
    }
}