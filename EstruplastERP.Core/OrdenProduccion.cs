using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public enum EstadoOrden
    {
        Cancelada = -1,
        Pendiente = 0,
        EnProceso = 1,
        Finalizada = 2
    }
    public class OrdenProduccion
    {
            public int Id { get; set; }

            public int ProductoId { get; set; }
            public Producto Producto { get; set; }
            public decimal Cantidad { get; set; }

            // --- NUEVOS CAMPOS (Estos son los que te faltan) ---
            public int? ClienteId { get; set; }
        public int? EmpleadoId { get; set; } // Puede ser nulo si nadie la tomó aún
        [ForeignKey("EmpleadoId")]
        public virtual Empleado Empleado { get; set; }
        public string Turno { get; set; }
            public string Observacion { get; set; }

            [Column(TypeName = "decimal(18,4)")]
            public decimal KilosEstimados { get; set; }

            // ✅ ESTA ES LA PROPIEDAD QUE TE DA ERROR:
            public EstadoOrden Estado { get; set; } = EstadoOrden.Pendiente;

            public DateTime FechaCreacion { get; set; } = DateTime.Now;
            public DateTime? FechaFin { get; set; }

            public List<ConsumoOrden> Consumos { get; set; } = new List<ConsumoOrden>();
    }
}