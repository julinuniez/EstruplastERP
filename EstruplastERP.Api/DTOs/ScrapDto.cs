namespace EstruplastERP.Api.Dtos
{
    public class ScrapDto
    {
        public int ClienteId { get; set; }
        public int ProductoScrapId { get; set; }
        public decimal KilosEntrada { get; set; }   
        public decimal KilosObtenidos { get; set; }
    }
}