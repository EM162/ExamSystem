using ITI.ExamSystem.Models;
using ITI.ExamSystem.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ITI.ExamSystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UserController : Controller
    {
        private readonly OnlineExaminationDBContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserController(OnlineExaminationDBContext context, RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        // GET: User
        public IActionResult Index()
        {
            var users = _context.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserViewModel
                {
                    UserID = u.UserID,
                    FullName = u.FullName,
                    Email = u.Email,
                    ProfileImagePath = u.ProfileImagePath,
                    RegistrationDate = u.RegistrationDate.Value,
                    IsDeleted = u.IsDeleted
                }).ToList();

            return View(users);
        }

        // Edit GET
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null) return NotFound();

            var allRoles = await _roleManager.Roles.ToListAsync();

            var vm = new UserViewModel
            {
                UserID = user.UserID,
                FullName = user.FullName,
                Email = user.Email,
                ProfileImagePath = user.ProfileImagePath,
                AvailableRoles = allRoles.Select(r => new RoleCheckbox
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    IsSelected = user.Roles.Any(ur => ur.RoleID.ToString() == r.Id)
                }).ToList()
            };

            return PartialView("_EditModal", vm);
        }

        
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.Roles)
                    .FirstOrDefaultAsync(u => u.UserID == vm.UserID);

                
                var selectedRoles = vm.AvailableRoles.Where(r => r.IsSelected).ToList();
                var currentRoles = user.Roles.Select(r => r.RoleID).ToList();
                var roleIdsToRemove = currentRoles
                .Select(id => id.ToString())
                .Except(selectedRoles.Select(r => r.RoleId));

               
                foreach (var roleId in roleIdsToRemove.Except(selectedRoles.Select(r => r.RoleId)))
                {
                    var role = await _roleManager.FindByIdAsync(roleId);
                    if (role != null)
                    {
                        var appUser = await _userManager.FindByIdAsync(user.UserID.ToString());
                        await _userManager.RemoveFromRoleAsync(appUser, role.Name);
                    }
                }

               
                foreach (var role in selectedRoles.Where(r => !currentRoles.Contains(int.Parse(r.RoleId))))
                {
                    var roleEntity = await _roleManager.FindByIdAsync(role.RoleId);
                    if (roleEntity != null)
                    {
                        var appUser = await _userManager.FindByIdAsync(user.UserID.ToString());
                        await _userManager.RemoveFromRoleAsync(appUser, roleEntity.Name);

                        //await _userManager.AddToRoleAsync(user, roleEntity.Name);
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return PartialView("_EditModal", vm);
        }
    }
}
