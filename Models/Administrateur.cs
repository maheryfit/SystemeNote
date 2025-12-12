using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SystemeNote.Models
{
    [Table("administrateur")]
    public class Administrateur
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nom_admin")]
        public required string NomAdmin { get; set; }

        [Required]
        [StringLength(255)]
        [Column("prenom_admin")]
        public required string PrenomAdmin { get; set; }

        public required ICollection<Etudiant> Etudiants { get; set; }
    }

}