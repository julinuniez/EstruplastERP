public class ModificarOrdenDto
{
    public decimal Largo { get; set; }
    public decimal Ancho { get; set; }
    public decimal Espesor { get; set; }
    public decimal Cantidad { get; set; }
    public decimal KilosTotales { get; set; }
    public decimal Desperdicio { get; set; } 
    public bool ConBrillo { get; set; }
    public bool LlevaFilm { get; set; }
    public bool EsGofrado { get; set; }
    public bool AditivoUV { get; set; }
    public string TipoCorona { get; set; }
    public string Color { get; set; }

    public List<ItemRecetaModificadaDto> RecetaNueva { get; set; }
}

public class ItemRecetaModificadaDto
{
    public int MateriaPrimaId { get; set; }
    public decimal CantidadKilos { get; set; } 
}