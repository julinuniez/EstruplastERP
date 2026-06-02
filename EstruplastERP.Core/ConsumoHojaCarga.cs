using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public class ConsumoHojaCarga
    {
        public int Id { get; set; }

        public int HojaCargaId { get; set; }
        [ForeignKey("HojaCargaId")]
        public HojaCarga HojaCarga { get; set; }

        public int MateriaPrimaId { get; set; }
        [ForeignKey("MateriaPrimaId")]  
        public Producto MateriaPrima { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CantidadRealKg { get; set; }
    }
}