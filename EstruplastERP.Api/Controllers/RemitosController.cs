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
            // A. VALIDACIONES PREVIAS
            // -------------------------------------------------------------
            // Recorremos los items antes de hacer nada para verificar stock.
            // Si falta uno, no se hace nada.
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

            // B. CREAR LA CABECERA DEL REMITO
            // -------------------------------------------------------------
            var nuevoRemito = new Remito
            {
                ClienteId = dto.ClienteId,
                NumeroRemito = dto.NumeroRemito,
                // Si la fecha viene vacía (default), usamos Ahora.
                Fecha = dto.Fecha != default ? dto.Fecha : DateTime.Now,
                Observacion = dto.Observacion,

                // GUARDAMOS EL NOMBRE COMO TEXTO (Plan B por si borran el cliente)
                ClienteNombre = cliente.RazonSocial,

                Detalles = new List<RemitoDetalle>()
            };

            // C. PROCESAR ITEMS (DESCUENTO Y MOVIMIENTOS)
            // -------------------------------------------------------------
            foreach (var itemDto in dto.Items)
            {
                var producto = await _context.Productos.FindAsync(itemDto.ProductoId);

                // 1. Descontar Stock Físico
                producto.StockActual -= itemDto.Cantidad;

                // 2. Agregar al detalle del Remito
                nuevoRemito.Detalles.Add(new RemitoDetalle
                {
                    ProductoId = itemDto.ProductoId,
                    Cantidad = itemDto.Cantidad,
                    Detalle = itemDto.Detalle, // Ej: "Color Rojo"
                    PrecioUnitarioSnapshot = 0 // Si tuvieras lista de precios, iría aquí
                });

                // 3. Registrar en el Historial de Movimientos (Kardex)
                _context.Movimientos.Add(new Movimiento
                {
                    Fecha = DateTime.Now,
                    ProductoId = producto.Id,
                    Cantidad = itemDto.Cantidad, // Cantidad involucrada
                    TipoMovimiento = "SALIDA_REMITO", // String identificador
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

        // ================================================================
        // 2. GET: HISTORIAL COMPLETO
        // ================================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetHistorial()
        {
            var remitos = await _context.Remitos
                .Include(r => r.Cliente)               // Cargar datos del cliente
                .Include(r => r.Detalles)              // Cargar lista de items
                    .ThenInclude(d => d.Producto)      // Cargar nombres de productos
                .OrderByDescending(r => r.Fecha)
                .ThenByDescending(r => r.Id)
                .ToListAsync();

            // Proyección (Mapeo) de datos para el Frontend
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
                    r.Cliente.Cuit,       // <--- Para el PDF
                    r.Cliente.Direccion   // <--- Para el PDF
                },

                Items = r.Detalles.Select(d => new {
                    d.Id,
                    ProductoNombre = d.Producto != null ? d.Producto.Nombre : "Producto Eliminado",
                    Sku = d.Producto != null ? d.Producto.CodigoSku : "-",
                    d.Cantidad,
                    d.Detalle // El comentario específico (ej: "Bobina 40 micrones")
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