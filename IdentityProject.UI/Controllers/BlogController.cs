using IdentityProject.UI.Data;
using IdentityProject.UI.Models.Dto.Blog;
using IdentityProject.UI.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityProject.UI.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        private readonly MyDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public BlogController(MyDbContext context, UserManager<User> userManager, IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        public IActionResult Index()
        {
            var blogs = _context.Blogs.Include(p => p.User).Select(p => new BlogDto()
            {
                Id = p.Id,
                Title = p.Title,
                Body = p.Body,
                UserName = p.User.UserName
            });
            return View(blogs);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(BlogDto blog)
        {
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            Blog newBlog = new Blog()
            {
                Title = blog.Title,
                Body = blog.Body,
                User = user
            };
            _context.Add(newBlog);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(long id)
        {
            var blog = _context.Blogs.Include(p => p.User).Where(p => p.Id == id).Select(p => new BlogDto()
            {
                Body = p.Body,
                Id = p.Id,
                Title = p.Title,
                UserId = p.UserId,
                UserName = p.User.UserName
            }).FirstOrDefault();

            var result = _authorizationService.AuthorizeAsync(User, blog, "IsBlogForUser").Result;

            if (result.Succeeded)
            {
                return View(blog);
            }
            else
            {
                return new ChallengeResult();
            }

        }

        [HttpPost]
        public IActionResult Edit(BlogDto blog)
        {
            //
            return View();
        }
    }
}
