using System.Diagnostics;
using ContractMontlyClaimSystemPOE.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        { // HomeController handles the main pages of the application 
            _logger = logger;
            // Logger instance for recording events, errors, and information
        }

        public IActionResult Index()
        { // Main landing page of the application
            return View();
        }

        public IActionResult Dashboard(string userRole)
        {
            // Displays the main dashboard page after user login
            // Accepts a userRole parameter to customize the dashboard view based on user permissions
            ViewBag.UserRole = userRole;// Store the user's role in ViewBag to make it accessible in the view This allows the view to display role-specific content and features
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
    }
}

