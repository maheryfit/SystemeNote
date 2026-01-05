using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SystemeNote.Models
{
    [Table("administrateur")]
    [Index(nameof(NomAdmin), nameof(PrenomAdmin), IsUnique = true)]

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

        public ICollection<Etudiant> Etudiants { get; set; }=new List<Etudiant>();
    }

}