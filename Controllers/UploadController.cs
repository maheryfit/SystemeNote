using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SystemeNote.Data;
using SystemeNote.Models;
using SystemeNote.Utils;

namespace SystemeNote.Controllers
{
    public class UploadController : Controller
    {
        private readonly AppDbContext _context;

        public UploadController(AppDbContext context)
        {
            _context = context;
        }

        // GET: /Upload
        public IActionResult Index()
        {
            return View();
        }

        #region Students
        // GET: /Upload/UploadStudents
        public IActionResult UploadStudents()
        {
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion");
            ViewData["AdministrateurId"] = new SelectList(_context.Administrateurs, "Id", "NomAdmin");
            ViewData["PlanifSemestreIdy"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre");
            return View();
        }

        // POST: /Upload/UploadStudents
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadStudents(IFormFile file, int promotionId, int administrateurId, int planifSemestreId)
        {
            var promotion = await _context.Promotions.FindAsync(promotionId) ?? throw new Exception("Promotion non trouvée.");
            var admin = await _context.Administrateurs.FindAsync(administrateurId) ?? throw new Exception("Administrateur non trouvé.");
            var planifSemestre = await _context.PlanifSemestres.FindAsync(planifSemestreId) ?? throw new Exception("Planification de semestre non trouvée.");

            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 8) throw new Exception("Le fichier CSV doit contenir au moins 8 colonnes.");
                var etu = new Etudiant
                {
                    Matricule = cols[0],
                    Nom = cols[1],
                    Prenom = cols[2],
                    DateNaissance = ParseDate(cols[3]),
                    Genre = ParseInt(cols[4]),
                    IsActif = bool.Parse(cols[5]),
                    DateAdmission = ParseDate(cols[6]),
                    MotDePasse = cols[7],
                    PromotionId = promotionId,
                    PlanifSemestreId = planifSemestreId,
                    Promotion = promotion,
                    Administrateur = admin,
                    PlanifSemestre = planifSemestre,
                    AdministrateurId = administrateurId,
                    NoteEtudiants = new List<NoteEtudiant>(),
                    HistoriqueSemestreEtudiants = new List<HistoriqueSemestreEtudiant>()
                };
                _context.Etudiants.Add(etu);
            });

