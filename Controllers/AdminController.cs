using FlyBuy.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlyBuy.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> UserManager;
        public RoleManager<IdentityRole> RoleManager;
        public IEnumerable<IdentityRole> Roles { get; set; }

        public AdminController(UserManager<ApplicationUser> UserManager, RoleManager<IdentityRole> RoleManager)
        {
            this.UserManager = UserManager;
            this.RoleManager = RoleManager;
        }

        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Index()
        {
            var users = UserManager.Users.Select(user => new UserViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                BirthDate = user.BirthDate,
                Role = string.Join(",", UserManager.GetRolesAsync(user).Result.ToArray())
            }).ToList();
            return View(users);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);

            if (user == null)
            {
                return BadRequest("User not found");
            }

            var model = new UserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                BirthDate = user.BirthDate,
            };
            return View(model);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return BadRequest("User not found" + model.Id);
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                user.BirthDate = model.BirthDate;
                var result = await UserManager.UpdateAsync(user);
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

            ApplicationUser User = await UserManager.FindByIdAsync(id);

            if (User != null)

            {
                await UserManager.DeleteAsync(User);
            }

            return new JsonResult(Ok());

        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpGet]
        public async Task<ActionResult> Create(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var model = new UserViewModel
            {
                UserName = user.UserName
            };
            var roles = RoleManager.Roles;
            ViewBag.Roles = new SelectList(roles.ToList(), "Id", "Name");
            return View(model);
        }

        [Authorize(Roles = "Admin,Manager")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUserRole(UserViewModel u)
        {
            var name = Convert.ToString(await RoleManager.FindByIdAsync(u.Role));
            var user = await UserManager.FindByIdAsync(u.Id);
            if (user == null)
            {
                return BadRequest("User does not exists" + name);
            }
            await UserManager.AddToRoleAsync(user, name);
            return RedirectToAction("Index");
        }
    }
}
