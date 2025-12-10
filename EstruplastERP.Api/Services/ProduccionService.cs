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

                var receta = await _context.Formulas.Where(f => f.ProductoTerminadoId == request.ProductoTerminadoId).ToListAsync();

                // Carga masiva de insumos
                var idsInsumos = receta.Select(r => r.MateriaPrimaId).Distinct().ToList();
                var inventarioInsumos = await _context.Productos.Where(p => idsInsumos.Contains(p.Id)).ToListAsync();

                // Validar y Restar Stock
                foreach (var ingrediente in receta)
                {
                    decimal consumoTotal = request.Cantidad * ingrediente.Cantidad;
                    var materiaPrima = inventarioInsumos.First(p => p.Id == ingrediente.MateriaPrimaId);

                    if (materiaPrima.StockActual < consumoTotal)
                        throw new Exception($"Sin stock de {materiaPrima.Nombre}");

                    materiaPrima.StockActual -= consumoTotal;

                    _context.Movimientos.Add(new Movimiento
                    {
                        Fecha = DateTime.Now,
                        ProductoId = ingrediente.MateriaPrimaId,
                        Cantidad = -consumoTotal,
                        TipoMovimiento = "CONSUMO",
                        Observacion = $"Prod. Orden Turno {request.Turno}",
                        EmpleadoId = request.EmpleadoId,
                        Turno = request.Turno
                    });
                }

                // Crear Producción
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

                // Sumar Stock Producto Terminado
                productoTerminado.StockActual += request.Cantidad;
                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = request.ProductoTerminadoId,
                    Cantidad = request.Cantidad,
                    TipoMovimiento = "ENTRADA_PROD",
                    Observacion = $"Entrada Orden Turno {request.Turno}",
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
                throw; // Re-lanzamos el error para que el Controller lo atrape
            }
        }
    }
}