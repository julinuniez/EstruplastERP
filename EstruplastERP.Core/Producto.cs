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

        // Identificación
        [MaxLength(50)]
        public string? CodigoSku { get; set; } 

        public string? Rubro { get; set; }// Valores sugeridos: "VIRGEN", "SCRAP", "TUTTI", "ADITIVO", "MASTERBATCH"
        public string? TipoMaterial { get; set; } // Ej: "PAI", "ABS", "BIO", "PP"
        public bool EsFazon { get; set; } = false;
        public bool EsScrap { get; set; } = false;
        public decimal? EspesorMinimo { get; set; }
        public decimal? EspesorMaximo { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal PesoEspecifico { get; set; } = 1;

        [System.Text.Json.Serialization.JsonIgnore]
        public decimal StockActual { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal StockMinimo { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioCosto { get; set; }
        public int? ProveedorId { get; set; }
        [ForeignKey("ProveedorId")]
        public Proveedor? Proveedor { get; set; }

        // Datos Multimedia
        public int? ClienteId { get; set; }
        public int? FamiliaId { get; set; }

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