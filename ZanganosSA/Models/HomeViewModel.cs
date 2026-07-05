using ZanganosSA.Models;

namespace ZanganosSA.Models
{
    public class HomeViewModel
    {
        public int TotalApiarios { get; set; }
        public int TotalColmenas { get; set; }
        public int ColmenasActivas { get; set; }
        public int ColmenasInactivas { get; set; }
        public int TotalInspecciones { get; set; }
        public decimal ProduccionTotalKg { get; set; }

        // Alertas: Tratamientos activos (FechaFin >= hoy) con countdown
        public List<Tratamiento> TratamientosActivos { get; set; } = new List<Tratamiento>();

        // Alertas: Alimentaciones próximas o vencidas
        public List<Alimentacion> ProximasAlimentaciones { get; set; } = new List<Alimentacion>();
    }
}
