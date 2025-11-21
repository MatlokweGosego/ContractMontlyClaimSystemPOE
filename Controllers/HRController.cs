using ContractMontlyClaimSystemPOE.Models;
using ContractMontlyClaimSystemPOE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Data.SqlClient;

namespace ContractMontlyClaimSystemPOE.Controllers
{
    public class HRController : Controller
    {
        private readonly IClaimService _claimService;
        private readonly IConfiguration _configuration;

        public HRController(IClaimService claimService, IConfiguration configuration)
        {
            _claimService = claimService;
            _configuration = configuration;
        }

        private string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole");
        }

        public async Task<IActionResult> Dashboard()
        {
            var userRole = GetCurrentUserRole();
            if (userRole != "HR")
            {
                return RedirectToAction("Dashboard", "Home");
            }
            return View();
        }

        public async Task<IActionResult> ApprovedClaims()
        {
            var userRole = GetCurrentUserRole();
            if (userRole != "HR")
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var approvedClaims = await GetApprovedClaimsForPayment();
            return View(approvedClaims);
        }

        public async Task<IActionResult> GenerateInvoice(int claimId)
        {
            var userRole = GetCurrentUserRole();
            if (userRole != "HR")
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var claim = await _claimService.GetClaimById(claimId);
            if (claim == null)
            {
                TempData["ErrorMessage"] = "Claim not found";
                return RedirectToAction("ApprovedClaims");
            }

            var invoice = new InvoiceViewModel
            {
                Claim = claim,
                InvoiceDate = DateTime.Now,
                InvoiceNumber = $"INV-{DateTime.Now:yyyyMMdd}-{claimId}",
                DueDate = DateTime.Now.AddDays(30)
            };

            return View(invoice);
        }

        public async Task<IActionResult> ManageLecturers()
        {
            var userRole = GetCurrentUserRole();
            if (userRole != "HR")
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var lecturers = await GetAllLecturers();
            return View(lecturers);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLecturer(int userId, string fullName, string email)
        {
            var success = await UpdateLecturerInfo(userId, fullName, email);

            if (success)
            {
                TempData["SuccessMessage"] = "Lecturer information updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update lecturer information.";
            }

            return RedirectToAction("ManageLecturers");
        }

        // PRIVATE HELPER METHODS
        private async Task<List<Claim>> GetApprovedClaimsForPayment()
        {
            return await _claimService.GetApprovedClaims();
        }

        private async Task<List<User>> GetAllLecturers()
        {
            var lecturers = new List<User>();
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Users WHERE role = 'Lecturer' ORDER BY full_names";

                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            lecturers.Add(new User
                            {
                                UserID = reader.GetInt32(reader.GetOrdinal("userID")),
                                FullNames = reader.GetString(reader.GetOrdinal("full_names")),
                                Surname = reader.GetString(reader.GetOrdinal("surname")),
                                Email = reader.GetString(reader.GetOrdinal("email")),
                                Role = reader.GetString(reader.GetOrdinal("role")),
                                Date = reader.GetDateTime(reader.GetOrdinal("date"))
                            });
                        }
                    }
                }
            }
            return lecturers;
        }

        private async Task<bool> UpdateLecturerInfo(int userId, string fullName, string email)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                var query = "UPDATE Users SET full_names = @FullName, email = @Email WHERE userID = @UserID";

                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", fullName);
                    command.Parameters.AddWithValue("@Email", email);
                    command.Parameters.AddWithValue("@UserID", userId);

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }
    }
}