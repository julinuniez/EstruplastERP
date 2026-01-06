using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

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
            // Filtramos órdenes NO canceladas del último año
            var fechaLimite = DateTime.Now.AddMonths(-12);

            // NOTA: Entity Framework a veces no puede traducir agrupaciones complejas de fechas a SQL directo.
            // Traemos los datos mínimos necesarios a memoria y agrupamos ahí (seguro y rápido para volúmenes moderados).
            var ordenesRaw = await _context.Ordenes
                .Where(o => o.FechaCreacion >= fechaLimite && o.Estado != EstadoOrden.Cancelada)
                .Select(o => new { o.FechaCreacion, o.KilosEstimados })
                .ToListAsync();

            var datos = ordenesRaw
                .GroupBy(o => new { o.FechaCreacion.Year, o.FechaCreacion.Month })
                .Select(g => new {
                    Periodo = $"{g.Key.Month:00}/{g.Key.Year}", // Formato MM/YYYY
                    Kilos = g.Sum(x => x.KilosEstimados),
                    CantidadOrdenes = g.Count()
                })
                .OrderBy(x => x.Periodo.Substring(3) + x.Periodo.Substring(0, 2)) // Ordenar por YYYYMM string
                .ToList();

            return Ok(datos);
        }

        [HttpGet("top-productos")]
        public async Task<IActionResult> GetTopProductos()
        {
            // Top 5 productos más fabricados (en Kilos) históricamente o del año
            var datos = await _context.Ordenes
                .Where(o => o.Estado != EstadoOrden.Cancelada)
                .Include(o => o.Producto) // En tu clase OrdenProduccion es 'Producto', no 'ProductoTerminado'
                .GroupBy(o => o.Producto.Nombre)
                .Select(g => new {
                    Producto = g.Key,
                    TotalKilos = g.Sum(x => x.KilosEstimados) // Corregido: KilosEstimados
                })
                .OrderByDescending(x => x.TotalKilos)
                .Take(5)
                .ToListAsync();

            return Ok(datos);
        }

        [HttpGet("resumen-kpis")]
        public async Task<IActionResult> GetKPIs()
        {
            // Datos rápidos para tarjetas superiores
            var hoy = DateTime.Today;
            var primerDiaMes = new DateTime(hoy.Year, hoy.Month, 1);

            var kilosMes = await _context.Ordenes
                .Where(o => o.FechaCreacion >= primerDiaMes && o.Estado != EstadoOrden.Cancelada)
                .SumAsync(o => o.KilosEstimados);

            var ordenesPendientes = await _context.Ordenes
                .CountAsync(o => o.Estado == EstadoOrden.Pendiente || o.Estado == EstadoOrden.EnProceso);

            return Ok(new
            {
                ProduccionMes = kilosMes,
                Pendientes = ordenesPendientes
            });
        }
    }
}