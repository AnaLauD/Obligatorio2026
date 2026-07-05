using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZanganosSA.Models
{
    [Table("Inspeccion")]
    public class Inspeccion
    {
        [Key]
        [Column("id_inspeccion")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Colmena")]
        [Column("id_colmena")]
        public int ColmenaId { get; set; }
        public Colmena? Colmena { get; set; }

        [Required]
        [Display(Name = "Fecha y Hora")]
        [Column("fecha_hora")]
        public DateTime FechaHora { get; set; } = DateTime.Now;

        [Display(Name = "Estado de la Colmena")]
        [Column("estado_colmena")]
        [StringLength(50)]
        public string? EstadoColmena { get; set; }

        [Display(Name = "Estado de la Reina")]
        [Column("estado_reina")]
        [StringLength(50)]
        public string? EstadoReina { get; set; }

        [Display(Name = "Población Estimada")]
        [Column("poblacion_estimada")]
        public int? PoblacionEstimada { get; set; }

        [Display(Name = "Observaciones")]
        [Column("observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }
    }
}
