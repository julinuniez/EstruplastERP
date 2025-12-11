using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos; // Si ya creaste el archivo en DTOs, bien. Si no, usa las clases de abajo.

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

            // Calculamos la diferencia
            decimal diferencia = ajuste.CantidadReal - producto.StockActual;

            if (diferencia == 0) return Ok(new { mensaje = "El stock ya coincide, no se hicieron cambios." });

            // Actualizamos el stock
            producto.StockActual = ajuste.CantidadReal;

            // Guardamos el motivo en el historial
            var movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                ProductoId = ajuste.ProductoId,
                Cantidad = diferencia,
                TipoMovimiento = "AJUSTE_INVENTARIO",
                Observacion = $"Ajuste Manual: {ajuste.Motivo}",
                Turno = "Administracion"
            };

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Inventario corregido correctamente." });
        }

        // ==========================================
        // 3. REGISTRAR REMITO (Salida Masiva)
        // ==========================================
        [HttpPost("registrar-remito")]
        public async Task<IActionResult> RegistrarRemito([FromBody] NuevoRemitoDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest("El remito no tiene ítems.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Crear la cabecera del Remito
                var nuevoRemito = new Remito
                {
                    Fecha = DateTime.Now,
                    NumeroRemito = dto.NumeroRemito,
                    ClienteNombre = dto.Cliente
                };
                _context.Remitos.Add(nuevoRemito);
                await _context.SaveChangesAsync(); // Guardamos para obtener el ID

                // 2. Procesar cada item
                foreach (var item in dto.Items)
                {
                    var producto = await _context.Productos.FindAsync(item.ProductoId);

                    if (producto == null)
                        throw new Exception($"Producto ID {item.ProductoId} no existe.");

                    // Convertimos a decimal para comparar (tu base de datos usa decimal)
                    decimal cantidadRequerida = (decimal)item.Cantidad;

                    if (producto.StockActual < cantidadRequerida)
                        throw new Exception($"Stock insuficiente para {producto.Nombre}. Tienes {producto.StockActual}, intentas sacar {cantidadRequerida}.");

                    // A. Restar Stock
                    producto.StockActual -= cantidadRequerida;

                    // B. Guardar Detalle del Remito
                    var detalle = new RemitoDetalle
                    {
                        RemitoId = nuevoRemito.Id,
                        ProductoId = producto.Id,
                        Cantidad = (double)cantidadRequerida // Convertimos si tu modelo usa double
                    };
                    _context.RemitoDetalles.Add(detalle);

                    // C. Registrar Movimiento
                    _context.Movimientos.Add(new Movimiento
                    {
                        Fecha = DateTime.Now,
                        ProductoId = producto.Id,
                        Cantidad = -cantidadRequerida, // Negativo
                        TipoMovimiento = $"SALIDA - Remito {dto.NumeroRemito}"
                    });
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "Remito registrado y stock actualizado." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // ==========================================
        // MÉTODOS GET AUXILIARES
        // ==========================================
        [HttpGet("inventario")]
        public async Task<ActionResult<IEnumerable<object>>> GetInventarioCompleto()
        {
            var inventario = await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.EsProductoTerminado,
                    p.StockActual,
                    p.StockMinimo,
                    p.PrecioCosto
                })
                .ToListAsync();

            return Ok(inventario);
        }

        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima && p.Activo)
                .Select(p => new { p.Id, p.Nombre, p.StockActual })
                .ToListAsync();
        }
    }

    // ==========================================
    // DTOs LOCALES (Si no tienes el archivo separado)
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

    // Estos son los que te faltaban:
    public class NuevoRemitoDto
    {
        public string Cliente { get; set; }
        public string NumeroRemito { get; set; }
        public List<ItemRemitoDto> Items { get; set; }
    }

    public class ItemRemitoDto
    {
        public int ProductoId { get; set; }
        public double Cantidad { get; set; }
    }
}