            TempData["Message"] = result;
            return RedirectToAction("Index", "Students");
        }
        #endregion

        #region Administrateurs
        public IActionResult UploadAdministrateurs() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadAdministrateurs(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 2) throw new Exception("Le fichier CSV doit contenir 2 colonnes : NomAdmin, PrenomAdmin");
                var nomAdmin = cols[0];
                var prenomAdmin = cols[1];
                if (string.IsNullOrWhiteSpace(nomAdmin) || string.IsNullOrWhiteSpace(prenomAdmin)) return;
                var exists = await _context.Administrateurs.AnyAsync(a => a.NomAdmin == nomAdmin && a.PrenomAdmin == prenomAdmin);
                if (!exists) _context.Administrateurs.Add(new Administrateur { NomAdmin = nomAdmin, PrenomAdmin = prenomAdmin });
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "Administrateurs");
        }
        #endregion

        #region Diplomes
        public IActionResult UploadDiplomes() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDiplomes(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 1) throw new Exception("Le fichier CSV doit contenir 1 colonne : NomDiplome");
                var nomDiplome = cols[0];
                if (string.IsNullOrWhiteSpace(nomDiplome)) return;
                var exists = await _context.Diplomes.AnyAsync(d => d.NomDiplome == nomDiplome);
                if (!exists) _context.Diplomes.Add(new Diplome { NomDiplome = nomDiplome });
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "Diplomes");
        }
        #endregion

        #region Promotions
        public IActionResult UploadPromotions() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPromotions(IFormFile file)
        {
            var diplomeMap = await _context.Diplomes.ToDictionaryAsync(d => d.NomDiplome, d => d.Id, StringComparer.OrdinalIgnoreCase);
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 4) throw new Exception("CSV must have 4 columns: NomPromotion, DateCreation, CodePromotion, NomDiplome");
                if (!diplomeMap.TryGetValue(cols[3], out var diplomeId)) throw new Exception($"Diplome '{cols[3]}' non trouvé.");
                var exists = await _context.Promotions.AnyAsync(p => p.CodePromotion == cols[2]);
                if (!exists) _context.Promotions.Add(new Promotion { NomPromotion = cols[0], DateCreation = DateOnly.Parse(cols[1]), CodePromotion = cols[2], DiplomeId = diplomeId });
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Matieres
        public IActionResult UploadMatieres() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadMatieres(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 2) throw new Exception("CSV must have 2 columns: NomMatiere, CodeMatiere");
                var nomMatiere = cols[0];
                var codeMatiere = cols[1];
                if (string.IsNullOrWhiteSpace(nomMatiere) || string.IsNullOrWhiteSpace(codeMatiere)) return;
                var exists = await _context.Matieres.AnyAsync(m => m.CodeMatiere == codeMatiere);
                if (!exists) _context.Matieres.Add(new Matiere { NomMatiere = nomMatiere, CodeMatiere = codeMatiere, ParcoursEtudes = new List<ParcoursEtude>() });
            });
            TempData["Message"] = result;
            return RedirectToPage("Index", "Matieres");
        }
        #endregion

        #region Configs
        public IActionResult UploadConfigs() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadConfigs(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 2) throw new Exception("CSV must have 2 columns: Description, Valeur");
                var description = cols[0];
                var valeur = cols[1];
                if (string.IsNullOrWhiteSpace(valeur)) return;
                var exists = await _context.Configs.AnyAsync(c => c.Valeur == valeur);
                if (!exists) _context.Configs.Add(new Config { Description = description, Valeur = valeur });
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "Configs");
        }
        #endregion

        #region Semestres
        public IActionResult UploadSemestres()
        {
            ViewData["DiplomeId"] = new SelectList(_context.Diplomes, "Id", "NomDiplome");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSemestres(IFormFile file, int diplomeId)
        {
            var diplome = await _context.Diplomes.FindAsync(diplomeId);
            if (diplome == null)
            {
                TempData["Message"] = "Erreur : Le diplôme sélectionné n'existe pas.";
                return RedirectToAction(nameof(UploadSemestres));
            }

            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 2) throw new Exception("Le fichier CSV doit contenir 2 colonnes : CodeSemestre, NomSemestre");
                var codeSemestre = cols[0];
                var nomSemestre = cols[1];
                if (string.IsNullOrWhiteSpace(codeSemestre) || string.IsNullOrWhiteSpace(nomSemestre)) return;
                var exists = await _context.Semestres.AnyAsync(s => s.CodeSemestre == codeSemestre);
                if (!exists)
                {
                    _context.Semestres.Add(new Semestre { CodeSemestre = codeSemestre, NomSemestre = nomSemestre, DiplomeId = diplomeId, Diplome = diplome, PlanifSemestres = new List<PlanifSemestre>() });
                }
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "Semestres");
        }
        #endregion

        #region OptionEtudes
        public IActionResult UploadOptionEtudes() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadOptionEtudes(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 1) throw new Exception("Le fichier CSV doit contenir 1 colonne : NomOptionEtude");
                var nomOptionEtude = cols[0];
                if (string.IsNullOrWhiteSpace(nomOptionEtude)) return;

                var exists = await _context.OptionEtudes.AnyAsync(o => o.NomOptionEtude == nomOptionEtude);
                if (!exists) _context.OptionEtudes.Add(new OptionEtude { NomOptionEtude = nomOptionEtude, PlanifSemestres = new List<PlanifSemestre>() });
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "OptionEtudes");
        }
        #endregion


        private static DateTime ParseDate(string s)
        {
            if (DateTime.TryParseExact(s, new[] { "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy", "MM/dd/yyyy" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
                return dt;
            if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dt))
                return dt;
            return DateTime.MinValue;
        }

        private static int ParseInt(string s)
        {
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) ? v : 0;
        }
    }
}