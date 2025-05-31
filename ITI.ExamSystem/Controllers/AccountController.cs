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
using ITI.ExamSystem.Services;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;



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
        private readonly IEmailSender _emailSender;

        private readonly OnlineExaminationDBContext _db;
        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 IEmailSender emailsender,
                                 RoleManager<IdentityRole> roleManager,
                                 OnlineExaminationDBContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
            _emailSender = emailsender;
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
                // 🔁 Role-based redirect logic
                if (roles.Contains("SuperAdmin"))
                    return RedirectToAction("GetAll", "SpAdmin");
                if (roles.Contains("Admin"))
                    return RedirectToAction("ReadStudents", "Admin");
                if (roles.Contains("Instructor"))
                    return RedirectToAction("InstructorProfile", "InstructorDashboard");
                if (roles.Contains("Student"))
                    return RedirectToAction("StudentCourse", "Course", new { studentId = user.Id });

                // Fallback
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

            var availableRoles_checkList= availableRoles.Select(r=> new RoleviewModel { id= int.Parse(r.Id) , Name=r.Name, IsSelected=false}).ToList();

            //var model = new RegisterViewModel
            //{
            //    Roles = availableRoles.Select(r => new SelectListItem
            //    {
            //        Value = r.Name,
            //        Text = r.Name
            //    }).ToList()
            //};

            var model = new RegisterViewModel
            {
                Roles = availableRoles_checkList
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

            int currentRoleId = int.Parse(currentRole.Id);

            var selectedRoles = model.Roles.Where(r => r.IsSelected).ToList();


            if (!selectedRoles.Any())
            {
                ModelState.AddModelError("", "Please select at least one role.");
                return View("Register", model);
            }

            // Validate each selected role
            foreach (var role in selectedRoles)
            {
                var selectedRole = await _roleManager.FindByIdAsync(role.id.ToString());
                if (selectedRole == null)
                {
                    ModelState.AddModelError("", $"Invalid role selection: {role.Name}");
                    return View("Register", model);
                }

                int selectedRoleId = int.Parse(selectedRole.Id);

                if (selectedRoleId <= currentRoleId)
                {
                    ModelState.AddModelError("",
                        $"You cannot create accounts for the {role.Name} role.");
                    return View("Register", model);
                }
            }

            if (!model.Roles.Any(r=>r.IsSelected))
            {
                ModelState.AddModelError("", "No Role selected!");
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
                model.SelectedRole = selectedRoles.FirstOrDefault()?.Name;

                await _userManager.AddToRolesAsync(user,model.Roles.Where(r=>r.IsSelected==true).Select(r => r.Name));

                //Add Email Sender Service 
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebUtility.UrlEncode(token);
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = encodedToken }, Request.Scheme);

                var message = $@"
                   Welcome to the Exam System!<br/>
                    Your temporary password is: <strong>{model.Password}</strong><br/>
                   Please confirm your email to activate your account by clicking <a href='{confirmationLink}'>here</a>.";

                await _emailSender.SendEmailAsync(user.Email, "Activate Your Exam Account", message);

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

                var selectedRoleName = model.SelectedRole;

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

        //Get: Confirm Email Registeration 
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            if (userId == null || token == null)
                return BadRequest("Invalid confirmation parameters");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");

            var decodedToken = WebUtility.UrlDecode(token);
            var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                return BadRequest($"Email confirmation failed: {errors}");
            }

            if (result.Succeeded)
            {
                return RedirectToAction("Login");
            }

            return BadRequest("Email confirmation failed");
        }

        //Post: Delete User
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
                return BadRequest("User ID is required.");

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound("User not found.");

            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
                return Ok("User deleted successfully.");

            // Return error details if deletion fails
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return BadRequest(ModelState);
        }


    }
}
