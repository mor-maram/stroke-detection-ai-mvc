namespace MediAI.Models
{
    public class Diagnosis
    {
        public int Id { get; set; }

        // بيانات المريض
        [ForeignKey("Patient")]
        public string? PatientId { get; set; }
        public Patient? Patient { get; set; }
        public string? ImageUrl { get; set; }

        // بيانات الطبيب
        [ForeignKey("Doctor")]
        public string? DoctorId { get; set; }
        public Doctor? Doctor { get; set; }

        // بيانات الطلب
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string? RequestDescription { get; set; }
        public DiagnosisStatus Status { get; set; } = DiagnosisStatus.Pending;
        public PriorityLevel Priority { get; set; } = PriorityLevel.Medium;

        // بيانات النتيجة
        public int? ResultIndex { get;set; }        
        public DateTime? DiagnosisDate { get; set; }
        public string? Condition { get; set; }

        public string? DiagnosisDescription { get; set; }
        public decimal? ConfidenceLevel { get; set; } // نسبة الثقة
        public string? TreatmentPlan { get; set; }
        public string? Notes { get; set; }

        [NotMapped]
        public string PatientName
        {
            get
            {
                if (this.Patient != null)
                    return this.Patient.FullName;
                else return string.Empty;
            }
        }

        [NotMapped]
        public string DoctorName
        {
            get
            {
                if (this.Doctor != null)
                    return this.Doctor.FullName;
                else return string.Empty;
            }
        }
        
        public MedicalReport? MedicalReport { get; set; }

        public ICollection<DiagnosisSymptom>? DiagnosisSymptoms { get; set; }
    }

    public enum DiagnosisStatus
    {
        Pending,
        InProgress,
        Completed,
        NeedsReview,
        Cancelled
    }

    public enum PriorityLevel
    {
        Low,
        Medium,
        High,
        Critical
    }
}