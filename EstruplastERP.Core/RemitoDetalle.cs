namespace EstruplastERP.Core
{
    public class RemitoDetalle
    {
        public int Id { get; set; }
        public int RemitoId { get; set; }
        public Remito Remito { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public decimal Cantidad { get; set; }
        public string? Detalle { get; set; } // ✅ NUEVO: Aquí se guarda "Rojo", "40 micrones"
        public decimal PrecioUnitarioSnapshot { get; set; }
    }
}