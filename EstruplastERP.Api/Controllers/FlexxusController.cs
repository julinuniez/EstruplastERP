using EstruplastERP.Core;
using EstruplastERP.Data;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
        }

        [HttpPost("importar-mp")]
        public async Task<IActionResult> ImportarMateriaPrima(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0) return BadRequest("Sube un archivo válido.");

            // Habilitar lectura de encodings viejos (Windows-1252 para ñ y acentos)
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            int actualizados = 0;
            int creados = 0;
            int errores = 0;

            try
            {
                // Carga rápida en memoria
                var productosDb = await _context.Productos
                    .ToDictionaryAsync(p => p.CodigoSku.ToUpper().Trim(), p => p);

                using (var stream = archivo.OpenReadStream())
                {
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                        {
                            ConfigureDataTable = (_) => new ExcelDataTableConfiguration() { UseHeaderRow = false }
                        });

                        var tabla = result.Tables[0];

                        // ==========================================
                        // ⚙️ CONFIGURACIÓN DE COLUMNAS
                        // ==========================================
                        int colSku = 0;     // Col A
                        int colNombre = 1;  // Col B
                        int colRubro = 4;   // Col E (Donde dice MASTERBATCH, etc.)

                        // Recorremos desde la fila 2 para saltar encabezados sucios
                        for (int i = 2; i < tabla.Rows.Count; i++)
                        {
                            DataRow row = tabla.Rows[i];
                            try
                            {
                                var skuRaw = row[colSku]?.ToString();
                                if (string.IsNullOrWhiteSpace(skuRaw)) continue;

                                var sku = skuRaw.ToUpper().Trim();

                                // Nombre
                                var nombreRaw = row[colNombre]?.ToString();
                                string nombre = string.IsNullOrWhiteSpace(nombreRaw) ? $"Producto {sku}" : nombreRaw.Trim();

                                // --- LÓGICA DE RUBRO 🧠 ---
                                string rubro = row[colRubro]?.ToString()?.ToUpper().Trim() ?? "";

                                // Por defecto asumimos que es Materia Prima si no dice lo contrario
                                bool esMP = true;
                                bool esPT = false;

                                if (rubro.Contains("TERMINADO") || rubro.Contains("PT"))
                                {
                                    esMP = false;
                                    esPT = true;
                                }
                                else if (rubro.Contains("MASTER") || rubro.Contains("MATERIA") || rubro.Contains("VIRGEN") || rubro.Contains("INSUMO"))
                                {
                                    esMP = true;
                                    esPT = false;
                                }

                                if (productosDb.TryGetValue(sku, out var productoExistente))
                                {
                                    // --- ACTUALIZAR ---
                                    bool cambio = false;

                                    if (productoExistente.Nombre != nombre)
                                    {
                                        productoExistente.Nombre = nombre;
                                        cambio = true;
                                    }

                                    // Actualizamos también la categoría si cambió en Flexxus
                                    if (productoExistente.EsMateriaPrima != esMP)
                                    {
                                        productoExistente.EsMateriaPrima = esMP;
                                        productoExistente.EsProductoTerminado = esPT;
                                        cambio = true;
                                    }

                                    if (cambio) actualizados++;
                                }
                                else
                                {
                                    // --- CREAR NUEVO ---
                                    var nuevoProd = new Producto
                                    {
                                        CodigoSku = sku,
                                        Nombre = nombre,
                                        StockActual = 0,
                                        PrecioCosto = 0,

                                        // Aquí asignamos lo que detectamos del Rubro
                                        EsMateriaPrima = esMP,
                                        EsProductoTerminado = esPT,

                                        FechaCreacion = DateTime.Now,
                                        StockMinimo = 10,
                                        Color = "A definir" // O podrías leerlo de otra columna si existe
                                    };

                                    _context.Productos.Add(nuevoProd);
                                    productosDb.Add(sku, nuevoProd);
                                    creados++;
                                }
                            }
                            catch
                            {
                                errores++;
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    mensaje = "Sincronización finalizada",
                    creados = creados,
                    actualizados = actualizados,
                    errores = errores
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error procesando archivo: " + ex.Message);
            }
        }
    }
}