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
    public class PlanifSemestresController : Controller
    {
        private readonly AppDbContext _context;

        public PlanifSemestresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: PlanifSemestres
        public async Task<IActionResult> Index()
        {
            var appDbContext = _context.PlanifSemestres
                                       .Include(p => p.Semestre)
                                       .Include(p => p.Promotion);
            return View(await appDbContext.ToListAsync());
        }

        // GET: PlanifSemestres/Create
        public IActionResult Create()
        {
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre");
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion");
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude");
            return View();
        }

        // POST: PlanifSemestres/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,NomPlanifSemestre,DateDebut,DateFin,SemestreId,PromotionId,OptionEtudeId")] PlanifSemestre planifSemestre)
        {
            if (ModelState.IsValid)
            {
                // Charger explicitement les entités Semestre et Promotion pour satisfaire les propriétés de navigation 'required'
                planifSemestre.Semestre = await _context.Semestres.FindAsync(planifSemestre.SemestreId) ?? throw new InvalidOperationException("Semestre non trouvé.");
                planifSemestre.Promotion = await _context.Promotions.FindAsync(planifSemestre.PromotionId) ?? throw new InvalidOperationException("Promotion non trouvée.");
                planifSemestre.OptionEtude = await _context.OptionEtudes.FindAsync(planifSemestre.OptionEtudeId) ?? throw new InvalidOperationException("Option d'étude non trouvée.");

                _context.Add(planifSemestre);
                // Initialiser les collections requises si elles ne le sont pas déjà dans le constructeur du modèle
                // planifSemestre.ParcoursEtudes = planifSemestre.ParcoursEtudes ?? new List<ParcoursEtude>();
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre", planifSemestre.SemestreId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", planifSemestre.PromotionId);
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude", planifSemestre.OptionEtudeId);
            return View(planifSemestre);
        }

        // Other CRUD actions (Details, Edit, Delete) would go here
    }
}