using IdentityProject.UI.Areas.Admin.Models.Dto;
using IdentityProject.UI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityProject.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;

        public RolesController(RoleManager<Role> roleManager, UserManager<User> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            var roles = _roleManager.Roles.Select(r => new RoleListDto
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });
            return View(roles);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AddNewRoleDto newRole)
        {
            if (!ModelState.IsValid)
            {
                return View(newRole);
            }

            Role role = new Role()
            {
                Name = newRole.Name,
                Description = newRole.Description
            };

            var result = _roleManager.CreateAsync(role).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Roles", new { area = "Admin" });
            }

            ViewBag.Errors = result.Errors.ToList();

            return View(newRole);

        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var role = _roleManager.FindByIdAsync(id).Result;

            EditRoleDto editRole = new EditRoleDto()
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            return View(editRole);
        }

        [HttpPost]
        public IActionResult Edit(EditRoleDto editRole)
        {
            var role = _roleManager.FindByIdAsync(editRole.Id).Result;

            role.Name = editRole.Name;
            role.Description = editRole.Description;

            var result = _roleManager.UpdateAsync(role).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Roles", new { area = "Admin" });
            }

            ViewBag.Errors = result.Errors.ToList();
            return View(editRole);
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            var role = _roleManager.FindByIdAsync(id).Result;

            DeleteRoleDto deleteRole = new DeleteRoleDto()
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description
            };

            return View(deleteRole);
        }

        [HttpPost]
        public IActionResult Delete(DeleteRoleDto deleteRole)
        {

            var role = _roleManager.FindByIdAsync(deleteRole.Id).Result;

            var result = _roleManager.DeleteAsync(role).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Roles", new { area = "Admin" });
            }

            string message = "";
            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;
            }

            TempData["Message"] = message;

            return View(deleteRole);

        }

        [HttpGet]
        public IActionResult UserInRole(string Name)
        {
            var usersInRole = _userManager.GetUsersInRoleAsync(Name).Result;

            return View(usersInRole.Select(p => new UserListDto
            {
                FirstName = p.FirstName,
                LastName = p.LastName,
                UserName = p.UserName,
                PhoneNumber = p.PhoneNumber,
                Id = p.Id,
            }));
        }

    }
}
