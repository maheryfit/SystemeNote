using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class ParcoursEtudesController : Controller
    {
        private readonly AppDbContext _context;
        public ParcoursEtudesController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.ParcoursEtudes.Include(p => p.Matiere).Include(p => p.UniteEnseignement).Include(p => p.PlanifSemestre).ToListAsync());

        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.ParcoursEtudes.Include(p => p.Matiere).Include(p => p.UniteEnseignement).Include(p => p.PlanifSemestre).FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create() { ViewData["MatiereId"] = new SelectList(_context.Matieres, "Id", "NomMatiere"); ViewData["UniteEnseignementId"] = new SelectList(_context.UniteEnseignements, "Id", "CodeUniteEnseignement"); ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre"); return View(); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,MatiereId,UniteEnseignementId,PlanifSemestreId")] ParcoursEtude parcoursEtude)
        {
            if (ModelState.IsValid) { _context.Add(parcoursEtude); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            ViewData["MatiereId"] = new SelectList(_context.Matieres, "Id", "NomMatiere", parcoursEtude.MatiereId);
            ViewData["UniteEnseignementId"] = new SelectList(_context.UniteEnseignements, "Id", "CodeUniteEnseignement", parcoursEtude.UniteEnseignementId);
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", parcoursEtude.PlanifSemestreId);
            return View(parcoursEtude);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.ParcoursEtudes.FindAsync(id); if (item == null) return NotFound(); ViewData["MatiereId"] = new SelectList(_context.Matieres, "Id", "NomMatiere", item.MatiereId); ViewData["UniteEnseignementId"] = new SelectList(_context.UniteEnseignements, "Id", "CodeUniteEnseignement", item.UniteEnseignementId); ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", item.PlanifSemestreId); return View(item); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MatiereId,UniteEnseignementId,PlanifSemestreId")] ParcoursEtude parcoursEtude)
        {
            if (id != parcoursEtude.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(parcoursEtude); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.ParcoursEtudes.Any(e => e.Id == parcoursEtude.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            ViewData["MatiereId"] = new SelectList(_context.Matieres, "Id", "NomMatiere", parcoursEtude.MatiereId);
            ViewData["UniteEnseignementId"] = new SelectList(_context.UniteEnseignements, "Id", "CodeUniteEnseignement", parcoursEtude.UniteEnseignementId);
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", parcoursEtude.PlanifSemestreId);
            return View(parcoursEtude);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.ParcoursEtudes.Include(p => p.Matiere).Include(p => p.UniteEnseignement).Include(p => p.PlanifSemestre).FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.ParcoursEtudes.FindAsync(id); if (item != null) _context.ParcoursEtudes.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    }
}
