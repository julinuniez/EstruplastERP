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
        // 1. IMPORTACIÓN FLEXXUS (CSV) - Mantiene tu lógica actual
        // =================================================================================
        [HttpPost("importar-maestro")]
        public async Task<IActionResult> ImportarMaestro(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Por favor, suba un archivo .csv válido.");

            int creados = 0;
            int actualizados = 0;

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

                        string skuLimpio = row.CodigoSku.Trim().ToUpper();
                        string nombreLimpio = row.Nombre?.Trim() ?? "SIN NOMBRE";
                        string rubroLimpio = row.Rubro?.Trim().ToUpper() ?? "OTROS";
                        string? tipoMaterialCsv = row.TipoMaterial?.Trim().ToUpper();

                        if (skuLimpio.Contains("/") || skuLimpio.Contains(":") || skuLimpio.Length < 3) continue;

                        bool esMateriaPrima = rubroLimpio.Contains("MATERIA PRIMA") || rubroLimpio.Contains("MASTERBATCH") || rubroLimpio.Contains("INSUMO");
                        bool esProductoTerminado = !esMateriaPrima;

                        string? tipoDetectado = null;
                        if (!string.IsNullOrEmpty(tipoMaterialCsv))
                        {
                            tipoDetectado = tipoMaterialCsv;
                        }
                        else
                        {
                            string n = nombreLimpio.ToUpper();
                            if (n.Contains("FREON") || n.Contains("RESISTENTE")) tipoDetectado = "PAI FREON"; // Cambio a nombre estándar
                            else if (n.Contains("BIO") || n.Contains("BIODEGRADABLE")) tipoDetectado = "BIO";
                            else if (n.Contains("ABS")) tipoDetectado = "ABS";
                            else if (n.Contains("MARBEA") || n.Contains("MB")) tipoDetectado = "MARBEA";
                            else if (n.Contains("PAI") || n.Contains("IMPACTO") || n.Contains("A.I.") || n.Contains("TUTI")) tipoDetectado = "PAI";
                            else if (n.Contains("PEAD") || n.Contains("ALTA") || n.Contains("HDPE")) tipoDetectado = "PEAD";
                            else if (n.Contains("PP") || n.Contains("POLIPROPILENO")) tipoDetectado = "PP";
                            else if (n.Contains("PEBD") || n.Contains("BAJA") || n.Contains("LDPE") || n.Contains("POLIETILENO")) tipoDetectado = "POLIETILENO"; // Estandarizado
                        }

                        var prod = productosDb.FirstOrDefault(p => p.CodigoSku.Trim().ToUpper().ToString() == skuLimpio);

                        if (prod != null)
                        {
                            bool huboCambios = false;
                            if (prod.Nombre != nombreLimpio) { prod.Nombre = nombreLimpio; huboCambios = true; }
                            if (prod.Rubro != rubroLimpio) { prod.Rubro = rubroLimpio; huboCambios = true; }
                            if (prod.EsMateriaPrima != esMateriaPrima) { prod.EsMateriaPrima = esMateriaPrima; huboCambios = true; }
                            if (prod.EsProductoTerminado != esProductoTerminado) { prod.EsProductoTerminado = esProductoTerminado; huboCambios = true; }

                            if (!string.IsNullOrEmpty(tipoDetectado) && prod.TipoMaterial != tipoDetectado)
                            {
                                prod.TipoMaterial = tipoDetectado;
                                huboCambios = true;
                            }

                            if (huboCambios)
                            {
                                _context.Entry(prod).State = EntityState.Modified;
                                actualizados++;
                            }
                        }
                        else
                        {
                            var nuevo = new Producto
                            {
                                CodigoSku = skuLimpio,
                                Nombre = nombreLimpio,
                                Rubro = rubroLimpio,
                                TipoMaterial = tipoDetectado,
                                EsMateriaPrima = esMateriaPrima,
                                EsProductoTerminado = esProductoTerminado,
                                PrecioCosto = 0,
                                EsGenerico = false,
                                EsFazon = false,
                                StockActual = 0,
                                StockMinimo = 100,
                                Activo = true,
                                FechaCreacion = DateTime.Now,
                                PesoEspecifico = esMateriaPrima ? 1.05m : 1.0m
                            };
                            _context.Productos.Add(nuevo);
                            creados++;
                        }
                    }

                    if (actualizados > 0 || creados > 0) await _context.SaveChangesAsync();
                }

                return Ok(new { mensaje = $"Proceso Flexxus terminado.\n🆕 Creados: {creados}\n🔄 Actualizados: {actualizados}" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error procesando archivo: {ex.Message}");
            }
        }

        // =================================================================================
        // 2. IMPORTACIÓN MULTI-CLIENTE (EXCEL) - Nueva Lógica Fazón/Scrap
        // =================================================================================
        [HttpPost("importar-excel-multicliente")]
        public async Task<IActionResult> ImportarExcelMultiCliente(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Por favor, suba un archivo Excel (.xlsx) válido.");

            int hojasProcesadas = 0;
            int productosProcesados = 0;
            var logs = new List<string>();

            try
            {
                using (var stream = new MemoryStream())
                {
                    await archivo.CopyToAsync(stream);
                    using (var package = new ExcelPackage(stream))
                    {
                        var clientesDb = await _context.Clientes.ToListAsync();
                        var productosDb = await _context.Productos.ToListAsync();

                        foreach (var worksheet in package.Workbook.Worksheets)
                        {
                            string nombreHoja = worksheet.Name.Trim().ToUpper();

                            // A. Buscar Cliente por nombre de hoja
                            var cliente = clientesDb.FirstOrDefault(c =>
                                c.RazonSocial.ToUpper().Contains(nombreHoja) ||
                                nombreHoja.Contains(c.RazonSocial.ToUpper()));

                            if (cliente == null)
                            {
                                logs.Add($"⚠️ Hoja '{worksheet.Name}': Ignorada (No se encontró cliente asociado).");
                                continue;
                            }

                            hojasProcesadas++;

                            // B. Encontrar cabecera de la tabla
                            int filaInicio = -1, colCodigo = -1, colDesc = -1, colTotal = -1;

                            // Buscamos en las primeras 15 filas
                            for (int r = 1; r <= 15; r++)
                            {
                                for (int c = 1; c <= 10; c++)
                                {
                                    var txt = worksheet.Cells[r, c].Text.ToUpper().Trim();
                                    if (txt == "CODIGO") { filaInicio = r; colCodigo = c; }
                                    if (txt == "DESCRIPCION") colDesc = c;
                                    if (txt == "TOTAL" || txt == "EXISTENCIAS") colTotal = c;
                                }
                                if (filaInicio != -1) break;
                            }

                            if (filaInicio == -1 || colTotal == -1) continue;

                            // C. Recorrer filas de productos
                            int fila = filaInicio + 1;
                            while (!string.IsNullOrEmpty(worksheet.Cells[fila, colCodigo].Text))
                            {
                                string codigoExcel = worksheet.Cells[fila, colCodigo].Text.Trim();
                                string descExcel = worksheet.Cells[fila, colDesc].Text.Trim();
                                string totalStr = worksheet.Cells[fila, colTotal].Text.Trim();

                                if (decimal.TryParse(totalStr, out decimal stockTotal))
                                {
                                    // 🔥 SOLO PROCESAR SI HAY STOCK > 0
                                    if (stockTotal > 0)
                                    {
                                        await ProcesarProductoCliente(codigoExcel, descExcel, stockTotal, cliente.Id, productosDb);
                                        productosProcesados++;
                                    }
                                }
                                fila++;
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new
                {
                    mensaje = $"✅ Importación Exitosa.\n📂 Clientes detectados: {hojasProcesadas}\n📦 Productos con stock actualizados: {productosProcesados}",
                    logs = logs
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error crítico en Excel: {ex.Message}");
            }
        }

        // --- FUNCIONES AUXILIARES (Detectar y Procesar) ---

        private async Task ProcesarProductoCliente(string codigoOriginal, string nombreOriginal, decimal stock, int clienteId, List<Producto> productosExistentes)
        {
            var (tipo, color) = DetectarMaterialYColor(codigoOriginal, nombreOriginal);
            string skuSistema = $"SCRAP-{tipo}-{color}-CLI-{clienteId}".ToUpper().Replace(" ", "");

            var prod = productosExistentes.FirstOrDefault(p => p.CodigoSku == skuSistema && p.ClienteId == clienteId);

            if (prod != null)
            {
                prod.StockActual = stock;
                prod.TipoMaterial = tipo;
                prod.Color = color;

                // 🔥 ACTUALIZACIÓN: Aseguramos que si ya existía, deje de ser MP si lo era
                prod.EsMateriaPrima = false;
                prod.EsScrap = true;

                if (prod.Id > 0) _context.Entry(prod).State = EntityState.Modified;
            }
            else
            {
                var nuevo = new Producto
                {
                    CodigoSku = skuSistema,
                    Nombre = $"[SCRAP] {tipo} - {color} ({codigoOriginal})",
                    Rubro = "SCRAP / MP CLIENTE",
                    TipoMaterial = tipo,
                    Color = color,
                    ClienteId = clienteId,

                    EsScrap = true,          // Es Scrap (Basura/Recuperable)
                    EsMateriaPrima = false,  // ⛔ NO es Materia Prima (No se puede usar en producción directa)
                    EsProductoTerminado = false,

                    StockActual = stock,
                    StockMinimo = 0,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    PesoEspecifico = 1
                };
                _context.Productos.Add(nuevo);
                productosExistentes.Add(nuevo);
            }
        }

        private (string Tipo, string Color) DetectarMaterialYColor(string sku, string nombre)
        {
            string s = sku.ToUpper();
            string n = nombre.ToUpper();

            // 1. REGLA MAESTRA DE LOS 16 CÓDIGOS PAI (Prioridad Absoluta)
            if (s.Contains("003") || n.Contains("BIO") || n.Contains("DEGRADABLE")) return ("BIO", "NATURAL");

            // PAI COLORES
            if (s.Contains("000") || n.Contains("TUTI") || n.Contains("TUTTI")) return ("PAI", "TUTI");
            if (s.Contains("001") || n.Contains("NATURAL")) return ("PAI", "NATURAL");
            if (s.Contains("002") || n.Contains("BLANCO")) return ("PAI", "BLANCO");
            if (s.Contains("004") || n.Contains("AMARILLO")) return ("PAI", "AMARILLO");
            if (s.Contains("005") || n.Contains("NARANJA")) return ("PAI", "NARANJA");
            if (s.Contains("006") || n.Contains("ROSA")) return ("PAI", "ROSA");
            if (s.Contains("007") || n.Contains("ROJO")) return ("PAI", "ROJO");
            if (s.Contains("008") || n.Contains("VIOLETA")) return ("PAI", "VIOLETA");
            if (s.Contains("009") || n.Contains("CELESTE")) return ("PAI", "CELESTE");
            if (s.Contains("010") || n.Contains("AZUL")) return ("PAI", "AZUL");
            if (s.Contains("011") || n.Contains("VERDE")) return ("PAI", "VERDE");
            if (s.Contains("012") || n.Contains("MARRON")) return ("PAI", "MARRON");
            if (s.Contains("013") || n.Contains("GRIS")) return ("PAI", "GRIS"); // Ojo con Gris Plata
            if (s.Contains("014") || n.Contains("PLATA")) return ("PAI", "GRIS PLATA");
            if (s.Contains("015") || n.Contains("NEGRO")) return ("PAI", "NEGRO");

            // 2. OTROS MATERIALES (Por nombre de Archivo o Descripción general)
            if (n.Contains("FREON") || n.Contains("RESISTENTE")) return ("RESISTENTE FREON", "-");
            if (n.Contains("ABS")) return ("ABS", "-");

            // PEAD / ALTA
            if (n.Contains("PEAD") || n.Contains("ALTA") || n.Contains("HDPE")) return ("PEAD", "-");

            // PP / POLIPROPILENO
            if (n.Contains("PP") || n.Contains("POLIPROPILENO")) return ("PP", "-");

            // POLIETILENO / BAJA / PEBD (Agrupados)
            if (n.Contains("PEBD") || n.Contains("BAJA") || n.Contains("LDPE") || n.Contains("POLIETILENO")) return ("POLIETILENO", "-");

            // 3. Fallback General
            if (n.Contains("PAI") || n.Contains("IMPACTO") || n.Contains("A.I.")) return ("PAI", "VARIOS");

            return ("OTROS", "-");
        }
    }
}