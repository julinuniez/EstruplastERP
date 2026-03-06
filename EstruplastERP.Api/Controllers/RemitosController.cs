using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text.RegularExpressions; // Necesario para Regex

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
        // 1. POST: CREAR NUEVO REMITO (CON VALIDACIÓN DE FORMATO)
        // ================================================================
        [HttpPost]
        public async Task<ActionResult> PostRemito(NuevoRemitoDto dto)
        {
            // -------------------------------------------------------------
            // 🔥 1. VALIDACIÓN DE FORMATO (0000-00000000)
            // -------------------------------------------------------------
            if (string.IsNullOrWhiteSpace(dto.NumeroRemito))
                return BadRequest("El número de remito es obligatorio.");

            // Expresión Regular: 4 dígitos, un guion, 8 dígitos.
            var regexRemito = new Regex(@"^\d{4}-\d{8}$");

            if (!regexRemito.IsMatch(dto.NumeroRemito))
            {
                return BadRequest("El formato del remito es inválido. Debe ser: 0000-00000000 (Ej: 0001-00001234).");
            }

            // -------------------------------------------------------------
            // 🔥 2. VALIDACIÓN DE DUPLICADOS (Opcional pero recomendado)
            // Evita que se cargue el mismo remito dos veces para el mismo cliente
            // -------------------------------------------------------------
            bool existe = await _context.Remitos.AnyAsync(r => r.NumeroRemito == dto.NumeroRemito && r.ClienteId == dto.ClienteId);
            if (existe)
            {
                return BadRequest($"El remito {dto.NumeroRemito} ya existe para este cliente.");
            }

            // -------------------------------------------------------------
            // 3. VALIDACIÓN DE STOCK
            // -------------------------------------------------------------
            foreach (var itemDto in dto.Items)
            {
                var producto = await _context.Productos.FindAsync(itemDto.ProductoId);

                if (producto == null)
                    return BadRequest($"El producto ID {itemDto.ProductoId} no existe.");

                if (producto.StockActual < itemDto.Cantidad)
                {
                    return BadRequest($"Stock insuficiente para '{producto.Nombre}'. Stock actual: {producto.StockActual}kg. Intentas despachar: {itemDto.Cantidad}kg.");
                }
            }

            var cliente = await _context.Clientes.FindAsync(dto.ClienteId);
            if (cliente == null) return BadRequest("El cliente seleccionado no existe.");

            var nuevoRemito = new Remito
            {
                ClienteId = dto.ClienteId,
                NumeroRemito = dto.NumeroRemito,
                Fecha = dto.Fecha != default ? dto.Fecha : DateTime.Now,
                Observacion = dto.Observacion,
                ClienteNombre = cliente.RazonSocial,
                Detalles = new List<RemitoDetalle>()
            };

            foreach (var itemDto in dto.Items)
            {
                var producto = await _context.Productos.FindAsync(itemDto.ProductoId);

                // Descuento de Stock
                producto.StockActual -= itemDto.Cantidad;

                nuevoRemito.Detalles.Add(new RemitoDetalle
                {
                    ProductoId = itemDto.ProductoId,
                    Cantidad = itemDto.Cantidad,
                    Detalle = itemDto.Detalle,
                    PrecioUnitarioSnapshot = 0
                });

                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = producto.Id,
                    Cantidad = itemDto.Cantidad,
                    TipoMovimiento = "SALIDA_REMITO",
                    Observacion = $"Remito #{dto.NumeroRemito} -> {cliente.RazonSocial}. ({itemDto.Detalle})",
                    ClienteId = cliente.Id,
                    PrecioUnitario = 0,
                    PrecioTotal = 0
                });
            }

            _context.Remitos.Add(nuevoRemito);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Remito generado con éxito", id = nuevoRemito.Id });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorial()
        {
            var remitos = await _context.Remitos
                .Include(r => r.Cliente)
                .Include(r => r.Detalles)
                    .ThenInclude(d => d.Producto)
                .OrderByDescending(r => r.Fecha)
                .ThenByDescending(r => r.Id)
                .ToListAsync();

            var resultado = remitos.Select(r => new
            {
                r.Id,
                r.NumeroRemito,
                r.Fecha,
                r.Observacion,

                ClienteNombreBackup = r.ClienteNombre,

                Cliente = r.Cliente == null ? null : new
                {
                    r.Cliente.Id,
                    r.Cliente.RazonSocial,
                    r.Cliente.Cuit,
                    r.Cliente.Direccion
                },

                Items = r.Detalles.Select(d => new {
                    d.Id,
                    ProductoNombre = d.Producto != null ? d.Producto.Nombre : "Producto Eliminado",
                    Sku = d.Producto != null ? d.Producto.CodigoSku : "-",
                    d.Cantidad,
                    d.Detalle
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
        public decimal Cantidad { get; set; }
        public string Detalle { get; set; }
    }
}