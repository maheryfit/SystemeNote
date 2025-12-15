using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class PlanifSemestresController : Controller
    {
        private readonly AppDbContext _context;
        public PlanifSemestresController(AppDbContext context) { _context = context; }

        public async Task<IActionResult> Index() => View(await _context.PlanifSemestres.Include(p => p.Semestre).Include(p => p.OptionEtude).Include(p => p.Promotion).ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var plan = await _context.PlanifSemestres.Include(p => p.Semestre).Include(p => p.OptionEtude).Include(p => p.Promotion).FirstOrDefaultAsync(m => m.Id == id);
            if (plan == null) return NotFound();
            return View(plan);
        }

        public IActionResult Create()
        {
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre");
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude");
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomPlanifSemestre,DateDebut,DateFin,SemestreId,OptionEtudeId,TotalCredit,PromotionId")] PlanifSemestre planifSemestre)
        {
            if (ModelState.IsValid) { _context.Add(planifSemestre); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre", planifSemestre.SemestreId);
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude", planifSemestre.OptionEtudeId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", planifSemestre.PromotionId);
            return View(planifSemestre);
        }

        public async Task<IActionResult> Edit(int? id) { if (id == null) return NotFound(); var plan = await _context.PlanifSemestres.FindAsync(id); if (plan == null) return NotFound(); ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre", plan.SemestreId); ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude", plan.OptionEtudeId); ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", plan.PromotionId); return View(plan); }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomPlanifSemestre,DateDebut,DateFin,SemestreId,OptionEtudeId,TotalCredit,PromotionId")] PlanifSemestre planifSemestre)
        {
            if (id != planifSemestre.Id) return NotFound();
            if (ModelState.IsValid) { try { _context.Update(planifSemestre); await _context.SaveChangesAsync(); } catch (DbUpdateConcurrencyException) { if (!_context.PlanifSemestres.Any(e => e.Id == planifSemestre.Id)) return NotFound(); else throw; } return RedirectToAction(nameof(Index)); }
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre", planifSemestre.SemestreId);
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude", planifSemestre.OptionEtudeId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", planifSemestre.PromotionId);
            return View(planifSemestre);
        }

        public async Task<IActionResult> Delete(int? id) { if (id == null) return NotFound(); var plan = await _context.PlanifSemestres.Include(p => p.Semestre).Include(p => p.OptionEtude).Include(p => p.Promotion).FirstOrDefaultAsync(m => m.Id == id); if (plan == null) return NotFound(); return View(plan); }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id) { var plan = await _context.PlanifSemestres.FindAsync(id); if (plan != null) _context.PlanifSemestres.Remove(plan); await _context.SaveChangesAsync(); return RedirectToAction(nameof(Index)); }
    }
}
