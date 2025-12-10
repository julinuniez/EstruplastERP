using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration; // Para leer el appsettings

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/Auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Registrar([FromBody] RegisterDto request)
        {
            // 1. Validar si ya existe
            if (await _context.Usuarios.AnyAsync(u => u.NombreUsuario == request.NombreUsuario))
            {
                return BadRequest("El nombre de usuario ya existe.");
            }

            // 2. Encriptar contraseña (Nunca guardar texto plano)
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            // 3. Crear Usuario
            var nuevoUsuario = new Usuario
            {
                NombreUsuario = request.NombreUsuario,
                Password = passwordHash, // Guardamos el hash
                Rol = request.Rol,
                EmpleadoId = request.EmpleadoId,
                Activo = true
            };

            _context.Usuarios.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Usuario registrado correctamente" });
        }

        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto request)
        {
            // 1. Buscar usuario
            var usuario = await _context.Usuarios
                .Include(u => u.Empleado) // Traemos datos del empleado si existen
                .FirstOrDefaultAsync(u => u.NombreUsuario == request.NombreUsuario);

            // 2. Validar existencia
            if (usuario == null)
            {
                return Unauthorized("Usuario no encontrado.");
            }

            // 3. Verificar Contraseña (Comparamos lo que escribió con el Hash de la BD)
            if (!BCrypt.Net.BCrypt.Verify(request.Password, usuario.Password))
            {
                return Unauthorized("Contraseña incorrecta.");
            }

            if (!usuario.Activo) return Unauthorized("Usuario inactivo.");

            // 4. Generar Token JWT
            string token = CrearToken(usuario);

            // 5. Devolver respuesta
            return Ok(new LoginResponseDto
            {
                Token = token,
                Usuario = usuario.NombreUsuario,
                Rol = usuario.Rol
            });
        }

        // Método privado para generar el JWT
        private string CrearToken(Usuario usuario)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            // Si tiene empleado vinculado, agregamos su nombre real al token
            if (usuario.Empleado != null)
            {
                claims.Add(new Claim("NombreCompleto", usuario.Empleado.NombreCompleto));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1), // El token dura 1 día
                    signingCredentials: creds
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}