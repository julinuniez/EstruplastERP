using EstruplastERP.Api.Controllers;
using EstruplastERP.Api.Dtos;
using EstruplastERP.Core;
using EstruplastERP.Data;
using Microsoft.EntityFrameworkCore;

namespace EstruplastERP.Api.Services
{
    public class ProduccionService
    {
        private readonly ApplicationDbContext _context;

        public ProduccionService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lógica de Verificación (MUDADA DEL CONTROLLER)
        public async Task<object> VerificarStock(NuevaOrdenDto request)
        {
            var receta = await _context.Formulas
                .Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId)
                .ToListAsync();

            if (!receta.Any())
                return new { posible = true, mensaje = "⚠️ Sin receta definida (No descuenta stock)" };

            var idsInsumos = receta.Select(r => r.MateriaPrimaId).Distinct().ToList();
            var inventarioInsumos = await _context.Productos
                .Where(p => idsInsumos.Contains(p.Id))
                .ToListAsync();

            foreach (var ingrediente in receta)
            {
                decimal consumoTotal = request.Cantidad * ingrediente.Cantidad;
                var materiaPrima = inventarioInsumos.FirstOrDefault(p => p.Id == ingrediente.MateriaPrimaId);

                if (materiaPrima == null)
                    return new { posible = false, mensaje = $"❌ Error: Insumo {ingrediente.MateriaPrimaId} no existe" };

                if (materiaPrima.StockActual < consumoTotal)
                    return new { posible = false, mensaje = $"❌ Falta {materiaPrima.Nombre}. Necesitas: {consumoTotal} - Hay: {materiaPrima.StockActual}" };
            }

            return new { posible = true, mensaje = "✅ Stock Disponible." };
        }

        // Lógica de Registro (MUDADA DEL CONTROLLER)
        public async Task<Produccion> RegistrarOrden(NuevaOrdenDto request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var productoTerminado = await _context.Productos.FindAsync(request.ProductoTerminadoId);
                if (productoTerminado == null) throw new Exception("Producto no encontrado");

                // 1. CREAR PRODUCCIÓN (Cabecera)
                var nuevaProduccion = new Produccion
                {
                    FechaRegistro = DateTime.Now,
                    ProductoTerminadoId = request.ProductoTerminadoId,
                    ClienteId = request.ClienteId,
                    EmpleadoId = request.EmpleadoId,
                    Cantidad = request.Cantidad,
                    Kilos = request.Kilos,
                    Turno = request.Turno,
                    Observacion = request.Observacion,
                    Lote = DateTime.Now.ToString("yyyyMMdd-HHmm")
                };
                _context.Producciones.Add(nuevaProduccion);

                // Guardamos para generar el ID de producción por si lo necesitamos en el historial
                await _context.SaveChangesAsync();

                // ==============================================================================
                // 2. VALIDAR Y RESTAR STOCK (CAMBIO CLAVE) 🛠️
                // ==============================================================================

                // Caso A: El frontend nos envió la receta real (con brillo/estearato calculado)
                if (request.Consumos != null && request.Consumos.Any())
                {
                    var idsInsumos = request.Consumos.Select(c => c.MateriaPrimaId).ToList();
                    var inventarioInsumos = await _context.Productos
                                                  .Where(p => idsInsumos.Contains(p.Id))
                                                  .ToListAsync();

                    foreach (var itemConsumo in request.Consumos)
                    {
                        var materiaPrima = inventarioInsumos.FirstOrDefault(p => p.Id == itemConsumo.MateriaPrimaId);

                        if (materiaPrima == null)
                            throw new Exception($"Insumo ID {itemConsumo.MateriaPrimaId} no existe");

                        // Verificación de Stock
                        if (materiaPrima.StockActual < itemConsumo.CantidadKilos)
                            throw new Exception($"Sin stock suficiente de {materiaPrima.Nombre}. Req: {itemConsumo.CantidadKilos}, Disp: {materiaPrima.StockActual}");

                        // Resta directa de Kilos (Ya calculados en Vue)
                        materiaPrima.StockActual -= itemConsumo.CantidadKilos;

                        // Registrar Movimiento
                        _context.Movimientos.Add(new Movimiento
                        {
                            Fecha = DateTime.Now,
                            ProductoId = materiaPrima.Id,
                            Cantidad = -itemConsumo.CantidadKilos, // Negativo porque es salida
                            TipoMovimiento = "CONSUMO",
                            Observacion = $"Insumo para Orden #{nuevaProduccion.Id} (Turno {request.Turno})",
                            EmpleadoId = request.EmpleadoId,
                            Turno = request.Turno
                        });
                    }
                }
                // Caso B: Fallback (Por seguridad) - Si el front no manda nada, usamos la fórmula fija
                else
                {
                    var recetaFija = await _context.Formulas.Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId).ToListAsync();
                    // ... (Aquí iría tu lógica antigua si quisieras mantener compatibilidad) ...
                    if (recetaFija.Any()) throw new Exception("El frontend no envió los consumos calculados.");
                }

                // ==============================================================================

                // 3. SUMAR STOCK AL PRODUCTO TERMINADO
                // (Esto queda igual que tu código original)
                productoTerminado.StockActual += request.Cantidad; // ¿O sumas Kilos? Depende tu negocio. Normalmente es Cantidad (bolsas)

                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = request.ProductoTerminadoId,
                    Cantidad = request.Cantidad,
                    TipoMovimiento = "ENTRADA_PROD",
                    Observacion = $"Entrada Prod. #{nuevaProduccion.Id}",
                    EmpleadoId = request.EmpleadoId,
                    Turno = request.Turno
                });

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return nuevaProduccion;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}