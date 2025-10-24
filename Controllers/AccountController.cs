using ContractMontlyClaimSystemPOE.Models;
using Microsoft.AspNetCore.Mvc;
using ContractMontlyClaimSystemPOE.Services;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;

namespace ContractMontlyClaimSystemPOE.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
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
                if (string.IsNullOrEmpty(role))
                {
                    ModelState.AddModelError("", "Please select a role");
                    return View();
                }

                // Store user role in session
                HttpContext.Session.SetString("UserRole", role);
                HttpContext.Session.SetString("UserEmail", email);

                // For demo, create a user if doesn't exist using RAW SQL
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    // Check if user exists
                    var checkQuery = "SELECT userID FROM Users WHERE email = @Email";
                    using (var checkCommand = new SqlCommand(checkQuery, connection))
                    {
                        checkCommand.Parameters.AddWithValue("@Email", email);
                        var existingUserId = await checkCommand.ExecuteScalarAsync();

                        if (existingUserId == null)
                        {
                            // Create new user
                            var insertQuery = @"
                                INSERT INTO Users (full_names, surname, email, role, password, date)
                                OUTPUT INSERTED.userID
                                VALUES (@FullNames, @Surname, @Email, @Role, @Password, @Date)";

                            using (var insertCommand = new SqlCommand(insertQuery, connection))
                            {
                                insertCommand.Parameters.AddWithValue("@FullNames", "Demo User");
                                insertCommand.Parameters.AddWithValue("@Surname", role);
                                insertCommand.Parameters.AddWithValue("@Email", email);
                                insertCommand.Parameters.AddWithValue("@Role", role);
                                insertCommand.Parameters.AddWithValue("@Password", password);
                                insertCommand.Parameters.AddWithValue("@Date", DateTime.Now);

                                var newUserId = (int)await insertCommand.ExecuteScalarAsync();
                                HttpContext.Session.SetInt32("UserId", newUserId);
                            }
                        }
                        else
                        {
                            HttpContext.Session.SetInt32("UserId", (int)existingUserId);
                        }
                    }
                }

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

                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var query = @"
                        INSERT INTO Users (full_names, surname, email, role, password, date)
                        OUTPUT INSERTED.userID
                        VALUES (@FullNames, @Surname, @Email, @Role, @Password, @Date)";

                    using (var command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@FullNames", $"{firstName} {lastName}");
                        command.Parameters.AddWithValue("@Surname", lastName);
                        command.Parameters.AddWithValue("@Email", email);
                        command.Parameters.AddWithValue("@Role", userRole);
                        command.Parameters.AddWithValue("@Password", password); // In real app, hash this
                        command.Parameters.AddWithValue("@Date", DateTime.Now);

                        var userId = (int)await command.ExecuteScalarAsync();

                        // Auto login after registration
                        HttpContext.Session.SetString("UserRole", userRole);
                        HttpContext.Session.SetString("UserEmail", email);
                        HttpContext.Session.SetInt32("UserId", userId);

                        return RedirectToAction("Dashboard", "Home", new { userRole = userRole });
                    }
                }
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