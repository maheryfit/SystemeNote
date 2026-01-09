using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using System.Security.Claims;
using SystemeNote.ViewModels;

namespace SystemeNote.Pages.DashboardEtudiants
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly string _connectionString;

        public IndexModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public EtudiantDashboardViewModel Dashboard { get; private set; } = new();

        public int? EtudiantId { get; private set; }
        public string? Matricule { get; private set; }

        public List<SelectListItem> PlanifSemestreOptions { get; private set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? PlanifSemestreId { get; set; }

        public async Task OnGet()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EtudiantId = int.TryParse(idValue, out var id) ? id : null;

            Matricule = User.FindFirstValue(ClaimTypes.Name);

            if (EtudiantId is null)
            {
                Dashboard = new EtudiantDashboardViewModel { NomPlanifSemestreActuel = "N/A" };
                return;
            }

            PlanifSemestreOptions = await GetPlanifSemestreOptionsAsync(EtudiantId.Value);

            // valeur par défaut: dernière planif où l'étudiant existe (si pas de querystring)
            PlanifSemestreId ??= PlanifSemestreOptions.FirstOrDefault() is { Value: var v } && int.TryParse(v, out var parsed)
                ? parsed
                : null;
            var ues = await GetUesForEtudiantAsync(PlanifSemestreId);
            var listeNotes = await GetNotesForEtudiantAsync(EtudiantId.Value, PlanifSemestreId);
            var moyenneSemestreActuel = await GetMoyennesForEtudiantAsync(EtudiantId.Value, PlanifSemestreId);

            Dashboard = new EtudiantDashboardViewModel
            {
                NomPlanifSemestreActuel = await GetPlanifSemestreNameAsync(PlanifSemestreId),
                TotalUeAdmis = 5,
                TotalUeAjournes = 2,
                MoyenneSemestreActuel = moyenneSemestreActuel,
                MoyenneSemestrePrecedent = null,
                EvolutionMoyenne = null,
                BarLabels = ues.Select(x => x.Nom).ToList(),
                BarValues = listeNotes
            };
        }

        private async Task<decimal> GetMoyennesForEtudiantAsync(int etudiantId, int? planifSemestreId)
        {
            if (planifSemestreId is null)
            {
                return 0;
            }
            const string sql = @"
                WITH NotesUnion AS
                (
                    SELECT
                        parc.unite_enseignement_id,
                        CAST(0 AS decimal(18, 4)) AS note_max,
                        ue.credits AS credits
                    FROM parcours_etude parc
                    INNER JOIN unite_enseignement ue ON parc.unite_enseignement_id = ue.id
                    WHERE parc.planif_semestre_id = @planifSemestreId

                    UNION ALL

                    SELECT
                        pe.unite_enseignement_id,
                        CAST(MAX(ne.note) * ue.credits AS decimal(18, 4)) AS note_max,
                        CAST(ue.credits AS int) AS credits
                    FROM note_etudiant ne
                    INNER JOIN parcours_etude pe ON ne.parcours_etudiant_id = pe.id
                    INNER JOIN unite_enseignement ue ON pe.unite_enseignement_id = ue.id
                    WHERE ne.etudiant_id = @etudiantId
                      AND pe.planif_semestre_id = @planifSemestreId
                    GROUP BY pe.unite_enseignement_id, ue.credits
                ),
                NoteEtudiant AS
                (
                    SELECT
                        unite_enseignement_id,
                        MAX(note_max) AS note_max,
                        MAX(credits) AS credits
                    FROM NotesUnion
                    GROUP BY unite_enseignement_id
                )
                SELECT
                    CAST(SUM(note_max) AS decimal(18, 4)) / NULLIF(CAST(SUM(credits) AS decimal(18, 4)), 0) AS moyenne
                FROM NoteEtudiant;

                SELECT TOP(6) Id, credits FROM unite_enseignement;
            ";
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@planifSemestreId", planifSemestreId.Value);
                    command.Parameters.AddWithValue("@etudiantId", etudiantId);

                    await using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        return reader.GetDecimal(0);
                    }
                }
            }
            return 0;
        }

        private async Task<List<double>> GetNotesForEtudiantAsync(int etudiantId, int? planifSemestreId)
        {
            if (planifSemestreId is null)
            {
                return [];
            }
            const string sql = @"
                WITH NotesUnion AS
                (
                    SELECT
                        parc.unite_enseignement_id,
                        CAST(0 AS int) AS note_max
                    FROM parcours_etude parc
                    WHERE parc.planif_semestre_id = @planifSemestreId

                    UNION ALL

                    SELECT
                        pe.unite_enseignement_id,
                        MAX(ne.note) AS note_max
                    FROM note_etudiant ne
                    INNER JOIN parcours_etude pe ON ne.parcours_etudiant_id = pe.id
                    WHERE ne.etudiant_id = @etudiantId
                      AND pe.planif_semestre_id = @planifSemestreId
                    GROUP BY pe.unite_enseignement_id
                )
                SELECT
                    unite_enseignement_id,
                    MAX(note_max) AS note_max
                FROM NotesUnion
                GROUP BY unite_enseignement_id
                ORDER BY unite_enseignement_id
            ";
            var result = new List<double>();
            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@planifSemestreId", planifSemestreId.Value);
                    command.Parameters.AddWithValue("@etudiantId", etudiantId);

                    await using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.Add(reader.GetDouble(1));
                    }
                }
            }
            return result;
        }

        private async Task<List<UeItem>> GetUesForEtudiantAsync(int? planifSemestreId)
        {
            if (planifSemestreId is null)
            {
                return [];
            }

            const string sql = @"
                SELECT ue.* FROM unite_enseignement ue
                INNER JOIN parcours_etude pe ON pe.unite_enseignement_id = ue.id
                WHERE pe.planif_semestre_id = @planifSemestreId
                ORDER BY ue.id
                ;
            ";

            var result = new List<UeItem>();

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@planifSemestreId", planifSemestreId.Value);
                    await using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        result.Add(new UeItem(reader.GetInt32(0), reader.GetString(1)));
                    }
                }
            }

            return result;
        }

        private sealed record UeItem(int Id, string Nom);

        private async Task<List<SelectListItem>> GetPlanifSemestreOptionsAsync(int etudiantId)
        {
            const string sql = @"
                SELECT ps.id, ps.nom_planif_semestre
                FROM planif_semestre ps
                INNER JOIN historique_semestre_etudiant hse ON hse.planif_semetre_id = ps.id
                WHERE hse.etudiant_id = @etudiantId
                ORDER BY hse.date_debut DESC, ps.id DESC;
            ";

            var items = new List<SelectListItem>();

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@etudiantId", etudiantId);

                    await using var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        var id = reader.GetInt32(0);
                        var nom = reader.GetString(1);

                        items.Add(new SelectListItem
                        {
                            Value = id.ToString(),
                            Text = nom
                        });
                    }
                }
            }

            return items;
        }

        private async Task<string> GetPlanifSemestreNameAsync(int? planifSemestreId)
        {
            if (planifSemestreId is null)
            {
                return "N/A";
            }

            const string sql = @"
                SELECT nom_planif_semestre
                FROM planif_semestre
                WHERE id = @id;
            ";

            await using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                await using (var command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@id", planifSemestreId.Value);

                    var result = await command.ExecuteScalarAsync();
                    return result?.ToString() ?? "N/A";
                }
            }
        }
    }
}

