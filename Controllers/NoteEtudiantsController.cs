using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public class NoteEtudiantsController : Controller
    {
        private readonly AppDbContext _context;
        public NoteEtudiantsController(AppDbContext context) { _context = context; }


        public async Task<IActionResult> Index(string sortOrder, string searchString, int? promotionFilter, int? matiereFilter, double? minNote, double? maxNote, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["EtudiantSortParm"] = String.IsNullOrEmpty(sortOrder) ? "etudiant_desc" : "";
            ViewData["MatiereSortParm"] = sortOrder == "matiere" ? "matiere_desc" : "matiere";
            ViewData["NoteSortParm"] = sortOrder == "note" ? "note_desc" : "note";
            ViewData["PromotionSortParm"] = sortOrder == "promotion" ? "promotion_desc" : "promotion";

            ViewData["CurrentFilter"] = searchString;
            ViewData["PromotionFilter"] = promotionFilter;
            ViewData["MatiereFilter"] = matiereFilter;
            ViewData["MinNote"] = minNote;
            ViewData["MaxNote"] = maxNote;
            ViewData["Title"] = "Liste des Notes";

            // Populate filter dropdowns
            ViewData["PromotionList"] = new SelectList(_context.Promotions, "Id", "NomPromotion");
            ViewData["MatiereList"] = new SelectList(_context.Matieres, "Id", "NomMatiere");

            var noteEtudiants = from n in _context.NoteEtudiants
                               .Include(n => n.Etudiant)
                               .Include(n => n.ParcoursEtude)
                                   .ThenInclude(p => p!.Matiere)
                               .Include(n => n.ParcoursEtude)
                                   .ThenInclude(p => p!.UniteEnseignement)
                               .Include(n => n.ParcoursEtude)
                                   .ThenInclude(p => p!.PlanifSemestre)
                               .Include(n => n.Promotion)
                                select n;

            // Multi-criteria search
            if (!String.IsNullOrEmpty(searchString))
            {
                noteEtudiants = noteEtudiants.Where(n =>
                    n.Etudiant!.Matricule.Contains(searchString) ||
                    n.Etudiant!.Nom.Contains(searchString) ||
                    n.Etudiant!.Prenom.Contains(searchString) ||
                    n.ParcoursEtude!.Matiere!.NomMatiere.Contains(searchString) ||
                    n.ParcoursEtude!.Matiere!.CodeMatiere.Contains(searchString));
            }

            // Filter by promotion
            if (promotionFilter.HasValue)
            {
                noteEtudiants = noteEtudiants.Where(n => n.PromotionId == promotionFilter.Value);
            }

            // Filter by matiere
            if (matiereFilter.HasValue)
            {
                noteEtudiants = noteEtudiants.Where(n => n.ParcoursEtude!.MatiereId == matiereFilter.Value);
            }

            // Filter by note range
            if (minNote.HasValue)
            {
                noteEtudiants = noteEtudiants.Where(n => n.Note >= minNote.Value);
            }

            if (maxNote.HasValue)
            {
                noteEtudiants = noteEtudiants.Where(n => n.Note <= maxNote.Value);
            }

            // Sorting
            switch (sortOrder)
            {
                case "etudiant_desc":
                    noteEtudiants = noteEtudiants.OrderByDescending(n => n.Etudiant!.Nom);
                    break;
                case "matiere":
                    noteEtudiants = noteEtudiants.OrderBy(n => n.ParcoursEtude!.Matiere!.NomMatiere);
                    break;
                case "matiere_desc":
                    noteEtudiants = noteEtudiants.OrderByDescending(n => n.ParcoursEtude!.Matiere!.NomMatiere);
                    break;
                case "note":
                    noteEtudiants = noteEtudiants.OrderBy(n => n.Note);
                    break;
                case "note_desc":
                    noteEtudiants = noteEtudiants.OrderByDescending(n => n.Note);
                    break;
                case "promotion":
                    noteEtudiants = noteEtudiants.OrderBy(n => n.Promotion!.NomPromotion);
                    break;
                case "promotion_desc":
                    noteEtudiants = noteEtudiants.OrderByDescending(n => n.Promotion!.NomPromotion);
                    break;
                default:
                    noteEtudiants = noteEtudiants.OrderBy(n => n.Etudiant!.Nom);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<NoteEtudiant>.CreateAsync(noteEtudiants.AsNoTracking(), pageNumber ?? 1, pageSize));
        }


        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.NoteEtudiants.Include(n => n.Etudiant).Include(n => n.ParcoursEtude).Include(n => n.Promotion).FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create()
        {
            ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule");
            ViewData["ParcoursEtudiantId"] = new SelectList(_context.ParcoursEtudes, "Id", "Id");
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion"); return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EtudiantId,ParcoursEtudiantId,Note,PromotionId")] NoteEtudiant noteEtudiant)
        {
            if (ModelState.IsValid) { _context.Add(noteEtudiant); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule", noteEtudiant.EtudiantId);
            ViewData["ParcoursEtudiantId"] = new SelectList(_context.ParcoursEtudes, "Id", "Id", noteEtudiant.ParcoursEtudiantId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", noteEtudiant.PromotionId);
            return View(noteEtudiant);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.NoteEtudiants.FindAsync(id); if (item == null) return NotFound(); ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule", item.EtudiantId); ViewData["ParcoursEtudiantId"] = new SelectList(_context.ParcoursEtudes, "Id", "Id", item.ParcoursEtudiantId); ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", item.PromotionId); return View(item); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EtudiantId,ParcoursEtudiantId,Note,PromotionId")] NoteEtudiant noteEtudiant)
        {
            if (id != noteEtudiant.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(noteEtudiant); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.NoteEtudiants.Any(e => e.Id == noteEtudiant.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule", noteEtudiant.EtudiantId);
            ViewData["ParcoursEtudiantId"] = new SelectList(_context.ParcoursEtudes, "Id", "Id", noteEtudiant.ParcoursEtudiantId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", noteEtudiant.PromotionId);
            return View(noteEtudiant);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.NoteEtudiants.Include(n => n.Etudiant).Include(n => n.ParcoursEtude).Include(n => n.Promotion).FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.NoteEtudiants.FindAsync(id); if (item != null) _context.NoteEtudiants.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    }
}
