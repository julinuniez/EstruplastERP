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

            // 1. DEFINIR MATERIALES Y FAMILIAS
            var materialesFazon = new[]
            {
        // A.I. (Familia 10)
        new { Codigo = "AI-FIN", Nombre = "A.I. FINO (FAZÓN)", FamiliaId = 11 },
        new { Codigo = "AI-GRU", Nombre = "A.I. GRUESO (FAZÓN)", FamiliaId = 12 },
        new { Codigo = "AI-BIC", Nombre = "A.I. BICAPA (FAZÓN)", FamiliaId = 13 },
        new { Codigo = "AI-TRI", Nombre = "A.I. TRICAPA (FAZÓN)", FamiliaId = 14 },

        // ABS (Familia 20)
        new { Codigo = "ABS-GRU", Nombre = "ABS GRUESO (FAZÓN)", FamiliaId = 21 },

        // POLI / BIO (Familia 30)
        new { Codigo = "POLI-FIN", Nombre = "PEAD/PP/BIO FINO (FAZÓN)", FamiliaId = 31 },
        new { Codigo = "POLI-GRU", Nombre = "PEAD/PP/BIO GRUESO (FAZÓN)", FamiliaId = 32 },

        // PEAD (Familia 40)
        new { Codigo = "PEAD-BIC", Nombre = "PEAD BICAPA (FAZÓN)", FamiliaId = 41 }
    };

            int creados = 0;

            foreach (var mat in materialesFazon)
            {
                // SKU ÚNICO: MP-CLI-{ID}-{CODIGO_MATERIAL}
                string sku = $"MP-CLI-{cliente.Id}-{mat.Codigo}";

                // Verificamos si existe (usando ToUpper para asegurar unicidad)
                bool existe = await _context.Productos.AnyAsync(p => p.CodigoSku == sku);

                if (!existe)
                {
                    var nuevoProducto = new Producto
                    {
                        // Nombre claro
                        Nombre = $"MP {mat.Nombre} - PROPIEDAD DE {cliente.RazonSocial.ToUpper()}",
                        CodigoSku = sku,

                        FamiliaId = mat.FamiliaId,
                        ClienteId = cliente.Id, // IMPORTANTE: Asigna el dueño

                        Rubro = "MATERIA PRIMA",
                        EsMateriaPrima = true,      // Es un insumo
                        EsProductoTerminado = false,
                        EsGenerico = false,

                        // 🔥 EL CAMBIO CRÍTICO PARA QUE TU SQL FUNCIONE 🔥
                        // En el commit viejo esto estaba en false. Lo ponemos en true.
                        EsFazon = true,

                        PesoEspecifico = 1.05m,
                        StockActual = 0,
                        StockMinimo = 0,
                        PrecioCosto = 0,
                        Activo = true,
                        FechaCreacion = DateTime.Now
                    };

                    _context.Productos.Add(nuevoProducto);
                    creados++;
                }
            }

            if (creados > 0)
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = $"✅ Se crearon {creados} materiales de stock para {cliente.RazonSocial}." });
            }
            else
            {
                return Ok(new { mensaje = $"ℹ️ El cliente ya tenía los materiales habilitados." });
            }
        }
    }
}