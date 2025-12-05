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

        // ¿Cuánto? (Positivo = Entrada, Negativo = Salida)
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }

        // ¿Por qué? (Ej: "Producción", "Compra", "Ajuste", "Venta")
        [MaxLength(50)]
        public string TipoMovimiento { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Observacion { get; set; }
    }
}