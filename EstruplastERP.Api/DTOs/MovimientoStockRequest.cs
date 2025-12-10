namespace EstruplastERP.Api.Dtos
{
    public class MovimientoStockRequest
    {
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; } // Positivo (Compra) o Negativo (Ajuste/Pérdida)
        public string Observacion { get; set; } = string.Empty; // Ej: "Remito Proveedor X"
    }
}