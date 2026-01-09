using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SystemeNote.Models
{

    [Table("etudiant")]
    [Index(nameof(Matricule), IsUnique = true)]
    public class Etudiant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(15)]
        [Column("matricule")]
        public required string Matricule { get; set; }

        [Required]
        [StringLength(30)]
        [Column("nom")]
        public required string Nom { get; set; }

        [Required]
        [StringLength(50)]
        [Column("prenom")]
        public required string Prenom { get; set; }

        [Required]
        [Column("date_naissance")]
        [DataType(DataType.Date)]
        public DateTime DateNaissance { get; set; }

        [Required]
        [Column("promotion_id")]
        public int PromotionId { get; set; }
        [ForeignKey("PromotionId")]
        public virtual  Promotion? Promotion { get; set; }

        [Required]
        [Column("genre")]
        public int Genre { get; set; }

        [Required]
        [Column("is_actif")]
        public bool IsActif { get; set; } = false;

        [Required]
        [Column("date_admission")]
        [DataType(DataType.Date)]
        public DateTime DateAdmission { get; set; }

        [Required]
        [Column("administrateur_id")]
        public int AdministrateurId { get; set; }


        [ForeignKey("AdministrateurId")]
        public virtual Administrateur? Administrateur { get; set; }

        [Required]
        [Column("planif_semestre_id")]
        public int PlanifSemestreId { get; set; }
        [ForeignKey("PlanifSemestreId")]
        public virtual  PlanifSemestre? PlanifSemestre { get; set; }

        [Required]
        [StringLength(255)]
        [Column("mot_de_passe")]
        [DataType(DataType.Password)]
        public string? MotDePasse { get; set; }="pass123";

        public ICollection<NoteEtudiant> NoteEtudiants { get; set; }=new List<NoteEtudiant>();
        public ICollection<HistoriqueSemestreEtudiant> HistoriqueSemestreEtudiants { get; set; }= new List<HistoriqueSemestreEtudiant>();
    }

}