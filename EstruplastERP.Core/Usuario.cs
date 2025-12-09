using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string NombreUsuario { get; set; } // Ej: "juanp"

        [Required]
        public string Password { get; set; }

        public string Rol { get; set; } // "Admin", "Recepcion"

        public bool Activo { get; set; } = true;

        // 👇 CONEXIÓN CON EMPLEADO (En vez de NombreCompleto)
        public int? EmpleadoId { get; set; }

        [ForeignKey("EmpleadoId")]
        public Empleado? Empleado { get; set; }
    }
}