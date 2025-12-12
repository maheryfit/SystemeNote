using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemNote.Models;

namespace SystemeNote.Controllers
{
    public class MatieresController : Controller
    {
        private readonly AppDbContext _context;
        public MatieresController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.Matieres.ToListAsync());

        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.Matieres.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomMatiere,CodeMatiere")] Matiere matiere)
        {
            if (ModelState.IsValid) { _context.Add(matiere); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(matiere);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.Matieres.FindAsync(id); if (item == null) return NotFound(); return View(item); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomMatiere,CodeMatiere")] Matiere matiere)
        {
            if (id != matiere.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(matiere); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.Matieres.Any(e => e.Id == matiere.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            return View(matiere);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.Matieres.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.Matieres.FindAsync(id); if (item != null) _context.Matieres.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    }
}
