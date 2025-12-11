using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Humanizer;
using Microsoft.EntityFrameworkCore;

namespace SystemeNote.Models
{   
    [Table("Promotion")]
    [Index(nameof(NomPromotion), IsUnique = true)]
    [Index(nameof(CodePromotion), IsUnique = true)]
    public class Promotion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 

        [Required]
        public required string NomPromotion { get; set; }

        [Required]
        public DateOnly DateCreation { get; set; }

        [Required]
        [StringLength(10)]
        public required string CodePromotion { get; set; } 

        [Required]
        [ForeignKey("Diplome")]
        public int DiplomeId { get; set; } 

        // Navigation property is not bound by forms; keep it optional for model binding
        public Diplome? Diplome { get; set; }

        public Promotion()
        {
        }

        public override string ToString()
        {
            return $"{NomPromotion} ({CodePromotion})";
        }
    }
}