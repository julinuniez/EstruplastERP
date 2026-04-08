using System.Collections.Generic;

namespace EstruplastERP.Api.Dtos
{
    public class ConsumoRealDto
    {
        public int MateriaPrimaId { get; set; }
        public decimal CantidadKilosReales { get; set; }
    }

    public class ConfirmacionCierreDto
    {
        public decimal KilosProducidosReales { get; set; }
        public decimal DesperdicioReal { get; set; }
        public string Observacion { get; set; } // 👈 Agregamos esto
        public List<ConsumoRealDto> ConsumosReales { get; set; } = new List<ConsumoRealDto>();
    }
}