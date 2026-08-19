using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ClientesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> GetClientes()
        {
            return await _context.Clientes
                .Where(c => c.Activo)
                .OrderBy(c => c.RazonSocial)
                .AsNoTracking()
                .ToListAsync();
        }

        // POST: api/Clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> PostCliente(Cliente cliente)
        {
            if (string.IsNullOrWhiteSpace(cliente.RazonSocial))
            {
                return BadRequest("La Razón Social es obligatoria.");
            }

            // Aseguramos valores por defecto
            cliente.Activo = true;

            // 🚀 PARACAÍDAS: Límite de pallets
            if (cliente.LimiteKilosPallet <= 0)
                cliente.LimiteKilosPallet = 1000m;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // PUT: api/Clientes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

            // 🚀 PARACAÍDAS: Límite de pallets
            if (cliente.LimiteKilosPallet <= 0)
                cliente.LimiteKilosPallet = 1000m;

            // Marcamos el estado como modificado para que EF actualice los campos
            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Clientes.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost("habilitar-fazon/{id}")]
        public async Task<IActionResult> HabilitarFazon(int id)
        {
            try
            {
                var cliente = await _context.Clientes.FindAsync(id);
                if (cliente == null) return NotFound("Cliente no encontrado.");

                if (cliente.EsFazon)
                {
                    return Ok(new { nuevo = false, mensaje = "ℹ️ Este cliente ya estaba habilitado para Fazón." });
                }

                cliente.EsFazon = true;
                await _context.SaveChangesAsync();

                return StatusCode(200, new { nuevo = true, mensaje = $"✅ Fazón habilitado correctamente para {cliente.RazonSocial}." });
            }
            catch (Exception ex)
            {
                // 🔥 EL CHIVATO: Capturamos el error profundo de SQL o C#
                string errorReal = ex.Message;
                string errorProfundo = ex.InnerException?.Message ?? "No hay detalle extra.";

                return StatusCode(500, new
                {
                    mensaje = "Explotó el servidor. Mirá el detalle:",
                    error = errorReal,
                    detalle = errorProfundo
                });
            }
        }

        // --- AGREGAR ESTO DENTRO DE ClientesController ---

        [HttpGet("reporte-fazon/{clienteId}")]
        public async Task<IActionResult> GetReporteFazon(int clienteId)
        {
            var cliente = await _context.Clientes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == clienteId);
            if (cliente == null) return NotFound("Cliente no encontrado");

            // 1. Obtener el Inventario Actual del cliente
            // 1. Obtener el Inventario Actual del cliente
            var inventario = await _context.Productos
                .AsNoTracking()
                .Where(p => p.ClienteId == clienteId && p.Activo)
                .Select(p => new ItemInventarioFazonDto
                {
                    Sku = p.CodigoSku,
                    Nombre = p.Nombre,
                    Stock = Math.Round(p.StockActual, 2)
                })
                .ToListAsync();

            // 2. Obtener Movimientos (Vamos a ampliar el rango a 180 días para probar)
            var fechaLimite = DateTime.Today.AddDays(-180);

            var movimientos = await _context.Movimientos
                .AsNoTracking()
                .Include(m => m.Producto)
                // Usamos el ID directamente para evitar problemas de navegación si Producto es nulo
                .Where(m => m.Producto.ClienteId == clienteId && m.Fecha >= fechaLimite)
                .ToListAsync();

            // 🚀 CHIVATO PARA VISUAL STUDIO: 
            // Poné un punto de interrupción (F9) acá abajo y mira cuánto vale 'movimientos.Count'
            var totalEncontrados = movimientos.Count;

            // 3. Clasificar Ingresos y Egresos (Hacemos el filtro más flexible)
            var ingresos = movimientos
                .Where(m => m.Cantidad > 0 ||
                            m.TipoMovimiento.ToUpper().Contains("ING") ||
                            m.TipoMovimiento.ToUpper().Contains("ENTRADA"))
                .Select(m => new MovimientoFazonDto
                {
                    Fecha = m.Fecha.ToString("dd/MM/yyyy"),
                    Sku = m.Producto?.CodigoSku ?? "S/D",
                    Material = m.Producto?.Nombre ?? "S/D",
                    Kilos = Math.Round(Math.Abs(m.Cantidad), 2),
                    Tipo = m.TipoMovimiento
                }).ToList();

            var egresos = movimientos
                .Where(m => m.Cantidad < 0 ||
                            m.TipoMovimiento.ToUpper().Contains("EGR") ||
                            m.TipoMovimiento.ToUpper().Contains("SAL") ||
                            m.TipoMovimiento.ToUpper().Contains("CON"))
                .Select(m => new MovimientoFazonDto
                {
                    Fecha = m.Fecha.ToString("dd/MM/yyyy"),
                    Sku = m.Producto?.CodigoSku ?? "S/D",
                    Material = m.Producto?.Nombre ?? "S/D",
                    Kilos = Math.Round(Math.Abs(m.Cantidad), 2),
                    Tipo = m.TipoMovimiento
                }).ToList();

            return Ok(new ReporteFazonDto
            {
                ClienteNombre = cliente.RazonSocial,
                Inventario = inventario,
                Ingresos = ingresos,
                Egresos = egresos
            });
        }
    }
    public class ReporteFazonDto
    {
        public string ClienteNombre { get; set; } = string.Empty;
        public List<ItemInventarioFazonDto> Inventario { get; set; } = new();
        public List<MovimientoFazonDto> Ingresos { get; set; } = new();
        public List<MovimientoFazonDto> Egresos { get; set; } = new();
    }

    public class ItemInventarioFazonDto
    {
        public string Sku { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public decimal Stock { get; set; }
    }

    public class MovimientoFazonDto
    {
        public string Fecha { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string Material { get; set; } = string.Empty;
        public decimal Kilos { get; set; }
        public string Tipo { get; set; } = string.Empty;
    }

}