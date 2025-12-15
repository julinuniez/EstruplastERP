namespace EstruplastERP.Api.Dtos
{
    // 1. PARA LECTURA EN LISTAS (GET /inventario)
    // Ligero: Sin receta, solo datos clave para grillas.
    public class ProductoListaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public decimal StockActual { get; set; }
        public decimal PrecioCosto { get; set; }
        public bool EsProductoTerminado { get; set; }
        public bool EsMateriaPrima { get; set; }
    }

    // 2. PARA EDICIÓN O DETALLE (GET /id)
    // Completo: Incluye dimensiones y la Receta limpia (sin ciclos).
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

    // 3. PARA CREAR/EDITAR (POST/PUT)
    // Es el que ya tenías, pero ahora en su propia casa.
    public class NuevoProductoDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string CodigoSku { get; set; } = string.Empty;
        public string? Color { get; set; }
        public decimal PrecioCosto { get; set; }
        public decimal StockMinimo { get; set; }
        public List<IngredienteDto>? Receta { get; set; }
    }

    public class ProductoEditarDto
    {
        public string Nombre { get; set; }
        public string CodigoSku { get; set; }
        public string? Color { get; set; }
        public decimal StockMinimo { get; set; }
    }

    // 4. INGREDIENTE (Usado en Detalle y Creación)
    public class IngredienteDto
    {
        public int MateriaPrimaId { get; set; }
        public string? NombreInsumo { get; set; } // Opcional, solo para lectura
        public decimal Cantidad { get; set; }
    }
}