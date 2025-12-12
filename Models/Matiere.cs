using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SystemeNote.Models;

namespace SystemNote.Models
{
    [Table("matiere")]
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

        public required ICollection<ParcoursEtude> ParcoursEtudes { get; set; }

    }

}