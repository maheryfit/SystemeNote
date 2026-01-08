using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
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

        public void OnGet()
        {
            var idValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            EtudiantId = int.TryParse(idValue, out var id) ? id : null;

            Matricule = User.FindFirstValue(ClaimTypes.Name);

            // SQUELETTE: ici tu utiliseras EtudiantId pour requêter la DB
            Dashboard = new EtudiantDashboardViewModel
            {
                NomPlanifSemestreActuel = "Planif S1 2025 (placeholder)",
                TotalUeAdmis = 5,
                TotalUeAjournes = 2,
                MoyenneSemestreActuel = null,
                MoyenneSemestrePrecedent = null,
                EvolutionMoyenne = null,
                BarLabels = new List<string> { "UE1", "UE2", "UE3" },
                BarValues = new List<double> { 4, 5, 6 }
            };
        }
    }
}

