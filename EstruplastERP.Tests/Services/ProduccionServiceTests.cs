using Microsoft.EntityFrameworkCore;
using Xunit;
using EstruplastERP.Core;
using EstruplastERP.Data;
using EstruplastERP.Api.Services;
using EstruplastERP.Api.Dtos;

namespace EstruplastERP.Tests
{
    public class ProduccionServiceTests
    {
        private ApplicationDbContext GetMemoryContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task VerificarStock_ConRetencionesDeOtrasOrdenes_DebeRetornarPosibleFalse()
        {
            // 1. ARRANGE: Preparamos la base de datos de mentira
            using var context = GetMemoryContext();

            // Creamos un material con 100kg FÍSICOS
            var pai = new Producto { Id = 1, Nombre = "PAI Virgen", StockActual = 100, EsMateriaPrima = true };
            context.Productos.Add(pai);

            // Simulamos que ya existe una Orden "Pendiente" (en máquina) que retiene 80kg
            var ordenExistente = new OrdenProduccion
            {
                Id = 10,
                Estado = EstadoOrden.Pendiente,
                NumeroPedidoCliente = "TEST-001", // 👈 Agregamos para cumplir validación
                Observacion = "Orden de prueba para test" // 👈 Agregamos para cumplir validación
            };

            ordenExistente.Consumos = new List<ConsumoOrden>
            {
                new ConsumoOrden { MateriaPrimaId = 1, CantidadKilos = 80 }
            };
            context.Ordenes.Add(ordenExistente);

            await context.SaveChangesAsync();

            var service = new ProduccionService(context);

            // Creamos la petición para una NUEVA orden que pide 50kg
            // (100 físicos - 80 retenidos = 20 libres). ¡50kg NO deberían alcanzar!
            var nuevaOrdenDto = new NuevaOrdenDto
            {
                Kilos = 50,
                Consumos = new List<DetalleConsumoDto>
                {
                    new DetalleConsumoDto { MateriaPrimaId = 1, CantidadKilos = 50 }
                }
            };

            // ==========================================
            // 2. ACT: Ejecutamos el método
            // ==========================================
            var resultado = await service.VerificarStock(nuevaOrdenDto);

            // ==========================================
            // 3. ASSERT: Comprobamos la respuesta
            // ==========================================
            // Transformamos a JSON (Igual que en tu controlador) para saltar la barrera de seguridad de tipos anónimos entre proyectos
            var jsonResult = System.Text.Json.JsonSerializer.Serialize(resultado);
            using var doc = System.Text.Json.JsonDocument.Parse(jsonResult);

            bool posible = doc.RootElement.GetProperty("posible").GetBoolean();
            string mensaje = doc.RootElement.GetProperty("mensaje").GetString();

            // Validamos que devuelva FALSE y nos avise qué material falta y cuánto queda libre
            Assert.False(posible);
            Assert.Contains("Falta PAI Virgen", mensaje);
            Assert.Contains("Libre: 20", mensaje);
        }
    }
}