namespace EstruplastERP.Core
{
    public class RemitoDetalle
    {
        public int Id { get; set; }

        public int RemitoId { get; set; }
        public Remito Remito { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; } // Relación con Producto

        public double Cantidad { get; set; }
    }
}