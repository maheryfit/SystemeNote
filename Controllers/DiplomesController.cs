using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public class DiplomesController : Controller
    {
        private readonly AppDbContext _context;

        public DiplomesController(AppDbContext context)
        {
            _context = context;
        }


        // GET: Diplomes
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";

            ViewData["CurrentFilter"] = searchString;
            ViewData["Title"] = "Liste des Diplômes";

            var diplomes = from d in _context.Diplomes
                           select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                diplomes = diplomes.Where(d => d.NomDiplome.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    diplomes = diplomes.OrderByDescending(d => d.NomDiplome);
                    break;
                default:
                    diplomes = diplomes.OrderBy(d => d.NomDiplome);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Diplome>.CreateAsync(diplomes.AsNoTracking(), pageNumber ?? 1, pageSize));
        }



        // GET: Diplomes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diplome = await _context.Diplomes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diplome == null)
            {
                return NotFound();
            }

            return View(diplome);
        }

        // GET: Diplomes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diplomes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomDiplome")] Diplome diplome)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diplome);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diplome);
        }

        // GET: Diplomes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diplome = await _context.Diplomes.FindAsync(id);
            if (diplome == null)
            {
                return NotFound();
            }
            return View(diplome);
        }

        // POST: Diplomes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomDiplome")] Diplome diplome)
        {
            if (id != diplome.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diplome);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiplomeExists(diplome.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(diplome);
        }

        // GET: Diplomes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var diplome = await _context.Diplomes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diplome == null)
            {
                return NotFound();
            }

            return View(diplome);
        }

        // POST: Diplomes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var diplome = await _context.Diplomes.FindAsync(id);
            if (diplome != null)
            {
                _context.Diplomes.Remove(diplome);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiplomeExists(int id)
        {
            return _context.Diplomes.Any(e => e.Id == id);
        }
        // POST: Diplomes/Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 1) throw new Exception("Le fichier CSV doit contenir 1 colonne : NomDiplome");
                var nomDiplome = cols[0];
                if (string.IsNullOrWhiteSpace(nomDiplome)) return;

                var exists = await _context.Diplomes.AnyAsync(d => d.NomDiplome == nomDiplome);
                if (!exists)
                {
                    _context.Diplomes.Add(new Diplome { NomDiplome = nomDiplome });
                }
            });

            TempData["Message"] = result;
            return RedirectToAction(nameof(Index));
        }
    }
}

