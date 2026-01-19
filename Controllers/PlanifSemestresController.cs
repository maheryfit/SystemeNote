using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils; // Ajout de cette directive using
using SystemeNote.ViewModels; // Ajout de cette directive using pour les ViewModels si nécessaire

namespace SystemeNote.Controllers
{
    [Authorize(Roles = "Administrateur")]
    public class PlanifSemestresController : Controller
    {
        private readonly AppDbContext _context;

        public PlanifSemestresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PlanifSemestres

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["SemestreSortParm"] = sortOrder == "semestre" ? "semestre_desc" : "semestre";
            ViewData["OptionSortParm"] = sortOrder == "option" ? "option_desc" : "option";
            ViewData["PromotionSortParm"] = sortOrder == "promotion" ? "promotion_desc" : "promotion";
            ViewData["DateDebutSortParm"] = sortOrder == "dateDebut" ? "dateDebut_desc" : "dateDebut";

            ViewData["CurrentFilter"] = searchString;
            ViewData["Title"] = "Planifications de Semestre";

            var planifSemestres = from p in _context.PlanifSemestres
                                  .Include(p => p.Semestre)
                                  .Include(p => p.OptionEtude)
                                  .Include(p => p.Promotion)
                                  select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                planifSemestres = planifSemestres.Where(p =>
                    p.NomPlanifSemestre.Contains(searchString) ||
                    p.Semestre!.NomSemestre.Contains(searchString) ||
                    p.OptionEtude!.NomOptionEtude.Contains(searchString) ||
                    p.Promotion!.NomPromotion.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    planifSemestres = planifSemestres.OrderByDescending(p => p.NomPlanifSemestre);
                    break;
                case "semestre":
                    planifSemestres = planifSemestres.OrderBy(p => p.Semestre!.NomSemestre);
                    break;
                case "semestre_desc":
                    planifSemestres = planifSemestres.OrderByDescending(p => p.Semestre!.NomSemestre);
                    break;
                case "option":
                    planifSemestres = planifSemestres.OrderBy(p => p.OptionEtude!.NomOptionEtude);
                    break;
                case "option_desc":
                    planifSemestres = planifSemestres.OrderByDescending(p => p.OptionEtude!.NomOptionEtude);
                    break;
                case "promotion":
                    planifSemestres = planifSemestres.OrderBy(p => p.Promotion!.NomPromotion);
                    break;
                case "promotion_desc":
                    planifSemestres = planifSemestres.OrderByDescending(p => p.Promotion!.NomPromotion);
                    break;
                case "dateDebut":
                    planifSemestres = planifSemestres.OrderBy(p => p.DateDebut);
                    break;
                case "dateDebut_desc":
                    planifSemestres = planifSemestres.OrderByDescending(p => p.DateDebut);
                    break;
                default:
                    planifSemestres = planifSemestres.OrderBy(p => p.NomPlanifSemestre);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<PlanifSemestre>.CreateAsync(planifSemestres.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: PlanifSemestres/Create
        public IActionResult Create()
        {
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre");
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion");
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude");
            return View();
        }

        // POST: PlanifSemestres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomPlanifSemestre,DateDebut,DateFin,SemestreId,PromotionId,OptionEtudeId,TotalCredit")] PlanifSemestre planifSemestre)
        {
            // --- Début du débogage ---
            Console.WriteLine("--- [POST] PlanifSemestres/Create ---");
            Console.WriteLine($"Nom reçu : {planifSemestre.NomPlanifSemestre}");
            Console.WriteLine($"SemestreId reçu : {planifSemestre.SemestreId}");
            Console.WriteLine($"PromotionId reçu : {planifSemestre.PromotionId}");
            Console.WriteLine($"OptionEtudeId reçu : {planifSemestre.OptionEtudeId}");
            Console.WriteLine($"TotalCredit reçu : {planifSemestre.TotalCredit}");
            // --- Fin du débogage ---
            planifSemestre.ParcoursEtudes = new List<ParcoursEtude>();
            planifSemestre.Etudiants = new List<Etudiant>();
            planifSemestre.HistoriqueSemestreEtudiants = new List<HistoriqueSemestreEtudiant>();


            // Les collections ParcoursEtudes, Etudiants, HistoriqueSemestreEtudiants
            // sont maintenant initialisées dans le constructeur du modèle PlanifSemestre.

            if (ModelState.IsValid)
            {
                Console.WriteLine("Le modèle est VALIDE. Ajout à la base de données.");
                _context.Add(planifSemestre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("Le modèle est INVALIDE. Retour à la vue.");
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre", planifSemestre.SemestreId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", planifSemestre.PromotionId);
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude", planifSemestre.OptionEtudeId);
            return View(planifSemestre);
        }

        // GET: PlanifSemestres/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var planifSemestre = await _context.PlanifSemestres
                .Include(p => p.Semestre)
                .Include(p => p.Promotion)
                .Include(p => p.OptionEtude)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (planifSemestre == null) return NotFound();

            return View(planifSemestre);
        }

        // GET: PlanifSemestres/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var planifSemestre = await _context.PlanifSemestres.FindAsync(id);
            if (planifSemestre == null) return NotFound();
            PopulateDropdowns(planifSemestre);
            return View(planifSemestre);
        }

        // POST: PlanifSemestres/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomPlanifSemestre,DateDebut,DateFin,SemestreId,PromotionId,OptionEtudeId,TotalCredit")] PlanifSemestre planifSemestre)
        {
            if (id != planifSemestre.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(planifSemestre); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!PlanifSemestreExists(planifSemestre.Id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(planifSemestre);
            return View(planifSemestre);
        }

        // GET: PlanifSemestres/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var planifSemestre = await _context.PlanifSemestres
                .Include(p => p.Semestre)
                .Include(p => p.Promotion)
                .Include(p => p.OptionEtude)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (planifSemestre == null) return NotFound();

            return View(planifSemestre);
        }

        // POST: PlanifSemestres/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var planifSemestre = await _context.PlanifSemestres.FindAsync(id);
            if (planifSemestre != null)
            {
                _context.PlanifSemestres.Remove(planifSemestre);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PlanifSemestreExists(int id)
        {
            return _context.PlanifSemestres.Any(e => e.Id == id);
        }

        private void PopulateDropdowns(object? selectedPlanifSemestre = null)
        {
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre",
                selectedPlanifSemestre is PlanifSemestre ps ? ps.SemestreId : null);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion",
                selectedPlanifSemestre is PlanifSemestre ps2 ? ps2.PromotionId : null);
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude",
                selectedPlanifSemestre is PlanifSemestre ps3 ? ps3.OptionEtudeId : null);
        }

