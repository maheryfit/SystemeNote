using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class SemestresController : Controller
    {
        private readonly AppDbContext _context;

        public SemestresController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Semestres.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var semestre = await _context.Semestres.FirstOrDefaultAsync(m => m.Id == id);
            if (semestre == null) return NotFound();
            return View(semestre);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomSemestre")] Semestre semestre)
        {
            if (ModelState.IsValid)
            {
                _context.Add(semestre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(semestre);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var semestre = await _context.Semestres.FindAsync(id);
            if (semestre == null) return NotFound();
            return View(semestre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomSemestre")] Semestre semestre)
        {
            if (id != semestre.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(semestre); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!SemestreExists(semestre.Id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            return View(semestre);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var semestre = await _context.Semestres.FirstOrDefaultAsync(m => m.Id == id);
            if (semestre == null) return NotFound();
            return View(semestre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var semestre = await _context.Semestres.FindAsync(id);
            if (semestre != null) _context.Semestres.Remove(semestre);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SemestreExists(int id) => _context.Semestres.Any(e => e.Id == id);
    }
}
