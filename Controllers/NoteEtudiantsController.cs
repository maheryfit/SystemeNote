using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class NoteEtudiantsController : Controller
    {
        private readonly AppDbContext _context;
        public NoteEtudiantsController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.NoteEtudiants.Include(n => n.Etudiant).Include(n => n.ParcoursEtude).Include(n => n.Promotion).ToListAsync());

        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.NoteEtudiants.Include(n => n.Etudiant).Include(n => n.ParcoursEtude).Include(n => n.Promotion).FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create() { ViewData["EtudiantId"] = new SelectList(_context.Etudiants, "Id", "Matricule"); ViewData["ParcoursEtudiantId"] = new SelectList(_context.ParcoursEtudes, "Id", "Id"); ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion"); return View(); }

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
