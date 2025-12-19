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
        // 1. POST: CREAR NUEVO REMITO (DESPACHO)
        // ================================================================
        [HttpPost]
        public async Task<ActionResult> PostRemito(NuevoRemitoDto dto)
        {
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
                    Turno = "Despacho",
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