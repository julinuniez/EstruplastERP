namespace EstruplastERP.Api.Dtos
{
    public class IngresoScrapDto
    {
        public int? ClienteId { get; set; } 
        public string NombreProducto { get; set; } 
        public decimal Cantidad { get; set; }
    }

    public class IngresoMolidoRequest
    {
        public int? ClienteId { get; set; }     // Null = Interno
        public int MaterialBaseId { get; set; } // ID del "Polipropileno" generico
        public string? Variedad { get; set; }   // "Rojo", "Sillas", "Baldes"
        public decimal Kilos { get; set; }
        public int? ProductoExistenteId { get; set; }
    }
}