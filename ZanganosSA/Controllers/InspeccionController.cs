using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZanganosSA.Data;
using ZanganosSA.Models;
using System.Text.RegularExpressions;

namespace ZanganosSA.Controllers
{
    public class InspeccionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InspeccionController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetColmenas()
        {
            var colmenas = await _context.Colmenas
                .Select(c => new { c.Id, Identificador = c.Id.ToString(), EstadoGeneral = c.EstadoColmena, EstadoReina = c.ReinaEstado })
                .ToListAsync();
            return Json(colmenas);
        }

        [HttpPost]
        public async Task<IActionResult> ProcesarVoz([FromBody] VoiceInput model)
        {
            if (string.IsNullOrEmpty(model.Texto))
            {
                return Json(new { success = false, message = "No se recibió ningún texto." });
            }

            // Filtro de groserías y lenguaje inapropiado
            var palabrasInapropiadas = new[] { "mierda", "puto", "puta", "carajo", "boludo", "pelotudo", "concha", "cabron", "cabrón", "pendejo", "orto", "forro", "culiao", "hijo de puta", "hdp", "lpm" };
            foreach (var palabra in palabrasInapropiadas)
            {
                if (Regex.IsMatch(model.Texto, $@"\b{Regex.Escape(palabra)}\b", RegexOptions.IgnoreCase))
                {
                    return Json(new { success = false, message = "Error de validación: Se ha detectado lenguaje inapropiado o groserías en el dictado." });
                }
            }

            // Normalización de texto en español (convertir números escritos a dígitos)
            string textoNormalizado = model.Texto.ToLower()
                .Replace("uno", "1")
                .Replace("dos", "2")
                .Replace("tres", "3")
                .Replace("cuatro", "4")
                .Replace("cinco", "5")
                .Replace("seis", "6")
                .Replace("siete", "7")
                .Replace("ocho", "8")
                .Replace("nueve", "9")
                .Replace("diez", "10");

            int colmenaId = model.ColmenaId;
            
            // Validar si el dictado está relacionado con apicultura o estados de colmena
            bool esRelacionado = Regex.IsMatch(textoNormalizado, 
                @"(colmena|reina|caj.n|panal|cuadro|abeja|apiario|miel|inspecci.n|cr.tico|malo|regular|buen|sano|saludable|excelente|perfecto|.ptimo|muert|ausent|viej|d.bil|lent|joven|activ|estado|fuerte|media|debil|critica)", 
                RegexOptions.IgnoreCase);
                
            if (!esRelacionado)
            {
                return Json(new { success = false, message = "Error de validación: El dictado de voz no contiene términos o estados relacionados con la apicultura." });
            }
            
            // Si el texto contiene indicación de una colmena, intentamos extraer su identificador/número
            var match = Regex.Match(textoNormalizado, @"\b(?:colmena|número|numero|la|#)?\s*(\d+)\b", RegexOptions.IgnoreCase);
            if (match.Success && int.TryParse(match.Groups[1].Value, out int parsedId))
            {
                // Verificar si existe la colmena con este ID en la BD
                var exists = await _context.Colmenas.AnyAsync(c => c.Id == parsedId);
                if (exists)
                {
                    colmenaId = parsedId;
                }
            }

            var colmena = await _context.Colmenas.FirstOrDefaultAsync(c => c.Id == colmenaId);
            if (colmena == null)
            {
                return Json(new { success = false, message = $"No se encontró la colmena seleccionada o mencionada." });
            }

            // Análisis de palabras clave para Estado Colmena (usa los valores CHECK del SQL: Fuerte, Media, Debil, Critica)
            string estadoColmena = colmena.EstadoColmena ?? "Media";
            if (Regex.IsMatch(model.Texto, @"(crítico|critico|cr.tico|muy malo|pésimo|pesimo|p.simo|grave|fatal|malísimo|malisimo|horrible|terrible|destruida|rota|en mal estado|moribunda|critica)", RegexOptions.IgnoreCase))
                estadoColmena = "Critica";
            else if (Regex.IsMatch(model.Texto, @"(débil|debil|d.bil|malo|mala|flojo|floja|requiere refuerzo)", RegexOptions.IgnoreCase))
                estadoColmena = "Debil";
            else if (Regex.IsMatch(model.Texto, @"(regular|medio|media|así así|asi asi|más o menos|mas o menos|pasable|no tan bien|no esta bien|no está bien)", RegexOptions.IgnoreCase))
                estadoColmena = "Media";
            else if (Regex.IsMatch(model.Texto, @"(bueno|buena|bien|fuerte|sano|saludable|excelente|perfecto|óptimo|optimo|muy bien|en buen estado|fenomenal|bárbaro|barbaro|diez puntos|10 puntos|de diez)", RegexOptions.IgnoreCase))
                estadoColmena = "Fuerte";

            // Análisis de palabras clave para Estado de la Reina (usa los valores CHECK del SQL: Sana, Joven, Vieja, Ausente, Reemplazada)
            string reinaEstado = colmena.ReinaEstado ?? "Sana";
            if (Regex.IsMatch(model.Texto, @"(muerta|ausente|no está|no esta|sin reina|reina muerta|reina ausente|no hay reina|perdió la reina|perdio la reina|reina perdida|huerfana|huérfana)", RegexOptions.IgnoreCase))
                reinaEstado = "Ausente";
            else if (Regex.IsMatch(model.Texto, @"(vieja|débil|debil|lenta|reina vieja|reina débil|reina debil|poco activa|inactiva|reina lenta)", RegexOptions.IgnoreCase))
                reinaEstado = "Vieja";
            else if (Regex.IsMatch(model.Texto, @"(reemplazada|nueva reina|cambio de reina|reina nueva|sustituida)", RegexOptions.IgnoreCase))
                reinaEstado = "Reemplazada";
            else if (Regex.IsMatch(model.Texto, @"(joven|activa|buena reina|reina joven|reina activa)", RegexOptions.IgnoreCase))
                reinaEstado = "Joven";
            else if (Regex.IsMatch(model.Texto, @"(sana|reina sana|reina buena|reina excelente|reina en buen estado)", RegexOptions.IgnoreCase))
                reinaEstado = "Sana";

            // Actualizar colmena
            colmena.EstadoColmena = estadoColmena;
            colmena.ReinaEstado = reinaEstado;

            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                colmenaId = colmena.Id,
                identificador = colmena.Id.ToString(),
                estadoGeneral = colmena.EstadoColmena,
                estadoReina = colmena.ReinaEstado,
                mensaje = $"Colmena #{colmena.Id} actualizada: Estado '{estadoColmena}', Reina '{reinaEstado}'."
            });
        }

        [HttpPost]
        public IActionResult ProcesarVision([FromBody] VisionInput model)
        {
            // Validar si es una imagen no relacionada con apicultura (sólo rechazar el preset de demostración non_apiculture)
            if (model.Preset == "non_apiculture")
            {
                return Json(new { success = false, message = "Error de análisis: La imagen seleccionada no corresponde a un panal, colmena o elemento relacionado con la apicultura." });
            }

            if (!string.IsNullOrEmpty(model.FileName))
            {
                string fn = model.FileName.ToLower();
                bool esApicultura = fn.Contains("panal") || fn.Contains("colmena") || fn.Contains("abeja") || 
                                    fn.Contains("miel") || fn.Contains("apiario") || fn.Contains("inspeccion") || 
                                    fn.Contains("hive") || fn.Contains("bee") || fn.Contains("honey") || 
                                    fn.Contains("comb") || fn.Contains("apicultura") || fn.Contains("beehive");
                
                if (!esApicultura)
                {
                    return Json(new { success = false, message = $"Error de análisis: La imagen subida ({model.FileName}) no corresponde a un panal, colmena o elemento relacionado con la apicultura." });
                }
            }

            // Simular análisis visual inteligente dependiendo de la imagen cargada/seleccionada
            string tipoMaterial = "Cajón de Colmena";
            string condicion = "Buen Estado";
            double nivelConfianza = 0.94;
            var boundingBoxes = new List<object>();

            if (model.Preset == "beehive_inspection")
            {
                tipoMaterial = "Cajón de Colmena (Madera)";
                condicion = "Desgaste moderado y roturas menores detectadas. Se aconseja mantenimiento preventivo.";
                nivelConfianza = 0.89;
                boundingBoxes.Add(new { label = "Madera dañada", x = 120, y = 180, w = 220, h = 150, confidence = 0.89 });
                boundingBoxes.Add(new { label = "Grieta en base", x = 320, y = 300, w = 180, h = 80, confidence = 0.82 });
            }
            else if (model.Preset == "healthy_honeycomb")
            {
                tipoMaterial = "Panal / Cuadro de Cera";
                condicion = "Excelente densidad de población. Panal 100% saludable sin signos de varroosis.";
                nivelConfianza = 0.97;
                boundingBoxes.Add(new { label = "Abejas Saludables", x = 80, y = 50, w = 400, h = 320, confidence = 0.97 });
                boundingBoxes.Add(new { label = "Celdas de Miel Llenas", x = 150, y = 100, w = 180, h = 150, confidence = 0.92 });
            }
            else if (!string.IsNullOrEmpty(model.DetectedLabel))
            {
                // Detección real por IA en el cliente (TensorFlow.js)
                string label = model.DetectedLabel.ToLower();
                double confidence = model.DetectedConfidence ?? 0.85;

                if (label.Contains("honeycomb") || label.Contains("bee") || label.Contains("wasp") || label.Contains("nest"))
                {
                    tipoMaterial = "Panal / Cuadro de Cera (Detección Real)";
                    condicion = $"Panal detectado con una precisión de {(confidence * 100):0}%. La estructura de celdas se observa en buenas condiciones generales.";
                    nivelConfianza = confidence;
                    boundingBoxes.Add(new { label = "Panal Saludable", x = 100, y = 80, w = 350, h = 280, confidence = confidence });
                }
                else
                {
                    // Ej: beehive, crate, wood, chest, etc.
                    tipoMaterial = $"Cajón de Colmena / Estructura ({model.DetectedLabel}) (Detección Real)";
                    condicion = $"Cajón de colmena detectado con una precisión de {(confidence * 100):0}%. La estructura exterior parece sólida y estable.";
                    nivelConfianza = confidence;
                    boundingBoxes.Add(new { label = "Estructura Madera", x = 120, y = 100, w = 300, h = 250, confidence = confidence });
                }
            }
            else
            {
                // Caso genérico o capturado por cámara sin etiqueta
                tipoMaterial = "Cajón e Instrumentos";
                condicion = "Análisis completado: Estructura estable. No se detectan anomalías críticas.";
                nivelConfianza = 0.85;
                boundingBoxes.Add(new { label = "Objeto Detectado", x = 100, y = 100, w = 300, h = 250, confidence = 0.85 });
            }

            return Json(new
            {
                success = true,
                tipoMaterial = tipoMaterial,
                condicion = condicion,
                nivelConfianza = nivelConfianza,
                boundingBoxes = boundingBoxes
            });
        }
    }

    public class VoiceInput
    {
        public int ColmenaId { get; set; }
        public string Texto { get; set; } = string.Empty;
    }

    public class VisionInput
    {
        public string Preset { get; set; } = string.Empty;
        public string Base64Image { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string? DetectedLabel { get; set; }
        public double? DetectedConfidence { get; set; }
    }
}
