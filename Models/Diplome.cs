using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SystemeNote.Models
{
    [Table("Diplome")]
    [Index(nameof(NomDiplome), IsUnique = true)]
    public class Diplome
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public required string NomDiplome { get; set; }
    }
}