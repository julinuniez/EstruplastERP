using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;
using EstruplastERP.Api.Services;
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
        public async Task<IActionResult> ModificarOrden(int id, [FromBody] ModificarOrdenDto dto)
        {
            // Abrimos una transacción para que si algo falla, no se guarde nada a medias
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Buscamos la orden con sus consumos actuales
                var orden = await _context.Ordenes
                    .Include(o => o.Consumos)
                    .ThenInclude(c => c.MateriaPrima)
                    .FirstOrDefaultAsync(o => o.Id == id);

                if (orden == null) return NotFound("Orden no encontrada.");

                if (orden.Estado == EstadoOrden.Finalizada || orden.Estado == EstadoOrden.Cancelada)
                    return BadRequest("No se puede modificar una orden que ya está Finalizada o Cancelada.");

                // 2. LA MAGIA DEL STOCK: Validamos si alcanza para la nueva receta
                foreach (var nuevoItem in dto.RecetaNueva)
                {
                    var mp = await _context.Productos.FindAsync(nuevoItem.MateriaPrimaId);
                    if (mp == null) continue;

                    // ¿Cuánto de esta materia prima ya le habíamos prestado a esta orden específica?
                    var reservaPrevia = orden.Consumos
                        .Where(c => c.MateriaPrimaId == mp.Id)
                        .Sum(c => c.CantidadEsperada);

                    // El stock real disponible para ESTA edición es: 
                    // (Lo que hay libre en galpón) + (Lo que ya tenía agarrado esta orden)
                    var stockLibreParaEstaOrden = (mp.StockActual - mp.StockReservado) + reservaPrevia;

                    if (nuevoItem.CantidadEsperada > stockLibreParaEstaOrden)
                    {
                        return BadRequest($"Stock insuficiente para {mp.Nombre}. Necesitás {nuevoItem.CantidadEsperada}kg pero solo tenés {stockLibreParaEstaOrden}kg disponibles (contando la reserva previa). La orden no se modificó.");
                    }
                }

                // 3. Si llegamos acá, HAY STOCK PARA TODO. Procedemos a limpiar lo viejo.
                foreach (var consumoViejo in orden.Consumos)
                {
                    // Devolvemos el stock reservado viejo a la fábrica
                    consumoViejo.MateriaPrima.StockReservado -= consumoViejo.CantidadEsperada;
                }
                // Borramos la receta vieja de la base de datos
                _context.OrdenMateriaPrima.RemoveRange(orden.Consumos);

                // 4. Aplicamos la receta nueva y reservamos el nuevo stock
                var nuevosConsumos = new List<OrdenMateriaPrima>();
                foreach (var item in dto.RecetaNueva)
                {
                    var mp = await _context.Productos.FindAsync(item.MateriaPrimaId);

                    // Retenemos el stock de la nueva receta
                    mp.StockReservado += item.CantidadEsperada;

                    nuevosConsumos.Add(new OrdenMateriaPrima
                    {
                        MateriaPrimaId = mp.Id,
                        CantidadEsperada = item.CantidadEsperada,
                        TipoInsumo = item.TipoInsumo // "VIRGEN", "MASTERBATCH", etc.
                    });
                }

                // 5. Actualizamos los datos principales de la orden
                orden.Consumos = nuevosConsumos;
                orden.KilosEstimados = dto.KilosTotales;
                orden.MermaPorcentaje = dto.Merma;

                // 🚨 ACA ESTÁ EL REQUISITO: Volvemos a marcar como NO impresa
                orden.Impresa = false;
                // Si estaba "En Cola" porque antes faltaba material, ahora quizás pasa a "Pendiente"
                orden.Estado = EstadoOrden.Pendiente;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Orden modificada correctamente. Stock recalculado y orden desmarcada como impresa." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error al modificar: {ex.Message}");
            }
        }

        [HttpPost("activar/{id}")]
        public async Task<IActionResult> ActivarOrden(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orden = await _context.Ordenes.Include(o => o.Consumos).FirstOrDefaultAsync(o => o.Id == id);
                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
                if (orden.Estado != EstadoOrden.EnCola) return BadRequest(new { mensaje = "Solo las órdenes 'En Cola' pueden ser enviadas a producción." });

                var faltantes = new List<string>();
                foreach (var consumo in orden.Consumos)
                {
                    var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);
                    if (mp != null && !(mp.Id >= 990 && mp.Id <= 999))
                    {
                        if (mp.StockActual < consumo.CantidadKilos)
                            faltantes.Add($"- {mp.Nombre}: Faltan {(consumo.CantidadKilos - mp.StockActual):N2} kg");
                    }
                }
                if (faltantes.Any()) return BadRequest(new { mensaje = "Faltan materiales:\n" + string.Join("\n", faltantes) });

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
                            TipoMovimiento = "CONSUMO",
                            Observacion = $"Reserva Orden #{id}",
                            ClienteId = orden.ClienteId
                        });
                    }
                }

                orden.Estado = EstadoOrden.Pendiente;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Materia prima reservada. Orden en Máquina." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = $"Error al activar: {ex.Message}" });
            }
        }

        [HttpPost("confirmar/{id}")]
        public async Task<IActionResult> ConfirmarOrden(int id)
        {
            var orden = await _context.Ordenes.Include(o => o.Producto).FirstOrDefaultAsync(o => o.Id == id);

            if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
            if (orden.Estado == EstadoOrden.Finalizada) return BadRequest(new { mensaje = "Esta orden ya fue finalizada." });

            if (orden.Producto != null)
            {
                orden.Producto.StockActual += orden.KilosEstimados;

                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = orden.ProductoId,
                    Cantidad = orden.KilosEstimados,
                    TipoMovimiento = "PRODUCCION_TERMINADA",
                    Observacion = $"Cierre Orden #{id}",
                    ClienteId = orden.ClienteId
                });
            }

            orden.Estado = EstadoOrden.Finalizada;
            orden.FechaFin = DateTime.Now;

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Producción confirmada. PT sumado al inventario." });
        }

        [HttpPost("cancelar/{id}")]
        public async Task<IActionResult> CancelarOrden(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var orden = await _context.Ordenes.Include(o => o.Consumos).FirstOrDefaultAsync(o => o.Id == id);
                if (orden == null) return NotFound(new { mensaje = "Orden no encontrada." });
                if (orden.Estado == EstadoOrden.Finalizada) return BadRequest(new { mensaje = "No se puede cancelar." });

                if (orden.Estado == EstadoOrden.Pendiente || orden.Estado == EstadoOrden.EnProceso)
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
                                TipoMovimiento = "DEVOLUCION",
                                Observacion = $"Cancelación Orden #{id}",
                                ClienteId = orden.ClienteId
                            });
                        }
                    }
                }

                orden.Estado = EstadoOrden.Cancelada;
                orden.FechaFin = DateTime.Now;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Orden cancelada correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
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