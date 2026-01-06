using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemeNote.Models
{
    [Table("planif_semestre")]
    [Index(nameof(NomPlanifSemestre), IsUnique = true)]

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
        public virtual Semestre? Semestre { get; set; }

        [ForeignKey("OptionEtudeId")]
        public virtual OptionEtude? OptionEtude { get; set; }

        [ForeignKey("PromotionId")]
        public virtual Promotion? Promotion { get; set; }

        public ICollection<ParcoursEtude> ParcoursEtudes { get; set; }= new List<ParcoursEtude>();
        public ICollection<Etudiant> Etudiants { get; set; }=new List<Etudiant>();
        public ICollection<HistoriqueSemestreEtudiant> HistoriqueSemestreEtudiants { get; set; } = new List<HistoriqueSemestreEtudiant>();
    
    }


}