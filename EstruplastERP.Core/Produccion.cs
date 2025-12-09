using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public class Produccion
    {
        public int Id { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relación con Producto
        public int ProductoTerminadoId { get; set; }
        [ForeignKey("ProductoTerminadoId")]
        public Producto? Producto { get; set; }

        // Relación con Cliente (Puede ser nulo si es para stock)
        public int? ClienteId { get; set; }
        public Cliente? Cliente { get; set; }

        // Relación con Empleado
        public int EmpleadoId { get; set; }
        public Empleado? Empleado { get; set; }

        public int Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Kilos { get; set; }

        public string? Lote { get; set; }
        public string? Observacion { get; set; }
        public string? Turno { get; set; } // Mañana, Tarde, Noche
    }
}