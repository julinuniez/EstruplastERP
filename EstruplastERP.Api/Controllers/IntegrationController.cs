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

        [HttpPost("importar-maestro")]
        public async Task<IActionResult> ImportarMaestro(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0) return BadRequest("Suba un archivo .csv válido.");

            int creados = 0, actualizados = 0;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                using (var stream = new StreamReader(archivo.OpenReadStream(), Encoding.Latin1))
                {
                    var contenido = await stream.ReadToEndAsync();
                    var lineas = contenido.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    var productosDb = await _context.Productos.ToListAsync();

                    for (int i = 2; i < lineas.Length; i++)
                    {
                        var columnas = lineas[i].Split(';');

                        if (columnas.Length < 5) continue;

                        string skuCrudo = columnas[0];
                        string nombreCrudo = columnas[1];
                        string rubroCrudo = columnas[4];

                        string skuLimpio = new string(skuCrudo.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToUpper();
                        string nombre = nombreCrudo.Replace("\"", "").Trim();
                        string rubro = rubroCrudo.Replace("\"", "").Trim().ToUpper();

                        if (skuLimpio.Length < 3) continue;
                        if (string.IsNullOrWhiteSpace(nombre)) nombre = "SIN NOMBRE";

                        bool esMP = rubro.Contains("MATERIA PRIMA") || rubro.Contains("MASTERBATCH") || rubro.Contains("INSUMO");
                        var (tipoDetectado, _) = DetectarMaterialYColor(skuLimpio, nombre, null);

                        var prodsCoincidentes = productosDb.Where(p =>
                            p.CodigoSku != null &&
                            new string(p.CodigoSku.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToUpper() == skuLimpio
                        ).ToList();

                        if (prodsCoincidentes.Any())
                        {
                            foreach (var prod in prodsCoincidentes)
                            {
                                prod.Nombre = nombre;

                                if (esMP)
                                {
                                    prod.EsMateriaPrima = true;
                                    prod.EsProductoTerminado = false;
                                }
                                if (tipoDetectado != "OTROS")
                                {
                                    prod.TipoMaterial = tipoDetectado;
                                }

                                _context.Productos.Update(prod);
                                actualizados++;
                            }
                        }
                        else
                        {
                            var nuevoProd = new Producto
                            {
                                CodigoSku = skuCrudo,
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
                            };

                            _context.Productos.Add(nuevoProd);
                            productosDb.Add(nuevoProd);
                            creados++;
                        }
                    }
                }

                if (actualizados > 0 || creados > 0)
                {
                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                }

                return Ok(new { mensaje = $"✅ Flexxus procesado: {creados} creados, {actualizados} actualizados." });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return StatusCode(500, $"Error crítico procesando CSV: {ex.Message}");
            }
        }

        [HttpPost("importar-excel-multicliente")]
        public async Task<IActionResult> ImportarExcelMultiCliente(IFormFile archivo, [FromForm] int? clienteIdFiltro = null)
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
                        try { package.Workbook.Calculate(); } catch { }

                        var clientesDb = await _context.Clientes.ToListAsync();
                        var productosDb = await _context.Productos.ToListAsync();

                        foreach (var worksheet in package.Workbook.Worksheets)
                        {
                            string nombreHoja = worksheet.Name.Trim().ToUpper();

                            var cliente = clientesDb.FirstOrDefault(c =>
                                c.RazonSocial.ToUpper().Replace(".", "").Trim() == nombreHoja.Replace(".", "").Trim() ||
                                c.RazonSocial.ToUpper().Contains(nombreHoja) ||
                                nombreHoja.Contains(c.RazonSocial.ToUpper()));

                            if (cliente == null) continue;

                            if (clienteIdFiltro.HasValue && cliente.Id != clienteIdFiltro.Value)
                            {
                                logs.Add($"⏭️ Hoja '{worksheet.Name}' ignorada por filtro.");
                                continue;
                            }

                            hojas++;

                            int filaInicio = -1, colCodigo = -1, colDesc = -1;
                            int colTotal = -1, colExistencias = -1, colSaldo = -1, colStockActual = -1;

                            for (int r = 1; r <= 20; r++)
                            {
                                for (int c = 1; c <= 20; c++)
                                {
                                    var celda = worksheet.Cells[r, c].Text.ToUpper().Trim().Replace(" ", "").Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U");

                                    if (celda == "CODIGO" || celda == "ARTICULO" || celda == "SKU" || celda == "ITEM") { filaInicio = r; colCodigo = c; }
                                    if (celda == "DESCRIPCION" || celda == "DETALLE" || celda == "MATERIAL" || celda == "PRODUCTO" || celda == "NOMBRE") colDesc = c;

                                    if (celda == "TOTAL" || celda.Contains("TOTAL")) colTotal = c;
                                    else if (celda == "EXISTENCIAS" || celda == "EXISTENCIA" || celda.Contains("EXISTENCIA")) colExistencias = c;
                                    else if (celda == "SALDO" || celda.Contains("SALDO")) colSaldo = c;
                                    else if (celda == "STOCKACTUAL" || celda == "STOCK" || celda == "CANTIDAD" || celda == "KILOS" || celda == "KG" || celda == "PESO") colStockActual = c;
                                }
                                if (filaInicio != -1 && colCodigo != -1 && (colTotal != -1 || colExistencias != -1 || colSaldo != -1 || colStockActual != -1)) break;
                            }

                            if (filaInicio == -1 || colCodigo == -1)
                            {
                                logs.Add($"❌ Hoja '{worksheet.Name}': No se detectaron columnas válidas.");
                                continue;
                            }

                            int fila = filaInicio + 1;
                            int filasVaciasConsecutivas = 0;

                            while (fila < 5000 && filasVaciasConsecutivas < 20)
                            {
                                string codigo = worksheet.Cells[fila, colCodigo].Text.Trim();
                                string desc = colDesc != -1 ? worksheet.Cells[fila, colDesc].Text.Trim() : "";

                                if (string.IsNullOrEmpty(codigo))
                                {
                                    filasVaciasConsecutivas++;
                                    fila++;
                                    continue;
                                }
                                filasVaciasConsecutivas = 0;

                                decimal stockFinal = 0;
                                bool stockEncontrado = false;

                                if (colStockActual != -1) stockFinal = LeerNumeroRobusto(worksheet.Cells[fila, colStockActual].Value, out stockEncontrado);

                                if (stockFinal == 0 && colTotal != -1)
                                {
                                    decimal s1 = LeerNumeroRobusto(worksheet.Cells[fila, colTotal].Value, out bool encontro1);
                                    if (s1 != 0 || encontro1) { stockFinal = s1; stockEncontrado = true; }
                                }

                                if (stockFinal == 0 && colExistencias != -1)
                                {
                                    decimal s2 = LeerNumeroRobusto(worksheet.Cells[fila, colExistencias].Value, out bool encontro2);
                                    if (s2 != 0 || encontro2) { stockFinal = s2; stockEncontrado = true; }
                                }

                                if (stockFinal == 0 && colSaldo != -1)
                                {
                                    decimal s3 = LeerNumeroRobusto(worksheet.Cells[fila, colSaldo].Value, out bool encontro3);
                                    if (s3 != 0 || encontro3) { stockFinal = s3; stockEncontrado = true; }
                                }

                                bool procesado = await ProcesarProductoCliente(codigo, desc, stockFinal, cliente.Id, productosDb, esModoScrap, nombreArchivo);
                                if (procesado) prods++;

                                fila++;
                            }
                        }
                        await _context.SaveChangesAsync();
                    }
                }
                return Ok(new { mensaje = $"✅ Importación: {hojas} hojas procesadas, {prods} productos registrados/actualizados.", logs = logs });
            }
            catch (Exception ex) { return StatusCode(500, $"Error crítico: {ex.Message}"); }
        }

        private decimal LeerNumeroRobusto(object valorCelda, out bool exito)
        {
            exito = false;
            if (valorCelda == null) return 0;

            if (valorCelda is double d) { exito = true; return (decimal)d; }
            if (valorCelda is decimal dec) { exito = true; return dec; }
            if (valorCelda is int i) { exito = true; return i; }

            string texto = valorCelda.ToString().Trim();
            if (string.IsNullOrEmpty(texto)) return 0;

            if (decimal.TryParse(texto, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal res1))
            {
                exito = true;
                return res1;
            }

            if (decimal.TryParse(texto, NumberStyles.Any, new CultureInfo("es-AR"), out decimal res2))
            {
                exito = true;
                return res2;
            }

            return 0;
        }

        private async Task<bool> ProcesarProductoCliente(string codigo, string nombre, decimal stock, int clienteId, List<Producto> productosDb, bool esModoScrap, string nombreArchivoContexto)
        {
            var (tipo, color) = DetectarMaterialYColor(codigo, nombre, nombreArchivoContexto);

            string prefijo = esModoScrap ? "MOLIDO" : "MP";
            string skuOriginal = new string(codigo.Where(c => char.IsLetterOrDigit(c)).ToArray());
            if (string.IsNullOrEmpty(skuOriginal)) skuOriginal = "SC";

            string skuSistema = $"{prefijo}-{tipo}-{color}-{skuOriginal}-CLI-{clienteId}".ToUpper().Replace(" ", "");

            var prod = productosDb.FirstOrDefault(p => p.CodigoSku == skuSistema && p.ClienteId == clienteId);

            if (prod != null)
            {
                if (stock <= 0)
                {
                    prod.StockActual = 0;
                    prod.Activo = false;
                    if (prod.Id > 0) _context.Entry(prod).State = EntityState.Modified;
                    return false;
                }

                prod.StockActual = stock;
                prod.Nombre = esModoScrap ? $"[MOLIDO] {tipo} - {color} ({codigo})" : $"[MP] {tipo} - {color} ({codigo})";
                prod.TipoMaterial = tipo;
                prod.Color = color;
                prod.EsScrap = false;
                prod.EsMateriaPrima = true;
                prod.Activo = true;
                if (prod.Id > 0) _context.Entry(prod).State = EntityState.Modified;
                return true;
            }
            else
            {
                if (stock <= 0) return false;

                var nuevo = new Producto
                {
                    CodigoSku = skuSistema,
                    Nombre = esModoScrap ? $"[MOLIDO] {tipo} - {color} ({codigo})" : $"[MP] {tipo} - {color} ({codigo})",
                    Rubro = esModoScrap ? "MOLIDO CLIENTE" : "MATERIA PRIMA CLIENTE",
                    TipoMaterial = tipo,
                    Color = color,
                    ClienteId = clienteId,
                    EsScrap = false,
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    StockActual = stock,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    PesoEspecifico = 1
                };
                _context.Productos.Add(nuevo);
                productosDb.Add(nuevo);
                return true;
            }
        }

        private (string Tipo, string Color) DetectarMaterialYColor(string sku, string nombre, string? contextoArchivo)
        {
            string s = sku?.ToUpper() ?? "";
            string n = nombre?.ToUpper() ?? "";
            string archivo = contextoArchivo?.ToUpper() ?? "";

            if (s.Contains("003") || n.Contains("BIO") || archivo.Contains("BIO")) return ("BIO", "NATURAL");

            bool esFreon = archivo.Contains("FREON") || archivo.Contains("RES. FREON") || n.Contains("FREON") || s.StartsWith("RF");

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

            if (archivo.Contains("ABS") || n.Contains("ABS")) return ("ABS", "-");
            if (archivo.Contains("PP") || archivo.Contains("POLIPROPILENO") || n.Contains("PP") || n.Contains("POLIPROPILENO")) return ("PP", "-");
            if (archivo.Contains("PEAD") || archivo.Contains("ALTA") || archivo.Contains("HDPE") || n.Contains("PEAD") || n.Contains("ALTA") || n.Contains("HDPE")) return ("PEAD", "-");
            if (archivo.Contains("PEBD") || archivo.Contains("BAJA") || archivo.Contains("POLIETILENO") || n.Contains("PEBD") || n.Contains("BAJA") || n.Contains("POLIETILENO")) return ("POLIETILENO", "-");
            if (archivo.Contains("PAI") || n.Contains("PAI") || n.Contains("IMPACTO")) return ("PAI", "VARIOS");

            return ("OTROS", "VARIOS");
        }
    }
}