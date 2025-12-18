using System.ComponentModel.DataAnnotations;

namespace EstruplastERP.Core
{
    public class Proveedor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La Razón Social es obligatoria")]
        [MaxLength(150)]
        public string RazonSocial { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Cuit { get; set; }

        [MaxLength(100)]
        public string? ContactoNombre { get; set; }

        [MaxLength(50)]
        public string? Telefono { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }

        [MaxLength(200)]
        public string? Direccion { get; set; }

        [MaxLength(500)]
        public string? Observaciones { get; set; }

        // Para borrado lógico (Soft Delete)
        public bool Activo { get; set; } = true;
    }
}