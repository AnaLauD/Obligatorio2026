using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZanganosSA.Models
{
    [Table("Alimentacion")]
    public class Alimentacion
    {
        [Key]
        [Column("id_alimentation")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Apiario")]
        [Column("id_apiario")]
        public int ApiarioId { get; set; }
        public Apiario? Apiario { get; set; }

        [Required]
        [Display(Name = "Tipo de Alimento")]
        [Column("tipo_alimento")]
        [StringLength(100)]
        public string TipoAlimento { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Fecha Programada")]
        [Column("fecha_programada")]
        public DateTime FechaProgramada { get; set; } = DateTime.Now;

        [Display(Name = "Cantidad (kg)")]
        [Column("cantidad", TypeName = "decimal(8,2)")]
        public decimal? Cantidad { get; set; }
    }
}
