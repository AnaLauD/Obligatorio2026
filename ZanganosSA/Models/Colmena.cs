using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZanganosSA.Models
{
    [Table("Colmena")]
    public class Colmena
    {
        [Key]
        [Column("id_colmena")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Apiario")]
        [Column("id_apiario")]
        public int ApiarioId { get; set; }
        public Apiario? Apiario { get; set; }

        [Display(Name = "Tipo de Caja")]
        [Column("tipo_caja")]
        [StringLength(50)]
        public string? TipoCaja { get; set; }

        [Display(Name = "Estado de la Colmena")]
        [Column("estado_colmena")]
        [StringLength(50)]
        public string? EstadoColmena { get; set; } // Fuerte, Media, Debil, Critica

        [Display(Name = "Estado de la Reina")]
        [Column("reina_estado")]
        [StringLength(50)]
        public string? ReinaEstado { get; set; } // Sana, Joven, Vieja, Ausente, Reemplazada

        [Display(Name = "Activa")]
        [Column("activa")]
        public bool Activa { get; set; } = true;

        [Display(Name = "Observaciones")]
        [Column("observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        // Relación 1 a N con Inspecciones
        public ICollection<Inspeccion> Inspecciones { get; set; } = new List<Inspeccion>();

        // Relaciones N a M manual con Cosechas
        public ICollection<ColmenaCosecha> ColmenaCosechas { get; set; } = new List<ColmenaCosecha>();

        // Relaciones N a M manual con Tratamientos
        public ICollection<ColmenaTratamiento> ColmenaTratamientos { get; set; } = new List<ColmenaTratamiento>();
    }
}
