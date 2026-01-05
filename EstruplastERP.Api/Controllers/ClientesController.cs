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

            // 1. DEFINICIÓN DE MATERIALES BASE
            var materialesFazon = new[]
            {
        // A.I. (Familia Base 10)
        new { Codigo = "AI-FIN", Nombre = "A.I. FINO (FAZÓN)", FamiliaId = 11 },
        new { Codigo = "AI-GRU", Nombre = "A.I. GRUESO (FAZÓN)", FamiliaId = 12 },
        new { Codigo = "AI-BIC", Nombre = "A.I. BICAPA (FAZÓN)", FamiliaId = 13 },
        new { Codigo = "AI-TRI", Nombre = "A.I. TRICAPA (FAZÓN)", FamiliaId = 14 },

        // ABS (Familia Base 20)
        new { Codigo = "ABS-GRU", Nombre = "ABS GRUESO (FAZÓN)", FamiliaId = 21 },

        // POLI / BIO (Familia Base 30)
        new { Codigo = "POLI-FIN", Nombre = "PEAD/PP/BIO FINO (FAZÓN)", FamiliaId = 31 },
        new { Codigo = "POLI-GRU", Nombre = "PEAD/PP/BIO GRUESO (FAZÓN)", FamiliaId = 32 },

        // PEAD (Familia Base 40)
        new { Codigo = "PEAD-BIC", Nombre = "PEAD BICAPA (FAZÓN)", FamiliaId = 41 }
    };

            // 2. OPTIMIZACIÓN: Traemos los SKUs existentes de este cliente de una sola vez
            // Esto evita hacer 8 viajes a la base de datos dentro del loop.
            var skusExistentes = await _context.Productos
                                               .Where(p => p.ClienteId == id && p.CodigoSku.StartsWith($"MP-CLI-{id}"))
                                               .Select(p => p.CodigoSku)
                                               .ToListAsync();

            int creados = 0;

            foreach (var mat in materialesFazon)
            {
                string sku = $"MP-CLI-{cliente.Id}-{mat.Codigo}";

                // Verificamos en memoria (rápido) en lugar de en base de datos (lento)
                if (!skusExistentes.Contains(sku))
                {
                    var nuevoProducto = new Producto
                    {
                        // Nombre claro para que se vea bien en la hoja de impresión
                        Nombre = $"MP: {mat.Nombre} ({cliente.RazonSocial.ToUpper()})",
                        CodigoSku = sku,

                        // === DATOS CLAVE PARA LA LÓGICA ===
                        FamiliaId = mat.FamiliaId,
                        ClienteId = cliente.Id,

                        // === ETIQUETADO PARA FILTROS ===
                        Rubro = "MATERIA PRIMA",
                        // === FLAGS ===
                        EsMateriaPrima = true,
                        EsProductoTerminado = false,
                        EsGenerico = false,
                        EsFazon = false,             // False porque es el material, no el servicio

                        // === VALORES POR DEFECTO ===
                        PesoEspecifico = 1.05m, // Promedio, luego se puede editar
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
                return Ok(new { mensaje = $"✅ Se habilitaron {creados} materiales de stock para {cliente.RazonSocial}." });
            }
            else
            {
                return Ok(new { mensaje = $"ℹ️ El cliente ya tenía todos los materiales habilitados." });
            }
        }
    }
}