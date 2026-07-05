using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZanganosSA.Models
{
    [Table("Barril_exportacion")]
    public class Barril
    {
        [Key]
        [Column("id_barril")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Cosecha")]
        [Column("id_cosecha")]
        public int CosechaId { get; set; }
        public Cosecha? Cosecha { get; set; }

        [Required]
        [Display(Name = "Peso (kg)")]
        [Column("peso_kg", TypeName = "decimal(6,2)")]
        public decimal PesoKg { get; set; }

        [Display(Name = "Estado")]
        [Column("estado")]
        [StringLength(50)]
        public string? Estado { get; set; } // Listo, Exportado, Reservado

        [Display(Name = "Destino")]
        [Column("destino")]
        [StringLength(100)]
        public string? Destino { get; set; }
    }
}
