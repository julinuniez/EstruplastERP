namespace EstruplastERP.Api.Dtos
{
    public class RegistrarCompraDto
    {
        public int ProductoId { get; set; }
        public int ProveedorId { get; set; }
        public decimal Cantidad { get; set; }
        public string? NumeroRemito { get; set; }
        public string? Observacion { get; set; }
    }
}