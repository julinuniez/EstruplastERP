using System;
using System.Collections.Generic;

namespace EstruplastERP.Api.Dtos
{
    // Este es el DTO oficial que tiene TODO lo nuevo
    public class NuevoRemitoDto
    {
        public int ClienteId { get; set; }  // Vinculación por ID
        public string NumeroRemito { get; set; }
        public DateTime Fecha { get; set; } // Fecha seleccionable
        public string Observacion { get; set; }
        public List<ItemRemitoDto> Items { get; set; }
    }

    public class ItemRemitoDto
    {
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; } // Decimal para evitar errores
        public string Detalle { get; set; }   // Para el color/medida
    }

    // Puedes mover aquí también los de Ingreso/Ajuste si quieres ser ordenado
    public class IngresoDto
    {
        public int ProductoId { get; set; }
        public decimal Cantidad { get; set; }
        public decimal NuevoPrecio { get; set; }
        public string? Proveedor { get; set; }
    }

    public class AjusteDto
    {
        public int ProductoId { get; set; }
        public decimal CantidadReal { get; set; }
        public string Motivo { get; set; }
    }
}