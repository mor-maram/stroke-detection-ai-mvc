namespace MediAI.Services
{
    public class ReportGenerator : IReportGenerator
    {
        private readonly ILogger<ReportGenerator> _logger;

        public ReportGenerator(ILogger<ReportGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<GeneratedReport> GenerateReportAsync(Diagnosis diagnosisResult, ReportFormat format)
        {
            try
            {
                IReportGenerator generator = format switch
                {
                    ReportFormat.PDF => new PdfReportGenerator(_logger),
                    ReportFormat.Word => new WordReportGenerator(_logger),
                    ReportFormat.Excel => new ExcelReportGenerator(_logger),
                    _ => throw new ArgumentOutOfRangeException(nameof(format), $"Format {format} is not supported")
                };

                return await generator.GenerateReportAsync(diagnosisResult, format);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating report");
                throw;
            }
        }
    }
}
