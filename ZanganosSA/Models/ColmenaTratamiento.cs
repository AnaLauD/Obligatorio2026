using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZanganosSA.Models
{
    [Table("Colmena_Tratamiento")]
    public class ColmenaTratamiento
    {
        [Column("id_colmena")]
        public int ColmenaId { get; set; }
        public Colmena? Colmena { get; set; }

        [Column("idTratamiento")]
        public int TratamientoId { get; set; }
        public Tratamiento? Tratamiento { get; set; }

        [Required]
        [Display(Name = "Fecha de Aplicación")]
        [Column("fec_aplicacion")]
        public DateTime FechaAplicacion { get; set; }
    }
}
