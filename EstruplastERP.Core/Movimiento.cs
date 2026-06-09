using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public class Movimiento
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;
        public int ProductoId { get; set; }
        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }

        // ¿Por qué? (Ej: "Producción", "Compra", "Ajuste", "Venta")
        [MaxLength(50)]
        public string TipoMovimiento { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Observacion { get; set; }

        public int? ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }
        public int? ProveedorId { get; set; }

        [ForeignKey("ProveedorId")]
        public Proveedor? Proveedor { get; set; }

        [MaxLength(50)]
        public string? NumeroRemito { get; set; }
        public int? OrdenProduccionId { get; set; }

        [ForeignKey("OrdenProduccionId")]
        public OrdenProduccion? OrdenProduccion { get; set; }

    }
}