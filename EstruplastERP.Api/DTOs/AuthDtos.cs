namespace EstruplastERP.Api.Dtos
{
    public class LoginDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterDto
    {
        public string NombreUsuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Rol { get; set; } = "Operario"; // Valor por defecto
        public int? EmpleadoId { get; set; } // Opcional, para vincular
    }

    // Lo que devolvemos al Vue cuando el login es exitoso
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Usuario { get; set; }
        public string Rol { get; set; }
    }
}