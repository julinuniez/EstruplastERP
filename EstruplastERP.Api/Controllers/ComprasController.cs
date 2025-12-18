using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComprasController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ComprasController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarCompra([FromBody] RegistrarCompraDto dto)
        {
            // 1. Validaciones
            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");

            // 2. Crear el Movimiento
            var movimiento = new Movimiento
            {
                Fecha = DateTime.Now,
                ProductoId = dto.ProductoId,
                ProveedorId = dto.ProveedorId,
                Cantidad = dto.Cantidad,

                // PRECIO E HISTORIAL
                PrecioUnitario = dto.PrecioUnitario,
                PrecioTotal = dto.Cantidad * dto.PrecioUnitario,

                // TRAZABILIDAD
                LoteProveedor = dto.Lote, // Asegúrate de haber agregado este campo a la entidad Movimiento
                NumeroRemito = dto.NumeroRemito,

                TipoMovimiento = "COMPRA", // Con este texto ya sabemos que fue una entrada
                Observacion = dto.Observacion ?? "Ingreso de Mercadería"

                // BORRAMOS LA LÍNEA 'EsIngreso = true'
            };

            // 3. Actualizar Stock (Aquí es donde definimos que SUMA)
            producto.StockActual += dto.Cantidad;

            // 4. Guardar con Transacción
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                _context.Movimientos.Add(movimiento);
                _context.Productos.Update(producto);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = "Compra registrada correctamente.",
                    nuevoStock = producto.StockActual
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                // Loguear el error real en consola para depurar
                Console.WriteLine(ex.ToString());
                return StatusCode(500, "Ocurrió un error al guardar la compra.");
            }
        }
    }
}