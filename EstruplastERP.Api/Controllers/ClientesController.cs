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

            // Listas de Materiales (Tu configuración actual)
            var materialesFazon = new[] {
        new { Codigo = "AI-FIN", Nombre = "A.I. FINO (FAZÓN)", FamiliaId = 11 },
        new { Codigo = "AI-GRU", Nombre = "A.I. GRUESO (FAZÓN)", FamiliaId = 12 },
        new { Codigo = "AI-BIC", Nombre = "A.I. BICAPA (FAZÓN)", FamiliaId = 13 },
        new { Codigo = "ABS-GRU", Nombre = "ABS GRUESO (FAZÓN)", FamiliaId = 21 },
        new { Codigo = "POLI-FIN", Nombre = "PEAD/PP/BIO FINO (FAZÓN)", FamiliaId = 31 },
        new { Codigo = "POLI-GRU", Nombre = "PEAD/PP/BIO GRUESO (FAZÓN)", FamiliaId = 32 },
        new { Codigo = "PEAD-BIC", Nombre = "PEAD BICAPA (FAZÓN)", FamiliaId = 41 }
    };

            var materialesScrap = new[] {
        new { Codigo = "SCRAP-AI", Nombre = "SCRAP A.I. (MOLIDO)", FamiliaId = 10 },
        new { Codigo = "SCRAP-ABS", Nombre = "SCRAP ABS (MOLIDO)", FamiliaId = 20 },
        new { Codigo = "SCRAP-POLI", Nombre = "SCRAP POLIETILENO (MOLIDO)", FamiliaId = 30 },
        new { Codigo = "SCRAP-PEAD", Nombre = "SCRAP PEAD (MOLIDO)", FamiliaId = 40 }
    };

            int creados = 0;

            // --- BUCLE 1: MP VIRGEN ---
            foreach (var mat in materialesFazon)
            {
                string sku = $"MP-CLI-{cliente.Id}-{mat.Codigo}";
                // Usamos Trim() y ToUpper() para asegurar que no haya diferencias tontas
                if (!await _context.Productos.AnyAsync(p => p.CodigoSku == sku))
                {
                    _context.Productos.Add(new Producto
                    {
                        Nombre = $"MP {mat.Nombre} - {cliente.RazonSocial.ToUpper()}",
                        CodigoSku = sku,
                        FamiliaId = mat.FamiliaId,
                        ClienteId = cliente.Id,
                        Rubro = "MATERIA PRIMA",
                        EsMateriaPrima = true,
                        EsFazon = true,
                        EsScrap = false,
                        Largo = 0,
                        Ancho = 0,
                        Espesor = 0,
                        StockActual = 0,
                        StockMinimo = 0,
                        PrecioCosto = 0,
                        PesoEspecifico = 1,
                        Activo = true
                    });
                    creados++;
                }
            }

            // --- BUCLE 2: SCRAP ---
            foreach (var scrap in materialesScrap)
            {
                string sku = $"SCRAP-CLI-{cliente.Id}-{scrap.Codigo}";
                if (!await _context.Productos.AnyAsync(p => p.CodigoSku == sku))
                {
                    _context.Productos.Add(new Producto
                    {
                        Nombre = $"{scrap.Nombre} - {cliente.RazonSocial.ToUpper()}",
                        CodigoSku = sku,
                        FamiliaId = scrap.FamiliaId,
                        ClienteId = cliente.Id,
                        Rubro = "SCRAP",
                        EsMateriaPrima = true,
                        EsFazon = true,
                        EsScrap = true, // 🔥 Flag Scrap Activado
                        Largo = 0,
                        Ancho = 0,
                        Espesor = 0,
                        StockActual = 0,
                        StockMinimo = 0,
                        PrecioCosto = 0,
                        PesoEspecifico = 1,
                        Activo = true
                    });
                    creados++;
                }
            }

            if (creados > 0)
            {
                await _context.SaveChangesAsync();
                // 🔥 ESTADO 201: CREADO (Verde)
                return StatusCode(201, new { nuevo = true, mensaje = $"✅ Se generaron {creados} ítems nuevos." });
            }
            else
            {
                // 🔥 ESTADO 200: OK PERO SIN CAMBIOS (Azul/Amarillo)
                return Ok(new { nuevo = false, mensaje = "ℹ️ El cliente ya tiene todo el servicio habilitado." });
            }
        }
    }
}