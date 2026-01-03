using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public class SemestresController : Controller
    {
        private readonly AppDbContext _context;

        public SemestresController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CodeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            ViewData["NameSortParm"] = sortOrder == "name" ? "name_desc" : "name";
            ViewData["DiplomeSortParm"] = sortOrder == "diplome" ? "diplome_desc" : "diplome";

            ViewData["CurrentFilter"] = searchString;
            ViewData["Title"] = "Liste des Semestres";

            var semestres = from s in _context.Semestres.Include(s => s.Diplome)
                            select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                semestres = semestres.Where(s => s.NomSemestre.Contains(searchString) || s.CodeSemestre.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "code_desc":
                    semestres = semestres.OrderByDescending(s => s.CodeSemestre);
                    break;
                case "name":
                    semestres = semestres.OrderBy(s => s.NomSemestre);
                    break;
                case "name_desc":
                    semestres = semestres.OrderByDescending(s => s.NomSemestre);
                    break;
                case "diplome":
                    semestres = semestres.OrderBy(s => s.Diplome!.NomDiplome);
                    break;
                case "diplome_desc":
                    semestres = semestres.OrderByDescending(s => s.Diplome!.NomDiplome);
                    break;
                default: // CodeSemestre ascending by default
                    semestres = semestres.OrderBy(s => s.CodeSemestre);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Semestre>.CreateAsync(semestres.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var semestre = await _context.Semestres
                .Include(s => s.Diplome)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (semestre == null) return NotFound();
            return View(semestre);
        }

        public IActionResult Create()
        {
            ViewData["DiplomeId"] = new SelectList(_context.Diplomes, "Id", "NomDiplome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodeSemestre,NomSemestre,DiplomeId")] Semestre semestre)
        {
            Console.WriteLine(semestre.CodeSemestre, semestre.NomSemestre);
            if (ModelState.IsValid)
            {
                _context.Add(semestre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiplomeId"] = new SelectList(_context.Diplomes, "Id", "NomDiplome", semestre.DiplomeId);
            return View(semestre);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var semestre = await _context.Semestres.FindAsync(id);
            if (semestre == null) return NotFound();
            ViewData["DiplomeId"] = new SelectList(_context.Diplomes, "Id", "NomDiplome", semestre.DiplomeId);
            return View(semestre);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodeSemestre,NomSemestre,DiplomeId")] Semestre semestre)
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
