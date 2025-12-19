using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Utils; // Ajout de cette directive using
using SystemeNote.ViewModels; // Ajout de cette directive using pour les ViewModels si nécessaire
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
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
            ViewData["DateDebutSortParm"] = sortOrder == "date_debut" ? "date_debut_desc" : "date_debut";
            ViewData["DateFinSortParm"] = sortOrder == "date_fin" ? "date_fin_desc" : "date_fin";
            ViewData["SemestreSortParm"] = sortOrder == "semestre" ? "semestre_desc" : "semestre";
            ViewData["PromotionSortParm"] = sortOrder == "promotion" ? "promotion_desc" : "promotion";
            ViewData["OptionEtudeSortParm"] = sortOrder == "option_etude" ? "option_etude_desc" : "option_etude";

            ViewData["CurrentFilter"] = searchString;

            var planifSemestres = _context.PlanifSemestres
                                          .Include(p => p.Semestre)
                                          .Include(p => p.Promotion)
                                          .Include(p => p.OptionEtude)
                                          .AsQueryable(); // Important pour le tri et la pagination

            if (!String.IsNullOrEmpty(searchString))
            {
                planifSemestres = planifSemestres.Where(p => p.NomPlanifSemestre.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    planifSemestres = planifSemestres.OrderByDescending(p => p.NomPlanifSemestre);
                    break;
                case "date_debut":
                    planifSemestres = planifSemestres.OrderBy(p => p.DateDebut);
                    break;
                case "date_debut_desc":
                    planifSemestres = planifSemestres.OrderByDescending(p => p.DateDebut);
                    break;
                // Ajoutez d'autres cas pour le tri par DateFin, Semestre, Promotion, OptionEtude si nécessaire
                case "semestre":
                    planifSemestres = planifSemestres.OrderBy(p => p.Semestre!.NomSemestre);
                    break;
                case "semestre_desc":
                    planifSemestres = planifSemestres.OrderByDescending(p => p.Semestre!.NomSemestre);
                    break;
                default: // Tri par défaut par NomPlanifSemestre ascendant
                    planifSemestres = planifSemestres.OrderBy(p => p.NomPlanifSemestre);
                    break;
            }

            int pageSize = 10; // Vous pouvez ajuster le nombre d'éléments par page
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

        // Les actions Ranking, ExportToExcel et ExportToPdf devraient être ici,
        // mais elles sont déjà présentes dans le contexte fourni.
        // Je ne les répète pas pour éviter la redondance.
        // Assurez-vous qu'elles sont bien dans votre fichier PlanifSemestresController.cs
        // et qu'elles utilisent le même ViewModel et la même logique de récupération de données.

        // ... (votre code existant pour Ranking, ExportToExcel, ExportToPdf) ...
    }
}