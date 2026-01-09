using Microsoft.EntityFrameworkCore;
using SystemeNote.Models;

namespace SystemeNote.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Etudiant>()
                .ToTable(tb => tb.HasTrigger("trg_etudiant_planifsemestre_history"));

            modelBuilder.Entity<Etudiant>()
                .HasOne(e => e.PlanifSemestre)
                .WithMany(p => p.Etudiants)
                .HasForeignKey(e => e.PlanifSemestreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Etudiant>()
                .HasOne(e => e.Administrateur)
                .WithMany(a => a.Etudiants)
                .HasForeignKey(e => e.AdministrateurId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueSemestreEtudiant>()
                .HasOne(h => h.PlanifSemestre)
                .WithMany(p => p.HistoriqueSemestreEtudiants)
                .HasForeignKey(h => h.PlanifSemetreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistoriqueSemestreEtudiant>()
                .HasOne(h => h.Etudiant)
                .WithMany(e => e.HistoriqueSemestreEtudiants)
                .HasForeignKey(h => h.EtudiantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NoteEtudiant>()
                .HasOne(n => n.Etudiant)
                .WithMany(e => e.NoteEtudiants)
                .HasForeignKey(n => n.EtudiantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NoteEtudiant>()
                .HasOne(n => n.ParcoursEtude)
                .WithMany(p => p.NoteEtudiants)
                .HasForeignKey(n => n.ParcoursEtudiantId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NoteEtudiant>()
                .HasOne(n => n.Promotion)
                .WithMany()
                .HasForeignKey(n => n.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParcoursEtude>()
                .HasOne(p => p.Matiere)
                .WithMany(m => m.ParcoursEtudes)
                .HasForeignKey(p => p.MatiereId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParcoursEtude>()
                .HasOne(p => p.UniteEnseignement)
                .WithMany(u => u.ParcoursEtudes)
                .HasForeignKey(p => p.UniteEnseignementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ParcoursEtude>()
                .HasOne(p => p.PlanifSemestre)
                .WithMany(ps => ps.ParcoursEtudes)
                .HasForeignKey(p => p.PlanifSemestreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlanifSemestre>()
                .HasOne(p => p.Semestre)
                .WithMany(s => s.PlanifSemestres)
                .HasForeignKey(p => p.SemestreId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlanifSemestre>()
                .HasOne(p => p.OptionEtude)
                .WithMany(o => o.PlanifSemestres)
                .HasForeignKey(p => p.OptionEtudeId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PlanifSemestre>()
                .HasOne(p => p.Promotion)
                .WithMany()
                .HasForeignKey(p => p.PromotionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<NoteEtudiant>()
                .HasOne(n => n.ParcoursEtude)
                .WithMany(p => p.NoteEtudiants)
                .HasForeignKey(n => n.ParcoursEtudiantId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}