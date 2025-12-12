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
        public OptionEtudesController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.OptionEtudes.ToListAsync());

        public async Task<IActionResult> Details(int? id) { if (id == null) return NotFound(); var item = await _context.OptionEtudes.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomOptionEtude")] OptionEtude optionEtude)
        {
            if (ModelState.IsValid) { _context.Add(optionEtude); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            return View(optionEtude);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var item = await _context.OptionEtudes.FindAsync(id); if (item == null) return NotFound(); return View(item); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomOptionEtude")] OptionEtude optionEtude)
        {
            if (id != optionEtude.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(optionEtude); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.OptionEtudes.Any(e => e.Id == optionEtude.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            return View(optionEtude);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var item = await _context.OptionEtudes.FirstOrDefaultAsync(m => m.Id == id); if (item == null) return NotFound(); return View(item); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var item = await _context.OptionEtudes.FindAsync(id); if (item != null) _context.OptionEtudes.Remove(item); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    }
}
