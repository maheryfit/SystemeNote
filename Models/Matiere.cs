using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemeNote.Models
{
    [Table("matiere")]
    [Index(nameof(NomMatiere), IsUnique = true)]
    [Index(nameof(CodeMatiere), IsUnique = true)]
    public class Matiere
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nom_matiere")]
        public required string NomMatiere { get; set; }

        [Required]
        [StringLength(255)]
        [Column("code_matiere")]
        public required string CodeMatiere { get; set; }

        public virtual ICollection<SystemeNote.Models.ParcoursEtude>? ParcoursEtudes { get; set; }

    }

}