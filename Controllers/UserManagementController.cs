namespace MediAI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : _BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        ApplicationDbContext dbContext;
        public UserManagementController(UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext,
           ISystemLogService logService) : base(logService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            this.dbContext = dbContext;
        }

        // عرض جميع المستخدمين
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users
                //.Where(u => (u.UserType == UserType.Admin || u.UserType == UserType.Doctor) && u.Email != SharedStatics.AdminEmail)
                .ToListAsync();
            return View(users);
        }

        // عرض صفحة إضافة مستخدم جديد
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // تنفيذ إضافة مستخدم جديد
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CreateUserDto model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                if (model.UserType == UserType.Admin)
                {
                    user = new Admin(dbContext)
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        UserType = UserType.Admin
                    };
                }
                else if (model.UserType == UserType.Doctor)
                {
                    user = new Doctor(dbContext)
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        UserType = UserType.Doctor
                    };
                }
                else if (model.UserType == UserType.Patient)
                {
                    user = new Patient(dbContext)
                    {
                        UserName = model.Email,
                        Email = model.Email,
                        UserType = UserType.Patient
                    };
                }

                if (user != null)
                {
                    var result = await _userManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, model.UserType.ToString());
                        await _logService.LogAsync(this.GetUserId(),"User_Created", user.Id, HttpContext.GetUserIP());
                        return RedirectToAction(nameof(Index));
                    }

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        // عرض صفحة تعديل مستخدم
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager
                .Users//.Where(u => (u.UserType == UserType.Admin || u.UserType == UserType.Doctor) && u.Email != SharedStatics.AdminEmail)
                .Where(u => u.Id == id).FirstOrDefaultAsync();
            if (user == null) return NotFound();

            var model = new EditUserDto
            {
                Id = user.Id,
                Email = user.Email,
                UserType = user.UserType
            };

            return View(model);
        }

        // تنفيذ تعديل مستخدم
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(EditUserDto model)
        {
            var user = await _userManager
                .Users//.Where(u => (u.UserType == UserType.Admin || u.UserType == UserType.Doctor) && u.Email != SharedStatics.AdminEmail)
                .Where(u => u.Id == model.Id).FirstOrDefaultAsync();
            //.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.Email = model.Email;
            user.UserType = model.UserType;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await _logService.LogAsync(this.GetUserId(),"User_Updated", user.Id, HttpContext.GetUserIP());
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // تنفيذ حذف مستخدم
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var user = await _userManager
                    .Users//.Where(u => (u.UserType == UserType.Admin || u.UserType == UserType.Doctor) && u.Email != SharedStatics.AdminEmail)
                    .Where(u => u.Id == id).FirstOrDefaultAsync();
                //.FindByIdAsync(id);
                if (user == null) return NotFound();

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _logService.LogAsync(this.GetUserId(), "User_Deleted", user.Id, HttpContext.GetUserIP());
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (Exception ex)
            {
            }
            return RedirectToAction(nameof(Index));
        }
    }
}