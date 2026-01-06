using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

namespace EstruplastERP.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecuperacionController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RecuperacionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. INGRESO DE SCRAP SUCIO (Balanza de entrada)
        [HttpPost("ingreso-scrap")]
        public async Task<IActionResult> IngresarScrap([FromBody] IngresoScrapDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null) return BadRequest("Cliente no existe.");

            // Generamos un SKU único para el scrap sucio de este cliente
            string skuScrap = $"SCRAP-CLI-{cliente.Id}";

            var prodScrap = await _context.Productos.FirstOrDefaultAsync(p => p.CodigoSku == skuScrap);

            // Si no existe el producto "Basura" para este cliente, lo creamos automático
            if (prodScrap == null)
            {
                prodScrap = new Producto
                {
                    Nombre = $"SCRAP SUCIO - {cliente.RazonSocial.ToUpper()}",
                    CodigoSku = skuScrap,
                    ClienteId = cliente.Id,
                    Rubro = "SCRAP", // Importante para filtros
                    EsMateriaPrima = false, // No se puede usar para producir directo
                    EsProductoTerminado = false,
                    StockActual = 0,
                    PesoEspecifico = 1,
                    PrecioCosto = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };
                _context.Productos.Add(prodScrap);
                await _context.SaveChangesAsync(); // Guardar para obtener ID
            }

            // Aumentar Stock Sucio
            prodScrap.StockActual += dto.Kilos;

            // Registrar Movimiento (Kardex)
            var movimiento = new Movimiento
            {
                ProductoId = prodScrap.Id,
                Fecha = DateTime.Now,
                Cantidad = dto.Kilos, // Positivo porque entra
                TipoMovimiento = "INGRESO_SCRAP",
                ClienteId = cliente.Id,
                Observacion = $"Ingreso Scrap Sucio - Remito: {dto.Remito}",
                PrecioUnitario = 0,
                PrecioTotal = 0
            };
            _context.Movimientos.Add(movimiento);

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"✅ Se ingresaron {dto.Kilos}kg de Scrap a la cuenta de {cliente.RazonSocial}." });
        }

        // 2. PROCESO DE PELETIZADO (Transformación: Sucio -> Limpio)
        [HttpPost("procesar-scrap")]
        public async Task<IActionResult> ProcesarScrap([FromBody] ProcesoScrapDto dto)
        {
            // A. Buscar Origen (Scrap Sucio)
            string skuScrap = $"SCRAP-CLI-{dto.ClienteId}";
            var prodScrap = await _context.Productos.FirstOrDefaultAsync(p => p.CodigoSku == skuScrap);

            if (prodScrap == null || prodScrap.StockActual < dto.KilosEntrada)
            {
                return BadRequest($"❌ Stock insuficiente de Scrap Sucio. Stock actual: {prodScrap?.StockActual ?? 0} kg.");
            }

            // B. Buscar Destino (Material Recuperado Limpio)
            // Buscamos si el cliente tiene un material propio tipo "RECUPERADO" o "TUTTI"
            // Si no tiene uno específico, podrías crear lógica para crearlo, 
            // pero asumiremos que usas los genéricos o uno creado previamente.

            // Opción: Buscar por SKU estandarizado de recuperado
            string skuLimpio = $"MP-CLI-{dto.ClienteId}-RECUP";

            var prodLimpio = await _context.Productos.FirstOrDefaultAsync(p => p.CodigoSku == skuLimpio);

            // Si no existe, lo creamos al vuelo
            if (prodLimpio == null)
            {
                var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
                prodLimpio = new Producto
                {
                    Nombre = $"MP RECUPERADO - {cliente.RazonSocial}",
                    CodigoSku = skuLimpio,
                    ClienteId = dto.ClienteId,
                    Rubro = "MATERIA PRIMA RECUPERADA",
                    EsMateriaPrima = true, // Ya se puede usar en recetas
                    EsProductoTerminado = false,
                    StockActual = 0,
                    PesoEspecifico = 1.05m, // Promedio AI
                    Activo = true
                };
                _context.Productos.Add(prodLimpio);
                await _context.SaveChangesAsync();
            }

            // C. Ejecutar Transformación
            prodScrap.StockActual -= dto.KilosEntrada; // Restamos lo sucio que entró a la tolva
            prodLimpio.StockActual += dto.KilosSalida; // Sumamos lo limpio que salió

            // D. Registrar Movimientos
            var movSalida = new Movimiento
            {
                ProductoId = prodScrap.Id,
                Fecha = DateTime.Now,
                Cantidad = -dto.KilosEntrada, // Negativo
                TipoMovimiento = "CONSUMO_RECUPERACION",
                ClienteId = dto.ClienteId,
                Observacion = $"Consumo por peletizado"
            };

            var movEntrada = new Movimiento
            {
                ProductoId = prodLimpio.Id,
                Fecha = DateTime.Now,
                Cantidad = dto.KilosSalida, // Positivo
                TipoMovimiento = "PRODUCCION_RECUPERADO",
                ClienteId = dto.ClienteId,
                Observacion = $"Salida de peletizado (Rendimiento: {((dto.KilosSalida / dto.KilosEntrada) * 100):N1}%)"
            };

            _context.Movimientos.AddRange(movSalida, movEntrada);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "♻️ Proceso registrado exitosamente." });
        }
    }

    // DTOs (Data Transfer Objects)
    public class IngresoScrapDto
    {
        public int ClienteId { get; set; }
        public decimal Kilos { get; set; }
        public string? Remito { get; set; }
    }

    public class ProcesoScrapDto
    {
        public int ClienteId { get; set; }
        public decimal KilosEntrada { get; set; } // Lo sucio
        public decimal KilosSalida { get; set; }  // Lo limpio
    }
}