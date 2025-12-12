using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SystemeNote.Models
{

    [Table("note_etudiant")]
    public class NoteEtudiant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("etudiant_id")]
        public int EtudiantId { get; set; }

        [Required]
        [Column("parcours_etudiant_id")]
        public int ParcoursEtudiantId { get; set; }

        [Required]
        [Column("note")]
        public double Note { get; set; }

        [Required]
        [Column("promotion_id")]
        public int PromotionId { get; set; }

        [ForeignKey("EtudiantId")]
        public virtual required Etudiant Etudiant { get; set; }

        [ForeignKey("ParcoursEtudiantId")]
        public virtual required ParcoursEtude ParcoursEtude { get; set; }

        [ForeignKey("PromotionId")]
        public virtual required Promotion Promotion { get; set; }
    }

}