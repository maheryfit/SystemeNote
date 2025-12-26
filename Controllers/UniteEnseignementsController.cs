using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

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
        public async Task<IActionResult> Index()
        {
            return View(await _context.UniteEnseignements.ToListAsync());
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