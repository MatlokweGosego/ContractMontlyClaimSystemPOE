using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        public IActionResult Claim() // Displays the form for submitting a new claim
        // Users can fill out and submit claim information through this view
        {
            return View();
        }

        public IActionResult TrackClaim()
        {
            // Displays the claim tracking interface
            // Users can check the status and details of their submitted claims here
            return View();
        }

        public IActionResult PreApprove()
        {
            // Displays the pre-approval dashboard for reviewers/managers
            // This view likely shows claims awaiting initial review and pre-approval
            return View();
        }

        public IActionResult ApproveClaim()
        {
            // Displays the final claim approval interface
            // Authorized users can review and give final approval to pre-approved claims here
            return View();
        }


    }
}

