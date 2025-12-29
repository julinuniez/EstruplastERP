using System.Collections.Generic;

namespace EstruplastERP.Api.Dtos
{
    // 1. PARA LECTURA EN LISTAS (GET /inventario)
    public class ProductoListaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
        public decimal PrecioCosto { get; set; }
        public bool EsProductoTerminado { get; set; }
        public bool EsMateriaPrima { get; set; }

        // ✅ AGREGADO: Necesario para que el Frontend sepa si bloquear medidas
        public bool EsGenerico { get; set; }
    }

    // 2. PARA EDICIÓN O DETALLE (GET /id)
    // Hereda de ProductoListaDto, así que ya tiene 'EsGenerico'
    public class ProductoDetalleDto : ProductoListaDto
    {
        public decimal Largo { get; set; }
        public decimal Ancho { get; set; }
        public decimal Espesor { get; set; }
        public decimal PesoEspecifico { get; set; }
        public decimal StockMinimo { get; set; }
        public string? Color { get; set; }

        public List<IngredienteDto> Receta { get; set; } = new List<IngredienteDto>();
    }

    // 3. PARA CREAR (POST)
    public class NuevoProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public string? Color { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal StockMinimo { get; set; }

        // Opcional: Si quieres poder definir si es Genérico desde el Front
        // public bool EsGenerico { get; set; } 

        public List<IngredienteDto>? Receta { get; set; }
    }

    // 4. PARA EDITAR (PUT)
    public class ProductoEditarDto
    {
        public string Nombre { get; set; }
        public string CodigoSku { get; set; }
        public string? Color { get; set; }
        public decimal StockMinimo { get; set; }
    }

    // 5. INGREDIENTE
    public class IngredienteDto
    {
        public int MateriaPrimaId { get; set; }
        public string? NombreInsumo { get; set; }
        public decimal Cantidad { get; set; }
    }

    // 6. PARA CONFIGURACIÓN TÉCNICA (Peso, Tipo, etc.)
    public class ProductoConfigDto
    {
        public decimal StockMinimo { get; set; }
        public decimal PesoEspecifico { get; set; }
        public bool EsMateriaPrima { get; set; }
        public bool EsProductoTerminado { get; set; }
        public bool EsFazon { get; set; }

        // 🔥 AGREGADO: La lista de ingredientes para guardar
        public List<IngredienteDto>? Receta { get; set; }
    }
}