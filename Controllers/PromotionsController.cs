        using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly AppDbContext _context;

        public PromotionsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Promotions
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewData["CodeSortParm"] = sortOrder == "code" ? "code_desc" : "code";
            ViewData["DiplomeSortParm"] = sortOrder == "diplome" ? "diplome_desc" : "diplome";
            ViewData["DateSortParm"] = sortOrder == "date" ? "date_desc" : "date";

            ViewData["CurrentFilter"] = searchString;
            ViewData["Title"] = "Liste des Promotions";

            var promotions = from p in _context.Promotions.Include(p => p.Diplome)
                             select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                promotions = promotions.Where(p =>
                    p.NomPromotion.Contains(searchString) ||
                    p.CodePromotion.Contains(searchString) ||
                    p.Diplome!.NomDiplome.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    promotions = promotions.OrderByDescending(p => p.NomPromotion);
                    break;
                case "code":
                    promotions = promotions.OrderBy(p => p.CodePromotion);
                    break;
                case "code_desc":
                    promotions = promotions.OrderByDescending(p => p.CodePromotion);
                    break;
                case "diplome":
                    promotions = promotions.OrderBy(p => p.Diplome!.NomDiplome);
                    break;
                case "diplome_desc":
                    promotions = promotions.OrderByDescending(p => p.Diplome!.NomDiplome);
                    break;
                case "date":
                    promotions = promotions.OrderBy(p => p.DateCreation);
                    break;
                case "date_desc":
                    promotions = promotions.OrderByDescending(p => p.DateCreation);
                    break;
                default:
                    promotions = promotions.OrderBy(p => p.NomPromotion);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<Promotion>.CreateAsync(promotions.AsNoTracking(), pageNumber ?? 1, pageSize));
        }
        // GET: Promotions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.Promotions
                .Include(p => p.Diplome)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (promotion == null) return NotFound();
            return View(promotion);
        }

        // GET: Promotions/Create
        public IActionResult Create()
        {
            ViewData["DiplomeId"] = new SelectList(_context.Diplomes, "Id", "NomDiplome");
            return View();
        }

        // POST: Promotions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomPromotion,DateCreation,CodePromotion,DiplomeId")] Promotion promotion)
        {
            if (ModelState.IsValid)
            {
                _context.Add(promotion);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiplomeId"] = new SelectList(_context.Diplomes, "Id", "NomDiplome", promotion.DiplomeId);
            return View(promotion);
        }

        // GET: Promotions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null) return NotFound();
            ViewData["DiplomeId"] = new SelectList(_context.Diplomes, "Id", "NomDiplome", promotion.DiplomeId);
            return View(promotion);
        }

        // POST: Promotions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomPromotion,DateCreation,CodePromotion,DiplomeId")] Promotion promotion)
        {
            if (id != promotion.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(promotion);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Promotions.Any(e => e.Id == promotion.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DiplomeId"] = new SelectList(_context.Diplomes, "Id", "NomDiplome", promotion.DiplomeId);
            return View(promotion);
        }

        // GET: Promotions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var promotion = await _context.Promotions.Include(p => p.Diplome).FirstOrDefaultAsync(m => m.Id == id);
            if (promotion == null) return NotFound();
            return View(promotion);
        }

        // POST: Promotions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null) _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}