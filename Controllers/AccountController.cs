using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class AccountController : Controller
    { //This method handles the request t view the login form
        public IActionResult Login()
        { //returns the default lofgin view tht has the login form
            return View();
        }

        [HttpPost]
        // It takes three parameters from the submitted form: email, password, and role.
        public IActionResult Login(string email, string password, string role)
        {  // This is a placeholder that immediately redirects the user based on their selected role.
            return RedirectToAction("Dashboard", "Home", new { userRole = role });
        }
        // This method handles requests to view the user registration form.
        public IActionResult Register()
        {
            return View();
            // Returns the default 'Register' view which should contain the registration form.
        }
    }
}
//Ill be back in 4hours ngikhatele ngiyafa