using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class ConfigsController : Controller
    {
        private readonly AppDbContext _context;
        public ConfigsController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.Configs.ToListAsync());

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
    }
}
