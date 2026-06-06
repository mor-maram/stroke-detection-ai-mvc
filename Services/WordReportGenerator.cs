using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;


namespace MediAI.Services
{
    public class WordReportGenerator : IReportGenerator
    {
        private readonly ILogger<ReportGenerator> _logger;

        public WordReportGenerator(ILogger<ReportGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<GeneratedReport> GenerateReportAsync(Diagnosis diagnosisResult, ReportFormat format)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var wordDocument = WordprocessingDocument.Create(memoryStream, WordprocessingDocumentType.Document);

                var mainPart = wordDocument.AddMainDocumentPart();
                mainPart.Document = new Document();
                var body = mainPart.Document.AppendChild(new Body());

                // Add title
                var title = body.AppendChild(new Paragraph());
                title.AppendChild(new Run(new Text("التقرير الطبي"))).RunProperties = new RunProperties { Bold = new Bold(), FontSize = new FontSize { Val = "28" } };
                //title.ParagraphProperties = new ParagraphProperties { Alignment = new EnumValue<JustificationValues>(JustificationValues.Center) };

                // Add patient info
                AddHeading(body, "معلومات المريض:");
                AddText(body, $"الاسم: {diagnosisResult.Patient.FullName}");
                AddText(body, $"تاريخ الميلاد: {diagnosisResult.Patient.DateOfBirth:yyyy/MM/dd}");

                // Add diagnosis info
                AddHeading(body, "التشخيص:");
                AddText(body, diagnosisResult.Condition);

                AddHeading(body, "الوصف:");
                AddText(body, diagnosisResult.DiagnosisDescription);

                AddHeading(body, "خطة العلاج:");
                AddText(body, diagnosisResult.TreatmentPlan);

                AddHeading(body, "ملاحظات:");
                AddText(body, diagnosisResult.Notes ?? "لا توجد ملاحظات");

                wordDocument.Save();
                memoryStream.Position = 0;

                return new GeneratedReport
                {
                    Content = memoryStream.ToArray(),
                    FileName = $"DiagnosisReport_{diagnosisResult.Id}_{DateTime.Now:yyyyMMdd}.docx",
                    MimeType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Word report");
                throw;
            }
        }

        private void AddHeading(Body body, string text)
        {
            var para = body.AppendChild(new Paragraph());
            para.AppendChild(new Run(new Text(text))).RunProperties = new RunProperties { Bold = new Bold(), FontSize = new FontSize { Val = "16" } };
        }

        private void AddText(Body body, string text)
        {
            var para = body.AppendChild(new Paragraph());
            para.AppendChild(new Run(new Text(text)));
            para.AppendChild(new Run(new Break()));
        }
    }
}