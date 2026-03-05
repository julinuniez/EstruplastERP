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
            // cliente.EsFazon ya viene del body o es false por defecto (bool)

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        // PUT: api/Clientes/5
        // Agregamos método PUT para poder editar la propiedad EsFazon desde el frontend si lo necesitas
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, Cliente cliente)
        {
            if (id != cliente.Id)
            {
                return BadRequest();
            }

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
    }
}