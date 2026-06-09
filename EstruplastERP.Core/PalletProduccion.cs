using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace EstruplastERP.Core
{
    public class PalletProduccion
    {
        public int Id { get; set; }
        public int OrdenProduccionId { get; set; }
        public int NumeroPallet { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Kilos { get; set; }
        public string Estado { get; set; } = "Pendiente"; // "Pendiente" o "Finalizada"
        public DateTime? FechaCierre { get; set; }

        // Propiedad de navegación
        [JsonIgnore]
        public OrdenProduccion OrdenProduccion { get; set; }
    }
}