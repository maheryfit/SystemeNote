using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;
using SystemeNote.ViewModels;

namespace SystemeNote.Controllers
{
    public class EtudiantController : Controller
    {
        private readonly AppDbContext _context;

        public EtudiantController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Etudiant
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? promotionFilter, bool? isActifFilter, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["MatriculeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "matricule_desc" : "";
            ViewData["NomSortParm"] = sortOrder == "nom" ? "nom_desc" : "nom";
            ViewData["PrenomSortParm"] = sortOrder == "prenom" ? "prenom_desc" : "prenom";
            ViewData["PromotionSortParm"] = sortOrder == "promotion" ? "promotion_desc" : "promotion";
            ViewData["DateAdmissionSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            ViewData["CurrentFilter"] = searchString;
            ViewData["PromotionFilter"] = promotionFilter;
            ViewData["IsActifFilter"] = isActifFilter;
            ViewData["Title"] = "Liste des Étudiants";

            // Populate filter dropdowns
            ViewData["PromotionList"] = new SelectList(_context.Promotions, "Id", "NomPromotion");

            var etudiants = from e in _context.Etudiants
                           .Include(e => e.Promotion)
                           .Include(e => e.PlanifSemestre)
                           .Include(e => e.Administrateur)
                            select e;

            // Multi-criteria search
            if (!String.IsNullOrEmpty(searchString))
            {
                etudiants = etudiants.Where(e =>
                    e.Matricule.Contains(searchString) ||
                    e.Nom.Contains(searchString) ||
                    e.Prenom.Contains(searchString));
            }

            // Filter by promotion
            if (promotionFilter.HasValue)
            {
                etudiants = etudiants.Where(e => e.PromotionId == promotionFilter.Value);
            }

            // Filter by status (actif/inactif)
            if (isActifFilter.HasValue)
            {
                etudiants = etudiants.Where(e => e.IsActif == isActifFilter.Value);
            }

            // Sorting
            switch (sortOrder)
            {
                case "matricule_desc":
                    etudiants = etudiants.OrderByDescending(e => e.Matricule);
                    break;
                case "nom":
                    etudiants = etudiants.OrderBy(e => e.Nom);
                    break;
                case "nom_desc":
                    etudiants = etudiants.OrderByDescending(e => e.Nom);
                    break;
                case "prenom":
                    etudiants = etudiants.OrderBy(e => e.Prenom);
                    break;
                case "prenom_desc":
                    etudiants = etudiants.OrderByDescending(e => e.Prenom);
                    break;
                case "promotion":
                    etudiants = etudiants.OrderBy(e => e.Promotion!.NomPromotion);
                    break;
                case "promotion_desc":
                    etudiants = etudiants.OrderByDescending(e => e.Promotion!.NomPromotion);
                    break;
                case "date":
                    etudiants = etudiants.OrderBy(e => e.DateAdmission);
                    break;
                case "date_desc":
                    etudiants = etudiants.OrderByDescending(e => e.DateAdmission);
                    break;
                default:
                    etudiants = etudiants.OrderBy(e => e.Matricule);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Etudiant>.CreateAsync(etudiants.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: Etudiant/AcademicRecord/5
        public async Task<IActionResult> AcademicRecord(int? id)
        {
            if (id == null) return NotFound();

            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant == null) return NotFound();

            // Récupérer toutes les notes de l'étudiant avec les informations associées
            var notes = await _context.NoteEtudiants
                .Where(n => n.EtudiantId == id)
                .Include(n => n.ParcoursEtude)
                    .ThenInclude(p => p!.Matiere)
                .Include(n => n.ParcoursEtude)
                    .ThenInclude(p => p!.UniteEnseignement)
                .Include(n => n.ParcoursEtude)
                    .ThenInclude(p => p!.PlanifSemestre)
                .ToListAsync();

            // Grouper les notes par semestre planifié
            var semestersGroup = notes
                .Where(n => n.ParcoursEtude?.PlanifSemestre != null)
                .GroupBy(n => n.ParcoursEtude!.PlanifSemestre);

            var semesterRecords = new List<SemesterRecord>();
            int totalCreditsObtained = 0;

            foreach (var group in semestersGroup)
            {
                var planifSemestre = group.Key;
                var uesGroup = group.GroupBy(n => n.ParcoursEtude!.UniteEnseignement);

                var ueRecords = new List<UeRecord>();
                int creditsInSemester = 0;

                foreach (var ueGroup in uesGroup)
                {
                    var ue = ueGroup.Key;
                    var matieresRecords = ueGroup
                        .GroupBy(n => n.ParcoursEtude!.Matiere) // Regrouper par matière pour trouver la note max
                        .Select(mg => new MatiereRecord
                        {
                            MatiereName = mg.Key!.NomMatiere,
                            Note = mg.Max(n => n.Note) // Prendre la note maximale pour la matière
                        }).ToList();

                    var ueAverage = matieresRecords.Any(m => m.Note.HasValue) ? matieresRecords.Average(m => m.Note ?? 0) : 0;
                    bool ueValidated = ueAverage >= 10; // Seuil de validation de l'UE (à configurer)

                    if (ueValidated)
                    {
                        creditsInSemester += ue!.Credits;
                    }

                    ueRecords.Add(new UeRecord
                    {
                        UeCode = ue.CodeUniteEnseignement,
                        Credits = ue.Credits,
                        UeAverage = ueAverage,
                        Matieres = matieresRecords
                    });
                }

                totalCreditsObtained += creditsInSemester;
                semesterRecords.Add(new SemesterRecord
                {
                    SemesterName = planifSemestre!.NomPlanifSemestre,
                    Status = creditsInSemester >= planifSemestre.TotalCredit ? "Validé" : "Ajourné",
                    CreditsObtained = creditsInSemester,
                    TotalCredits = planifSemestre.TotalCredit,
                    UEs = ueRecords.OrderBy(u => u.UeCode).ToList()
                });
            }

            var viewModel = new AcademicRecordViewModel
            {
                Etudiant = etudiant,
                Semesters = semesterRecords.OrderBy(s => s.SemesterName).ToList(),
                TotalCreditsObtained = totalCreditsObtained
            };

            return View(viewModel);
        }

        // GET: Etudiant/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var etudiant = await _context.Etudiants
                .Include(e => e.Administrateur)
                .Include(e => e.PlanifSemestre)
                .Include(e => e.Promotion)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (etudiant == null) return NotFound();

            return View(etudiant);
        }

        // GET: Etudiant/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Etudiant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Matricule,Nom,Prenom,DateNaissance,PromotionId,Genre,IsActif,DateAdmission,AdministrateurId,PlanifSemestreId,MotDePasse")] Etudiant etudiant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(etudiant);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(etudiant);
            return View(etudiant);
        }

        // GET: Etudiant/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            Console.WriteLine("Edit id: {0}", id);
            if (id == null) return NotFound();
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant == null) return NotFound();
            PopulateDropdowns(etudiant);
            return View(etudiant);
        }

        // POST: Etudiant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Matricule,Nom,Prenom,DateNaissance,PromotionId,Genre,IsActif,DateAdmission,AdministrateurId,PlanifSemestreId,MotDePasse")] Etudiant etudiant)
        {
            if (id != etudiant.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(etudiant); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!_context.Etudiants.Any(e => e.Id == etudiant.Id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(etudiant);
            return View(etudiant);
        }

        // GET: Etudiant/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var etudiant = await _context.Etudiants
                .Include(e => e.Promotion)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (etudiant == null) return NotFound();

            return View(etudiant);
        }

        // POST: Etudiant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant != null)
            {
                _context.Etudiants.Remove(etudiant);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(object? selectedEtudiant = null)
        {
            ViewData["AdministrateurId"] = new SelectList(_context.Administrateurs, "Id", "NomAdmin", selectedEtudiant);
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", selectedEtudiant);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", selectedEtudiant);
        }
    }
}