using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace MediAI.Services
{
    public class PdfReportGenerator : IReportGenerator
    {
        private readonly ILogger<ReportGenerator> _logger;

        public PdfReportGenerator(ILogger<ReportGenerator> logger)
        {
            _logger = logger;
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
        }

        public async Task<GeneratedReport> GenerateReportAsync(Diagnosis diagnosisResult, ReportFormat format)
        {
            try
            {
                var document = new MedicalReportDocument(diagnosisResult);
                var stream = new MemoryStream();

                document.GeneratePdf(stream);
                stream.Position = 0;
                //File.WriteAllBytes(@$"F:\Programming\Workspace\DevOps\NajeebProjects\MediAI\MediAI\wwwroot\reports\pdf\DiagnosisReport_{diagnosisResult.Id}_{DateTime.Now:yyyyMMdd}.pdf",a);
                return new GeneratedReport
                {
                    Content = stream.ToArray(),
                    FileName = @$"DiagnosisReport_{diagnosisResult.Id}_{DateTime.Now:yyyyMMdd}.pdf",
                    MimeType = "application/pdf"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF report");
                throw;
            }
        }
    }

    public class MedicalReportDocument : IDocument
    {
        private readonly Diagnosis _diagnosisResult;

        public MedicalReportDocument(Diagnosis diagnosisResult)
        {
            _diagnosisResult = diagnosisResult;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.DefaultTextStyle(x => x.FontSize(12));

                page.Header()
                    .AlignCenter()
                    .Text("التقرير الطبي")
                    .Style(TextStyle.Default.FontSize(20).Bold());

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Text($"المريض: {_diagnosisResult.Patient.FullName}");
                        column.Item().Text($"التاريخ: {_diagnosisResult.DiagnosisDate:yyyy/MM/dd}");
                        column.Item().Text($"الطبيب: {_diagnosisResult.Doctor.FullName}");

                        column.Item().PaddingTop(20).Text("التشخيص:").Bold();
                        column.Item().Text(_diagnosisResult.Condition);

                        column.Item().PaddingTop(10).Text("الوصف:").Bold();
                        column.Item().Text(_diagnosisResult.DiagnosisDescription);

                        column.Item().PaddingTop(10).Text("خطة العلاج:").Bold();
                        column.Item().Text(_diagnosisResult.TreatmentPlan);

                        column.Item().PaddingTop(10).Text("ملاحظات:").Bold();
                        column.Item().Text(_diagnosisResult.Notes ?? "لا توجد ملاحظات");
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(x =>
                    {
                        x.Span("صفحة ");
                        x.CurrentPageNumber();
                        x.Span(" من ");
                        x.TotalPages();
                    });
            });
        }
    }
}