using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class UniteEnseignementsController : Controller
    {
        private readonly AppDbContext _context;
        public UniteEnseignementsController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.UniteEnseignements.ToListAsync());

        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.UniteEnseignements.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodeUniteEnseignement,Credits")] UniteEnseignement ue)
        {
            if (ModelState.IsValid) { _context.Add(ue); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(ue);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.UniteEnseignements.FindAsync(id); if (item == null) return NotFound(); return View(item); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodeUniteEnseignement,Credits")] UniteEnseignement ue)
        {
            if (id != ue.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(ue); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.UniteEnseignements.Any(e => e.Id == ue.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            return View(ue);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.UniteEnseignements.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.UniteEnseignements.FindAsync(id); if (item != null) _context.UniteEnseignements.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    }
}
