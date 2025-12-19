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
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["FirstNameSortParm"] = sortOrder == "fname" ? "fname_desc" : "fname";
            ViewData["MatriculeSortParm"] = sortOrder == "matricule" ? "matricule_desc" : "matricule";

            ViewData["CurrentFilter"] = searchString;

            var etudiants = from e in _context.Etudiants.Include(e => e.Promotion)
                            select e;

            if (!String.IsNullOrEmpty(searchString))
            {
                etudiants = etudiants.Where(s => s.Nom.Contains(searchString)
                                       || s.Prenom.Contains(searchString)
                                       || s.Matricule.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    etudiants = etudiants.OrderByDescending(s => s.Nom);
                    break;
                case "fname":
                    etudiants = etudiants.OrderBy(s => s.Prenom);
                    break;
                case "fname_desc":
                    etudiants = etudiants.OrderByDescending(s => s.Prenom);
                    break;
                case "matricule":
                    etudiants = etudiants.OrderBy(s => s.Matricule);
                    break;
                case "matricule_desc":
                    etudiants = etudiants.OrderByDescending(s => s.Matricule);
                    break;
                default:
                    etudiants = etudiants.OrderBy(s => s.Nom);
                    break;
            }

            int pageSize = 10; // Vous pouvez ajuster le nombre d'éléments par page
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