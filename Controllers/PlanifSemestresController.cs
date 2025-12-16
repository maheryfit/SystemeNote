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
                                       .Include(p => p.Promotion)
                                       .Include(p => p.OptionEtude)
                                       .OrderByDescending(p => p.Id);
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
        public async Task<IActionResult> Create([Bind("Id,NomPlanifSemestre,DateDebut,DateFin,SemestreId,PromotionId,OptionEtudeId,TotalCredit")] PlanifSemestre planifSemestre)
        {
            // --- Début du débogage ---
            Console.WriteLine("--- [POST] PlanifSemestres/Create ---");
            Console.WriteLine($"Nom reçu : {planifSemestre.NomPlanifSemestre}");
            Console.WriteLine($"SemestreId reçu : {planifSemestre.SemestreId}");
            Console.WriteLine($"PromotionId reçu : {planifSemestre.PromotionId}");
            Console.WriteLine($"OptionEtudeId reçu : {planifSemestre.OptionEtudeId}");
            Console.WriteLine($"TotalCredit reçu : {planifSemestre.TotalCredit}");
            // --- Fin du débogage ---
            planifSemestre.ParcoursEtudes = new List<ParcoursEtude>();
            planifSemestre.Etudiants = new List<Etudiant>();
            planifSemestre.HistoriqueSemestreEtudiants = new List<HistoriqueSemestreEtudiant>();


            // Les collections ParcoursEtudes, Etudiants, HistoriqueSemestreEtudiants
            // sont maintenant initialisées dans le constructeur du modèle PlanifSemestre.

            if (ModelState.IsValid)
            {
                Console.WriteLine("Le modèle est VALIDE. Ajout à la base de données.");
                _context.Add(planifSemestre);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("Le modèle est INVALIDE. Retour à la vue.");
            ViewData["SemestreId"] = new SelectList(_context.Semestres, "Id", "NomSemestre", planifSemestre.SemestreId);
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion", planifSemestre.PromotionId);
            ViewData["OptionEtudeId"] = new SelectList(_context.OptionEtudes, "Id", "NomOptionEtude", planifSemestre.OptionEtudeId);
            return View(planifSemestre);
        }

        // Other CRUD actions (Details, Edit, Delete) would go here
        
    }


}