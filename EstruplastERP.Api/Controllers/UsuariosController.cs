using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EstruplastERP.Data;
using EstruplastERP.Core;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsuariosController(ApplicationDbContext context) { _context = context; }

        // DTO para recibir los datos del login
        public class LoginDto { public string Usuario { get; set; } public string Password { get; set; } }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Empleado) // 👈 IMPORTANTE: Traemos los datos del empleado vinculado
                .FirstOrDefaultAsync(u => u.NombreUsuario == login.Usuario && u.Password == login.Password && u.Activo);

            if (usuario == null) return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });

            // LÓGICA DE NOMBRE:
            // Si tiene empleado vinculado, usamos su nombre real.
            // Si no (ej: el Admin genérico), usamos su nombre de usuario.
            string nombreAMostrar = (usuario.Empleado != null ? usuario.Empleado.NombreCompleto : usuario.NombreUsuario) ?? "usuario";

            // LÓGICA DE PUESTO/ROL:
            // Si el empleado tiene puesto, lo usamos. Si no, usamos el Rol del usuario.
            string rolAMostrar = (usuario.Empleado != null ? usuario.Empleado.Puesto : usuario.Rol) ?? "Sin rol";

            return Ok(new
            {
                id = usuario.Id,
                nombre = nombreAMostrar,
                rol = rolAMostrar
            });
        }
    }
}