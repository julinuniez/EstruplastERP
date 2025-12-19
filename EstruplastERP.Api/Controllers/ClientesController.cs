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

        // POST: api/Clientes/habilitar-fazon/5
        [HttpPost("habilitar-fazon/{id}")]
        public async Task<IActionResult> HabilitarFazon(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound("Cliente no encontrado.");

            // LISTA EXACTA SOLICITADA POR EL USUARIO
            var materialesFazon = new[]
            {
                new { Codigo = "AI-FIN", Nombre = "A.I. FINO (FAZÓN)" },
                new { Codigo = "AI-GRU", Nombre = "A.I. GRUESO (FAZÓN)" },
                new { Codigo = "AI-BIC", Nombre = "A.I. BICAPA (FAZÓN)" },
                new { Codigo = "AI-TRI", Nombre = "A.I. TRICAPA (FAZÓN)" },

                new { Codigo = "ABS-GRU", Nombre = "ABS GRUESO (FAZÓN)" },

                new { Codigo = "POLI-FIN", Nombre = "PEAD/PP/BIO FINO (FAZÓN)" },
                new { Codigo = "POLI-GRU", Nombre = "PEAD/PP/BIO GRUESO (FAZÓN)" },

                new { Codigo = "PEAD-BIC", Nombre = "PEAD BICAPA (FAZÓN)" }
            };

            int creados = 0;

            foreach (var mat in materialesFazon)
            {
                // SKU ÚNICO: MP-CLI-{ID}-{CODIGO_MATERIAL}
                // Ej: MP-CLI-10-AI-FIN
                string sku = $"MP-CLI-{cliente.Id}-{mat.Codigo}";

                bool existe = await _context.Productos.AnyAsync(p => p.CodigoSku == sku);

                if (!existe)
                {
                    var nuevoProducto = new Producto
                    {
                        // Nombre claro: "MP A.I. FINO (FAZÓN) - PROPIEDAD DE JUAN"
                        Nombre = $"MP {mat.Nombre} - PROPIEDAD DE {cliente.RazonSocial.ToUpper()}",
                        CodigoSku = sku,
                        EsMateriaPrima = true,
                        EsProductoTerminado = false,
                        EsGenerico = false,
                        EsFazon = false, // Es el material físico en stock
                        PesoEspecifico = 1.05m, // Valor ref
                        StockActual = 0,
                        StockMinimo = 0,
                        PrecioCosto = 0,
                        Activo = true,
                        FechaCreacion = DateTime.Now,
                        ClienteId = cliente.Id
                    };

                    _context.Productos.Add(nuevoProducto);
                    creados++;
                }
            }

            if (creados > 0)
            {
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = $"Se crearon {creados} materiales de stock para {cliente.RazonSocial}." });
            }
            else
            {
                return Ok(new { mensaje = $"El cliente ya tenía los materiales habilitados." });
            }
        }
    }
}