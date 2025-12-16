using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class OptionEtudesController : Controller
    {
        private readonly AppDbContext _context;

        public OptionEtudesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: OptionEtudes
        public async Task<IActionResult> Index()
        {
            return View(await _context.OptionEtudes.ToListAsync());
        }

        // GET: OptionEtudes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionEtude = await _context.OptionEtudes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (optionEtude == null)
            {
                return NotFound();
            }

            return View(optionEtude);
        }

        // GET: OptionEtudes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: OptionEtudes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomOptionEtude")] OptionEtude optionEtude)
        {
            if (ModelState.IsValid)
            {
                _context.Add(optionEtude);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(optionEtude);
        }

        // GET: OptionEtudes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var optionEtude = await _context.OptionEtudes.FindAsync(id);
            if (optionEtude == null)
            {
                return NotFound();
            }
            return View(optionEtude);
        }

        // POST: OptionEtudes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomOption")] OptionEtude optionEtude)
        {
            if (id != optionEtude.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try { _context.Update(optionEtude); await _context.SaveChangesAsync(); }
                catch (DbUpdateConcurrencyException) { if (!OptionEtudeExists(optionEtude.Id)) return NotFound(); else throw; }
                return RedirectToAction(nameof(Index));
            }
            return View(optionEtude);
        }

        // GET: OptionEtudes/Delete/5
        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var optionEtude = await _context.OptionEtudes.FirstOrDefaultAsync(m => m.Id == id); if (optionEtude == null) return NotFound(); return View(optionEtude); }

        // POST: OptionEtudes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var optionEtude = await _context.OptionEtudes.FindAsync(id); if (optionEtude != null) _context.OptionEtudes.Remove(optionEtude); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }

        private bool OptionEtudeExists(int id) { return _context.OptionEtudes.Any(e => e.Id == id); }
    }
}