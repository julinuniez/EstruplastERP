namespace EstruplastERP.Api.Dtos
{
    public class CrearOrdenDto
    {
        // Coincide con form.value.productoTerminadoId
        public int ProductoTerminadoId { get; set; }

        public int? ClienteId { get; set; }
        public decimal Cantidad { get; set; } // Unidades
        public int EmpleadoId { get; set; }
        public string Turno { get; set; }
        public string Observacion { get; set; }

        // Coincide con form.value.kilosTotales
        public decimal Kilos { get; set; }

        // Lista de consumos (receta calculada)
        public List<ConsumoDto> Consumos { get; set; }
    }

    public class ConsumoDto
    {
        public int MateriaPrimaId { get; set; }
        public decimal CantidadKilos { get; set; }
    }
}