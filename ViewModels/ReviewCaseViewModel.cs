namespace MediAI.ViewModels
{
    public class ReviewCaseViewModel
    {
        public int DiagnosisId { get; set; }

        public string PatientName { get; set; }
        public string ImageUrl { get; set; }

        public string Condition { get; set; }
        public string DiagnosisDescription { get; set; }
        public decimal? ConfidenceLevel { get; set; }

        public string Notes { get; set; }
        public string TreatmentPlan { get; set; }
        //public decimal? ConfidenceLevel { get; set; }  // نسبة الثقة من الذكاء الاصطناعي       
        //public List<string> ImagePaths { get; set; }        
    }
}
