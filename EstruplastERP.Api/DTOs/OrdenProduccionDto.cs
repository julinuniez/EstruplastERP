namespace EstruplastERP.Api.DTOs
{
    public class OrdenProduccionDto
    {
        public int ProductoTerminadoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal Largo { get; set; }
        public decimal Ancho { get; set; }
        public decimal Espesor { get; set; }
        public string? Color { get; set; }
        public int? ClienteId { get; set; }
    }
}
