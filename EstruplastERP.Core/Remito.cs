using System;
using System.Collections.Generic;

namespace EstruplastERP.Core
{
    public class Remito
    {
        public int Id { get; set; }
        public int ClienteId { get; set; } // Obligatorio para la relación
        public Cliente Cliente { get; set; }

        public string NumeroRemito { get; set; } // "0001-00001234"
        public string? ClienteNombre { get; set; } // Backup del nombre
        public DateTime Fecha { get; set; } // ✅ NUEVO
        public string? Observacion { get; set; }
        
        public List<RemitoDetalle> Detalles { get; set; } = new List<RemitoDetalle>();
    }
}