using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public class ConfigsController : Controller
    {
        private readonly AppDbContext _context;
        public ConfigsController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["ValeurSortParm"] = String.IsNullOrEmpty(sortOrder) ? "valeur_desc" : "";
            ViewData["DescriptionSortParm"] = sortOrder == "description" ? "description_desc" : "description";

            ViewData["CurrentFilter"] = searchString;
            ViewData["Title"] = "Liste des Configurations";

            var configs = from c in _context.Configs
                          select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                configs = configs.Where(c =>
                    c.Valeur.Contains(searchString) ||
                    (c.Description != null && c.Description.Contains(searchString)));
            }

            switch (sortOrder)
            {
                case "valeur_desc":
                    configs = configs.OrderByDescending(c => c.Valeur);
                    break;
                case "description":
                    configs = configs.OrderBy(c => c.Description);
                    break;
                case "description_desc":
                    configs = configs.OrderByDescending(c => c.Description);
                    break;
                default:
                    configs = configs.OrderBy(c => c.Valeur);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Config>.CreateAsync(configs.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.Configs.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Description,Valeur")] Config config)
        {
            if (ModelState.IsValid) { _context.Add(config); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(config);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.Configs.FindAsync(id); if (item == null) return NotFound(); return View(item); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Valeur")] Config config)
        {
            if (id != config.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(config); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.Configs.Any(e => e.Id == config.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            return View(config);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.Configs.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.Configs.FindAsync(id); if (item != null) _context.Configs.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }


        #region Configs

        // POST: /Upload/UploadConfigs
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadConfigs(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 2) throw new Exception("CSV must have 2 columns: Description, Valeur");
                var description = cols[0];
                var valeur = cols[1];
                if (string.IsNullOrWhiteSpace(valeur)) return;

                // Assuming 'valeur' should be unique for simplicity. Adjust if needed.
                var exists = await _context.Configs.AnyAsync(c => c.Valeur == valeur);
                if (!exists)
                {
                    _context.Configs.Add(new Config { Description = description, Valeur = valeur });
                }
            });

            TempData["Message"] = result;
            return RedirectToAction(nameof(Index));
        }
        #endregion

    }
}
