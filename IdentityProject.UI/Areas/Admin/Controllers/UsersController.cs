using IdentityProject.UI.Areas.Admin.Models.Dto;
using IdentityProject.UI.Models.Dto;
using IdentityProject.UI.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityProject.UI.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            var users = _userManager.Users.Select(u => new UserListDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                PhoneNumber = u.PhoneNumber,
                EmailConfirmed = u.EmailConfirmed,
                AccessFailedCount = u.AccessFailedCount
            }).ToList();

            return View(users);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(RegisterDto register)
        {
            if (ModelState.IsValid == false)
            {
                return View(register);
            }

            User newUser = new User()
            {
                FirstName = register.FirstName,
                LastName = register.LastName,
                Email = register.Email,
                UserName = register.Email,
            };

            var result = _userManager.CreateAsync(newUser, register.Password).Result;
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "users", new { area = "admin" });
            }

            string message = "";
            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;
            }
            TempData["Message"] = message;
            return View(register);
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            UserEditDto userEdit = new UserEditDto()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email
            };

            return View(userEdit);
        }

        [HttpPost]
        public IActionResult Edit(UserEditDto userEdit)
        {
            var user = _userManager.FindByIdAsync(userEdit.Id).Result;
            user.FirstName = userEdit.FirstName;
            user.LastName = userEdit.LastName;
            user.PhoneNumber = userEdit.PhoneNumber;
            user.Email = userEdit.Email;
            user.UserName = userEdit.UserName;

            var result = _userManager.UpdateAsync(user).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Users", new { area = "Admin" });
            }
            string message = "";
            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;
            }
            TempData["Message"] = message;
            return View(userEdit);
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            UserDeleteDto userDelete = new UserDeleteDto()
            {
                Email = user.Email,
                FullName = $"{user.FirstName}  {user.LastName}",
                Id = user.Id,
                UserName = user.UserName,
            };
            return View(userDelete);
        }

        [HttpPost]
        public IActionResult Delete(UserDeleteDto userDelete)
        {
            var user = _userManager.FindByIdAsync(userDelete.Id).Result;

            var result = _userManager.DeleteAsync(user).Result;

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Users", new { area = "Admin" });

            }

            string message = "";
            foreach (var item in result.Errors.ToList())
            {
                message += item.Description + Environment.NewLine;
            }
            TempData["Message"] = message;

            return View(userDelete);
        }

        [HttpGet]
        public IActionResult AddUserRole(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            var roles = new List<SelectListItem>(_roleManager.Roles.Select(r => new SelectListItem
            {
                Text = r.Name,
                Value = r.Name
            }));

            return View(new AddUserRoleDto
            {
                Id = id,
                Roles = roles,
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}"
            });
        }

        [HttpPost]
        public IActionResult AddUserRole(AddUserRoleDto newRole)
        {
            var user = _userManager.FindByIdAsync(newRole.Id).Result;

            var result = _userManager.AddToRoleAsync(user, newRole.Role).Result;


            return RedirectToAction("UserRoles", "Users", new {id = user.Id , area = "Admin" });
        }

        [HttpGet]
        public IActionResult UserRoles(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;
            var role = _userManager.GetRolesAsync(user).Result;

            ViewBag.UserInfo = $"{user.FirstName} {user.LastName} {user.Email}";
            return View(role);
        }

    }
}
