using ContractMontlyClaimSystemPOE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ContractMontlyClaimSystemPOE.Models;

namespace ContractMontlyClaimSystemPOE.Controllers
{
    public class ClaimsController : Controller
    {
        private readonly IClaimService _claimService;

        public ClaimsController(IClaimService claimService)
        {
            _claimService = claimService;
        }

        private int? GetCurrentUserId()
        {
            return HttpContext.Session.GetInt32("UserId");
        }

        private string GetCurrentUserRole()
        {
            return HttpContext.Session.GetString("UserRole");
        }

        public IActionResult Claim()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SubmitClaim(Claim claim, IFormFile supportingDocument)
        {
            try
            {
                var userId = GetCurrentUserId();
                if (userId == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                claim.LecturerID = userId.Value;
                var claimId = await _claimService.SubmitClaim(claim, supportingDocument);

                TempData["SuccessMessage"] = "Claim submitted successfully!";
                return RedirectToAction("TrackClaim");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error submitting claim: {ex.Message}");
                return View("Claim", claim);
            }
        }

        public async Task<IActionResult> TrackClaim()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var claims = await _claimService.GetClaimsByLecturer(userId.Value);
            return View(claims);
        }

        public async Task<IActionResult> PreApprove()
        {
            var userRole = GetCurrentUserRole();
            if (userRole != "Coordinator" && userRole != "Manager")
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var pendingClaims = await _claimService.GetPendingClaims();
            return View(pendingClaims);
        }

        public async Task<IActionResult> ApproveClaim()
        {
            var userRole = GetCurrentUserRole();
            if (userRole != "Manager")
            {
                return RedirectToAction("Dashboard", "Home");
            }

            var pendingClaims = await _claimService.GetPendingClaims();
            return View(pendingClaims);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveClaim(int claimId)
        {
            try
            {
                var userRole = GetCurrentUserRole();
                var success = await _claimService.ApproveClaim(claimId, userRole);

                if (success)
                {
                    TempData["SuccessMessage"] = "Claim approved successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to approve claim.";
                }

                return RedirectToAction(userRole == "Manager" ? "ApproveClaim" : "PreApprove");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error approving claim: {ex.Message}";
                return RedirectToAction("PreApprove");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RejectClaim(int claimId)
        {
            try
            {
                var userRole = GetCurrentUserRole();
                var success = await _claimService.RejectClaim(claimId, userRole);

                if (success)
                {
                    TempData["SuccessMessage"] = "Claim rejected successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to reject claim.";
                }

                return RedirectToAction(userRole == "Manager" ? "ApproveClaim" : "PreApprove");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error rejecting claim: {ex.Message}";
                return RedirectToAction("PreApprove");
            }
        }
    }
}