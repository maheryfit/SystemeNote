using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public class UniteEnseignementsController : Controller
    {
        private readonly AppDbContext _context;

        public UniteEnseignementsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: UniteEnseignements
        public async Task<IActionResult> Index(string sortOrder, string searchString, int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["CodeSortParm"] = String.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
            ViewData["CreditSortParm"] = sortOrder == "credit" ? "credit_desc" : "credit";
            ViewData["CurrentFilter"] = searchString;
            ViewData["Title"] = "Unités d'enseignement";

            var uniteEnseignements = from u in _context.UniteEnseignements
                                     select u;

            if (!String.IsNullOrEmpty(searchString))
            {
                uniteEnseignements = uniteEnseignements.Where(u => u.CodeUniteEnseignement.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "code_desc":
                    uniteEnseignements = uniteEnseignements.OrderByDescending(u => u.CodeUniteEnseignement);
                    break;
                case "credit":
                    uniteEnseignements = uniteEnseignements.OrderBy(u => u.Credits);
                    break;
                case "credit_desc":
                    uniteEnseignements = uniteEnseignements.OrderByDescending(u => u.Credits);
                    break;
                default:
                    uniteEnseignements = uniteEnseignements.OrderBy(u => u.CodeUniteEnseignement);
                    break;
            }

            int pageSize = 10;
            return View(await PaginatedList<UniteEnseignement>.CreateAsync(uniteEnseignements.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        // GET: UniteEnseignements/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var uniteEnseignement = await _context.UniteEnseignements.FirstOrDefaultAsync(m => m.Id == id);
            if (uniteEnseignement == null) return NotFound();
            return View(uniteEnseignement);
        }

        // GET: UniteEnseignements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: UniteEnseignements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CodeUniteEnseignement,Credits")] UniteEnseignement uniteEnseignement)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(uniteEnseignement);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(uniteEnseignement);
        }

        // GET: UniteEnseignements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var uniteEnseignement = await _context.UniteEnseignements.FindAsync(id);
            if (uniteEnseignement == null) return NotFound();
            return View(uniteEnseignement);
        }

        // POST: UniteEnseignements/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CodeUniteEnseignement,Credits")] UniteEnseignement uniteEnseignement)
        {
            if (id != uniteEnseignement.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try { _context.Update(uniteEnseignement); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!UniteEnseignementExists(uniteEnseignement.Id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            return View(uniteEnseignement);
        }

        // GET: UniteEnseignements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var uniteEnseignement = await _context.UniteEnseignements.FirstOrDefaultAsync(m => m.Id == id);
            if (uniteEnseignement == null) return NotFound();
            return View(uniteEnseignement);
        }

        // POST: UniteEnseignements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var uniteEnseignement = await _context.UniteEnseignements.FindAsync(id);
            if (uniteEnseignement != null) _context.UniteEnseignements.Remove(uniteEnseignement);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UniteEnseignementExists(int id) => _context.UniteEnseignements.Any(e => e.Id == id);
    }
}