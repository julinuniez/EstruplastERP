using System.ComponentModel.DataAnnotations;

namespace EstruplastERP.Core
{
    public class Empleado
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreCompleto { get; set; } = string.Empty;

        [MaxLength(50)]
        public string? DNI { get; set; } 

        // Puesto: "Extrusor", "Confección", "Depósito"
        [MaxLength(50)]
        public string? Puesto { get; set; }

        public bool Activo { get; set; } = true; // Si lo despiden, lo ponemos en false y no se borra
    }
}