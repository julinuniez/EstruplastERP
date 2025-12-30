using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
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

            int creados = 0;
            int actualizados = 0;
            var errores = new List<string>();

            try
            {
                var config = new CsvConfiguration(new CultureInfo("es-AR"))
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    ShouldSkipRecord = args => args.Row.Parser.Row == 1, // Saltar líneas vacías si las hay al principio
                    MissingFieldFound = null,
                    BadDataFound = null,
                    Encoding = Encoding.Latin1 // Importante para acentos de Flexxus
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
                        string rubroLimpio = row.Rubro?.Trim().ToUpper() ?? "OTROS"; // 🔥 Leemos el Rubro

                        // Validaciones básicas de basura en el CSV
                        if (skuLimpio.Contains("/") || skuLimpio.Contains(":") || skuLimpio.Length < 3)
                        {
                            continue;
                        }

                        // 🔥 LÓGICA DE CLASIFICACIÓN AUTOMÁTICA
                        // Si el rubro contiene "MATERIA PRIMA", lo marcamos como tal.
                        bool esMateriaPrima = rubroLimpio.Contains("MATERIA PRIMA") ||
                      rubroLimpio.Contains("MASTERBATCH") ||
                      rubroLimpio.Contains("INSUMO");
                        bool esProductoTerminado = !esMateriaPrima; // Lo contrario (por defecto)

                        var prod = productosDb.FirstOrDefault(p => p.CodigoSku.Trim().ToUpper() == skuLimpio);

                        if (prod != null)
                        {
                            // --- ACTUALIZAR EXISTENTE ---
                            bool huboCambios = false;

                            if (prod.Nombre != nombreLimpio)
                            {
                                prod.Nombre = nombreLimpio;
                                huboCambios = true;
                            }

                            // Actualizamos el Rubro si cambió
                            if (prod.Rubro != rubroLimpio)
                            {
                                prod.Rubro = rubroLimpio;
                                // También actualizamos los flags para mantener consistencia
                                prod.EsMateriaPrima = esMateriaPrima;
                                prod.EsProductoTerminado = esProductoTerminado;
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
                            // --- CREAR NUEVO ---
                            var nuevo = new Producto
                            {
                                CodigoSku = skuLimpio,
                                Nombre = nombreLimpio,
                                Rubro = rubroLimpio, // 🔥 Guardamos el Rubro

                                // Configuramos los flags según lo que leímos del Rubro
                                EsMateriaPrima = esMateriaPrima,
                                EsProductoTerminado = esProductoTerminado,

                                // Valores por defecto
                                PrecioCosto = 0,
                                EsGenerico = false,
                                EsFazon = false,
                                StockActual = 0,
                                StockMinimo = 100,
                                Activo = true,
                                FechaCreacion = DateTime.Now,

                                // Asignamos peso específico por defecto según tipo
                                PesoEspecifico = esMateriaPrima ? 1.05m : 1.00m
                            };

                            _context.Productos.Add(nuevo);
                            creados++;
                        }
                    }

                    if (actualizados > 0 || creados > 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new
                {
                    mensaje = $"Proceso terminado.\n🆕 Creados: {creados}\n🔄 Actualizados: {actualizados}\n(Rubros y tipos actualizados correctamente).",
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