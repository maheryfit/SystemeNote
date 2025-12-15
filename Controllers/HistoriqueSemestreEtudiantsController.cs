using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class HistoriqueSemestreEtudiantsController : Controller
    {
        private readonly AppDbContext _context;
        public HistoriqueSemestreEtudiantsController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.HistoriqueSemestreEtudiants.Include(h => h.Etudiant).Include(h => h.PlanifSemestre).ToListAsync());

        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.HistoriqueSemestreEtudiants.Include(h => h.Etudiant).Include(h => h.PlanifSemestre).FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create() { ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule"); ViewData["PlanifSemetreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre"); return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EtudiantId,PlanifSemetreId,DateDebut,DateFin")] HistoriqueSemestreEtudiant histor)
        {
            if (ModelState.IsValid) { _context.Add(histor); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule", histor.EtudiantId);
            ViewData["PlanifSemetreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", histor.PlanifSemetreId);
            return View(histor);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.HistoriqueSemestreEtudiants.FindAsync(id); if (item == null) return NotFound(); ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule", item.EtudiantId); ViewData["PlanifSemetreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", item.PlanifSemetreId); return View(item); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,EtudiantId,PlanifSemetreId,DateDebut,DateFin")] HistoriqueSemestreEtudiant histor)
        {
            if (id != histor.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(histor); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.HistoriqueSemestreEtudiants.Any(e => e.Id == histor.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule", histor.EtudiantId);
            ViewData["PlanifSemetreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", histor.PlanifSemetreId);
            return View(histor);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.HistoriqueSemestreEtudiants.Include(h => h.Etudiant).Include(h => h.PlanifSemestre).FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.HistoriqueSemestreEtudiants.FindAsync(id); if (item != null) _context.HistoriqueSemestreEtudiants.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    }
}
