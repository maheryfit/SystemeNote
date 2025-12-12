using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;

namespace SystemeNote.Controllers
{
    public class EtudiantController : Controller
    {
        private readonly AppDbContext _context;

        public EtudiantController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Etudiant
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.Etudiants.Include(e => e.Administrateur).Include(e => e.Promotion).Include(e => e.PlanifSemestre);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Etudiant/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var etudiant = await _context.Etudiants
                .Include(e => e.Administrateur)
                .Include(e => e.Promotion)
                .Include(e => e.PlanifSemestre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (etudiant == null) return NotFound();

            return View(etudiant);
        }

        // GET: Etudiant/Create
        public IActionResult Create()
        {
            ViewData["AdministrateurId"] = new SelectList(_context.Administrateurs, "Id", "NomAdmin");
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion");
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre");
            return View();
        }

        // POST: Etudiant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Matricule,Nom,Prenom,DateNaissance,PromotionId,Genre,IsActif,DateAdmission,AdministrateurId,PlanifSemestreId,MotDePasse")] Etudiant etudiant)
        {
            if (ModelState.IsValid)
            {
                _context.Add(etudiant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdministrateurId"] = new SelectList(_context.Administrateurs, "Id", "NomAdmin", etudiant.AdministrateurId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", etudiant.PromotionId);
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", etudiant.PlanifSemestreId);
            return View(etudiant);
        }

        // GET: Etudiant/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant == null) return NotFound();
            ViewData["AdministrateurId"] = new SelectList(_context.Administrateurs, "Id", "NomAdmin", etudiant.AdministrateurId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", etudiant.PromotionId);
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", etudiant.PlanifSemestreId);
            return View(etudiant);
        }

        // POST: Etudiant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Matricule,Nom,Prenom,DateNaissance,PromotionId,Genre,IsActif,DateAdmission,AdministrateurId,PlanifSemestreId,MotDePasse")] Etudiant etudiant)
        {
            if (id != etudiant.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(etudiant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EtudiantExists(etudiant.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AdministrateurId"] = new SelectList(_context.Administrateurs, "Id", "NomAdmin", etudiant.AdministrateurId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", etudiant.PromotionId);
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre", etudiant.PlanifSemestreId);
            return View(etudiant);
        }

        // GET: Etudiant/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var etudiant = await _context.Etudiants
                .Include(e => e.Administrateur)
                .Include(e => e.Promotion)
                .Include(e => e.PlanifSemestre)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (etudiant == null) return NotFound();

            return View(etudiant);
        }

        // POST: Etudiant/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant != null) _context.Etudiants.Remove(etudiant);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EtudiantExists(int id)
        {
            return _context.Etudiants.Any(e => e.Id == id);
        }
    }
}