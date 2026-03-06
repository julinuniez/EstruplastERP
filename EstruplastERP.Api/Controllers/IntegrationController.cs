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
            // 1. LIMPIEZA EXTREMA (Evitar efecto bola de nieve con los corchetes)
            codigo = codigo.Replace("[MOLIDO]", "").Replace("[MP]", "").Trim();
            nombre = nombre.Replace("[MOLIDO]", "").Replace("[MP]", "").Trim();

            // 2. DETECCIÓN INTELIGENTE
            var (tipo, color) = DetectarMaterialYColor(codigo, nombre, nombreArchivoContexto);

            // 3. GENERADOR DE SKU CORTO (Ej: MOL-PEAD-NAT-003-C2)
            string prefijo = esModoScrap ? "MOL" : "MP";

            // Extraemos solo los números del código original para que sea corto (si no hay, letras cortas)
            string numOriginal = new string(codigo.Where(char.IsDigit).ToArray());
            if (string.IsNullOrEmpty(numOriginal)) numOriginal = new string(codigo.Where(char.IsLetterOrDigit).ToArray());
            if (numOriginal.Length > 5) numOriginal = numOriginal.Substring(numOriginal.Length - 5); // Máximo 5 caracteres
            if (string.IsNullOrEmpty(numOriginal)) numOriginal = "SC";

            // Acortamos el color (NATURAL -> NAT, BLANCO -> BLA)
            string colorCorto = color.Length >= 3 && color != "VARIOS" && color != "TUTI" ? color.Substring(0, 3) : color;

            string skuSistema = $"{prefijo}-{tipo}-{colorCorto}-{numOriginal}-C{clienteId}".ToUpper().Replace(" ", "");

            // 4. GENERADOR DE NOMBRE LIMPIO
            string nombreFinal = esModoScrap ? $"[MOLIDO] {tipo} - {color} ({codigo})" : $"[MP] {tipo} - {color} ({codigo})";

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
                prod.Nombre = nombreFinal;
                prod.TipoMaterial = tipo;
                prod.Color = color;
                prod.EsScrap = esModoScrap;
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
                    Nombre = nombreFinal,
                    Rubro = esModoScrap ? "MOLIDO CLIENTE" : "MATERIA PRIMA CLIENTE",
                    TipoMaterial = tipo,
                    Color = color,
                    ClienteId = clienteId,
                    EsScrap = esModoScrap   ,
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
            string combo = $"{sku} {nombre} {contextoArchivo}".ToUpper();

            // 1. FILTRO ANTI-ENSALADA: Buscamos explícitamente el material primero
            string material = "OTROS";

            if (combo.Contains("ABS")) material = "ABS";
            else if (combo.Contains("PEAD") || combo.Contains("ALTA") || combo.Contains("HDPE")) material = "PEAD";
            else if (combo.Contains("PEBD") || combo.Contains("BAJA") || combo.Contains("POLIETILENO")) material = "PEBD";
            else if (combo.Contains("PP") || combo.Contains("POLIPROPILENO")) material = "PP";
            else if (combo.Contains("PAI") || combo.Contains("IMPACTO")) material = "PAI";
            else if (combo.Contains("BIO")) material = "BIO";
            else if (combo.Contains("FREON") || combo.Contains("RES. FREON") || sku?.StartsWith("RF") == true) material = "RESISTENTE FREON";

            // 2. DETECCIÓN DE COLOR INDEPENDIENTE
            string color = "-";

            if (combo.Contains("000") || combo.Contains("TUTI") || combo.Contains("TUTTI")) color = "TUTI";
            else if (combo.Contains("001") || combo.Contains("NATURAL") || (combo.Contains("003") && material == "BIO")) color = "NATURAL";
            else if (combo.Contains("002") || combo.Contains("BLANCO")) color = "BLANCO";
            else if (combo.Contains("003") && material == "PEAD") color = "NATURAL"; // Si es 003 pero decía PEAD explícitamente, es Natural, no Bio.
            else if (combo.Contains("004") || combo.Contains("AMARILLO")) color = "AMARILLO";
            else if (combo.Contains("005") || combo.Contains("NARANJA")) color = "NARANJA";
            else if (combo.Contains("006") || combo.Contains("ROSA")) color = "ROSA";
            else if (combo.Contains("007") || combo.Contains("ROJO")) color = "ROJO";
            else if (combo.Contains("008") || combo.Contains("VIOLETA")) color = "VIOLETA";
            else if (combo.Contains("009") || combo.Contains("CELESTE")) color = "CELESTE";
            else if (combo.Contains("010") || combo.Contains("AZUL")) color = "AZUL";
            else if (combo.Contains("011") || combo.Contains("VERDE")) color = "VERDE";
            else if (combo.Contains("012") || combo.Contains("MARRON")) color = "MARRON";
            else if (combo.Contains("013") || combo.Contains("GRIS")) color = "GRIS";
            else if (combo.Contains("014") || combo.Contains("PLATA")) color = "GRIS PLATA";
            else if (combo.Contains("015") || combo.Contains("NEGRO")) color = "NEGRO";

            // 3. REGLAS DE RESCATE (Fallback)
            if (material == "OTROS" && color != "-") material = "PAI"; // Si tiene color de la lista pero no dice material, históricamente usás PAI.
            if (material != "OTROS" && color == "-") color = "VARIOS"; // Si detectó material pero no color.
            if (material == "BIO" && color == "VARIOS") color = "NATURAL"; // BIO suele ser natural.

            return (material, color);
        }
    }
}