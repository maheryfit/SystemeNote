using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public class MatieresController : Controller
    {
        private readonly AppDbContext _context;
        public MatieresController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CodeSortParm"] = sortOrder == "code" ? "code_desc" : "code";

            ViewData["CurrentFilter"] = searchString;
            ViewData["Title"] = "Liste des Matières";

            var matieres = from m in _context.Matieres
                           select m;

            if (!String.IsNullOrEmpty(searchString))
            {
                matieres = matieres.Where(m =>
                    m.NomMatiere.Contains(searchString) ||
                    m.CodeMatiere.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    matieres = matieres.OrderByDescending(m => m.NomMatiere);
                    break;
                case "code":
                    matieres = matieres.OrderBy(m => m.CodeMatiere);
                    break;
                case "code_desc":
                    matieres = matieres.OrderByDescending(m => m.CodeMatiere);
                    break;
                default:
                    matieres = matieres.OrderBy(m => m.NomMatiere);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Matiere>.CreateAsync(matieres.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

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
    

    
        #region Matieres

        // POST: /Upload/UploadMatieres
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 2) throw new Exception("CSV must have 2 columns: NomMatiere, CodeMatiere");
                var nomMatiere = cols[0];
                var codeMatiere = cols[1];
                if (string.IsNullOrWhiteSpace(nomMatiere) || string.IsNullOrWhiteSpace(codeMatiere)) return;

                var exists = await _context.Matieres.AnyAsync(m => m.CodeMatiere == codeMatiere);
                if (!exists)
                {
                    _context.Matieres.Add(new Matiere { NomMatiere = nomMatiere, CodeMatiere = codeMatiere, ParcoursEtudes = new List<ParcoursEtude>() });
                }
            });

            TempData["Message"] = result;
            return RedirectToAction(nameof(Index));
        }
        #endregion
    }
}
