using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SystemeNote.Data;
using SystemeNote.Models;

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
            ViewData["PromotionId"] = new SelectList(_context.Promotions, "Id", "NomPromotion");
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre");
            return View();
        }

        // POST: /Upload
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadFile(IFormFile file, int promotionId, int adminId, int planifSemestreId)
        {
            if (file == null || file.Length == 0)
            {
                ViewBag.Message = "Please select a file.";
                return View();
            }

            var promotion = await _context.Promotions.FindAsync(promotionId);
            var admin = await _context.Administrateurs.FindAsync(adminId);
            var planifSemestre = await _context.PlanifSemestres.FindAsync(planifSemestreId);

            if (promotion == null || planifSemestre == null || admin == null)
            {
                ViewBag.Message = "Invalid Promotion or Semester Plan selected.";
                return RedirectToAction(nameof(Index));
            }

            var imported = 0;
            var errors = 0;


            try
            {
                using var stream = file.OpenReadStream();
                using var reader = new StreamReader(stream);

                // Assume first line is header
                var header = await reader.ReadLineAsync();
                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;

                    // Basic CSV split. For complex CSVs consider using CsvHelper.
                    var cols = line.Split(',').Select(c => c.Trim(' ', '"')).ToArray();

                    try
                    {
                        // Expected columns (in order): Matricule, Nom, Prenom, DateNaissance, Genre, IsActif, DateAdmission, MotDePasse
                        if (cols.Length < 8) throw new Exception("Not enough columns");

                        var etu = new Etudiant
                        {
                            Matricule = cols[0],
                            Nom = cols[1],
                            Prenom = cols[2],
                            DateNaissance = ParseDate(cols[3]),
                            Genre = ParseInt(cols[4]),
                            IsActif = false,
                            DateAdmission = ParseDate(cols[6]),
                            MotDePasse = cols[7],
                            Promotion = promotion,
                            AdministrateurId = ParseInt(cols[8]),
                            Administrateur = admin,
                            PlanifSemestre = planifSemestre,
                            NoteEtudiants = new List<NoteEtudiant>(),
                            HistoriqueSemestreEtudiants = new List<HistoriqueSemestreEtudiant>(),
                        };

                        _context.Etudiants.Add(etu);
                        imported++;
                    }
                    catch
                    {
                        errors++;
                        continue;
                    }
                }

                await _context.SaveChangesAsync();
                ViewBag.Message = $"Import finished. Imported: {imported}. Errors: {errors}.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Import failed: " + ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        private static DateTime ParseDate(string s)
        {
            if (DateTime.TryParseExact(s, new[] { "yyyy-MM-dd", "yyyy/MM/dd", "dd/MM/yyyy", "MM/dd/yyyy", "o", "s" }, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
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