using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SystemNote.Models;

namespace SystemeNote.Models
{
    [Table("parcours_etude")]
    public class ParcoursEtude
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [Column("matiere_id")]
        public int MatiereId { get; set; }

        [Required]
        [Column("unite_enseignement_id")]
        public int UniteEnseignementId { get; set; }

        [Required]
        [Column("planif_semestre_id")]
        public int PlanifSemestreId { get; set; }

        [ForeignKey("MatiereId")]
        public virtual required Matiere Matiere { get; set; }

        [ForeignKey("UniteEnseignementId")]
        public virtual required UniteEnseignement UniteEnseignement { get; set; }

        [ForeignKey("PlanifSemestreId")]
        public virtual required PlanifSemestre PlanifSemestre { get; set; }

        public required ICollection<NoteEtudiant> NoteEtudiants { get; set; }
    }

}