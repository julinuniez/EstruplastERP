using System.ComponentModel.DataAnnotations;

namespace EstruplastERP.Core
{
    public class CategoriaInsumo
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string RolLogico { get; set; } = string.Empty;
    }
}