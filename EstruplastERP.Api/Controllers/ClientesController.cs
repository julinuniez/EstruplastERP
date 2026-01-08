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

            cliente.Activo = true;

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        [HttpPost("habilitar-fazon/{id}")]
        public async Task<IActionResult> HabilitarFazon(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound("Cliente no encontrado.");

            // ==========================================
            // 1. LISTA DE MATERIAS PRIMAS (Vírgenes)
            // ==========================================
            var materialesFazon = new[]
            {
        new { Codigo = "AI-FIN", Nombre = "A.I. FINO (FAZÓN)", FamiliaId = 11 },
        new { Codigo = "AI-GRU", Nombre = "A.I. GRUESO (FAZÓN)", FamiliaId = 12 },
        new { Codigo = "AI-BIC", Nombre = "A.I. BICAPA (FAZÓN)", FamiliaId = 13 },
        new { Codigo = "ABS-GRU", Nombre = "ABS GRUESO (FAZÓN)", FamiliaId = 21 },
        new { Codigo = "POLI-FIN", Nombre = "PEAD/PP/BIO FINO (FAZÓN)", FamiliaId = 31 },
        new { Codigo = "POLI-GRU", Nombre = "PEAD/PP/BIO GRUESO (FAZÓN)", FamiliaId = 32 },
        new { Codigo = "PEAD-BIC", Nombre = "PEAD BICAPA (FAZÓN)", FamiliaId = 41 }
    };

            // ==========================================
            // 2. LISTA DE SCRAP (Molido/Recuperado)
            // ==========================================
            var materialesScrap = new[]
            {
        // El Scrap suele mantener la familia base (10, 20, 30) o la específica si prefieres
        new { Codigo = "SCRAP-AI", Nombre = "SCRAP A.I. (MOLIDO)", FamiliaId = 10 },
        new { Codigo = "SCRAP-ABS", Nombre = "SCRAP ABS (MOLIDO)", FamiliaId = 20 },
        new { Codigo = "SCRAP-POLI", Nombre = "SCRAP POLIETILENO (MOLIDO)", FamiliaId = 30 },
        new { Codigo = "SCRAP-PEAD", Nombre = "SCRAP PEAD (MOLIDO)", FamiliaId = 40 }
    };

            int creados = 0;

            // --- BUCLE 1: CREAR MP VIRGEN ---
            foreach (var mat in materialesFazon)
            {
                string sku = $"MP-CLI-{cliente.Id}-{mat.Codigo}";
                if (!await _context.Productos.AnyAsync(p => p.CodigoSku == sku))
                {
                    _context.Productos.Add(new Producto
                    {
                        Nombre = $"MP {mat.Nombre} - PROPIEDAD DE {cliente.RazonSocial.ToUpper()}",
                        CodigoSku = sku,
                        FamiliaId = mat.FamiliaId,
                        ClienteId = cliente.Id,
                        Rubro = "MATERIA PRIMA",
                        EsMateriaPrima = true,
                        EsFazon = true,   // ✅ Importante
                        EsScrap = false,  // ❌ No es scrap
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    });
                    creados++;
                }
            }

            // --- BUCLE 2: CREAR SCRAP ---
            foreach (var scrap in materialesScrap)
            {
                // SKU: SCRAP-CLI-15-AI
                string sku = $"SCRAP-CLI-{cliente.Id}-{scrap.Codigo}";

                if (!await _context.Productos.AnyAsync(p => p.CodigoSku == sku))
                {
                    _context.Productos.Add(new Producto
                    {
                        Nombre = $"{scrap.Nombre} - PROPIEDAD DE {cliente.RazonSocial.ToUpper()}",
                        CodigoSku = sku,
                        FamiliaId = scrap.FamiliaId,
                        ClienteId = cliente.Id,

                        Rubro = "SCRAP",
                        EsMateriaPrima = true, // El scrap TAMBIÉN se usa para fabricar
                        EsProductoTerminado = false,
                        EsFazon = true,        // Es de terceros

                        // 🔥 LA BANDERA NUEVA
                        EsScrap = true,

                        StockActual = 0,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    });
                    creados++;
                }
            }

            if (creados > 0) await _context.SaveChangesAsync();

            return Ok(new { mensaje = $"✅ Proceso finalizado. Se agregaron {creados} ítems (MP y Scrap)." });
        }
    }
}