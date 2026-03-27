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
        public async Task<ActionResult> GetOrdenesRecientes()
        {
            var lista = await _context.Ordenes
                .Include(o => o.Producto)
                .Include(o => o.Cliente)
                .Include(o => o.Consumos).ThenInclude(c => c.MateriaPrima)
                .OrderByDescending(o => o.FechaCreacion)
                .Take(50)
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
                dynamic check = await _produccionService.VerificarStock(dto);
                var jsonCheck = System.Text.Json.JsonSerializer.Serialize(check);
                using var doc = System.Text.Json.JsonDocument.Parse(jsonCheck);

                bool hayStock = doc.RootElement.GetProperty("posible").GetBoolean();

                var orden = await _produccionService.RegistrarOrden(dto, hayStock);

                string msg = hayStock
                    ? "Hay stock. Orden enviada directo a Máquina."
                    : "Material insuficiente. Orden guardada en Cola.";

                return CreatedAtAction("GetOrden", new { id = orden.Id }, new { mensaje = msg, id = orden.Id });
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
                if (orden.Estado != EstadoOrden.EnCola) return BadRequest(new { mensaje = "Solo las órdenes 'En Cola' pueden ser enviadas a producción." });

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
        public async Task<IActionResult> ConfirmarOrden(int id)
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

                // 🚨 1. ACÁ SÍ DESCONTAMOS LA MATERIA PRIMA FÍSICA
                foreach (var consumo in orden.Consumos)
                {
                    var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (mp != null && !(mp.Id >= 990 && mp.Id <= 999))
                    {
                        mp.StockActual -= consumo.CantidadKilos;

                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = mp.Id,
                            Cantidad = -consumo.CantidadKilos,
                            TipoMovimiento = "CONSUMO_PRODUCCION",
                            Observacion = $"Cierre OP #{id}",
                            ClienteId = orden.ClienteId
                        });
                    }
                }

                // 🚨 2. SUMAMOS EL PRODUCTO TERMINADO AL INVENTARIO
                if (orden.Producto != null)
                {
                    orden.Producto.StockActual += orden.KilosEstimados;

                    _context.Movimientos.Add(new Movimiento
                    {
                        Fecha = DateTime.Now,
                        ProductoId = orden.ProductoId,
                        Cantidad = orden.KilosEstimados,
                        TipoMovimiento = "PRODUCCION_TERMINADA",
                        Observacion = $"Cierre OP #{id}",
                        ClienteId = orden.ClienteId
                    });
                }

                orden.Estado = EstadoOrden.Finalizada;
                orden.FechaFin = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Producción confirmada. Materia prima consumida y PT sumado al inventario." });
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