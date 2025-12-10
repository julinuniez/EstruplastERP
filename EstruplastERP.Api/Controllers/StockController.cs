using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StockController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. INGRESO DE MERCADERÍA (Compra)
        // ==========================================
        [HttpPost("ingresar")]
        public async Task<IActionResult> IngresarStock([FromBody] IngresoDto ingreso)
        {
            if (ingreso.Cantidad <= 0) return BadRequest("La cantidad debe ser positiva.");

            var producto = await _context.Productos.FindAsync(ingreso.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");

            // A. Actualizamos el Precio de Costo (Última Compra)
            if (ingreso.NuevoPrecio > 0)
            {
                producto.PrecioCosto = ingreso.NuevoPrecio;
            }

            // B. Sumamos Stock
            producto.StockActual += ingreso.Cantidad;

            // C. Movimiento
            var movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                ProductoId = ingreso.ProductoId,
                Cantidad = ingreso.Cantidad,
                TipoMovimiento = "COMPRA_INSUMO",
                Observacion = $"Prov: {ingreso.Proveedor} | Precio act. a ${ingreso.NuevoPrecio}",
                Turno = "General"
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Stock y Precio actualizados", nuevoStock = producto.StockActual });
        }

        // ==========================================
        // 2. AJUSTE DE STOCK (Corrección Manual)
        // ==========================================
        [HttpPost("ajuste")]
        public async Task<IActionResult> AjustarStock([FromBody] AjusteDto ajuste)
        {
            var producto = await _context.Productos.FindAsync(ajuste.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");

            // Calculamos la diferencia (Ej: Sistema dice 100, Realidad 90 -> Ajuste -10)
            decimal diferencia = ajuste.CantidadReal - producto.StockActual;

            if (diferencia == 0) return Ok(new { mensaje = "El stock ya coincide, no se hicieron cambios." });

            // Actualizamos el stock
            producto.StockActual = ajuste.CantidadReal;

            // Guardamos el motivo en el historial
            var movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                ProductoId = ajuste.ProductoId,
                Cantidad = diferencia, // Guardamos cuánto se ajustó (+ o -)
                TipoMovimiento = "AJUSTE_INVENTARIO",
                Observacion = $"Ajuste Manual: {ajuste.Motivo}",
                Turno = "Administracion"
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Inventario corregido correctamente." });
        }

        // ==========================================
        // 3. NUEVO: REGISTRAR REMITO (Salida Masiva) 🚚
        // ==========================================
        [HttpPost("registrar-remito")]
        public async Task<IActionResult> RegistrarRemito([FromBody] NuevoRemitoDto data)
        {
            if (data.Items == null || data.Items.Count == 0)
                return BadRequest("El remito no tiene ítems.");

            // Usamos transacción para asegurar que o se descuentan TODOS o NINGUNO
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                foreach (var item in data.Items)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);

                    if (producto == null)
                        throw new Exception($"El producto ID {item.ProductoId} no existe.");

                    // Validación de Stock (Opcional: quitar si quieres permitir negativo)
                    if (producto.StockActual < item.Cantidad)
                        throw new Exception($"Stock insuficiente para '{producto.Nombre}'. Hay {producto.StockActual}, intentas sacar {item.Cantidad}.");

                    // 1. Descuento de Stock
                    producto.StockActual -= item.Cantidad;

                    // 2. Registro de Movimiento
                    var movimiento = new Movimiento
                    {
                        Fecha = DateTime.Now,
                        ProductoId = item.ProductoId,
                        Cantidad = -item.Cantidad, // Negativo porque sale
                        TipoMovimiento = "VENTA_REMITO",
                        Observacion = $"Cliente: {data.Cliente} | Remito: {data.NumeroRemito}",
                        Turno = "Despacho"
                    };
                    _context.Movimientos.Add(movimiento);
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Remito registrado y stock actualizado correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Devolvemos BadRequest con el mensaje del error (ej: Stock insuficiente)
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // ==========================================
        // MÉTODOS DE LECTURA (GET)
        // ==========================================
        [HttpGet("inventario")]
        public async Task<ActionResult<IEnumerable<object>>> GetInventarioCompleto()
        {
            var inventario = await _context.Productos
                .Where(p => p.Activo == true)
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.EsProductoTerminado, // Necesario para el filtro en Despacho
                    Categoria = p.EsMateriaPrima ? "Materia Prima" : "Producto Terminado",
                    p.StockActual,
                    p.StockMinimo,
                    p.PrecioCosto,
                    Estado = p.StockActual <= p.StockMinimo ? "CRITICO" : "NORMAL",
                    ValorTotal = p.StockActual * p.PrecioCosto
                })
                .ToListAsync();

            return Ok(inventario);
        }

        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima == true)
                .Select(p => new { p.Id, p.Nombre, p.StockActual, p.CodigoSku })
                .ToListAsync();
        }

        // ==========================================
        // CLASES DTO (Data Transfer Objects)
        // ==========================================
        public class IngresoDto
        {
            public int ProductoId { get; set; }
            public decimal Cantidad { get; set; }
            public decimal NuevoPrecio { get; set; }
            public string? Proveedor { get; set; }
        }

        public class AjusteDto
        {
            public int ProductoId { get; set; }
            public decimal CantidadReal { get; set; }
            public string Motivo { get; set; }
        }

        // Nuevos DTOs para Remitos
        public class NuevoRemitoDto
        {
            public string Cliente { get; set; }
            public string NumeroRemito { get; set; }
            public List<ItemRemitoDto> Items { get; set; }
        }

        public class ItemRemitoDto
        {
            public int ProductoId { get; set; }
            public decimal Cantidad { get; set; }
        }
    }
}