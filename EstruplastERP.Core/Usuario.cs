using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        public string NombreUsuario { get; set; } 

        [Required]
        public string Password { get; set; }

        public string Rol { get; set; } // "Admin", "Recepcion"

        public bool Activo { get; set; } = true;
    }
}