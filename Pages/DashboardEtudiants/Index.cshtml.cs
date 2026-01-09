using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
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

            Dashboard = new EtudiantDashboardViewModel
            {
                NomPlanifSemestreActuel = await GetPlanifSemestreNameAsync(PlanifSemestreId),
                TotalUeAdmis = 5,
                TotalUeAjournes = 2,
                MoyenneSemestreActuel = null,
                MoyenneSemestrePrecedent = null,
                EvolutionMoyenne = null,
                BarLabels = new List<string> { "UE1", "UE2", "UE3" },
                BarValues = new List<double> { 4, 5, 6 }
            };
        }

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

