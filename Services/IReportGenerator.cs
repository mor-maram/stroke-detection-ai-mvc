namespace MediAI.Services
{
    public interface IReportGenerator
    {
        Task<GeneratedReport> GenerateReportAsync(Diagnosis diagnosisResult, ReportFormat format);
    }
}
