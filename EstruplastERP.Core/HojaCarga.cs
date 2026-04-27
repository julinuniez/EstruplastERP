using System;
using System.Collections.Generic;

namespace EstruplastERP.Core
{
    public class HojaCarga
    {
        public int Id { get; set; }

        // Ej: "HC-1234"
        public string CodigoLote { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public DateTime? FechaDeclaracion { get; set; }

        public EstadoHojaCarga Estado { get; set; } = EstadoHojaCarga.Pendiente;

        public string? Observaciones { get; set; }

        // 🚀 Las OPs que están atadas a este pastón
        public List<OrdenProduccion> Ordenes { get; set; } = new List<OrdenProduccion>();

        // 🚀 Los materiales REALES que el operario tiró a la máquina para todo el grupo
        public List<ConsumoHojaCarga> ConsumosReales { get; set; } = new List<ConsumoHojaCarga>();
    }
}