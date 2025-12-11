using System;
using System.Collections.Generic;

namespace EstruplastERP.Core
{
    public class Remito
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string NumeroRemito { get; set; } // Ej: "0001-000025"
        public string ClienteNombre { get; set; } // Guardamos el nombre para historial

        // Relación con los items del remito
        public List<RemitoDetalle> Detalles { get; set; } = new List<RemitoDetalle>();
    }
}