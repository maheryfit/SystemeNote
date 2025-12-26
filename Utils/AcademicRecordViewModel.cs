using SystemeNote.Models;

namespace SystemeNote.ViewModels
{
    // Modèle principal pour la page du relevé de notes
    public class AcademicRecordViewModel
    {
        public required Etudiant Etudiant { get; set; }
        public required List<SemesterRecord> Semesters { get; set; }
        public int TotalCreditsObtained { get; set; }
    }

    // Représente les données pour un seul semestre
    public class SemesterRecord
    {
        public required string SemesterName { get; set; }
        public required string Status { get; set; } // "Validé" ou "Ajourné"
        public int CreditsObtained { get; set; }
        public int TotalCredits { get; set; }
        public required List<UeRecord> UEs { get; set; }
    }

    // Représente les données pour une Unité d'Enseignement (UE)
    public class UeRecord
    {
        public required string UeCode { get; set; }
        public int Credits { get; set; }
        public double UeAverage { get; set; }
        public required List<MatiereRecord> Matieres { get; set; }
    }

    // Représente les données pour une matière dans une UE
    public class MatiereRecord
    {
        public required string MatiereName { get; set; }
        public double? Note { get; set; }
    }
}