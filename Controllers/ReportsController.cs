namespace MediAI.Controllers
{
    //[Authorize]
    public class ReportsController : _BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IReportGenerator _reportGenerator;

        public ReportsController(ApplicationDbContext context, 
            IReportGenerator reportGenerator,
            ISystemLogService logService) :base(logService)
        {
            _context = context;
            _reportGenerator = reportGenerator;
        }

        /// <summary>
        /// إنشاء تقرير طبي
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> GenerateReport(GenerateReportDto reportDto)
        {
            var diagnosisResult = await _context.Diagnosis
                //.Include(dr => dr.DiagnosisRequest)
                .Include(dr => dr.Patient)
                .Include(dr => dr.Doctor)
                .FirstOrDefaultAsync(dr => dr.Id == reportDto.DiagnosisId);

            if (diagnosisResult == null)
                return NotFound("نتيجة التشخيص غير موجودة");

            if (!string.IsNullOrWhiteSpace(diagnosisResult.PatientId) && diagnosisResult.Patient == null)
                diagnosisResult.Patient = _context.Users.FirstOrDefault(p => p.Id == diagnosisResult.PatientId)?.ToPatient();

            if (!string.IsNullOrWhiteSpace(diagnosisResult.DoctorId) && diagnosisResult.Doctor == null)
                diagnosisResult.Doctor = _context.Users.FirstOrDefault(p => p.Id == diagnosisResult.DoctorId)?.ToDoctor();

            var reportUrl = await _reportGenerator.GenerateReportAsync(diagnosisResult, reportDto.Format);
            //reportUrl.co
            var report = new MedicalReport
            {
                DiagnosisId = diagnosisResult.Id,
                PatientId = diagnosisResult.PatientId,
                ReportDate = DateTime.Now,
                ReportUrl = reportUrl.FileName,
                Format = reportDto.Format,
                Notes = reportDto.Notes
            };

            //reportUrl.Content
            /*_context.MedicalReports.Add(report);
            await _context.SaveChangesAsync();*/

            //return Ok(new { ReportId = report.Id, ReportUrl = report.ReportUrl });
            await _logService.LogAsync(this.GetUserId(),"Report_Generated", report.DiagnosisId.ToString(), HttpContext.GetUserIP());
            return File(reportUrl.Content, reportUrl.MimeType, reportUrl.FileName);

        }

        /// <summary>
        /// تحميل تقرير طبي
        /// </summary>
        /*[HttpGet("{id}/download")]
        public async Task<IActionResult> DownloadReport(int id)
        {
            var report = await _context.MedicalReports.FindAsync(id);
            if (report == null)
                return NotFound();

            // هنا يجب تنفيذ منطق تحميل الملف الفعلي
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", report.ReportUrl.TrimStart('/'));

            if (!System.IO.File.Exists(filePath))
                return NotFound("الملف غير موجود");

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var contentType = report.Format switch
            {
                ReportFormat.PDF => "application/pdf",
                ReportFormat.Word => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ReportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/octet-stream"
            };

            return File(memory, contentType, Path.GetFileName(filePath));
        }*/
    }
}
