using EstruplastERP.Api.Dtos;
using EstruplastERP.Core;
using EstruplastERP.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EstruplastERP.Api.Services
{
    public class ProduccionService
    {
        private readonly ApplicationDbContext _context;

        public ProduccionService(ApplicationDbContext context)
        {
            _context = context;
        }

        private decimal AplicarToleranciaStock(decimal cantidadPedida, decimal stockActual)
        {
            if (cantidadPedida < 100m) return cantidadPedida;

            decimal diferencia = Math.Abs(cantidadPedida - stockActual);

            if (diferencia <= 0.3m)
            {
                return stockActual;
            }

            return cantidadPedida;
        }

        private async Task<List<DetalleConsumoDto>> AplicarSustitucionFazon(int clienteId, List<DetalleConsumoDto> consumosOriginales)
        {
            var reglas = await _context.ClientesMaterialesFazon
                .Where(c => c.ClienteId == clienteId)
                .ToListAsync();

            if (!reglas.Any()) return consumosOriginales;

            var consumosFinales = new List<DetalleConsumoDto>();

            foreach (var item in consumosOriginales)
            {
                var regla = reglas.FirstOrDefault(r => r.MaterialGenericoId == item.MateriaPrimaId);
                consumosFinales.Add(new DetalleConsumoDto
                {
                    MateriaPrimaId = regla != null ? regla.MaterialRealId : item.MateriaPrimaId,
                    CantidadKilos = item.CantidadKilos
                });
            }
            return consumosFinales;
        }

        public async Task<object> VerificarStock(NuevaOrdenDto request)
        {
            List<DetalleConsumoDto> itemsParaVerificar = new List<DetalleConsumoDto>();

            if (request.Consumos != null && request.Consumos.Any())
            {
                itemsParaVerificar = request.Consumos;
            }
            else
            {
                var recetaDb = await _context.Formulas
                    .Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId)
                    .ToListAsync();

                if (!recetaDb.Any())
                    return new { posible = true, mensaje = "⚠️ Sin receta definida." };

                itemsParaVerificar = recetaDb.Select(r => new DetalleConsumoDto
                {
                    MateriaPrimaId = r.MateriaPrimaId,
                    CantidadKilos = (request.Kilos * r.Cantidad) / 100M
                }).ToList();
            }

            if (request.ClienteId.GetValueOrDefault() > 0)
            {
                itemsParaVerificar = await AplicarSustitucionFazon(request.ClienteId.Value, itemsParaVerificar);
            }

            var ids = itemsParaVerificar.Select(i => i.MateriaPrimaId).Distinct().ToList();
            var inventario = await _context.Productos.Where(p => ids.Contains(p.Id)).ToListAsync();

            foreach (var item in itemsParaVerificar)
            {
                var mp = inventario.FirstOrDefault(p => p.Id == item.MateriaPrimaId);
                if (mp == null) return new { posible = false, mensaje = $"❌ Error: Insumo ID {item.MateriaPrimaId} no existe." };

                bool esGenerico = mp.Id >= 990 && mp.Id <= 999;

                if (!esGenerico)
                {
                    var retenidoPorOtras = await _context.Ordenes
                        .Where(o => o.Estado != EstadoOrden.Finalizada && o.Estado != EstadoOrden.Cancelada)
                        .SelectMany(o => o.Consumos)
                        .Where(c => c.MateriaPrimaId == mp.Id)
                        .SumAsync(c => (decimal?)c.CantidadKilos) ?? 0;

                    var stockLibre = mp.StockActual - retenidoPorOtras;

                    decimal cantidadAjustada = AplicarToleranciaStock(item.CantidadKilos, stockLibre);

                    if (stockLibre < cantidadAjustada)
                    {
                        return new { posible = false, mensaje = $"❌ Falta {mp.Nombre}. Req: {item.CantidadKilos:N2} - Libre: {stockLibre:N2}" };
                    }
                }
            }
            return new { posible = true, mensaje = "✅ Stock Disponible." };
        }

        public async Task<OrdenProduccion> RegistrarOrden(NuevaOrdenDto request, bool hayStock)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var productoTerminado = await _context.Productos.FindAsync(request.ProductoTerminadoId);
                if (productoTerminado == null) throw new Exception("Producto no encontrado");

                var nuevaOrden = new OrdenProduccion
                {
                    FechaCreacion = DateTime.Now,
                    ProductoId = request.ProductoTerminadoId,
                    ClienteId = request.ClienteId,
                    NumeroPedidoCliente = request.NumeroPedidoCliente,
                    NotaPedido = request.NotaPedido,
                    Cantidad = request.Cantidad,
                    KilosEstimados = request.Kilos,
                    Desperdicio = request.Desperdicio,
                    EsBobina = request.EsBobina,
                    Observacion = request.Observacion,
                    Estado = EstadoOrden.Pendiente,
                    Largo = request.Largo,
                    Ancho = request.Ancho,
                    Color = request.Color,
                    Espesor = request.Espesor,
                    ConBrillo = request.ConBrillo,
                    LlevaFilm = request.LlevaFilm,
                    EsGofrado = request.EsGofrado,
                    AditivoUV = request.AditivoUV,
                    TipoCorona = request.TipoCorona,
                    Consumos = new List<ConsumoOrden>()
                };

                List<DetalleConsumoDto> consumosCalculados = request.Consumos;

                if (consumosCalculados == null || !consumosCalculados.Any())
                {
                    var recetaDb = await _context.Formulas.Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId).ToListAsync();
                    consumosCalculados = recetaDb.Select(r => new DetalleConsumoDto
                    {
                        MateriaPrimaId = r.MateriaPrimaId,
                        CantidadKilos = (request.Kilos * r.Cantidad) / 100M
                    }).ToList();
                }

                if (request.ClienteId.GetValueOrDefault() > 0)
                {
                    consumosCalculados = await AplicarSustitucionFazon(request.ClienteId.Value, consumosCalculados);
                }

                if (consumosCalculados.Any())
                {
                    foreach (var item in consumosCalculados)
                    {
                        var mp = await _context.Productos.FindAsync(item.MateriaPrimaId);
                        decimal stockActualMP = mp?.StockActual ?? 0;

                        decimal cantidadFinal = AplicarToleranciaStock(item.CantidadKilos, stockActualMP);

                        nuevaOrden.Consumos.Add(new ConsumoOrden
                        {
                            MateriaPrimaId = item.MateriaPrimaId,
                            CantidadKilos = cantidadFinal
                        });
                    }
                }

                _context.Ordenes.Add(nuevaOrden);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return nuevaOrden;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<List<ItemFormulaVisualDto>> ObtenerRecetaProyectada(int productoId, int clienteId, decimal kilosAProducir)
        {
            var recetaDb = await _context.Formulas
                .Include(f => f.MateriaPrima)
                .Where(f => f.ProductoTerminadoId == productoId)
                .ToListAsync();

            var consumosOriginales = recetaDb.Select(r => new DetalleConsumoDto
            {
                MateriaPrimaId = r.MateriaPrimaId,
                CantidadKilos = (kilosAProducir * r.Cantidad) / 100M
            }).ToList();

            // 🚀 SE USA EL MISMO METODO DE VERIFICAR STOCK EN VEZ DEL HARDCODEO VIEJO
            var consumosFinales = await AplicarSustitucionFazon(clienteId, consumosOriginales);

            var listaVisual = new List<ItemFormulaVisualDto>();

            for (int i = 0; i < consumosOriginales.Count; i++)
            {
                var original = consumosOriginales[i];
                var final = consumosFinales[i];
                bool esSustitucion = original.MateriaPrimaId != final.MateriaPrimaId;

                string nombreFinal = "";
                if (esSustitucion)
                {
                    var mpNueva = await _context.Productos.FindAsync(final.MateriaPrimaId);
                    nombreFinal = mpNueva?.Nombre ?? "Material Sustituto";
                }
                else
                {
                    var mpOriginal = recetaDb.First(r => r.MateriaPrimaId == original.MateriaPrimaId).MateriaPrima;
                    nombreFinal = mpOriginal.Nombre;
                }

                listaVisual.Add(new ItemFormulaVisualDto
                {
                    MateriaPrimaId = final.MateriaPrimaId,
                    Nombre = nombreFinal,
                    CantidadRequerida = final.CantidadKilos,
                    EsSustitucion = esSustitucion
                });
            }

            return listaVisual;
        }
    }
}