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

        // 2. AJUSTE DE STOCK
        [HttpPost("ajuste")]
        public async Task<IActionResult> AjustarStock([FromBody] AjusteDto ajuste)
        {
            var producto = await _context.Productos.FindAsync(ajuste.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");

            decimal diferencia = ajuste.CantidadReal - producto.StockActual;
            if (diferencia == 0) return Ok(new { mensaje = "Stock sin cambios." });

            producto.StockActual = ajuste.CantidadReal;

            _context.Movimientos.Add(new Movimiento
            {
                Fecha = DateTime.Now,
                ProductoId = ajuste.ProductoId,
                Cantidad = diferencia,
                TipoMovimiento = "AJUSTE_INVENTARIO",
                Observacion = $"Ajuste Manual: {ajuste.Motivo}",
                Turno = "Administracion"
            });

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Inventario corregido." });
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
    // ❌ AQUÍ YA NO HAY CLASES DEFINIDAS, SE USAN LAS DE LA CARPETA DTOS
}