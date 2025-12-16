using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RemitosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RemitosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ================================================================
        // 1. POST: CREAR NUEVO REMITO
        // ================================================================
        [HttpPost]
        public async Task<ActionResult> PostRemito(NuevoRemitoDto dto)
        {
            // A. Validaciones
            foreach (var itemDto in dto.Items)
            {
                var producto = await _context.Productos.FindAsync(itemDto.ProductoId);

                if (producto == null)
                    return BadRequest($"Producto ID {itemDto.ProductoId} no existe.");

                // Usamos decimal para comparar correctamente
                if (producto.StockActual < itemDto.Cantidad)
                {
                    return BadRequest($"Stock insuficiente para '{producto.Nombre}'. Tienes {producto.StockActual}, intentas sacar {itemDto.Cantidad}.");
                }
            }

            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null) return BadRequest("Cliente no encontrado.");

            // B. Crear la Cabecera
            var nuevoRemito = new Remito
            {
                ClienteId = dto.ClienteId,
                NumeroRemito = dto.NumeroRemito,
                // Si no envía fecha, usa Ahora. Si envía, usa la seleccionada.
                Fecha = dto.Fecha != default ? dto.Fecha : DateTime.Now,
                Observacion = dto.Observacion,
                ClienteNombre = cliente.RazonSocial,
                Detalles = new List<RemitoDetalle>()
            };

            // C. Procesar Items
            foreach (var itemDto in dto.Items)
            {
                var producto = await _context.Productos.FindAsync(itemDto.ProductoId);

                // 1. Descontar Stock Físico
                producto.StockActual -= itemDto.Cantidad;

                // 2. Crear Detalle (Aquí guardamos el color/medida)
                nuevoRemito.Detalles.Add(new RemitoDetalle
                {
                    ProductoId = itemDto.ProductoId,
                    Cantidad = itemDto.Cantidad,
                    Detalle = itemDto.Detalle, // <--- CAMPO CLAVE
                    PrecioUnitarioSnapshot = 0
                });

                // Opcional: Registrar movimiento en historial general si lo usas
                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = producto.Id,
                    Cantidad = -itemDto.Cantidad, // Negativo porque sale
                    TipoMovimiento = $"SALIDA_REMITO {dto.NumeroRemito}",
                    Observacion = $"Cliente: {cliente.RazonSocial} | {itemDto.Detalle}",
                    Turno = "Despacho"
                });
            }

            // D. Guardar todo
            _context.Remitos.Add(nuevoRemito);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Éxito", id = nuevoRemito.Id });
        }

        // ================================================================
        // 2. GET: HISTORIAL
        // ================================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorial()
        {
            var remitos = await _context.Remitos
                .Include(r => r.Cliente)
                .Include(r => r.Detalles).ThenInclude(d => d.Producto)
                .OrderByDescending(r => r.Fecha)
                .ToListAsync();

            // Mapeamos a un objeto anónimo para enviar al Frontend
            var resultado = remitos.Select(r => new
            {
                r.Id,
                r.NumeroRemito,
                r.Fecha,
                ClienteNombre = r.Cliente != null ? r.Cliente.RazonSocial : "---",
                r.Observacion,
                TotalItems = r.Detalles.Count,
                TotalKilos = r.Detalles.Sum(d => d.Cantidad),
                Items = r.Detalles.Select(d => new {
                    ProductoNombre = d.Producto.Nombre,
                    Sku = d.Producto.CodigoSku,
                    Cantidad = d.Cantidad,
                    Detalle = d.Detalle // <--- Enviamos el color al historial
                }).ToList()
            });

            return Ok(resultado);
        }
    }

    public class NuevoRemitoDto
    {
        public int ClienteId { get; set; }
        public string NumeroRemito { get; set; }
        public DateTime Fecha { get; set; }
        public string Observacion { get; set; }
        public List<ItemRemitoDto> Items { get; set; }
    }

    public class ItemRemitoDto
    {
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; } // Decimal es clave para evitar errores
        public string Detalle { get; set; }   // Color, medida, etc.
    }
}