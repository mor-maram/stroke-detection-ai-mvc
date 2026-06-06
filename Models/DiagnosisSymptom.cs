namespace MediAI.Models
{
    public class DiagnosisSymptom
    {
        public int? DiagnosisId { get; set; }
        public Diagnosis? Diagnosis { get; set; }

        public int? SymptomId { get; set; }
        public Symptom? Symptom { get; set; }
    }
}
