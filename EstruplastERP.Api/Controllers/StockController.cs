using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos; // ✅ Importamos el archivo del PASO 1

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

        // 1. INGRESO DE MERCADERÍA
        [HttpPost("ingresar")]
        public async Task<IActionResult> IngresarStock([FromBody] IngresoDto ingreso)
        {
            if (ingreso.Cantidad <= 0) return BadRequest("La cantidad debe ser positiva.");

            var producto = await _context.Productos.FindAsync(ingreso.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");

            // Actualizar Precio
            if (ingreso.NuevoPrecio > 0) producto.PrecioCosto = ingreso.NuevoPrecio;

            // Sumar Stock
            producto.StockActual += ingreso.Cantidad;

            // Movimiento
            _context.Movimientos.Add(new Movimiento
            {
                Fecha = DateTime.Now,
                ProductoId = ingreso.ProductoId,
                Cantidad = ingreso.Cantidad,
                TipoMovimiento = "COMPRA_INSUMO",
                Observacion = $"Prov: {ingreso.Proveedor}",
                Turno = "General"
            });

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Stock actualizado", nuevoStock = producto.StockActual });
        }

        [HttpPost("ajuste")]
        public async Task<IActionResult> AjustarStock([FromBody] AjusteDto ajuste)
        {
            Console.WriteLine($"--> 1. INICIO: ID={ajuste.ProductoId}, PrecioNuevo={ajuste.NuevoPrecio}");

            var producto = await _context.Productos.FindAsync(ajuste.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");

            // Imprimimos el precio viejo para comparar
            Console.WriteLine($"--> 2. PRECIO ACTUAL DB: {producto.PrecioCosto}");

            bool huboCambios = false;

            // ---------------------------------------------------------
            // 1. ACTUALIZAR PRECIO (FORZADO)
            // ---------------------------------------------------------
            if (ajuste.NuevoPrecio.HasValue && ajuste.NuevoPrecio.Value >= 0)
            {
                // Asignamos el valor directamente
                producto.PrecioCosto = ajuste.NuevoPrecio.Value;

                // 🔥 ESTA LÍNEA ES LA CLAVE: OBLIGAMOS A LA BD A MARCARLO COMO SUCIO
                _context.Entry(producto).Property(p => p.PrecioCosto).IsModified = true;

                huboCambios = true;
                Console.WriteLine($"--> 3. CAMBIO PRECIO APLICADO: {producto.PrecioCosto}");
            }

            // ---------------------------------------------------------
            // 2. ACTUALIZAR STOCK
            // ---------------------------------------------------------
            decimal diferencia = ajuste.CantidadReal - producto.StockActual;

            if (diferencia != 0)
            {
                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = ajuste.ProductoId,
                    Cantidad = diferencia,
                    TipoMovimiento = "AJUSTE_INVENTARIO",
                    Observacion = $"Ajuste: {ajuste.Motivo}",
                    Turno = "Administracion",
                    PrecioUnitario = producto.PrecioCosto,
                    PrecioTotal = producto.PrecioCosto * Math.Abs(diferencia)
                });

                producto.StockActual = ajuste.CantidadReal;
                _context.Entry(producto).Property(p => p.StockActual).IsModified = true; // Forzamos también el stock
                huboCambios = true;
            }

            // ---------------------------------------------------------
            // 3. GUARDAR CAMBIOS (CON DEBUG)
            // ---------------------------------------------------------
            if (huboCambios)
            {
                try
                {
                    // Guardamos y capturamos cuántas filas se tocaron
                    int filasAfectadas = await _context.SaveChangesAsync();

                    Console.WriteLine($"--> 4. EXITOSO: Se modificaron {filasAfectadas} registros en la BD.");

                    return Ok(new { mensaje = "Datos actualizados correctamente." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> ERROR BD: {ex.Message}");
                    if (ex.InnerException != null) Console.WriteLine($"--> INNER: {ex.InnerException.Message}");

                    return StatusCode(500, "Error al guardar en base de datos: " + ex.Message);
                }
            }
            else
            {
                Console.WriteLine("--> SIN CAMBIOS: El sistema no detectó diferencias.");
                return Ok(new { mensaje = "No se detectaron cambios necesarios." });
            }
        }

        // MÉTODOS GET
        [HttpGet("inventario")]
        public async Task<ActionResult<IEnumerable<object>>> GetInventarioCompleto()
        {
            return await _context.Productos
                .Where(p => p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.EsProductoTerminado,
                    p.StockActual,
                    p.StockMinimo,
                    p.PrecioCosto
                })
                .ToListAsync();
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
}