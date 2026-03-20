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
                    Kilos = Math.Round(g.Sum(x => x.KilosEstimados), 0), // 🚨 Redondeado
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
                    Kilos = Math.Round(g.Sum(x => x.KilosEstimados), 0) // 🚨 Redondeado
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

            var datos = await query.Include(o => o.Producto).GroupBy(o => o.Producto.Nombre)
                .Select(g => new { Producto = g.Key, TotalKilos = Math.Round(g.Sum(x => x.KilosEstimados), 0) }) // 🚨 Redondeado
                .OrderByDescending(x => x.TotalKilos).Take(5).ToListAsync();
            return Ok(datos);
        }

        [HttpGet("top-materiales")]
        public async Task<IActionResult> GetTopMateriales([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var query = _context.Ordenes.Where(o => o.Estado == EstadoOrden.Finalizada && o.FechaFin != null).AsQueryable();
            if (mes.HasValue && anio.HasValue)
                query = query.Where(o => o.FechaFin.Value.Month == mes && o.FechaFin.Value.Year == anio);

            // 🚨 MODIFICADO: Agrupamos por Material y Cliente, y concatenamos el nombre
            var datos = await query.SelectMany(o => o.Consumos)
                .Where(c => c.MateriaPrimaId < 990 || c.MateriaPrimaId > 999)
                .GroupBy(c => new
                {
                    NombreMaterial = c.MateriaPrima.Nombre,
                    // Si el producto tiene ClienteId, traemos la RazonSocial
                    NombreCliente = c.MateriaPrima.Cliente != null ? c.MateriaPrima.Cliente.RazonSocial : null
                })
                .Select(g => new
                {
                    // Si tiene cliente, armamos "Material (Cliente)". Si no, solo "Material"
                    Material = g.Key.NombreCliente != null
                        ? $"{g.Key.NombreMaterial} ({g.Key.NombreCliente})"
                        : g.Key.NombreMaterial,

                    TotalKilos = Math.Round(g.Sum(c => c.CantidadKilos), 0)
                })
                .OrderByDescending(x => x.TotalKilos)
                .Take(5)
                .ToListAsync();

            return Ok(datos);
        }

        [HttpGet("top-clientes")]
        public async Task<IActionResult> GetTopClientes([FromQuery] int? mes, [FromQuery] int? anio)
        {
            var query = _context.Ordenes.Where(o => o.Estado == EstadoOrden.Finalizada && o.ClienteId != null && o.FechaFin != null).AsQueryable();
            if (mes.HasValue && anio.HasValue)
                query = query.Where(o => o.FechaFin.Value.Month == mes && o.FechaFin.Value.Year == anio);

            var datos = await query.Include(o => o.Cliente).GroupBy(o => o.Cliente.RazonSocial)
                .Select(g => new { Cliente = g.Key, TotalKilos = Math.Round(g.Sum(x => x.KilosEstimados), 0) }) // 🚨 Redondeado
                .OrderByDescending(x => x.TotalKilos).Take(7).ToListAsync();
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

            // 🚨 NUEVO: Calculamos los Kilos Pendientes (Todo lo que está en planta sin finalizar)
            var kilosPendientes = await _context.Ordenes
                .Where(o => o.Estado == EstadoOrden.EnCola ||
                            o.Estado == EstadoOrden.Pendiente ||
                            o.Estado == EstadoOrden.EnProceso)
                .SumAsync(o => (decimal?)o.KilosEstimados) ?? 0;

            return Ok(new
            {
                ProduccionMes = Math.Round(kilosMes, 0),
                ProduccionMesAnterior = Math.Round(kilosMesAnt, 0),
                KilosPendientes = Math.Round(kilosPendientes, 0) // 🚨 Mandamos el nuevo dato
            });
        }
    }
}