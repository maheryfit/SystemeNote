using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SystemeNote.Models
{

    [Table("option_etude")]
    public class OptionEtude
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nom_option_etude")]
        public required string NomOptionEtude { get; set; }

        public required ICollection<PlanifSemestre> PlanifSemestres { get; set; }
    }
}