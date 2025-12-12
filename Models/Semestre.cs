using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemeNote.Models
{

    [Table("semestre")]
    public class Semestre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nom_semestre")]
        public required string NomSemestre { get; set; }

        public required ICollection<PlanifSemestre> PlanifSemestres { get; set; }
    }
}