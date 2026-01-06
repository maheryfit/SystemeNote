using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SystemeNote.Models
{


    [Table("unite_enseignement")]
    [Index(nameof(CodeUniteEnseignement), IsUnique = true)]
    public class UniteEnseignement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        [Column("code_unite_enseignement")]
        public required string CodeUniteEnseignement { get; set; }

        [Required]
        [Column("credits")]
        public int Credits { get; set; }

        public virtual ICollection<ParcoursEtude>? ParcoursEtudes { get; set; }= new List<ParcoursEtude>();
    }

}