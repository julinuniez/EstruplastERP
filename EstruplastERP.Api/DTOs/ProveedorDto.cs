using System.ComponentModel.DataAnnotations;

namespace EstruplastERP.Api.Dtos
{
    public class ProveedorDto
    {
        [Required(ErrorMessage = "La Razón Social es obligatoria")]
        public string RazonSocial { get; set; } = string.Empty;

        public string? Cuit { get; set; }
        public string? ContactoNombre { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public string? Observaciones { get; set; }
    }
}