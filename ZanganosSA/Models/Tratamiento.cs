using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZanganosSA.Models
{
    [Table("Tratam_sanitarios")]
    public class Tratamiento
    {
        [Key]
        [Column("idTratamiento")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Medicamento")]
        [Column("medicamento")]
        [StringLength(100)]
        public string Medicamento { get; set; } = string.Empty;

        [Display(Name = "Dosis")]
        [Column("dosis")]
        [StringLength(50)]
        public string? Dosis { get; set; }

        [Required]
        [Display(Name = "Fecha de Inicio")]
        [Column("fech_inicio")]
        public DateTime FechaInicio { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Fecha de Fin")]
        [Column("fecha_fin")]
        public DateTime FechaFin { get; set; } = DateTime.Now;

        [Display(Name = "Duración (Días)")]
        [Column("duracion")]
        public int? DuracionDias { get; set; }

        // Relaciones N a M con Colmenas vía tabla intermedia
        public ICollection<ColmenaTratamiento> ColmenaTratamientos { get; set; } = new List<ColmenaTratamiento>();
    }
}
