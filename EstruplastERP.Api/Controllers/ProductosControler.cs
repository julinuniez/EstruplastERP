using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProductosController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("inventario-completo")]
        public async Task<ActionResult<IEnumerable<object>>> GetInventarioCompleto()
        {
            var productos = await _context.Productos
                .Where(p => p.Activo) 
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.StockActual,
                    p.StockMinimo,
                    p.PesoEspecifico,
                    p.EsMateriaPrima,       
                    p.EsProductoTerminado,  
                    p.EsFazon,
                    p.PrecioCosto
                })
                .ToListAsync();

            return Ok(productos);
        }

        // ==========================================
        // 2. GET: MATERIAS PRIMAS
        // ==========================================
        [HttpGet("materias-primas")]
        public async Task<ActionResult<IEnumerable<object>>> GetMateriasPrimas()
        {
            return await _context.Productos
                .Where(p => p.EsMateriaPrima && p.Activo)
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.PesoEspecifico,
                    p.StockActual
                })
                .ToListAsync();
        }

        // ==========================================
        // 3. GET: TODOS LOS PRODUCTOS (CORREGIDO EL FALLO)
        // ==========================================
        // GET: api/Productos?clienteId=5
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetProductos([FromQuery] int? clienteId)
        {
            
            var query = _context.Productos
                .Where(p => p.Activo)
                .AsQueryable();
            if (clienteId.HasValue && clienteId.Value > 0)
            {
              
                query = query.Where(p => p.ClienteId == null || p.ClienteId == clienteId);
            }
            else
            {
                
                query = query.Where(p => p.ClienteId == null);
            }
            var productos = await query
                .OrderBy(p => p.Nombre)
                .Select(p => new
                {
                    p.Id,
                    p.Nombre,
                    p.CodigoSku,
                    p.EsProductoTerminado,
                    p.EsMateriaPrima,
                    p.EsGenerico,
                    EsFazon = p.ClienteId != null,
                    p.StockActual,
                    p.PesoEspecifico,
                    p.Largo,
                    p.Ancho,
                    p.Espesor,
                    p.Color
                })
                .ToListAsync();

            return Ok(productos);
        }

        // ==========================================
        // 4. GET: UN PRODUCTO (Para Edición)
        // ==========================================
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDetalleDto>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Include(p => p.Formulas)
                .ThenInclude(f => f.MateriaPrima)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (producto == null) return NotFound("❌ Producto no encontrado.");

            // =========================================================
            // 2. LÓGICA DE HERENCIA DE RECETA (Cascade)
            // =========================================================
            List<Formula> formulasFinales;

            if (producto.Formulas != null && producto.Formulas.Any())
            {
                formulasFinales = producto.Formulas.ToList();
            }
            else if (producto.ProductoPadreId != null)
            {
                formulasFinales = await _context.Formulas
                    .Include(f => f.MateriaPrima)
                    .Where(f => f.ProductoTerminadoId == producto.ProductoPadreId)
                    .ToListAsync();
            }
            // C. Si no tiene nada, lista vacía.
            else
            {
                formulasFinales = new List<Formula>();
            }

            // =========================================================
            // 3. MAPEO A DTO
            // =========================================================
            var dto = new ProductoDetalleDto
            {
                Id = producto.Id,
                Nombre = producto.Nombre,
                CodigoSku = producto.CodigoSku,
                StockActual = producto.StockActual,
                PrecioCosto = producto.PrecioCosto,
                StockMinimo = producto.StockMinimo,
                Largo = producto.Largo,
                Ancho = producto.Ancho,
                Espesor = producto.Espesor,
                PesoEspecifico = producto.PesoEspecifico,
                Color = producto.Color,
                EsProductoTerminado = producto.EsProductoTerminado,
                EsMateriaPrima = producto.EsMateriaPrima,
                EsGenerico = producto.EsGenerico,

                // Usamos la lista 'formulasFinales' que calculamos arriba
                Receta = formulasFinales.Select(f => new IngredienteDto
                {
                    MateriaPrimaId = f.MateriaPrimaId,
                    NombreInsumo = f.MateriaPrima?.Nombre ?? "(MP No Encontrada)",
                    Cantidad = f.Cantidad
                }).ToList()
            };

            return dto;
        }

        // ==========================================
        // 5. POST: CREAR
        // ==========================================
        [HttpPost("crear")]
        public async Task<IActionResult> CrearProductoConReceta([FromBody] NuevoProductoDto data)
        {
            if (string.IsNullOrWhiteSpace(data.Nombre) || string.IsNullOrWhiteSpace(data.CodigoSku))
                return BadRequest("❌ Nombre y SKU son obligatorios.");

            if (await _context.Productos.AnyAsync(p => p.CodigoSku == data.CodigoSku))
                return BadRequest("❌ El Código SKU ya existe.");

            bool esProductoTerminado = data.Receta != null && data.Receta.Count > 0;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var nuevoProducto = new Producto
                {
                    Nombre = data.Nombre.Trim(),
                    CodigoSku = data.CodigoSku.Trim().ToUpper(),
                    EsProductoTerminado = esProductoTerminado,
                    EsMateriaPrima = !esProductoTerminado,
                    // Si viene en el DTO úsalo, si no, asume false
                    // EsGenerico = data.EsGenerico, 
                    StockMinimo = data.StockMinimo,
                    PrecioCosto = data.PrecioCosto,
                    Color = data.Color,
                    StockActual = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Productos.Add(nuevoProducto);
                await _context.SaveChangesAsync();

                if (esProductoTerminado && data.Receta != null)
                {
                    foreach (var item in data.Receta)
                    {
                        _context.Formulas.Add(new Formula
                        {
                            ProductoTerminadoId = nuevoProducto.Id,
                            MateriaPrimaId = item.MateriaPrimaId,
                            Cantidad = item.Cantidad
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return Ok(new { mensaje = "✅ Ítem creado correctamente.", id = nuevoProducto.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error en servidor: " + ex.Message);
            }
        }


        [HttpPost("habilitar-fazon/{clienteId}")]
        public async Task<IActionResult> HabilitarFazonCliente(int clienteId)
        {
            // 1. Validar que el cliente exista
            var cliente = await _context.Clientes.FindAsync(clienteId);
            if (cliente == null) return NotFound("❌ El cliente no existe.");

            // 2. Validar que no tenga ya su "Bolsa de Material" creada
            // Buscamos si existe alguna Materia Prima vinculada a este ClienteId
            var existeMaterial = await _context.Productos
                .AnyAsync(p => p.ClienteId == clienteId && p.EsMateriaPrima);

            if (existeMaterial)
                return BadRequest("⚠️ Este cliente ya tiene habilitado el servicio de Fazon (Ya tiene stock asignado).");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 3. Crear la Materia Prima "Cuenta Corriente"
                var nuevoMaterial = new Producto
                {
                    // Nombre claro para identificarlo
                    Nombre = $"Material Recuperado: {cliente.RazonSocial}",

                    // SKU automático: MP-CLI-005 (rellena con ceros)
                    CodigoSku = $"MP-CLI-{clienteId.ToString("D3")}",

                    EsMateriaPrima = true,       // Se consume
                    EsProductoTerminado = false, // No se vende directamente
                    EsGenerico = false,          // Es un ítem único
                    ClienteId = clienteId,       // ✅ VINCULADO AL CLIENTE (Candado)

                    StockActual = 0,             // Empieza en 0
                    StockMinimo = 0,
                    PrecioCosto = 0,             // El material es del cliente, costo 0 para nosotros
                    PesoEspecifico = 1.05m,      // Valor por defecto (PAI), editable luego si es otro material

                    Activo = true,
                    FechaCreacion = DateTime.Now
                };

                _context.Productos.Add(nuevoMaterial);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new
                {
                    mensaje = $"✅ Fazon habilitado para {cliente.RazonSocial}.",
                    productoId = nuevoMaterial.Id,
                    nombreMaterial = nuevoMaterial.Nombre
                });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al crear material: " + ex.Message);
            }
        }
        // ==========================================
        // 6. PUT: ACTUALIZAR
        // ==========================================
        [HttpPut("actualizar/{id}")]
        public async Task<IActionResult> ActualizarProducto(int id, [FromBody] ProductoEditarDto data)
        {
            if (id <= 0) return BadRequest("ID inválido.");

            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.Id == id);
            if (producto == null) return NotFound("❌ Producto no encontrado.");

            producto.Nombre = data.Nombre.Trim();
            producto.CodigoSku = data.CodigoSku.Trim().ToUpper();
            producto.StockMinimo = data.StockMinimo;
            producto.Color = data.Color;
            // producto.EsGenerico = data.EsGenerico; // Descomentar si agregas esto al DTO

            try
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "✅ Ítem actualizado correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al actualizar: " + ex.Message);
            }
        }

        // ==========================================
        // 7. DELETE: ELIMINAR
        // ==========================================
        [HttpDelete("eliminar/{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound("Producto no encontrado.");

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                bool esIngredienteDeOtros = await _context.Formulas.AnyAsync(f => f.MateriaPrimaId == id);
                if (esIngredienteDeOtros)
                    return BadRequest(new { mensaje = "❌ No se puede eliminar: Es ingrediente de otro producto." });

                var movimientos = await _context.Movimientos.Where(m => m.ProductoId == id).ToListAsync();
                if (movimientos.Any()) _context.Movimientos.RemoveRange(movimientos);

                var formulasDelProducto = await _context.Formulas.Where(f => f.ProductoTerminadoId == id).ToListAsync();
                if (formulasDelProducto.Any()) _context.Formulas.RemoveRange(formulasDelProducto);

                _context.Productos.Remove(producto);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Ok(new { mensaje = "✅ Producto eliminado correctamente." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, new { mensaje = "Error crítico al eliminar." });
            }
        }

        // ==========================================
        // 8. PUT: CONFIGURACIÓN TÉCNICA (Peso, Tipos)
        // ==========================================
        [HttpPut("configurar/{id}")]
        public async Task<IActionResult> ConfigurarProducto(int id, [FromBody] ProductoConfigDto dto)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return NotFound("Producto no encontrado");

            // 1. Actualizar Datos Técnicos
            producto.StockMinimo = dto.StockMinimo;
            producto.PesoEspecifico = dto.PesoEspecifico;
            producto.EsMateriaPrima = dto.EsMateriaPrima;
            producto.EsProductoTerminado = dto.EsProductoTerminado;
            producto.EsFazon = dto.EsFazon;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 2. ACTUALIZAR RECETA (Borrar vieja e insertar nueva)
                if (dto.Receta != null)
                {
                    // A. Borramos los ingredientes actuales de este producto
                    var formulasViejas = await _context.Formulas
                        .Where(f => f.ProductoTerminadoId == id)
                        .ToListAsync();

                    _context.Formulas.RemoveRange(formulasViejas);

                    // B. Insertamos los nuevos
                    foreach (var item in dto.Receta)
                    {
                        _context.Formulas.Add(new Formula
                        {
                            ProductoTerminadoId = id,
                            MateriaPrimaId = item.MateriaPrimaId,
                            Cantidad = item.Cantidad
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync(); // Confirmar todo

                return Ok(new { mensaje = "✅ Configuración y Receta guardadas." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, "Error al guardar: " + ex.Message);
            }
        }
    }
}