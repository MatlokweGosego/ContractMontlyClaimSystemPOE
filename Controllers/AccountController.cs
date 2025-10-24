using ContractMontlyClaimSystemPOE.Models;
using ContractMontlyClaimSystemPOE.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace ContractMontlyClaimSystemPOE.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password, string role)
        {
            try
            {
                // For demo purposes, we'll create a session with the selected role
                // In a real application, you would validate against the database
                if (string.IsNullOrEmpty(role))
                {
                    ModelState.AddModelError("", "Please select a role");
                    return View();
                }

                // Store user role in session
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserEmail", email);

                // For demo, create a user if doesn't exist
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    user = new User
                    {
                        FullNames = "Demo User",
                        Surname = role,
                        Email = email,
                        Role = role,
                        Password = password,
                        Date = DateTime.Now
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }

                HttpContext.Session.SetInt32("UserId", user.UserID);

                return RedirectToAction("Dashboard", "Home", new { userRole = role });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Login error: {ex.Message}");
                return View();
            }
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password, string userRole)
        {
            try
            {
                if (string.IsNullOrEmpty(userRole))
                {
                    ModelState.AddModelError("", "Please select a role");
                    return View();
                }

                var user = new User
                {
                    FullNames = $"{firstName} {lastName}",
                    Surname = lastName,
                    Email = email,
                    Password = password, // In real app, hash this
                    Role = userRole,
                    Date = DateTime.Now
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Auto login after registration
                HttpContext.Session.SetString("UserRole", userRole);
                HttpContext.Session.SetString("UserEmail", email);
                HttpContext.Session.SetInt32("UserId", user.UserID);

                return RedirectToAction("Dashboard", "Home", new { userRole = userRole });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Registration error: {ex.Message}");
                return View();
            }
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
//Ill be back in 4hours ngikhatele ngiyafa