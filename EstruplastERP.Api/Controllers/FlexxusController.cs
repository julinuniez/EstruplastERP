using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using OfficeOpenXml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;
using EstruplastERP.Data;
using EstruplastERP.Core;
using EstruplastERP.Api.Dtos;

namespace EstruplastERP.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FlexxusController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FlexxusController(ApplicationDbContext context)
        {
            _context = context;
            ExcelPackage.License.SetNonCommercialPersonal("FreelanceDev");
        }

        [HttpPost("importar-mp")]
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

                    for (int i = 1; i < lineas.Length; i++)
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

                        string tipoDetectado = esMP ? "MATERIA PRIMA" : "OTRO";

                        var prod = productosDb.FirstOrDefault(p =>
                            p.CodigoSku != null &&
                            new string(p.CodigoSku.Where(c => char.IsLetterOrDigit(c)).ToArray()).ToUpper() == skuLimpio
                        );

                        if (prod != null)
                        {
                            bool huboCambios = false;

                            if (prod.Nombre != nombre)
                            {
                                prod.Nombre = nombre;
                                huboCambios = true;
                            }

                            if (prod.Rubro != rubro)
                            {
                                prod.Rubro = rubro;
                                prod.EsMateriaPrima = esMP;
                                prod.EsProductoTerminado = !esMP;
                                huboCambios = true;
                            }

                            if (tipoDetectado != "OTROS" && tipoDetectado != "OTRO" && prod.TipoMaterial != tipoDetectado)
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
                                PesoEspecifico = esMP ? 1.05m : 1.00m
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

                            int colCodigo = 1;
                            int colDesc = 2;
                            int colStockReal = 6;

                            int fila = 2;
                            int filasVaciasConsecutivas = 0;

                            while (fila < 5000 && filasVaciasConsecutivas < 10)
                            {
                                string codigo = worksheet.Cells[fila, colCodigo].Text.Trim();
                                string desc = worksheet.Cells[fila, colDesc].Text.Trim();

                                if (string.IsNullOrWhiteSpace(codigo) || codigo == "-")
                                {
                                    filasVaciasConsecutivas++;
                                    fila++;
                                    continue;
                                }

                                if (codigo.ToUpper() == "TOTAL" || codigo.ToUpper().Contains("TOTAL GENERAL"))
                                {
                                    break;
                                }

                                filasVaciasConsecutivas = 0;

                                decimal stockFinal = LeerNumeroRobusto(worksheet.Cells[fila, colStockReal].Value, out bool stockEncontrado);

                                if (stockFinal <= 0)
                                {
                                    fila++;
                                    continue;
                                }

                                bool procesado = await ProcesarProductoCliente(codigo, desc, stockFinal, cliente.Id, productosDb, esModoScrap, nombreArchivo);
                                if (procesado) prods++;

                                fila++;
                            }
                        }

                        await _context.SaveChangesAsync();
                    }
                }
                return Ok(new { mensaje = $"✅ Importación: {hojas} hojas procesadas, {prods} productos con stock actualizados.", logs = logs });
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
            codigo = codigo.Replace("[MOLIDO]", "").Replace("[MP]", "").Trim();
            string descripcionExcel = nombre.Replace("[MOLIDO]", "").Replace("[MP]", "").Trim().ToUpper();

            if (string.IsNullOrWhiteSpace(descripcionExcel)) descripcionExcel = "SIN DESCRIPCION";

            string prefijo = esModoScrap ? "MOL" : "MP";

            string codigoLimpio = new string(codigo.Where(char.IsLetterOrDigit).ToArray()).ToUpper();
            if (string.IsNullOrEmpty(codigoLimpio)) codigoLimpio = "SC";
            if (codigoLimpio.Length > 8) codigoLimpio = codigoLimpio.Substring(codigoLimpio.Length - 8);

            string skuSistema = $"{prefijo}-{codigoLimpio}-C{clienteId}".ToUpper();
            string nombreFinal = esModoScrap ? $"[MOLIDO] {descripcionExcel} ({codigo})" : $"[MP] {descripcionExcel} ({codigo})";

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

                decimal diferencia = stock - prod.StockActual;

                if (diferencia != 0 && prod.Id > 0)
                {
                    var movimiento = new Movimiento
                    {
                        ProductoId = prod.Id,
                        Fecha = DateTime.Now,
                        Cantidad = Math.Abs(diferencia),
                        TipoMovimiento = diferencia > 0 ? "Ajuste Ingreso" : "Ajuste Egreso",
                        Observacion = $"Importación Excel. Anterior: {prod.StockActual} Kg | Nuevo: {stock} Kg",
                        ClienteId = clienteId
                    };

                    _context.Set<Movimiento>().Add(movimiento);
                }

                prod.StockActual = stock;
                prod.Nombre = nombreFinal;
                prod.TipoMaterial = descripcionExcel;
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
                    TipoMaterial = descripcionExcel,
                    ClienteId = clienteId,
                    EsScrap = esModoScrap,
                    EsMateriaPrima = true,
                    EsProductoTerminado = false,
                    StockActual = stock,
                    Activo = true,
                    FechaCreacion = DateTime.Now,
                    PesoEspecifico = 1.1m
                };

                _context.Productos.Add(nuevo);
                productosDb.Add(nuevo);

                var movimientoInicial = new Movimiento
                {
                    Producto = nuevo,
                    Fecha = DateTime.Now,
                    Cantidad = stock,
                    TipoMovimiento = "Ingreso Inicial",
                    Observacion = "Importación Excel (Alta de producto nuevo).",
                    ClienteId = clienteId
                };

                _context.Set<Movimiento>().Add(movimientoInicial);

                return true;
            }
        }
    }
}