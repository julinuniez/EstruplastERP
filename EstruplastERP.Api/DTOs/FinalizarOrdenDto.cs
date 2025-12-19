namespace EstruplastERP.Api.Dtos
{
    public class FinalizarOrdenDto
    {
        public List<AdicionDto>? Adiciones { get; set; }
        public decimal Desperdicio { get; set; }
        public string? Observacion { get; set; }
    }

    public class AdicionDto
    {
        public int MateriaPrimaId { get; set; }
        public decimal Cantidad { get; set; }
        public string? Motivo { get; set; }
    }
}