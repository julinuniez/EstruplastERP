using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Text; // Para Encoding
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
        }

        [HttpPost("importar-maestro")]
        public async Task<IActionResult> ImportarMaestro(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Por favor, suba un archivo .csv válido.");

            // 1. CORRECCIÓN: Usamos variables normales (mutables)
            int creados = 0;
            int actualizados = 0;
            var errores = new List<string>(); // Las listas sí se pueden modificar

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
                        string nombreLimpio = row.Nombre.Trim();

                        if (skuLimpio.Contains("/") || skuLimpio.Contains(":") || skuLimpio.Length < 3 || string.IsNullOrEmpty(nombreLimpio))
                        {
                            continue; 
                        }

                        var prod = productosDb.FirstOrDefault(p => p.CodigoSku.Trim().ToUpper() == skuLimpio);

                        if (prod != null)
                        {
                            if (prod.Nombre != nombreLimpio)
                            {
                                prod.Nombre = nombreLimpio;
                                _context.Entry(prod).State = EntityState.Modified;
                                // 2. CORRECCIÓN: Sumamos a la variable local
                                actualizados++;
                            }
                        }
                        else
                        {
                            var nuevo = new Producto
                            {
                                CodigoSku = skuLimpio,
                                Nombre = nombreLimpio,
                                PrecioCosto = 0,
                                EsMateriaPrima = true,
                                EsProductoTerminado = false,
                                EsGenerico = false,
                                EsFazon = false,
                                StockActual = 0,
                                StockMinimo = 100,
                                Activo = true,
                                FechaCreacion = DateTime.Now
                            };

                            _context.Productos.Add(nuevo);
                            // 2. CORRECCIÓN: Sumamos a la variable local
                            creados++;
                        }
                    }

                    if (actualizados > 0 || creados > 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }

                // 3. CORRECCIÓN: Creamos el objeto anónimo AL FINAL
                return Ok(new
                {
                    mensaje = $"Proceso terminado.\n🆕 Creados: {creados}\n🔄 Actualizados: {actualizados}\n(Los precios quedaron en $0 porque el archivo no los incluye).",
                    detalles = new { Creados = creados, Actualizados = actualizados, Errores = errores }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error procesando archivo: {ex.Message}");
            }
        }
    }
}