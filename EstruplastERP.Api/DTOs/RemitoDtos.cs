namespace EstruplastERP.Api.Dtos
{
    public class NuevoRemitoDto
    {
        public string Cliente { get; set; }       // El nombre del cliente
        public string NumeroRemito { get; set; }  // El número "0001-..."
        public List<ItemRemitoDto> Items { get; set; } // La lista del carrito
    }
    public class ItemRemitoDto
    {
        public int ProductoId { get; set; }
        public double Cantidad { get; set; }
    }
}