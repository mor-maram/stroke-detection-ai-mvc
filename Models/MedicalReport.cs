namespace MediAI.Models
{
    public class MedicalReport
    {
        public int Id { get; set; }

        [ForeignKey("Diagnosis")]
        public int? DiagnosisId { get; set; }
        public Diagnosis? Diagnosis { get; set; }

        [ForeignKey("Patient")]
        public string? PatientId { get; set; }
        public Patient? Patient { get; set; }

        public DateTime ReportDate { get; set; } = DateTime.Now;
        public string? ReportUrl { get; set; }
        public ReportFormat Format { get; set; } = ReportFormat.PDF;
        public string? Notes { get; set; }
    }

    public enum ReportFormat
    {
        PDF,
        Word,
        Excel
    }
}
