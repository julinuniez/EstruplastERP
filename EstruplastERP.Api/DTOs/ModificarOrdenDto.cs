namespace EstruplastERP.Api.DTOs
{
    public class ModificarOrdenDto
    {
        // Medidas y cantidades
        public int Largo { get; set; }
        public int Ancho { get; set; }
        public decimal Espesor { get; set; }
        public int Cantidad { get; set; }
        public decimal KilosTotales { get; set; }

        // Aditivos (para actualizar los tildes en la base de datos)
        public bool ConBrillo { get; set; }
        public bool ConEstearato { get; set; }
        public bool AditivoUv { get; set; }
        public bool AditivoCaucho { get; set; }
        public int? MasterbatchId { get; set; }

        // La nueva receta recalculada por el frontend
        public List<ItemRecetaModificadaDto> RecetaNueva { get; set; }
    }

    public class ItemRecetaModificadaDto
    {
        public int MateriaPrimaId { get; set; }
        public decimal CantidadEsperada { get; set; }
        public string TipoInsumo { get; set; }
    }
}
