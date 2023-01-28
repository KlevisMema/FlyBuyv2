using FlyBuy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FlyBuy.Controllers
{
    public class RolesController : Controller
    {
        public UserManager<ApplicationUser> UserManager;
        public RoleManager<IdentityRole> RoleManager;
        public IEnumerable<IdentityRole> Roles { get; set; }
        public RolesController(UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
        {
            UserManager = userManager;
            RoleManager = roleManager;
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Index()
        {
            Roles = RoleManager.Roles;
            return View(Roles);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var role = await RoleManager.FindByIdAsync(id);

            if (role == null)
            {
                return BadRequest("Roli nuk gjendet");
            }
            var model = new EditRole
            {
                Id = role.Id,
                Name = role.Name
            };

            return View(model);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRole model)
        {
            var role = await RoleManager.FindByIdAsync(model.Id);

            if (role == null)
            {
                return BadRequest("Roli nuk gjendet" + model.Id);
            }
            else
            {
                role.Name = model.Name;
                var result = await RoleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);

            }
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<JsonResult> DeleteAsync(string id)
        {

            var role = await RoleManager.FindByIdAsync(id);
            if (role != null)
            {
                await RoleManager.DeleteAsync(role);
            }

            return new JsonResult(Ok());

        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {

                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.Name
                };

                IdentityResult result = await RoleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }

            return View(model);
        }
    }
}