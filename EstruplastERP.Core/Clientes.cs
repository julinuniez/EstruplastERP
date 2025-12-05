using System.ComponentModel.DataAnnotations;

namespace EstruplastERP.Core
{
    public class Cliente
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string RazonSocial { get; set; } = string.Empty; // Ej: "Coca Cola"

        [MaxLength(20)]
        public string? Cuit { get; set; }

        public bool Activo { get; set; } = true;
    }
}