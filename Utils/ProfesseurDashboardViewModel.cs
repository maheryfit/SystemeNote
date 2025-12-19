using System.Collections.Generic;
namespace SystemeNote.ViewModels
{
    public class ProfesseurDashboardViewModel
    {
        public List<SemestreStats> SemestreStats { get; set; } = new List<SemestreStats>();
        public List<PromotionProgress> PromotionProgress { get; set; } = new List<PromotionProgress>();
    }

    public class SemestreStats
    {
        public int PlanifSemestreId { get; set; }
        public string NomPlanifSemestre { get; set; } = "";
        public int TotalEtudiants { get; set; }
        public int Admis { get; set; }
        public int Ajournes { get; set; }
        public double PourcentageAdmis { get; set; }
        public double PourcentageAjournes { get; set; }
    }

    public class PromotionProgress
    {
        public int PromotionId { get; set; }
        public string NomPromotion { get; set; } = "";
        public int AdmisCurrent { get; set; }
        public int AdmisPrevious { get; set; }
        public double PourcentageProgression { get; set; }
    }
}
