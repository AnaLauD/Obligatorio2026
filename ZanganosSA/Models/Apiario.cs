using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZanganosSA.Models
{
    [Table("Apiario")]
    public class Apiario
    {
        [Key]
        [Column("id_apiario")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Departamento")]
        [Column("departamento")]
        [StringLength(50)]
        public string Departamento { get; set; } = string.Empty;

        [Display(Name = "Seccional Policial")]
        [Column("seccional_policial")]
        [StringLength(50)]
        public string? SeccionalPolicial { get; set; }

        [Required]
        [Display(Name = "Fecha de Registro")]
        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        [Display(Name = "Latitud")]
        [Column("latitud", TypeName = "decimal(10,8)")]
        public decimal? Latitud { get; set; }

        [Display(Name = "Longitud")]
        [Column("longitud", TypeName = "decimal(11,8)")]
        public decimal? Longitud { get; set; }

        // Relación 1 a N con Colmenas
        public ICollection<Colmena> Colmenas { get; set; } = new List<Colmena>();

        // Relación 1 a N con Alimentaciones
        public ICollection<Alimentacion> Alimentaciones { get; set; } = new List<Alimentacion>();
    }
}
