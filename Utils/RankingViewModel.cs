using SystemeNote.Models;

namespace SystemeNote.ViewModels
{
    // Modèle principal pour la page de classement
    public class SemesterRankingViewModel
    {
        public required PlanifSemestre PlanifSemestre { get; set; }
        public required List<StudentRankingRecord> StudentRankings { get; set; }
        public required SemesterStatistics Stats { get; set; }
    }

    // Représente les données pour un étudiant dans le classement
    public class StudentRankingRecord
    {
        public int Rank { get; set; }
        public required Etudiant Etudiant { get; set; }
        public double OverallAverage { get; set; }
        public required string Status { get; set; } // Admis, Ajourné
        public required List<UeGradeRecord> UeGrades { get; set; }
    }

    // Représente la note d'une UE pour un étudiant
    public class UeGradeRecord
    {
        public required string UeCode { get; set; }
        public required string UeName { get; set; }
        public double UeAverage { get; set; }
    }

    // Représente les statistiques globales du semestre
    public class SemesterStatistics
    {
        public int TotalStudents { get; set; }
        public int AdmisCount { get; set; }
        public int AjourneCount { get; set; }
        public double AdmisPercentage => TotalStudents > 0 ? (double)AdmisCount / TotalStudents * 100 : 0;
        public double AjournePercentage => TotalStudents > 0 ? (double)AjourneCount / TotalStudents * 100 : 0;
        public double ClassAverage { get; set; }
        public double GradeVariance { get; set; }
    }
}