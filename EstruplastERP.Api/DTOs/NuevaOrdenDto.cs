namespace EstruplastERP.Api.Dtos
{
    public class NuevaOrdenDto
    {
        public int ProductoTerminadoId { get; set; }
        public int? ClienteId { get; set; }
        public int EmpleadoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Kilos { get; set; }
        public string? Turno { get; set; }
        public string? Observacion { get; set; }
    }
}