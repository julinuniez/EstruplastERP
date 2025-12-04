using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public class Producto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? Descripcion { get; set; }

        // Identificación
        [MaxLength(50)]
        public string? CodigoSku { get; set; } 

        [MaxLength(50)]
        public string? CodigoBarras { get; set; } 

        // Inventario y Precios
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockActual { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StockMinimo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }

        // Datos Multimedia
        public string? ImagenUrl { get; set; }

        // Lógica de Negocio
        public bool EsMateriaPrima { get; set; }      
        public bool EsProductoTerminado { get; set; } 

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
    }
}