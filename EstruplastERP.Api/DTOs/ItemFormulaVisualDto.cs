namespace EstruplastERP.Api.Dtos
{
    public class ItemFormulaVisualDto
    {
        public int MateriaPrimaId { get; set; }
        public string Nombre { get; set; }
        public bool EsSustitucion { get; set; } // true = Es material del cliente
        public decimal CantidadRequerida { get; set; }
        public int? SustitutoId { get; set; }
        public string SustitutoNombre { get; set; }
        public sbyte Nota { get; set; }
    }
}