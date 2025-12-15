using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;
            var promotions = from p in _context.Promotions.Include(p => p.Diplome)
                             select p;
            if (!String.IsNullOrEmpty(searchString))
            {
                promotions = promotions.Where(s => s.NomPromotion.Contains(searchString)
                                       || s.CodePromotion.Contains(searchString));
            }
            return View(await promotions.AsNoTracking().ToListAsync());
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