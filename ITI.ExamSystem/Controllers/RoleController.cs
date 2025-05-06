using eF_Kres.ModelViews;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ITI.ExamSystem.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var roles = _roleManager.Roles
                .Select(r => new RoleviewModel
                {
                    RoleId = r.Id,
                    RoleName = r.Name,
                    //Description = r.Description
                }).ToList();

            return View(roles);
        }

        public IActionResult Create()
        {
            return PartialView("_RoleModal", new RoleviewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(RoleviewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole
                {
                    Name = vm.RoleName,
                    //Description = vm.Description
                });

                if (result.Succeeded)
                    return RedirectToAction("Index");
            }
            return PartialView("_RoleModal", vm);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound();

            var vm = new RoleviewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                //Description = role.Description
            };

            return PartialView("_RoleModal", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleviewModel vm)
        {
            if (ModelState.IsValid)
            {
                var role = await _roleManager.FindByIdAsync(vm.RoleId);
                if (role == null) return NotFound();

                role.Name = vm.RoleName;
                //role.Description = vm.Description;

                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                    return RedirectToAction("Index");
            }
            return PartialView("_RoleModal", vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }
            return RedirectToAction("Index");
        }
    }
}
