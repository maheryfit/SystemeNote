using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SystemeNote.Models
{

    [Table("semestre")]
    [Index(nameof(CodeSemestre), IsUnique = true)]
    [Index(nameof(NomSemestre), IsUnique = true)]
    public class Semestre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        [Column("code_semestre")]
        public required string CodeSemestre { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nom_semestre")]
        public required string NomSemestre { get; set; }

        [Required]
        [Column("diplome_id")]
        public int DiplomeId { get; set; }

        [ForeignKey("DiplomeId")]
        public virtual required Diplome Diplome { get; set; }

        public required ICollection<PlanifSemestre> PlanifSemestres { get; set; }
    }
}