using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
namespace SystemeNote.Models
{
    [Table("config")]
    public class Config
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [StringLength(25)]
        [Column("valeur")]
        public required string Valeur { get; set; }
    }
}