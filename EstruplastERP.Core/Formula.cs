using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public class Formula
    {
        public int Id { get; set; }

        public int ProductoTerminadoId { get; set; }

        [ForeignKey("ProductoTerminadoId")]
        public Producto? ProductoTerminado { get; set; }

        public int MateriaPrimaId { get; set; }

        [ForeignKey("MateriaPrimaId")]
        public Producto? MateriaPrima { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal Cantidad { get; set; }
    }
}