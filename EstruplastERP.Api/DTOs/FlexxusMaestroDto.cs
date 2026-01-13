using CsvHelper.Configuration.Attributes;

namespace EstruplastERP.Api.Dtos
{
    public class FlexxusMaestroDto
    {
        [Index(0)]
        public string CodigoSku { get; set; }

        [Index(1)]
        public string Nombre { get; set; }

        // Agregados como opcionales para no romper si no vienen
        [Index(2)]
        [Optional]
        public decimal? Precio { get; set; }

        [Index(3)]
        [Optional]
        public int? Stock { get; set; }

        [Index(4)]
        public string Rubro { get; set; }
    }
}