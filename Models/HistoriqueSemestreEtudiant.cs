using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SystemeNote.Models
{

    [Table("historique_semestre_etudiant")]
    public class HistoriqueSemestreEtudiant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("etudiant_id")]
        public int EtudiantId { get; set; }

        [Required]
        [Column("planif_semetre_id")]
        public int PlanifSemetreId { get; set; }

        [Required]
        [Column("date_debut")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Column("date_fin")]
        [DataType(DataType.Date)]
        public DateTime? DateFin { get; set; }

        [ForeignKey("EtudiantId")]
        public virtual required Etudiant Etudiant { get; set; }

        [ForeignKey("PlanifSemetreId")]
        public virtual required PlanifSemestre PlanifSemestre { get; set; }
    }

}