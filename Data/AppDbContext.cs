using Microsoft.EntityFrameworkCore;
using SystemeNote.Models;
using SystemNote.Models;

namespace SystemeNote.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        // Define your DbSets here, for example:
        public DbSet<Diplome> Diplomes { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Administrateur> Administrateurs { get; set; }
        public DbSet<Semestre> Semestres { get; set; }
        public DbSet<OptionEtude> OptionEtudes { get; set; }
        public DbSet<PlanifSemestre> PlanifSemestres { get; set; }
        public DbSet<HistoriqueSemestreEtudiant> HistoriqueSemestreEtudiants { get; set; }
        public DbSet<Matiere> Matieres { get; set; }
        public DbSet<ParcoursEtude> ParcoursEtudes { get; set; }
        public DbSet<NoteEtudiant> NoteEtudiants { get; set; }
        public DbSet<UniteEnseignement> UniteEnseignements { get; set; }
        public DbSet<Config> Configs { get; set; }

    }
}