using System;
using System.Text.Json.Serialization;

namespace EstruplastERP.Core
{
    public class PalletProduccion
    {
        public int Id { get; set; }
        public int OrdenProduccionId { get; set; }
        public int NumeroPallet { get; set; }
        public decimal Kilos { get; set; }
        public string Estado { get; set; } = "Pendiente"; // "Pendiente" o "Finalizada"
        public DateTime? FechaCierre { get; set; }

        // Propiedad de navegación
        [JsonIgnore]
        public OrdenProduccion OrdenProduccion { get; set; }
    }
}