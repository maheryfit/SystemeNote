using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SystemeNote.Models
{
    [Table("planif_semestre")]
    public class PlanifSemestre
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        [Column("nom_planif_semestre")]
        public required string NomPlanifSemestre { get; set; }

        [Required]
        [Column("date_debut")]
        [DataType(DataType.Date)]
        public DateTime DateDebut { get; set; }

        [Required]
        [Column("date_fin")]
        [DataType(DataType.Date)]
        public DateTime DateFin { get; set; }

        [Required]
        [Column("semestre_id")]
        public int SemestreId { get; set; }

        [Required]
        [Column("option_etude_id")]
        public int OptionEtudeId { get; set; }

        [Required]
        [Column("total_credit")]
        public int TotalCredit { get; set; }

        [Required]
        [Column("promotion_id")]
        public int PromotionId { get; set; }

        [ForeignKey("SemestreId")]
        public virtual required Semestre Semestre { get; set; }

        [ForeignKey("OptionEtudeId")]
        public virtual required OptionEtude OptionEtude { get; set; }

        [ForeignKey("PromotionId")]
        public virtual required Promotion Promotion { get; set; }

        public required ICollection<ParcoursEtude> ParcoursEtudes { get; set; }
        public required ICollection<Etudiant> Etudiants { get; set; }
        public required ICollection<HistoriqueSemestreEtudiant> HistoriqueSemestreEtudiants { get; set; }
    }


}