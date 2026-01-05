using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public partial class AdministrateursController : Controller
    {
        private readonly AppDbContext _context;
        public AdministrateursController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NomSortParm"] = String.IsNullOrEmpty(sortOrder) ? "nom_desc" : "";
            ViewData["PrenomSortParm"] = sortOrder == "prenom" ? "prenom_desc" : "prenom";

            ViewData["CurrentFilter"] = searchString;
            ViewData["Title"] = "Liste des Administrateurs";

            var administrateurs = from a in _context.Administrateurs
                                  select a;

            if (!String.IsNullOrEmpty(searchString))
            {
                administrateurs = administrateurs.Where(a =>
                    a.NomAdmin.Contains(searchString) ||
                    a.PrenomAdmin.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "nom_desc":
                    administrateurs = administrateurs.OrderByDescending(a => a.NomAdmin);
                    break;
                case "prenom":
                    administrateurs = administrateurs.OrderBy(a => a.PrenomAdmin);
                    break;
                case "prenom_desc":
                    administrateurs = administrateurs.OrderByDescending(a => a.PrenomAdmin);
                    break;
                default:
                    administrateurs = administrateurs.OrderBy(a => a.NomAdmin);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Administrateur>.CreateAsync(administrateurs.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.Administrateurs.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomAdmin,PrenomAdmin")] Administrateur admin)
        {
            if (ModelState.IsValid)
            {

                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(admin);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.Administrateurs.FindAsync(id); if (item == null) return NotFound(); return View(item); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomAdmin,PrenomAdmin")] Administrateur admin)
        {
            if (id != admin.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(admin); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.Administrateurs.Any(e => e.Id == admin.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            return View(admin);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.Administrateurs.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.Administrateurs.FindAsync(id); if (item != null) _context.Administrateurs.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    }
}

