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
        [MaxLength(50)]
        public string? Color { get; set; } // Ej: "Gris Goff"

        [Column(TypeName = "decimal(18,2)")]
        public decimal Largo { get; set; } // Ej: 1550

        [Column(TypeName = "decimal(18,2)")]
        public decimal Ancho { get; set; } // Ej: 870

        [Column(TypeName = "decimal(18,2)")]
        public decimal Espesor { get; set; } // Ej: 2,5

        [Column(TypeName = "decimal(18,4)")]
        public decimal PesoEspecifico { get; set; }

        public int? ProductoPadreId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Producto? ProductoPadre { get; set; }
        // Inventario y Precios
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockActual { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StockMinimo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }

        // Datos Multimedia
        public string? ImagenUrl { get; set; }
        public int? ClienteId { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public Cliente? Cliente { get; set; }

        // Lógica de Negocio
        public bool EsGenerico { get; set; }
        public bool EsMateriaPrima { get; set; }      
        public bool EsProductoTerminado { get; set; }
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public ICollection<Formula> Formulas { get; set; } = new List<Formula>();

    }
}