        public async Task<IActionResult> Ranking(int? id)
        {
            if (id == null) return NotFound();
            var viewModel = await GetRankingViewModelAsync(id.Value);
            if (viewModel == null) return NotFound();
            return View(viewModel);
        }

        // Excel export removed per request (annulation de l'export Excel)

        [Obsolete]
        public async Task<IActionResult> ExportToPdf(int? id)
        {
            if (id == null) return NotFound();
            var viewModel = await GetRankingViewModelAsync(id.Value);
            if (viewModel == null) return NotFound();

            string pdfName = $"Classement-{viewModel.PlanifSemestre.NomPlanifSemestre}-{DateTime.Now:yyyyMMdd}.pdf";
            var pdfResult = new ViewAsPdf("RankingPdf", viewModel) { FileName = pdfName };

            // Définir explicitement le chemin vers wkhtmltopdf pour éviter l'erreur de configuration globale manquante
            pdfResult.WkhtmltopdfPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Rotativa");

            return pdfResult;
        }

        private async Task<SemesterRankingViewModel?> GetRankingViewModelAsync(int id)
        {
            var planifSemestre = await _context.PlanifSemestres.FindAsync(id);
            if (planifSemestre == null) return null;

            // 1. Récupérer tous les étudiants inscrits à ce semestre (PlanifSemestreId)
            // Cela permet d'inclure même ceux qui n'ont pas encore de notes (ils auront 0 par défaut).
            var etudiants = await _context.Etudiants
                .Where(e => e.PlanifSemestreId == id)
                .ToListAsync();

            // 2. Récupérer la structure du semestre : tous les parcours (Matières/UEs) prévus
            var parcoursSemestre = await _context.ParcoursEtudes
                .Where(p => p.PlanifSemestreId == id)
                .Include(p => p.Matiere)
                .Include(p => p.UniteEnseignement)
                .ToListAsync();

            // Identifier les matières uniques du semestre (le dénominateur pour la moyenne)
            var matieresIds = parcoursSemestre.Select(p => p.MatiereId).Distinct().ToList();
            int nombreMatieres = matieresIds.Count;

            // 3. Récupérer toutes les notes enregistrées pour ce semestre
            var notes = await _context.NoteEtudiants
                .Where(n => n.ParcoursEtude!.PlanifSemestreId == id)
                .Include(n => n.ParcoursEtude) // Nécessaire pour filtrer par MatiereId
                .ToListAsync();

            var rankings = new List<StudentRankingRecord>();

            foreach (var etudiant in etudiants)
            {
                // Filtrer les notes de l'étudiant courant
                var notesEtudiant = notes.Where(n => n.EtudiantId == etudiant.Id).ToList();

                double sommeNotes = 0;
                var ueRecords = new List<UeGradeRecord>();

                // --- Calcul de la moyenne générale ---
                // "la note moyenne de l'etudiant dans une semetre est la somme des note des matiere du semestre , diviser par le nombre de matiere du semestre."
                foreach (var matiereId in matieresIds)
                {
                    // Récupérer les notes pour cette matière spécifique
                    var notesMatiere = notesEtudiant
                        .Where(n => n.ParcoursEtude != null && n.ParcoursEtude.MatiereId == matiereId)
                        .Select(n => n.Note)
                        .ToList();

                    // "si il y plusieurs note dans une matiere d'un eleve, on prend celui du max"
                    // "supposons qu'une eleve n'a pas encore de note dans une matiere. donc par defaut 0"
                    double noteRetenue = notesMatiere.Any() ? notesMatiere.Max() : 0.0;

                    sommeNotes += noteRetenue;
                }

                double moyenneGenerale = nombreMatieres > 0 ? sommeNotes / nombreMatieres : 0;

                // --- Calcul des moyennes par UE (pour l'affichage) ---
                var ues = parcoursSemestre.GroupBy(p => p.UniteEnseignement).Where(g => g.Key != null);
                foreach (var ueGroup in ues)
                {
                    var ue = ueGroup.Key!;
                    var matieresUeIds = ueGroup.Select(p => p.MatiereId).Distinct().ToList();
                    double sommeNotesUe = 0;
                    foreach (var mId in matieresUeIds)
                    {
                        var notesMatiere = notesEtudiant.Where(n => n.ParcoursEtude?.MatiereId == mId).Select(n => n.Note).ToList();
                        sommeNotesUe += notesMatiere.Any() ? notesMatiere.Max() : 0.0;
                    }
                    double moyenneUe = matieresUeIds.Count > 0 ? sommeNotesUe / matieresUeIds.Count : 0;
                    ueRecords.Add(new UeGradeRecord { UeCode = ue.CodeUniteEnseignement, UeName = ue.CodeUniteEnseignement, UeAverage = moyenneUe });
                }

                rankings.Add(new StudentRankingRecord
                {
                    Etudiant = etudiant,
                    OverallAverage = moyenneGenerale,
                    Status = moyenneGenerale >= 10 ? "Admis" : "Ajourné",
                    UeGrades = ueRecords.OrderBy(u => u.UeCode).ToList()
                });
            }

            // Trier par moyenne décroissante et assigner les rangs
            rankings = rankings.OrderByDescending(r => r.OverallAverage).ToList();
            for (int i = 0; i < rankings.Count; i++)
            {
                rankings[i].Rank = i + 1;
            }

            var total = rankings.Count;
            var avgClass = total > 0 ? rankings.Average(r => r.OverallAverage) : 0;
            var variance = total > 1 ? rankings.Sum(r => Math.Pow(r.OverallAverage - avgClass, 2)) / (total - 1) : 0;

            return new SemesterRankingViewModel { PlanifSemestre = planifSemestre, StudentRankings = rankings, Stats = new SemesterStatistics { TotalStudents = total, AdmisCount = rankings.Count(r => r.Status == "Admis"), AjourneCount = rankings.Count(r => r.Status == "Ajourné"), ClassAverage = avgClass, GradeVariance = variance } };
        }
    }
}