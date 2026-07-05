namespace ZanganosSA.Services
{
    // Simulador de Servicios de IA para el Proyecto Integrador
    public class IAAsistenteService
    {
        // Simula la transcripción de voz a texto en campo
        public string SimularDictadoVoz(string inputAudioId)
        {
            // En un entorno real, aquí se llamaría a Azure Speech to Text o Whisper (OpenAI)
            return "Colmena 4 en buen estado, reina joven avistada";
        }

        // Simula la interpretación del lenguaje natural a datos estructurados
        public NLPResult SimularProcesamientoLenguajeNatural(string textoDictado)
        {
            // En un entorno real, esto sería una llamada a un LLM para extraer entidades
            return new NLPResult
            {
                ColmenaId = 4,
                EstadoGeneral = "Buena",
                EstadoReina = "Joven y Activa"
            };
        }
    }

    public class IAVisionComputacionalService
    {
        // Simula el análisis de una imagen de un material (cajón/marco)
        public VisionResult AnalizarEstadoMaterial(string rutaImagen)
        {
            // En un entorno real, aquí se usaría un modelo de Computer Vision (ej. YOLO)
            return new VisionResult
            {
                TipoMaterial = "Cajón",
                Condicion = "Requiere recambio por desgaste",
                NivelConfianza = 0.92
            };
        }
    }

    // DTOs para simuladores
    public class NLPResult
    {
        public int ColmenaId { get; set; }
        public string EstadoGeneral { get; set; } = string.Empty;
        public string EstadoReina { get; set; } = string.Empty;
    }

    public class VisionResult
    {
        public string TipoMaterial { get; set; } = string.Empty;
        public string Condicion { get; set; } = string.Empty;
        public double NivelConfianza { get; set; }
    }
}
