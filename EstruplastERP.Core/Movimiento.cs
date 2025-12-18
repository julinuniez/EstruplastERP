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

        [MaxLength(20)]
        public string? Turno { get; set; } // "Mañana", "Tarde", "Noche"

        [MaxLength(200)]
        public string? Observacion { get; set; }

        public int? EmpleadoId { get; set; }

        [ForeignKey("EmpleadoId")]
        public Empleado? Empleado { get; set; }

        public int? ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }
        public int? ProveedorId { get; set; }

        [ForeignKey("ProveedorId")]
        public Proveedor? Proveedor { get; set; }

        [MaxLength(50)]
        public string? NumeroRemito { get; set; }
        [MaxLength(50)]
        public string? LoteProveedor { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioTotal { get; set; } = 0;
    }
}