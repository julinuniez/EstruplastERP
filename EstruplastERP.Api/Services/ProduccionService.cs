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

        private async Task<List<DetalleConsumoDto>> ExplosionarRecetasAsync(List<DetalleConsumoDto> consumosOriginales)
        {
            var consumosFinales = new List<DetalleConsumoDto>();

            foreach (var consumo in consumosOriginales)
            {
                var mp = await _context.Productos.FindAsync(consumo.MateriaPrimaId);

                if (mp != null && mp.EsPremezcla)
                {
                    var subReceta = await _context.Formulas
                        .Where(f => f.ProductoTerminadoId == mp.Id)
                        .ToListAsync();

                    if (!subReceta.Any())
                    {
                        consumosFinales.Add(consumo);
                        continue;
                    }

                    var subConsumos = subReceta.Select(r => new DetalleConsumoDto
                    {
                        MateriaPrimaId = r.MateriaPrimaId,
                        CantidadKilos = (consumo.CantidadKilos * r.Cantidad) / 100M
                    }).ToList();

                    var subConsumosExplosionados = await ExplosionarRecetasAsync(subConsumos);
                    consumosFinales.AddRange(subConsumosExplosionados);
                }
                else
                {
                    consumosFinales.Add(consumo);
                }
            }

            var consumosAgrupados = consumosFinales
                .GroupBy(c => c.MateriaPrimaId)
                .Select(g => new DetalleConsumoDto
                {
                    MateriaPrimaId = g.Key,
                    CantidadKilos = g.Sum(c => c.CantidadKilos)
                }).ToList();

            return consumosAgrupados;
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

            itemsParaVerificar = await ExplosionarRecetasAsync(itemsParaVerificar);

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

                    if (stockLibre < item.CantidadKilos)
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

                consumosCalculados = await ExplosionarRecetasAsync(consumosCalculados);

                if (request.ClienteId.GetValueOrDefault() > 0)
                {
                    consumosCalculados = await AplicarSustitucionFazon(request.ClienteId.Value, consumosCalculados);
                }

                if (consumosCalculados.Any())
                {
                    foreach (var item in consumosCalculados)
                    {
                        nuevaOrden.Consumos.Add(new ConsumoOrden
                        {
                            MateriaPrimaId = item.MateriaPrimaId,
                            CantidadKilos = item.CantidadKilos
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
            var productoTerminado = await _context.Productos.FindAsync(productoId);
            var recetaDb = await _context.Formulas.Include(f => f.MateriaPrima).Where(f => f.ProductoTerminadoId == productoId).ToListAsync();
            var materialesCliente = await _context.Productos.Where(p => p.ClienteId == clienteId && p.EsMateriaPrima && p.FamiliaId != null).ToListAsync();

            var listaVisual = new List<ItemFormulaVisualDto>();

            foreach (var itemReceta in recetaDb)
            {
                int idFinal = itemReceta.MateriaPrimaId;
                string nombreFinal = itemReceta.MateriaPrima.Nombre;
                bool esSustitucion = false;
                int familiaBuscada = itemReceta.MateriaPrima.FamiliaId ?? 0;

                if (productoTerminado != null)
                {
                    string nombrePT = productoTerminado.Nombre.ToUpper();

                    if (familiaBuscada == 10)
                    {
                        if (nombrePT.Contains("FINO")) familiaBuscada = 11;
                        else if (nombrePT.Contains("GRUESO")) familiaBuscada = 12;
                        else if (nombrePT.Contains("BICAPA")) familiaBuscada = 13;
                        else if (nombrePT.Contains("TRICAPA")) familiaBuscada = 14;
                    }
                    else if (familiaBuscada == 20)
                    {
                        if (nombrePT.Contains("GRUESO")) familiaBuscada = 21;
                    }
                    else if (familiaBuscada == 30 || familiaBuscada == 40)
                    {
                        if (nombrePT.Contains("FINO")) familiaBuscada = 31;
                        else if (nombrePT.Contains("GRUESO")) familiaBuscada = 32;
                        else if (nombrePT.Contains("BICAPA")) familiaBuscada = 41;
                    }
                }

                var sustituto = materialesCliente.FirstOrDefault(m => m.FamiliaId == familiaBuscada);

                if (sustituto != null)
                {
                    idFinal = sustituto.Id;
                    nombreFinal = sustituto.Nombre;
                    esSustitucion = true;
                }

                listaVisual.Add(new ItemFormulaVisualDto
                {
                    MateriaPrimaId = idFinal,
                    Nombre = nombreFinal,
                    CantidadRequerida = (kilosAProducir * itemReceta.Cantidad) / 100M,
                    EsSustitucion = esSustitucion
                });
            }

            return listaVisual;
        }
    }
}