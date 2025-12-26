using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.ViewModels;

namespace SystemeNote.Controllers
{
    public class ProfesseurController : Controller
    {
        private readonly AppDbContext _context;

        public ProfesseurController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Récupérer toutes les données nécessaires en une seule requête
            var allNotes = await _context.NoteEtudiants
                .Include(n => n.Etudiant)
                .Include(n => n.ParcoursEtude)!.ThenInclude(p => p.Matiere)
                .Include(n => n.ParcoursEtude)!.ThenInclude(p => p.UniteEnseignement)
                .Include(n => n.ParcoursEtude)!.ThenInclude(p => p.PlanifSemestre)!.ThenInclude(ps => ps.Promotion)
                .ToListAsync();

            // 2. Calculer la moyenne de chaque étudiant pour chaque semestre planifié
            var studentAveragesByPlanif = allNotes
                .Where(n => n.ParcoursEtude?.PlanifSemestre != null && n.Etudiant != null)
                .GroupBy(n => new { n.Etudiant, n.ParcoursEtude!.PlanifSemestre })
                .Select(studentPlanifGroup =>
                {
                    var ueAverages = studentPlanifGroup
                        .GroupBy(n => n.ParcoursEtude!.UniteEnseignement)
                        .Select(ueGroup =>
                        {
                            var maxNotePerMatiere = ueGroup
                                .GroupBy(n => n.ParcoursEtude!.Matiere)
                                .Select(matiereGroup => matiereGroup.Select(n => n.Note).DefaultIfEmpty(0).Max());
                            return maxNotePerMatiere.Any() ? maxNotePerMatiere.Average() : 0;
                        });
                    var overallAverage = ueAverages.Any() ? ueAverages.Average() : 0;
                    return new
                    {
                        studentPlanifGroup.Key.Etudiant,
                        studentPlanifGroup.Key.PlanifSemestre,
                        Average = overallAverage
                    };
                })
                .ToList();

            // 3. Calculer les statistiques par semestre
            var semestreStats = studentAveragesByPlanif
                .GroupBy(s => s.PlanifSemestre)
                .Select(g =>
                {
                    var averages = g.Select(item => item.Average).ToList();
                    var total = averages.Count;
                    var admis = averages.Count(a => a >= 10);
                    var moyenne = total > 0 ? averages.Average() : 0;
                    return new SemestreStats
                    {
                        PlanifSemestreId = g.Key.Id,
                        NomPlanifSemestre = g.Key.NomPlanifSemestre,
                        TotalEtudiants = total,
                        Admis = admis,
                        Ajournes = total - admis,
                        PourcentageAdmis = total > 0 ? (double)admis / total * 100 : 0,
                        PourcentageAjournes = total > 0 ? (double)(total - admis) / total * 100 : 0,
                        MoyenneClasse = moyenne
                    };
                })
                .OrderBy(s => s.NomPlanifSemestre)
                .ToList();

            // 4. Calculer la progression par promotion
            var promotionProgress = new List<PromotionProgress>();
            var planifsByPromotion = studentAveragesByPlanif
                .Select(s => s.PlanifSemestre)
                .Distinct()
                .GroupBy(ps => ps.Promotion);

            foreach (var promoGroup in planifsByPromotion)
            {
                if (promoGroup.Key == null) continue;

                var orderedPlanifs = promoGroup.OrderBy(p => p.DateDebut).ToList();
                if (orderedPlanifs.Count < 1) continue;

                var currentPlanif = orderedPlanifs.Last();
                var previousPlanif = orderedPlanifs.Count > 1 ? orderedPlanifs[^2] : null;

                var admisCurrent = studentAveragesByPlanif
                    .Count(s => s.PlanifSemestre.Id == currentPlanif.Id && s.Average >= 10);

                var admisPrevious = previousPlanif != null
                    ? studentAveragesByPlanif.Count(s => s.PlanifSemestre.Id == previousPlanif.Id && s.Average >= 10)
                    : 0;

                double progression = 0;
                if (admisPrevious > 0)
                {
                    progression = ((double)admisCurrent - admisPrevious) / admisPrevious * 100;
                }
                else if (admisCurrent > 0)
                {
                    progression = 100; // Progression de 0 à >0
                }

                promotionProgress.Add(new PromotionProgress
                {
                    PromotionId = promoGroup.Key.Id,
                    NomPromotion = promoGroup.Key.NomPromotion,
                    AdmisCurrent = admisCurrent,
                    AdmisPrevious = admisPrevious,
                    PourcentageProgression = Math.Round(progression, 2)
                });
            }

            var model = new ProfesseurDashboardViewModel
            {
                SemestreStats = semestreStats,
                PromotionProgress = promotionProgress.OrderBy(p => p.NomPromotion).ToList()
            };
            return View(model);
        }
    }
}
