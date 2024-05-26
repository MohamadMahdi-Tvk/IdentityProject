using IdentityProject.UI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace IdentityProject.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [Authorize(Policy = "Buyer")]
        public string JustBuyer()
        {
            return "شما خریدار هستید";
        }

        [Authorize(Policy = "BloodType")]
        public string Blood()
        {
            return "A+ & O +";
        }


        [Authorize(Policy = "Credit")]
        public string Credit()
        {
            return "You Have More Than 10000T In Your Wallet";
        }
    }
}
