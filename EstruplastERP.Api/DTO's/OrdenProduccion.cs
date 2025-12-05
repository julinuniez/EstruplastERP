namespace EstruplastERP.Api.DTO_s
{
    public class OrdenProduccion
    {
        public int ProductoTerminadoId { get; set; }
        public decimal Cantidad { get; set; }
        public int EmpleadoId { get; set; }
        public string Turno { get; set; } = "Mañana";
        public int? ClienteId { get; set; }
    }
}
