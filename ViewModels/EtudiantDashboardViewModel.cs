namespace SystemeNote.ViewModels
{
    public class EtudiantDashboardViewModel
    {
        public string? NomPlanifSemestreActuel { get; set; }

        // KPI: UE validées/ajournées (pour la planif actuelle)
        public int TotalUeAdmis { get; set; }
        public int TotalUeAjournes { get; set; }

        // KPI: moyenne semestre actuel
        public double? MoyenneSemestreActuel { get; set; }

        // Progression de moyenne: comparer dernière planif vs actuelle
        public double? MoyenneSemestrePrecedent { get; set; }
        public double? EvolutionMoyenne { get; set; } // (actuel - précédent)

        // Graph (bar)
        public List<string> BarLabels { get; set; } = new();
        public List<double> BarValues { get; set; } = new();
    }
}