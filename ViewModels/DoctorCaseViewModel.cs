namespace MediAI.ViewModels
{
    public class DoctorCaseViewModel
    {
        public int RequestId { get; set; }
        public string PatientName { get; set; }
        public string AnalysisType { get; set; }
        public string Condition { get; set; }
        public decimal Confidence { get; set; }
        public string Status { get; set; }
    }
}
