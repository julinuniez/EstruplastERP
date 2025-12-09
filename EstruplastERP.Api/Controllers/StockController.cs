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

        // DTOs
        public class IngresoDto
        {
            public int ProductoId { get; set; }
            public decimal Cantidad { get; set; }
            public decimal NuevoPrecio { get; set; } // <--- AGREGADO: Precio de compra
            public string? Proveedor { get; set; }
        }

        public class AjusteDto
        {
            public int ProductoId { get; set; }
            public decimal CantidadReal { get; set; } // Lo que contaste en el galpón
            public string Motivo { get; set; }
        }

        // 1. INGRESO DE MERCADERÍA (Compra) + ACTUALIZACIÓN DE PRECIO
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

        // 2. AJUSTE DE STOCK (Corrección Manual)
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

        // 3. GET INVENTARIO (Igual que antes)
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
                    Categoria = p.EsMateriaPrima ? "Materia Prima" : "Producto Terminado",
                    p.StockActual,
                    p.StockMinimo,
                    p.PrecioCosto, // <--- Agregamos esto para verlo en la tabla
                    Estado = p.StockActual <= p.StockMinimo ? "CRITICO" : "NORMAL",
                    ValorTotal = p.StockActual * p.PrecioCosto
                })
                .ToListAsync();

            return Ok(inventario);
        }

        // 4. GET MATERIAS PRIMAS (Igual que antes)
        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima == true)
                .Select(p => new { p.Id, p.Nombre, p.StockActual, p.CodigoSku })
                .ToListAsync();
        }
    }
}