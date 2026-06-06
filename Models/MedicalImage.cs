namespace MediAI.Models
{
    public class MedicalImage
    {
        public int Id { get; set; }

        [Required]
        public string? ImageUrl { get; set; }

        [StringLength(50)]
        public string? ImageType { get; set; } // MRI, X-Ray, etc.

        public DateTime UploadDate { get; set; } = DateTime.Now;

        [ForeignKey("Diagnosis")]
        public int? DiagnosisId { get; set; }
        public Diagnosis? Diagnosis { get; set; }
    }
}
