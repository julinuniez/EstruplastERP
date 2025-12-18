namespace EstruplastERP.Api.Dtos
{
    public class RegistrarCompraDto
    {
        public int ProductoId { get; set; }
        public int ProveedorId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; } // Este es el precio DE ESTA COMPRA específica
        public string? NumeroRemito { get; set; }
        public string? Lote { get; set; } // Nuevo campo
        public string? Observacion { get; set; }
    }
}