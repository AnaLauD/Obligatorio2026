using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZanganosSA.Models
{
    [Table("Cosecha")]
    public class Cosecha
    {
        [Key]
        [Column("id_cosecha")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Fecha de Cosecha")]
        [Column("fecha")]
        public DateTime FechaCosecha { get; set; } = DateTime.Now;

        [Required]
        [Display(Name = "Peso Total (kg)")]
        [Column("kg_total", TypeName = "decimal(10,2)")]
        public decimal KgTotal { get; set; }

        [Required]
        [Display(Name = "Lote")]
        [Column("lote")]
        [StringLength(50)]
        public string Lote { get; set; } = string.Empty;

        [Display(Name = "Observaciones")]
        [Column("observaciones")]
        [StringLength(500)]
        public string? Observaciones { get; set; }

        public ICollection<ColmenaCosecha> ColmenaCosechas { get; set; } = new List<ColmenaCosecha>();

        public ICollection<Barril> Barriles { get; set; } = new List<Barril>();
    }

    // Tabla intermedia para relación N a M entre Colmena y Cosecha
    [Table("Cosecha_Colmena")]
    public class ColmenaCosecha
    {
        [Column("id_colmena")]
        public int ColmenaId { get; set; }
        public Colmena? Colmena { get; set; }

        [Column("id_cosecha")]
        public int CosechaId { get; set; }
        public Cosecha? Cosecha { get; set; }
    }
}
