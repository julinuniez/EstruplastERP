namespace EstruplastERP.Api.Dtos
{
    public class NuevaOrdenDto
    {
        public int ProductoTerminadoId { get; set; }
        public int? ClienteId { get; set; }
        public string? NumeroPedidoCliente { get; set; }
        public string? NotaPedido { get; set; }
        public int Cantidad { get; set; }
        public decimal Largo { get; set; }
        public decimal Ancho { get; set; }
        public decimal Espesor { get; set; }
        public string? Color { get; set; }
        public decimal Kilos { get; set; }
        public string? Observacion { get; set; }
        public List<DetalleConsumoDto> Consumos { get; set; } = new List<DetalleConsumoDto>();
    }

    public class DetalleConsumoDto
    {
        public int MateriaPrimaId { get; set; }
        public decimal CantidadKilos { get; set; } // Ya mandamos los kilos calculados
    }
}