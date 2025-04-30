using ITI.ExamSystem.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using eF_Kres.ModelViews;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;



namespace ITI.ExamSystem.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }


        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly OnlineExaminationDBContext _db;
        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 RoleManager<IdentityRole> roleManager,
                                 OnlineExaminationDBContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
        }


        // 1=> /Account/Login
        [HttpGet]
        public IActionResult Login() => View("AdminLogin");


        // Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                if (user.MustChangePassword)
                {
                    TempData["UserId"] = user.Id;
                    return RedirectToAction("ForceChangePassword");
                }
                var roles = await _userManager.GetRolesAsync(user);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.Email, user.Email)
                };

                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                //CookieAuthenticationDefaults.AuthenticationScheme
                await HttpContext.SignInAsync(IdentityConstants.ApplicationScheme, principal,
                    new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe,
                        ExpiresUtc = DateTime.UtcNow.AddHours(6)
                    });

                //redirect role base condition//


                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }


        //2=>  /Account/Register
        [HttpGet]
        public async Task<IActionResult> Register()
        {

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var currentRoleName = currentRoles.FirstOrDefault();

            // Get the current user's role ID
            var currentRole = await _roleManager.FindByNameAsync(currentRoleName);
            if (currentRole == null) return Forbid();

            // Parse role IDs to integers for comparison
            int currentRoleId = int.Parse(currentRole.Id);

            // Get all roles with higher numeric IDs (lower privilege)
            var allRoles = await _roleManager.Roles.ToListAsync();
            var availableRoles = allRoles
                .Where(r => int.Parse(r.Id) > currentRoleId)
                .OrderBy(r => int.Parse(r.Id))
                .ToList();

            var model = new RegisterViewModel
            {
                Roles = availableRoles.Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToList()
            };


            return View("Register",model);
        }




        // POST: /Account/Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("Register", model);

            //get current user
            var currentUser = await _userManager.GetUserAsync(User);
            var currentRoles = await _userManager.GetRolesAsync(currentUser);
            var currentRole = await _roleManager.FindByNameAsync(currentRoles.FirstOrDefault());

            //valdate it
            var selectedRole = await _roleManager.FindByNameAsync(model.SelectedRole);
            if (selectedRole == null)
            {
                ModelState.AddModelError("", "Invalid role selection");
                return View("Register", model);
            }

            // Convert IDs to integers for comparison
            int currentRoleId = int.Parse(currentRole.Id);
            int selectedRoleId = int.Parse(selectedRole.Id);

            // Ensure selected role has lower privilege (higher numeric ID)
            if (selectedRoleId <= currentRoleId)
            {
                ModelState.AddModelError("", "You cannot create accounts for this role.");
                return View("Register", model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                RegistrationDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);


            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, selectedRole.Name);

                //--
                var customUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = user.PasswordHash,
                    RegistrationDate = DateTime.UtcNow,
                    IdentityUserId = user.Id,
                    IsDeleted = false
                };
                _db.Users.Add(customUser);
                await _db.SaveChangesAsync();

                var trackedUser = await _db.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.IdentityUserId == user.Id);

                var selectedRoleName = model.Role;

                var dbRole = await _db.Roles
                    .FirstOrDefaultAsync(r => r.RoleName.ToLower() == selectedRoleName.ToLower());

                if (dbRole == null)
                {
                    ModelState.AddModelError("", $"Role '{selectedRoleName}' not found in DB.");
                    return View("FirstRegister", model);
                }


                if (trackedUser != null && dbRole != null)
                {
                    trackedUser.Roles.Add(dbRole);
                    await _db.SaveChangesAsync();
                }
                //--

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View("Register", model);
        }

        //3=> Account/Logout
        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }


        //4=>  Change Password
        [HttpGet]
        public IActionResult ForceChangePassword()
        {
            var userId = TempData["UserId"]?.ToString();
            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login");
            return View("ForceChangePassword", new ForceChangePasswordViewModel { UserId = userId });
        }

        [HttpPost]
        public async Task<IActionResult> ForceChangePassword(ForceChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View("ForceChangePassword",model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
                return RedirectToAction("Login");


            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);


            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                await _userManager.UpdateAsync(user);
                TempData["SuccessMessage"] = "Password changed successfully. You can now log in.";
                return RedirectToAction("Login");
            }
            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);


            return View("ForceChangePassword",model);

        }

        //5. FirstRegister
        [HttpGet]
        public IActionResult FirstRegister() => View("FirstRegister");


        [HttpPost]
        public async Task<IActionResult> FirstRegister(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("FirstRegister", model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
                RegistrationDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);


            if (result.Succeeded)
            {
                var role = model.Role ?? "Student";
                //Temp creating the role
                //if (!await _roleManager.RoleExistsAsync(role))
                //    await _roleManager.CreateAsync(new IdentityRole(role));

                if (!await _roleManager.RoleExistsAsync(role))
                {
                    ModelState.AddModelError("", $"The role '{role}' does not exist.");
                    return View("FirstRegister", model);
                }
                await _userManager.AddToRoleAsync(user, role);

                //--
                var customUser = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    PasswordHash = user.PasswordHash,
                    RegistrationDate = DateTime.UtcNow,
                    IdentityUserId = user.Id,
                    IsDeleted = false
                };
                _db.Users.Add(customUser);
                await _db.SaveChangesAsync();

                var trackedUser = await _db.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.IdentityUserId == user.Id);

                var selectedRoleName = model.Role;

                var dbRole = await _db.Roles
                    .FirstOrDefaultAsync(r => r.RoleName.ToLower() == selectedRoleName.ToLower());

                if (dbRole == null)
                {
                    ModelState.AddModelError("", $"Role '{selectedRoleName}' not found in DB.");
                    return View("FirstRegister", model);
                }


                if (trackedUser != null && dbRole != null)
                {
                    trackedUser.Roles.Add(dbRole);
                    await _db.SaveChangesAsync(); 
                }
                //--

                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View("FirstRegister", model);
        }

    }
}
