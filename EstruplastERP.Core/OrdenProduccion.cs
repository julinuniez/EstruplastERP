using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace EstruplastERP.Core
{
    public enum EstadoOrden
    {
        Cancelada = -1,
        Pendiente = 0,
        EnProceso = 1,
        Finalizada = 2,
        EnCola = 3
    }

    public class OrdenProduccion
    {
        public int Id { get; set; }

        public string NumeroPedidoCliente { get; set; }
        public string? NotaPedido { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
        public decimal Cantidad { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Largo { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Ancho { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Espesor { get; set; }
        public string? Color { get; set; }

        public int? ClienteId { get; set; }
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }
        public string Observacion { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal KilosEstimados { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Desperdicio { get; set; } = 8;
        public EstadoOrden Estado { get; set; } = EstadoOrden.Pendiente;
        public bool EsBobina { get; set; } = false;
        public bool ConBrillo { get; set; }
        public bool LlevaFilm { get; set; }
        public string? TipoCorona { get; set; } = "Ninguno";
        public bool EsImpreso { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaFin { get; set; }

        public List<ConsumoOrden> Consumos { get; set; } = new List<ConsumoOrden>();
    }
}