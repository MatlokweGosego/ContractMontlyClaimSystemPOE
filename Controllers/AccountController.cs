using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string password, string role)
        {
            return RedirectToAction("Dashboard", "Home", new { userRole = role });
        }

        public IActionResult Register()
        {
            return View();
        }
    }
}
