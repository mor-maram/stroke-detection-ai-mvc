namespace MediAI.Controllers
{
    public class AdminController : _BaseController
    {
        private readonly ILogger<AdminController> _logger;
        RoleManager<IdentityRole> roleManager;
        UserManager<User> userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> signInManager;
        private readonly PasswordHasher<User> _passwordHasher;
        public AdminController(ILogger<AdminController> logger,
           RoleManager<IdentityRole> roleManager,
           UserManager<User> userManager,
           SignInManager<User> signInManager,
           ApplicationDbContext context,
           ISystemLogService logService):base(logService)
        {
            this._logger = logger;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this._context = context;
            _passwordHasher = new PasswordHasher<User>();
        }

        public async Task<IActionResult> AdminDashboard()
        {
            var LatestUsers = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = u.FullName,
                        Email = u.Email,
                        UserType = u.UserType,
                        CreatedAt = u.CreatedAt,
                        IsActive = u.IsActive
                    }).ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalPatients = await _context.Patients.CountAsync(),
                TotalDoctors = await _context.Doctors.CountAsync(),
                TotalDiagnosiss = await _context.Diagnosis.CountAsync(),
                PendingDiagnosiss = await _context.Diagnosis.CountAsync(r => r.Status == DiagnosisStatus.Pending),
                CompletedDiagnosiss = await _context.Diagnosis.CountAsync(r => r.Status == DiagnosisStatus.Completed),
                CriticalDiagnosiss = await _context.Diagnosis.CountAsync(r => r.Priority == PriorityLevel.Critical),
                TotalReports = await _context.MedicalReports.CountAsync(),
                TotalNotifications = await _context.Notifications.CountAsync(),

                LatestUsers = await _context.Users
                    .OrderByDescending(u => u.CreatedAt)
                    .Take(5)
                    .Select(u => new UserDto
                    {
                        Id = u.Id,
                        Name = $"{u.FirstName} {u.LastName}",
                        Email = u.Email,
                        UserType = u.UserType,
                        CreatedAt = u.CreatedAt,
                        IsActive = u.IsActive
                    }).ToListAsync(),
                RecentActivities=await _context.SystemLogs
                .Include(r=>r.User)
                .OrderByDescending(l=>l.Id)
                .Take(100).ToListAsync(),
            };

            return View(model);
        }

        public async Task<IActionResult> DoctorsList()
        {
            var lst = await _context.Users
                .Where(u => u.UserType == UserType.Doctor).ToListAsync();
            var result = lst.DeserializeToList<Doctor>();            
            return View(result);
        }

        [HttpPost]
        //[Authorize("Admin")]
        public async Task<IActionResult> AddDoctor(CreateDoctorDto model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "البيانات غير صالحة" });
            }

            try
            {
                string doctorRoleName = UserType.Doctor.ToString();
                var doctor = new Doctor(this._context)
                {
                    Id = Guid.NewGuid().ToString(),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.Email,
                    NormalizedUserName = model.Email?.ToUpper(),
                    Email = model.Email,
                    NormalizedEmail = model.Email?.ToUpper(),
                    EmailConfirmed = true,
                    PhoneNumber = model.PhoneNumber,
                    PhoneNumberConfirmed = true,  
                    LicenseNumber=model.LicenseNumber,
                    YearsOfExperience=model.YearsOfExperience,
                    UserType = UserType.Doctor,
                };
                doctor.PasswordHash = _passwordHasher.HashPassword(doctor, model.Password);
                _context.Users.Add(doctor);
                _context.SaveChanges();
                await _logService.LogAsync(this.GetUserId(), "Doctor_Added", doctor.Id, HttpContext.GetUserIP());                
                try
                {
                    //var x1 = userManager.CreateAsync(doctor, model.Password).Result;
                    var y1 = userManager.AddToRoleAsync(doctor, doctorRoleName).Result;
                }
                catch (Exception Ex)
                {

                }
                _context.SaveChanges();

                return Json(new { success = true, data = doctor });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDoctor(Doctor doctor)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "البيانات غير صالحة" });
            }

            try
            {
                string doctorRoleName = UserType.Doctor.ToString();
                var existingDoctor = _context.Users.Find(doctor.Id);
                if (existingDoctor != null)
                {
                    existingDoctor.FirstName = doctor.FirstName;
                    existingDoctor.LastName = doctor.LastName;
                    existingDoctor.UserName = doctor.Email;
                    existingDoctor.Email = doctor.Email;
                    existingDoctor.NormalizedUserName = doctor.Email?.ToUpper();                    
                    existingDoctor.NormalizedEmail = doctor.Email?.ToUpper();
                    existingDoctor.EmailConfirmed = true;
                    //UserType = UserType.Doctor,                

                    doctor.PasswordHash = _passwordHasher.HashPassword(doctor, doctor.Email);
                    _context.Update(existingDoctor);
                    _context.SaveChanges();
                    await _logService.LogAsync(this.GetUserId(),"Doctor_Updated", doctor.Id, HttpContext.GetUserIP());                    
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "الطبيب غير موجود" });            
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // الحصول على بيانات الطبيب لتعديلها
        [HttpGet]
        public IActionResult GetDoctor(string id)
        {
            var doctor = _context.Users.Where(u => u.UserType == UserType.Doctor).
                Where(u => u.Id == id)
                .Select(d => new
                {
                    d.Id,
                    d.FirstName,
                    d.LastName,                    
                    d.Email
                })
                .FirstOrDefault();
            return Json(doctor);
        }

        // عرض تفاصيل الطبيب
        [HttpGet]
        public IActionResult GetDoctorDetails(string id)
        {
            var doctor = _context.Users.Where(u => u.UserType == UserType.Doctor)
                .Where(d => d.Id == id)
                .Select(d => new
                {
                    d.FirstName,
                    d.LastName,                    
                    d.Email,
                    d.IsActive
                })
                .FirstOrDefault();
            return Json(doctor);
        }

        // حذف طبيب
        [HttpPost]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            var doctor = await _context.Users
                .Where(u => u.UserType == UserType.Doctor)
                .Where(u => u.Id == id)
                .ToListAsync();
            if (doctor != null)
            {
                _context.Users.Remove(doctor.FirstOrDefault());
                _context.SaveChanges();
                await _logService.LogAsync(this.GetUserId(),"Doctor_Deleted", id, HttpContext.GetUserIP());
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "الطبيب غير موجود" });
        }       
    }
}