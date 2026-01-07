using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SystemeNote.ViewModels;
using System.Text.Json;

namespace SystemeNote.Pages.DashboardEtudiants
{
    public class IndexModel : PageModel
    {
        private readonly string _connectionString;

        public IndexModel(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string not found.");
        }

        public EtudiantDashboardViewModel Dashboard { get; private set; } = new();

        public void OnGet()
        {
            // SQUELETTE: remplacer par vos requêtes (ADO.NET ou EF)
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

