namespace MediAI.Controllers
{
    public class HomeController : _BaseController
    {
        private readonly ILogger<HomeController> _logger;
        RoleManager<IdentityRole> roleManager;
        UserManager<User> userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> signInManager;
        //private readonly IStringLocalizer<SharedResources> _localizer;

        public HomeController(ILogger<HomeController> logger,
           RoleManager<IdentityRole> roleManager,
           UserManager<User> userManager,
           SignInManager<User> signInManager,
           //IStringLocalizer<SharedResources> localizer,
           ApplicationDbContext context,
           ISystemLogService logService) : base(logService)
        {
            this._logger = logger;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
            this.userManager = userManager;
            //this._localizer = localizer;
            this._context = context;
            Initialize();
        }


        public async Task<bool> Initialize()
        {
            string adminRoleName = UserType.Admin.ToString();
            string doctorRoleName = UserType.Doctor.ToString();
            string patientRoleName = UserType.Patient.ToString();
            if (!roleManager.RoleExistsAsync(adminRoleName).Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Id = adminRoleName ,
                    Name = adminRoleName
                };
                var x = roleManager.CreateAsync(role).Result;

                Admin admin = new Admin(_context)
                {
                    UserName = SharedStatics.AdminEmail,
                    Email = SharedStatics.AdminEmail,
                    EmailConfirmed = true,
                    Id = Guid.NewGuid().ToString(),
                    FirstName = "System",
                    LastName = "Admin",
                    NormalizedUserName = SharedStatics.AdminEmail?.ToUpper(),
                    NormalizedEmail = SharedStatics.AdminEmail?.ToUpper(),
                    PhoneNumberConfirmed = true,
                    UserType = UserType.Admin,
                };
                
                var x1 = userManager.CreateAsync(admin, SharedStatics.AdminPass).Result;
                var y1 = userManager.AddToRoleAsync(admin,adminRoleName ).Result;
            }

            if (!roleManager.RoleExistsAsync(doctorRoleName).Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Id = doctorRoleName,
                    Name = doctorRoleName
                };
                var x = roleManager.CreateAsync(role).Result;
            }

            if (!roleManager.RoleExistsAsync(patientRoleName).Result)
            {
                IdentityRole role = new IdentityRole
                {
                    Id = patientRoleName,
                    Name = patientRoleName
                };
                var x = roleManager.CreateAsync(role).Result;
            }

            if (_context.Users.Count() != 0)
            {
                var users = userManager.Users.Where(u => u.Email == SharedStatics.AdminEmail).ToList();
                if (users != null && users.Count() > 0)
                {
                    foreach (var user in users)
                    {
                        if (!user.EmailConfirmed)
                        {
                            user.EmailConfirmed = true;
                            await userManager.UpdateAsync(user);
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public bool DeleteUsers()
        {
            if (_context.Users.Count() == 0)
            {
                var uList = userManager.Users.ToList();
                foreach (var user in uList)
                {
                    _context.Users.Remove(user);
                }
            }

            return true;
        }

        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole(UserType.Admin.ToString()))
                    return RedirectToAction("AdminDashboard", "Admin");
                else if (User.IsInRole(UserType.Doctor.ToString()))
                    return RedirectToAction("DoctorDashboard", "Doctor");                
                else if (User.IsInRole(UserType.Patient.ToString()))
                    return RedirectToAction("PatientDashboard", "Patient");
            }
            return View();
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider
                .MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions()
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1),
                    Path = "/"
                });
            return LocalRedirect(returnUrl);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
