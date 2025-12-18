using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public class ConsumoOrden
    {
        public int Id { get; set; }

        // Relación con la Orden (Padre)
        public int OrdenProduccionId { get; set; }
        public OrdenProduccion OrdenProduccion { get; set; }

        // Relación con el Insumo (Que es un Producto en realidad)
        public int? MateriaPrimaId { get; set; }

        [ForeignKey("MateriaPrimaId")]
        public virtual Producto MateriaPrima { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal CantidadKilos { get; set; }
    }
}