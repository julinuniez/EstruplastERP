using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        public bool EsGenerico { get; set; }
        public bool EsPremezcla { get; set; }
        public bool EsCritico { get; set; }
    }

    // 2. PARA EDICIÓN O DETALLE (GET /id)
    public class ProductoDetalleDto : ProductoListaDto
    {
        public decimal Largo { get; set; }
        public decimal Ancho { get; set; }
        public decimal Espesor { get; set; }
        public decimal EspesorMinimo { get; set; }
        public decimal EspesorMaximo { get; set; }
        public decimal PesoEspecifico { get; set; }
        public decimal StockMinimo { get; set; }
        public string? Rubro { get; set; }
        public List<IngredienteDto> Receta { get; set; } = new List<IngredienteDto>();
    }

    // 3. PARA CREAR (POST)
    public class NuevoProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public decimal PrecioCosto { get; set; }
        public decimal StockMinimo { get; set; }
        public bool EsCritico { get; set; }
        public int? ProveedorId { get; set; }
        public List<IngredienteDto>? Receta { get; set; }
    }

    // 4. PARA EDITAR (PUT)
    public class ProductoEditarDto
    {
<<<<<<< HEAD
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public decimal StockMinimo { get; set; }
        public bool EsCritico { get; set; }
=======
        public string Nombre { get; set; }
        public string CodigoSku { get; set; }

        [JsonPropertyName("stockMinimo")] 
        public decimal StockMinimo { get; set; }

        public string? Color { get; set; }

        [JsonPropertyName("stockActual")] 
        public decimal StockActual { get; set; }

        [JsonPropertyName("precioCosto")] 
        public decimal PrecioCosto { get; set; }
>>>>>>> master
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
<<<<<<< HEAD
        public bool EsPremezcla { get; set; }
        public bool EsCritico { get; set; }
        public decimal PrecioCosto { get; set; }
        public string? Rubro { get; set; }
=======
        public decimal StockActual { get; set; }

>>>>>>> master
        public List<IngredienteDto>? Receta { get; set; }
    }
}