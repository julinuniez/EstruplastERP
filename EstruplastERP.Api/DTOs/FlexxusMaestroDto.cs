using CsvHelper.Configuration.Attributes; 

namespace EstruplastERP.Api.Dtos
{
    public class FlexxusMaestroDto
    {
        [CsvHelper.Configuration.Attributes.Index(0)]
        public string CodigoSku { get; set; }

        [CsvHelper.Configuration.Attributes.Index(1)]
        public string Nombre { get; set; }

        [CsvHelper.Configuration.Attributes.Index(4)]
        public string Rubro { get; set; }
    }
}