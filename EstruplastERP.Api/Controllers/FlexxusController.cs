using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
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
        }

        [HttpPost("importar-mp")]
        public async Task<IActionResult> ImportarMaestro(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
                return BadRequest("Por favor, suba un archivo .csv válido.");

            int creados = 0;
            int actualizados = 0;
            var errores = new List<string>();

            try
            {
                // Configuración para leer CSV argentino (punto y coma) y encoding de Flexxus
                var config = new CsvConfiguration(new CultureInfo("es-AR"))
                {
                    Delimiter = ";",
                    HasHeaderRecord = true,
                    ShouldSkipRecord = args => args.Row.Parser.Row == 1, // Salta líneas vacías al inicio
                    MissingFieldFound = null, // No falla si faltan columnas opcionales
                    BadDataFound = null,
                    Encoding = Encoding.Latin1 // Importante para ñ y acentos
                };

                using (var stream = new StreamReader(archivo.OpenReadStream(), Encoding.Latin1))
                using (var csv = new CsvReader(stream, config))
                {
                    var registros = csv.GetRecords<FlexxusMaestroDto>().ToList();

                    // Traemos la DB a memoria para comparar rápido
                    var productosDb = await _context.Productos.ToListAsync();

                    foreach (var row in registros)
                    {
                        if (string.IsNullOrWhiteSpace(row.CodigoSku)) continue;

                        string skuLimpio = row.CodigoSku.Trim().ToUpper();
                        string nombreLimpio = row.Nombre?.Trim() ?? "SIN NOMBRE";
                        string rubroLimpio = row.Rubro?.Trim().ToUpper() ?? "OTROS";

                        // 1. Validaciones anti-basura del CSV
                        if (skuLimpio.Contains("/") || skuLimpio.Contains(":") || skuLimpio.Length < 3)
                        {
                            continue;
                        }

                        // 2. Lógica de Clasificación Automática (Materia Prima vs Producto)
                        bool esMateriaPrima = rubroLimpio.Contains("MATERIA PRIMA") ||
                                              rubroLimpio.Contains("MASTERBATCH") ||
                                              rubroLimpio.Contains("INSUMO");
                        bool esProductoTerminado = !esMateriaPrima;

                        // Buscamos si ya existe
                        var prod = productosDb.FirstOrDefault(p => p.CodigoSku.Trim().ToUpper() == skuLimpio);

                        if (prod != null)
                        {
                            // --- ACTUALIZAR PRODUCTO EXISTENTE ---
                            bool huboCambios = false;

                            if (prod.Nombre != nombreLimpio)
                            {
                                prod.Nombre = nombreLimpio;
                                huboCambios = true;
                            }

                            // Si cambia el rubro, actualizamos también los flags
                            if (prod.Rubro != rubroLimpio)
                            {
                                prod.Rubro = rubroLimpio;
                                prod.EsMateriaPrima = esMateriaPrima;
                                prod.EsProductoTerminado = esProductoTerminado;
                                huboCambios = true;
                            }

                            // Actualizar PRECIO solo si el CSV trae dato (no es null)
                            if (row.Precio.HasValue)
                            {
                                if (prod.PrecioCosto != row.Precio.Value)
                                {
                                    prod.PrecioCosto = row.Precio.Value;
                                    huboCambios = true;
                                }
                            }

                            // Actualizar STOCK solo si el CSV trae dato (no es null)
                            if (row.Stock.HasValue)
                            {
                                if (prod.StockActual != row.Stock.Value)
                                {
                                    prod.StockActual = row.Stock.Value;
                                    huboCambios = true;
                                }
                            }

                            if (huboCambios)
                            {
                                _context.Entry(prod).State = EntityState.Modified;
                                actualizados++;
                            }
                        }
                        else
                        {
                            // --- CREAR PRODUCTO NUEVO ---
                            var nuevo = new Producto
                            {
                                CodigoSku = skuLimpio,
                                Nombre = nombreLimpio,
                                Rubro = rubroLimpio,

                                // Flags automáticos
                                EsMateriaPrima = esMateriaPrima,
                                EsProductoTerminado = esProductoTerminado,

                                // Si es nuevo y no trae precio/stock, ponemos 0
                                PrecioCosto = row.Precio ?? 0,
                                StockActual = row.Stock ?? 0,

                                // Valores por defecto de tu negocio
                                EsGenerico = false,
                                EsFazon = false,
                                StockMinimo = 100,
                                Activo = true,
                                FechaCreacion = DateTime.Now,

                                // Peso específico por defecto (1.05 para MP, 1.00 para el resto)
                                PesoEspecifico = esMateriaPrima ? 1.05m : 1.00m
                            };

                            _context.Productos.Add(nuevo);
                            creados++;
                        }
                    }

                    // Guardamos cambios si hubo alguno
                    if (actualizados > 0 || creados > 0)
                    {
                        await _context.SaveChangesAsync();
                    }
                }

                return Ok(new
                {
                    mensaje = $"Importación Flexxus finalizada.\n🆕 Creados: {creados}\n🔄 Actualizados: {actualizados}",
                    detalles = new { Creados = creados, Actualizados = actualizados, Errores = errores }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error en FlexxusController: {ex.Message}");
            }
        }
    }
}