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
            ViewData["PlanifSemestreId"] = new SelectList(_context.PlanifSemestres, "Id", "NomPlanifSemestre");
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
            return RedirectToAction("Index", "Etudiant");
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
            return RedirectToAction("Index", "Promotions");
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
            return RedirectToAction("Index", "Matieres");
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
            var diplomes = await _context.Diplomes.ToDictionaryAsync(m => m.NomDiplome, m => m.Id);
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 2) throw new Exception("Le fichier CSV doit contenir 2 colonne : NomOptionEtude, NomDiplome");
                var nomOptionEtude = cols[0];
                var nomDiplome = cols[1];
                if (string.IsNullOrWhiteSpace(nomOptionEtude)) return;
                if (!diplomes.TryGetValue(nomDiplome, out var diplomeId)) throw new Exception($"Diplôme '{nomDiplome}' non trouvé.");
                var exists = await _context.OptionEtudes.AnyAsync(o => o.NomOptionEtude == nomOptionEtude);
                if (!exists) _context.OptionEtudes.Add(new OptionEtude
                    {
                        NomOptionEtude = nomOptionEtude,
                        DiplomeId = diplomeId,
                        PlanifSemestres = new List<PlanifSemestre>()
                    });
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "OptionEtudes");
        }
        #endregion

        #region UniteEnseignements
        public IActionResult UploadUniteEnseignements() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadUniteEnseignements(IFormFile file)
        {
            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 2) throw new Exception("Le fichier CSV doit contenir 2 colonnes : CodeUniteEnseignement, Credits");
                var code = cols[0];
                if (string.IsNullOrWhiteSpace(code)) return;
                var credits = int.Parse(cols[1]);

                var exists = await _context.UniteEnseignements.AnyAsync(u => u.CodeUniteEnseignement == code);
                if (!exists)
                {
                    _context.UniteEnseignements.Add(new UniteEnseignement { CodeUniteEnseignement = code, Credits = credits, ParcoursEtudes = new List<ParcoursEtude>() });
                }
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "UniteEnseignements");
        }
        #endregion

        #region ParcoursEtudes
        public IActionResult UploadParcoursEtudes() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadParcoursEtudes(IFormFile file, bool createMissingMatieres)
        {
            var matieres = await _context.Matieres.ToDictionaryAsync(m => m.CodeMatiere, m => m.Id);
            var newMatieres = new Dictionary<string, Matiere>(); // Cache pour les nouvelles matières créées dans ce lot

            var ues = await _context.UniteEnseignements.ToDictionaryAsync(u => u.CodeUniteEnseignement, u => u.Id);
            var planifs = await _context.PlanifSemestres.ToDictionaryAsync(p => p.NomPlanifSemestre, p => p.Id);

            var errors = new List<string>();
            var addedParcoursKeys = new HashSet<string>(); // Pour éviter les doublons dans le fichier CSV lui-même

            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 3) return;
                var codeMatiere = cols[0];
                var codeUE = cols[1];
                var nomPlanif = cols[2];

                // Vérifier si cette combinaison a déjà été traitée dans ce fichier
                var key = $"{codeMatiere}|{codeUE}|{nomPlanif}";
                if (addedParcoursKeys.Contains(key)) return;

                var missing = new List<string>();

                int? matiereId = null;
                Matiere? newMatiereEntity = null;

                if (matieres.TryGetValue(codeMatiere, out var existingId))
                {
                    matiereId = existingId;
                }
                else if (createMissingMatieres)
                {
                    // Vérifier si on l'a déjà créée dans ce lot
                    if (newMatieres.TryGetValue(codeMatiere, out var m))
                    {
                        newMatiereEntity = m;
                    }
                    else
                    {
                        newMatiereEntity = new Matiere
                        {
                            CodeMatiere = codeMatiere,
                            NomMatiere = codeMatiere, // On utilise le code comme nom par défaut
                            ParcoursEtudes = new List<ParcoursEtude>()
                        };
                        _context.Matieres.Add(newMatiereEntity);
                        newMatieres[codeMatiere] = newMatiereEntity;
                    }
                }
                else
                {
                    missing.Add($"Matière '{codeMatiere}'");
                }

                if (!ues.TryGetValue(codeUE, out var ueId)) missing.Add($"UE '{codeUE}'");
                if (!planifs.TryGetValue(nomPlanif, out var planifId)) missing.Add($"Planif '{nomPlanif}'");

                if (missing.Any())
                {
                    errors.Add($"Ligne ignorée ({codeMatiere}/{codeUE}) : Introuvable -> {string.Join(", ", missing)}");
                    return;
                }

                // Si c'est une nouvelle matière, le parcours n'existe pas encore en base.
                // Si c'est une matière existante, on vérifie en base.
                bool exists = false;
                if (matiereId.HasValue)
                {
                    exists = await _context.ParcoursEtudes.AnyAsync(p => p.MatiereId == matiereId.Value && p.UniteEnseignementId == ueId && p.PlanifSemestreId == planifId);
                }

                if (!exists)
                {
                    var parcours = new ParcoursEtude
                    {
                        UniteEnseignementId = ueId,
                        PlanifSemestreId = planifId,
                        NoteEtudiants = new List<NoteEtudiant>()
                    };

                    if (matiereId.HasValue)
                    {
                        parcours.MatiereId = matiereId.Value;
                    }
                    else
                    {
                        parcours.Matiere = newMatiereEntity!;
                    }

                    _context.ParcoursEtudes.Add(parcours);
                    addedParcoursKeys.Add(key);
                }
            });

            if (errors.Any())
            {
                result += "<br/><div class='text-danger mt-2'><strong>Erreurs rencontrées :</strong><br/>" + string.Join("<br/>", errors.Take(20));
                if (errors.Count > 20) result += $"<br/>... et {errors.Count - 20} autres erreurs.";
                result += "</div>";
            }

            TempData["Message"] = result;
            return RedirectToAction("Index","ParcoursEtudes");
        }
        #endregion

        #region NoteEtudiants
        public IActionResult UploadNoteEtudiants() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadNoteEtudiants(IFormFile file, bool createMissingStudents)
        {
            var etudiants = await _context.Etudiants.ToDictionaryAsync(e => e.Matricule, e => e.Id);
            var newEtudiants = new Dictionary<string, Etudiant>();

            int? defaultAdminId = null;
            if (createMissingStudents)
            {
                var admin = await _context.Administrateurs.FirstOrDefaultAsync();
                if (admin != null) defaultAdminId = admin.Id;
            }

            // Création d'un dictionnaire pour rechercher rapidement les ParcoursEtudes
            // Clé composite : "CodeMatiere|CodeUE|NomPlanif"
            var parcoursMap = await _context.ParcoursEtudes
               .Include(p => p.Matiere)
               .Include(p => p.UniteEnseignement)
               .Include(p => p.PlanifSemestre)
               .ToDictionaryAsync(p => $"{p.Matiere!.CodeMatiere}|{p.UniteEnseignement!.CodeUniteEnseignement}|{p.PlanifSemestre!.NomPlanifSemestre}", p => p);

            var errors = new List<string>();

            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 5) return;
                var matricule = cols[0];
                var codeMatiere = cols[1];
                var codeUE = cols[2];
                var nomPlanif = cols[3];
                if (!double.TryParse(cols[4], NumberStyles.Any, CultureInfo.InvariantCulture, out var note)) return;

                if (note < 0 || note > 20)
                {
                    errors.Add($"Note invalide ({note}) pour {matricule} : doit être entre 0 et 20.");
                    return;
                }

                var key = $"{codeMatiere}|{codeUE}|{nomPlanif}";
                if (!parcoursMap.TryGetValue(key, out var parcours))
                {
                    errors.Add($"Parcours introuvable pour : {codeMatiere} / {codeUE} / {nomPlanif}");
                    return;
                }

                int? etudiantId = null;
                Etudiant? newEtudiant = null;

                if (etudiants.TryGetValue(matricule, out var existingId))
                {
                    etudiantId = existingId;
                }
                else if (createMissingStudents && defaultAdminId.HasValue)
                {
                    if (newEtudiants.TryGetValue(matricule, out var cached))
                    {
                        newEtudiant = cached;
                    }
                    else
                    {
                        newEtudiant = new Etudiant
                        {
                            Matricule = matricule,
                            Nom = matricule, // Valeur par défaut
                            Prenom = "Inconnu", // Valeur par défaut
                            DateNaissance = DateTime.Now,
                            Genre = 1,
                            IsActif = true,
                            DateAdmission = DateTime.Now,
                            AdministrateurId = defaultAdminId.Value,
                            PlanifSemestreId = parcours.PlanifSemestreId,
                            PromotionId = parcours.PlanifSemestre!.PromotionId,
                            MotDePasse = "password123"
                        };
                        _context.Etudiants.Add(newEtudiant);
                        newEtudiants[matricule] = newEtudiant;
                    }
                }
                else
                {
                    errors.Add($"Étudiant inconnu : {matricule}");
                    return;
                }

                bool exists = false;
                if (etudiantId.HasValue)
                {
                    exists = await _context.NoteEtudiants.AnyAsync(n => n.EtudiantId == etudiantId.Value && n.ParcoursEtudiantId == parcours.Id);
                }

                if (!exists)
                {
                    var noteEtudiant = new NoteEtudiant
                    {
                        ParcoursEtudiantId = parcours.Id,
                        Note = note,
                        PromotionId = parcours.PlanifSemestre!.PromotionId
                    };

                    if (etudiantId.HasValue) noteEtudiant.EtudiantId = etudiantId.Value;
                    else noteEtudiant.Etudiant = newEtudiant!;

                    _context.NoteEtudiants.Add(noteEtudiant);
                }
            });

            if (errors.Any())
            {
                result += "<br/><div class='text-danger mt-2'><strong>Erreurs rencontrées :</strong><br/>" + string.Join("<br/>", errors.Take(20));
                if (errors.Count > 20) result += $"<br/>... et {errors.Count - 20} autres erreurs.";
                result += "</div>";
            }

            TempData["Message"] = result;
            return RedirectToAction("Index", "NoteEtudiants");
        }
        #endregion

        #region PlanifSemestres
        public IActionResult UploadPlanifSemestres() => View();
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPlanifSemestres(IFormFile file)
        {
            var semestres = await _context.Semestres.ToDictionaryAsync(s => s.CodeSemestre, s => s.Id);
            var options = await _context.OptionEtudes.ToDictionaryAsync(o => o.NomOptionEtude, o => o.Id);
            var promotions = await _context.Promotions.ToDictionaryAsync(p => p.CodePromotion, p => p.Id);

            var result = await UploadHelper.ProcessUpload(file, _context, async (cols) =>
            {
                if (cols.Length < 7) throw new Exception("Le fichier CSV doit contenir 7 colonnes : NomPlanifSemestre, DateDebut, DateFin, CodeSemestre, NomOptionEtude, TotalCredit, CodePromotion");
                var nomPlanif = cols[0];
                var dateDebut = ParseDate(cols[1]);
                var dateFin = ParseDate(cols[2]);
                var codeSemestre = cols[3];
                var nomOption = cols[4];
                var totalCredit = ParseInt(cols[5]);
                var codePromotion = cols[6];
                if (!semestres.TryGetValue(codeSemestre, out var semestreId)) throw new Exception($"Semestre '{codeSemestre}' non trouvé.");
                if (!options.TryGetValue(nomOption, out var optionId)) throw new Exception($"Option d'étude '{nomOption}' non trouvée.");
                if (!promotions.TryGetValue(codePromotion, out var promotionId)) throw new Exception($"Promotion '{codePromotion}' non trouvée.");
                var exists = await _context.PlanifSemestres.AnyAsync(p => p.NomPlanifSemestre == nomPlanif);
                if (!exists)
                {
                    _context.PlanifSemestres.Add(new PlanifSemestre
                    {
                        NomPlanifSemestre = nomPlanif,
                        SemestreId = semestreId,
                        OptionEtudeId = optionId,
                        TotalCredit = totalCredit,
                        PromotionId = promotionId,
                        DateDebut = dateDebut,
                        DateFin = dateFin
                    });
                }
            });
            TempData["Message"] = result;
            return RedirectToAction("Index", "PlanifSemestres");
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