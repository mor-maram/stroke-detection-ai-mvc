using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace MediAI.Services
{
    public class ExcelReportGenerator : IReportGenerator
    {
        private readonly ILogger<ReportGenerator> _logger;

        public ExcelReportGenerator(ILogger<ReportGenerator> logger)
        {
            _logger = logger;
        }

        public async Task<GeneratedReport> GenerateReportAsync(Diagnosis diagnosisResult, ReportFormat format)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                using var spreadsheet = SpreadsheetDocument.Create(memoryStream, SpreadsheetDocumentType.Workbook);

                var workbookPart = spreadsheet.AddWorkbookPart();
                workbookPart.Workbook = new Workbook();

                var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                worksheetPart.Worksheet = new Worksheet(new SheetData());

                var sheets = workbookPart.Workbook.AppendChild(new Sheets());
                sheets.Append(new Sheet()
                {
                    Id = workbookPart.GetIdOfPart(worksheetPart),
                    SheetId = 1,
                    Name = "التقرير الطبي"
                });

                var sheetData = worksheetPart.Worksheet.GetFirstChild<SheetData>();

                // Add headers
                AddRow(sheetData, 1, new[] { "التقرير الطبي" }, true);
                AddRow(sheetData, 2, new[] { "معلومات المريض" }, true);
                AddRow(sheetData, 3, new[] { "الاسم", diagnosisResult.Patient.FullName });
                AddRow(sheetData, 4, new[] { "تاريخ الميلاد", diagnosisResult.Patient.DateOfBirth.ToString("yyyy/MM/dd") });

                AddRow(sheetData, 6, new[] { "التشخيص" }, true);
                AddRow(sheetData, 7, new[] { diagnosisResult.Condition });

                AddRow(sheetData, 9, new[] { "الوصف" }, true);
                AddRow(sheetData, 10, new[] { diagnosisResult.DiagnosisDescription });

                AddRow(sheetData, 12, new[] { "خطة العلاج" }, true);
                AddRow(sheetData, 13, new[] { diagnosisResult.TreatmentPlan });

                AddRow(sheetData, 15, new[] { "ملاحظات" }, true);
                AddRow(sheetData, 16, new[] { diagnosisResult.Notes ?? "لا توجد ملاحظات" });

                workbookPart.Workbook.Save();
                memoryStream.Position = 0;

                return new GeneratedReport
                {
                    Content = memoryStream.ToArray(),
                    FileName = $"DiagnosisReport_{diagnosisResult.Id}_{DateTime.Now:yyyyMMdd}.xlsx",
                    MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating Excel report");
                throw;
            }
        }

        private void AddRow(SheetData sheetData, uint rowIndex, string[] values, bool isBold = false)
        {
            var row = new Row { RowIndex = rowIndex };
            sheetData.Append(row);

            for (int i = 0; i < values.Length; i++)
            {
                var cell = new Cell
                {
                    CellReference = GetColumnName(i + 1) + rowIndex,
                    DataType = CellValues.String
                };

                var cellValue = new CellValue(values[i]);
                cell.Append(cellValue);

                if (isBold)
                {
                    cell.StyleIndex = 1; // Bold style
                }

                row.Append(cell);
            }
        }

        private string GetColumnName(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = string.Empty;

            while (dividend > 0)
            {
                int modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
        }
    }
}