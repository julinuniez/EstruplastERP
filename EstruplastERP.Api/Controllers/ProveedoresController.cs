using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProveedoresController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProveedoresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ======================================================
        // 1. OBTENER TODOS (LISTADO)
        // GET: api/Proveedores
        // ======================================================
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Proveedor>>> GetProveedores()
        {
            // Solo devolvemos los que están "Activos"
            return await _context.Proveedores
                                 .Where(p => p.Activo)
                                 .OrderBy(p => p.RazonSocial)
                                 .ToListAsync();
        }

        // ======================================================
        // 2. OBTENER UNO POR ID
        // GET: api/Proveedores/5
        // ======================================================
        [HttpGet("{id}")]
        public async Task<ActionResult<Proveedor>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null || !proveedor.Activo)
            {
                return NotFound("Proveedor no encontrado.");
            }

            return proveedor;
        }

        // ======================================================
        // 3. CREAR (AGREGAR)
        // POST: api/Proveedores
        // ======================================================
        [HttpPost]
        public async Task<ActionResult<Proveedor>> CrearProveedor(ProveedorDto dto)
        {
            // Validación: Evitar duplicados por CUIT o Razón Social
            if (await _context.Proveedores.AnyAsync(p => p.RazonSocial == dto.RazonSocial && p.Activo))
            {
                return BadRequest("Ya existe un proveedor con esa Razón Social.");
            }

            var proveedor = new Proveedor
            {
                RazonSocial = dto.RazonSocial,
                Cuit = dto.Cuit,
                ContactoNombre = dto.ContactoNombre,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                Observaciones = dto.Observaciones,
                Activo = true
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProveedor), new { id = proveedor.Id }, proveedor);
        }

        // ======================================================
        // 4. EDITAR (MODIFICAR)
        // PUT: api/Proveedores/5
        // ======================================================
        [HttpPut("{id}")]
        public async Task<IActionResult> EditarProveedor(int id, ProveedorDto dto)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound("El proveedor no existe.");
            }

            // Actualizamos los campos
            proveedor.RazonSocial = dto.RazonSocial;
            proveedor.Cuit = dto.Cuit;
            proveedor.ContactoNombre = dto.ContactoNombre;
            proveedor.Telefono = dto.Telefono;
            proveedor.Email = dto.Email;
            proveedor.Direccion = dto.Direccion;
            proveedor.Observaciones = dto.Observaciones;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProveedorExists(id)) return NotFound();
                else throw;
            }

            return NoContent(); 
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();
            bool tieneMovimientos = await _context.Movimientos.AnyAsync(m => m.ProveedorId == id);

            if (tieneMovimientos)
            {
                proveedor.Activo = false;
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "El proveedor se ha desactivado porque tiene historial de compras." });
            }
            else
            {
                _context.Proveedores.Remove(proveedor);
                await _context.SaveChangesAsync();
                return Ok(new { mensaje = "El proveedor fue eliminado permanentemente." });
            }
        }

        private bool ProveedorExists(int id)
        {
            return _context.Proveedores.Any(e => e.Id == id);
        }
    }
}