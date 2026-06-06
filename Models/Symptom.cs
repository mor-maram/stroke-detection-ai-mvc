namespace MediAI.Models
{
    public class Symptom
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string? Name { get; set; }
        
        public Severity Severity { get; set; }
        public string? Description { get; set; }

        public ICollection<DiagnosisSymptom>? DiagnosisSymptoms { get; set; }
    }

    public enum Severity
    {
        Mild,
        Moderate,
        Severe
    }
}
