using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using OfficeOpenXml;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IntegrationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IntegrationController(ApplicationDbContext context)
        {
            _context = context;
            ExcelPackage.License.SetNonCommercialPersonal("FreelanceDev");
        }

        // =================================================================================
        // 1. IMPORTACIÓN FLEXXUS (CSV)
        // =================================================================================
        [HttpPost("importar-maestro")]
        public async Task<IActionResult> ImportarMaestro(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0) return BadRequest("Suba un archivo .csv válido.");

            int creados = 0, actualizados = 0;

            try
            {
                var config = new CsvConfiguration(new CultureInfo("es-AR"))
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    ShouldSkipRecord = args => args.Row.Parser.Row == 1,
                    MissingFieldFound = null,
                    BadDataFound = null,
                    Encoding = Encoding.Latin1
                };

                using (var stream = new StreamReader(archivo.OpenReadStream(), Encoding.Latin1))
                using (var csv = new CsvReader(stream, config))
                {
                    var registros = csv.GetRecords<FlexxusMaestroDto>().ToList();
                    var productosDb = await _context.Productos.ToListAsync();

                    foreach (var row in registros)
                    {
                        if (string.IsNullOrWhiteSpace(row.CodigoSku)) continue;

                        string sku = row.CodigoSku.Trim().ToUpper();
                        string nombre = row.Nombre?.Trim() ?? "SIN NOMBRE";
                        string rubro = row.Rubro?.Trim().ToUpper() ?? "OTROS";

                        if (sku.Contains("/") || sku.Length < 3) continue;

                        bool esMP = rubro.Contains("MATERIA PRIMA") || rubro.Contains("MASTERBATCH") || rubro.Contains("INSUMO");

                        // Usamos la misma lógica de detección, pero sin contexto de archivo (null)
                        var (tipoDetectado, _) = DetectarMaterialYColor(sku, nombre, null);
                        if (!string.IsNullOrEmpty(row.TipoMaterial)) tipoDetectado = row.TipoMaterial.ToUpper();

                        var prod = productosDb.FirstOrDefault(p => p.CodigoSku.Trim().ToUpper() == sku);

                        if (prod != null)
                        {
                            // Actualización
                            bool cambios = false;
                            if (prod.Nombre != nombre) { prod.Nombre = nombre; cambios = true; }
                            if (esMP && !prod.EsMateriaPrima) { prod.EsMateriaPrima = true; prod.EsProductoTerminado = false; cambios = true; }
                            if (tipoDetectado != "OTROS" && prod.TipoMaterial != tipoDetectado) { prod.TipoMaterial = tipoDetectado; cambios = true; }

                            if (cambios) { _context.Entry(prod).State = EntityState.Modified; actualizados++; }
                        }
                        else
                        {
                            // Creación
                            _context.Productos.Add(new Producto
                            {
                                CodigoSku = sku,
                                Nombre = nombre,
                                Rubro = rubro,
                                TipoMaterial = tipoDetectado,
                                EsMateriaPrima = esMP,
                                EsProductoTerminado = !esMP,
                                EsScrap = false,
                                StockActual = 0,
                                StockMinimo = 100,
                                Activo = true,
                                FechaCreacion = DateTime.Now,
                                PesoEspecifico = esMP ? 1.05m : 1.0m
                            });
                            creados++;
                        }
                    }
                    if (actualizados > 0 || creados > 0) await _context.SaveChangesAsync();
                }
                return Ok(new { mensaje = $"Flexxus: {creados} creados, {actualizados} actualizados." });
            }
            catch (Exception ex) { return StatusCode(500, $"Error: {ex.Message}"); }
        }

        // =================================================================================
        // 2. IMPORTACIÓN MULTI-CLIENTE (EXCEL) - LÓGICA ROBUSTA DE STOCK
        // =================================================================================
        [HttpPost("importar-excel-multicliente")]
        public async Task<IActionResult> ImportarExcelMultiCliente(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0) return BadRequest("Suba un Excel válido.");

            string nombreArchivo = archivo.FileName.ToUpper();
            bool esModoScrap = nombreArchivo.Contains("SCRAP");

            int hojas = 0, prods = 0;
            var logs = new List<string>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await archivo.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        // Intentamos calcular fórmulas. Si falla, seguimos con los valores en caché.
                        try { package.Workbook.Calculate(); } catch { }

                        var clientesDb = await _context.Clientes.ToListAsync();
                        var productosDb = await _context.Productos.ToListAsync();

                        foreach (var worksheet in package.Workbook.Worksheets)
                        {
                            string nombreHoja = worksheet.Name.Trim().ToUpper();

                            // Buscar cliente por coincidencia de nombre
                            var cliente = clientesDb.FirstOrDefault(c =>
                                c.RazonSocial.ToUpper().Replace(".", "").Trim() == nombreHoja.Replace(".", "").Trim() ||
                                c.RazonSocial.ToUpper().Contains(nombreHoja) ||
                                nombreHoja.Contains(c.RazonSocial.ToUpper()));

                            if (cliente == null)
                            {
                                // logs.Add($"⚠️ Hoja '{worksheet.Name}': Ignorada (Sin cliente)."); 
                                continue;
                            }

                            hojas++;

                            // A. Detectar Cabecera
                            int filaInicio = -1, colCodigo = -1, colDesc = -1;
                            int colTotal = -1, colExistencias = -1, colSaldo = -1;

                            // Buscamos en las primeras 20 filas y 20 columnas
                            for (int r = 1; r <= 20; r++)
                            {
                                for (int c = 1; c <= 20; c++)
                                {
                                    var celda = worksheet.Cells[r, c].Text.ToUpper().Trim();

                                    if (celda == "CODIGO") { filaInicio = r; colCodigo = c; }
                                    if (celda == "DESCRIPCION") colDesc = c;

                                    // Búsqueda flexible de columnas de stock
                                    if (celda == "TOTAL") colTotal = c;
                                    else if (colTotal == -1 && celda.Contains("TOTAL")) colTotal = c;

                                    if (celda == "EXISTENCIAS") colExistencias = c;
                                    else if (colExistencias == -1 && celda.Contains("EXISTENCIA")) colExistencias = c;

                                    if (celda == "SALDO") colSaldo = c;
                                }
                                // Si encontramos Código y al menos una columna de stock, paramos
                                if (filaInicio != -1 && (colTotal != -1 || colExistencias != -1 || colSaldo != -1)) break;
                            }

                            if (filaInicio == -1) continue;

                            // B. Recorrer Filas
                            int fila = filaInicio + 1;
                            int filasVaciasConsecutivas = 0;

                            while (fila < 5000 && filasVaciasConsecutivas < 5)
                            {
                                // Leer Código y Descripción
                                string codigo = worksheet.Cells[fila, colCodigo].Text.Trim();
                                string desc = worksheet.Cells[fila, colDesc].Text.Trim();

                                if (string.IsNullOrEmpty(codigo) && string.IsNullOrEmpty(desc))
                                {
                                    filasVaciasConsecutivas++;
                                    fila++;
                                    continue;
                                }
                                filasVaciasConsecutivas = 0; // Reseteamos si encontramos dato

                                // C. Extracción Robusta del Stock
                                decimal stockFinal = 0;
                                bool stockEncontrado = false;

                                // Prioridad 1: TOTAL
                                if (colTotal != -1) stockFinal = LeerNumeroRobusto(worksheet.Cells[fila, colTotal].Value, out stockEncontrado);

                                // Prioridad 2: EXISTENCIAS (Si Total falló o dio 0)
                                if (stockFinal == 0 && colExistencias != -1)
                                {
                                    decimal s2 = LeerNumeroRobusto(worksheet.Cells[fila, colExistencias].Value, out bool encontro2);
                                    if (s2 != 0 || encontro2) { stockFinal = s2; stockEncontrado = true; }
                                }

                                // Prioridad 3: SALDO
                                if (stockFinal == 0 && colSaldo != -1)
                                {
                                    decimal s3 = LeerNumeroRobusto(worksheet.Cells[fila, colSaldo].Value, out bool encontro3);
                                    if (s3 != 0 || encontro3) { stockFinal = s3; stockEncontrado = true; }
                                }

                                // Procesar si hay código válido (aunque el stock sea 0, lo creamos/actualizamos)
                                if (!string.IsNullOrEmpty(codigo))
                                {
                                    await ProcesarProductoCliente(codigo, desc, stockFinal, cliente.Id, productosDb, esModoScrap, nombreArchivo);
                                    prods++;
                                }
                                fila++;
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok(new { mensaje = $"✅ Importación: {hojas} hojas procesadas, {prods} productos actualizados.", logs = logs });
            }
            catch (Exception ex) { return StatusCode(500, $"Error crítico: {ex.Message}"); }
        }

        // --- HELPER PARA LEER NÚMEROS SIN FALLAR POR PUNTOS O COMAS ---
        private decimal LeerNumeroRobusto(object valorCelda, out bool exito)
        {
            exito = false;
            if (valorCelda == null) return 0;

            // 1. Si ya es número, devolver directo
            if (valorCelda is double d) { exito = true; return (decimal)d; }
            if (valorCelda is decimal dec) { exito = true; return dec; }
            if (valorCelda is int i) { exito = true; return i; }

            string texto = valorCelda.ToString().Trim();
            if (string.IsNullOrEmpty(texto)) return 0;

            // 2. Intentar parseo estándar (Cultura Invariante - Puntos)
            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal res1))
            {
                exito = true;
                return res1;
            }

            // 3. Intentar parseo local (Argentina/España - Comas)
            if (decimal.TryParse(texto, NumberStyles.Any, new CultureInfo("es-AR"), out decimal res2))
            {
                exito = true;
                return res2;
            }

            return 0; // No se pudo leer
        }

        private async Task ProcesarProductoCliente(string codigo, string nombre, decimal stock, int clienteId, List<Producto> productosDb, bool esScrap, string nombreArchivoContexto)
        {
            // 1. Detectar Material usando el contexto del archivo
            var (tipo, color) = DetectarMaterialYColor(codigo, nombre, nombreArchivoContexto);

            // 2. Generar SKU Sistema
            string prefijo = esScrap ? "SCRAP" : "MP";
            string skuSistema = $"{prefijo}-{tipo}-{color}-CLI-{clienteId}".ToUpper().Replace(" ", "");

            var prod = productosDb.FirstOrDefault(p => p.CodigoSku == skuSistema && p.ClienteId == clienteId);

            if (prod != null)
            {
                prod.StockActual = stock; // Actualizamos el stock
                prod.TipoMaterial = tipo;
                prod.Color = color;
                prod.EsScrap = esScrap;
                prod.EsMateriaPrima = !esScrap;
                if (prod.Id > 0) _context.Entry(prod).State = EntityState.Modified;
            }
            else
            {
                var nuevo = new Producto
                {
                    CodigoSku = skuSistema,
                    Nombre = esScrap ? $"[SCRAP] {tipo} - {color} ({codigo})" : $"[MP] {tipo} - {color} ({codigo})",
                    Rubro = esScrap ? "SCRAP / MP CLIENTE" : "MATERIA PRIMA CLIENTE",
                    TipoMaterial = tipo,
                    Color = color,
                    ClienteId = clienteId,
                    EsScrap = esScrap,
                    EsMateriaPrima = !esScrap,
                    EsProductoTerminado = false,
                    StockActual = stock, // Stock Inicial detectado
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    PesoEspecifico = 1
                };
                _context.Productos.Add(nuevo);
                productosDb.Add(nuevo);
            }
        }

        // =================================================================================
        // 🔥 LÓGICA MAESTRA DE DETECCIÓN DE MATERIALES
        // =================================================================================
        private (string Tipo, string Color) DetectarMaterialYColor(string sku, string nombre, string? contextoArchivo)
        {
            string s = sku?.ToUpper() ?? "";
            string n = nombre?.ToUpper() ?? "";
            string archivo = contextoArchivo?.ToUpper() ?? "";

            // 1. REGLAS DE ORO
            if (s.Contains("003") || n.Contains("BIO") || archivo.Contains("BIO")) return ("BIO", "NATURAL");

            // 2. RESISTENTE FREON (RF)
            bool esFreon = archivo.Contains("FREON") || archivo.Contains("RES. FREON") || n.Contains("FREON") || s.StartsWith("RF");

            // 3. CÓDIGOS NUMÉRICOS (000-015)
            string colorDetectado = "-";

            if (s.Contains("000") || n.Contains("TUTI") || n.Contains("TUTTI")) colorDetectado = "TUTI";
            else if (s.Contains("001") || n.Contains("NATURAL")) colorDetectado = "NATURAL";
            else if (s.Contains("002") || n.Contains("BLANCO")) colorDetectado = "BLANCO";
            else if (s.Contains("004") || n.Contains("AMARILLO")) colorDetectado = "AMARILLO";
            else if (s.Contains("005") || n.Contains("NARANJA")) colorDetectado = "NARANJA";
            else if (s.Contains("006") || n.Contains("ROSA")) colorDetectado = "ROSA";
            else if (s.Contains("007") || n.Contains("ROJO")) colorDetectado = "ROJO";
            else if (s.Contains("008") || n.Contains("VIOLETA")) colorDetectado = "VIOLETA";
            else if (s.Contains("009") || n.Contains("CELESTE")) colorDetectado = "CELESTE";
            else if (s.Contains("010") || n.Contains("AZUL")) colorDetectado = "AZUL";
            else if (s.Contains("011") || n.Contains("VERDE")) colorDetectado = "VERDE";
            else if (s.Contains("012") || n.Contains("MARRON")) colorDetectado = "MARRON";
            else if (s.Contains("013") || n.Contains("GRIS")) colorDetectado = "GRIS";
            else if (s.Contains("014") || n.Contains("PLATA")) colorDetectado = "GRIS PLATA";
            else if (s.Contains("015") || n.Contains("NEGRO")) colorDetectado = "NEGRO";

            if (colorDetectado != "-")
            {
                if (esFreon) return ("RESISTENTE FREON", colorDetectado);
                return ("PAI", colorDetectado);
            }

            // 4. OTROS MATERIALES
            if (archivo.Contains("ABS") || n.Contains("ABS")) return ("ABS", "-");
            if (archivo.Contains("PP") || archivo.Contains("POLIPROPILENO") || n.Contains("PP") || n.Contains("POLIPROPILENO")) return ("PP", "-");
            if (archivo.Contains("PEAD") || archivo.Contains("ALTA") || archivo.Contains("HDPE") || n.Contains("PEAD") || n.Contains("ALTA") || n.Contains("HDPE")) return ("PEAD", "-");
            if (archivo.Contains("PEBD") || archivo.Contains("BAJA") || archivo.Contains("POLIETILENO") || n.Contains("PEBD") || n.Contains("BAJA") || n.Contains("POLIETILENO")) return ("POLIETILENO", "-");
            if (archivo.Contains("PAI") || n.Contains("PAI") || n.Contains("IMPACTO")) return ("PAI", "VARIOS");

            return ("OTROS", "VARIOS");
        }
    }
}