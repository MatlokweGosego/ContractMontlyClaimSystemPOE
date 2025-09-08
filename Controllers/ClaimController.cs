using Microsoft.AspNetCore.Mvc;

namespace ContractMonthlyClaimSystem.Controllers
{
    public class ClaimsController : Controller
    {
        public IActionResult Claim()
        {
            return View();
        }

        public IActionResult TrackClaim()
        {
            return View();
        }

        public IActionResult PreApprove()
        {
            return View();
        }

        public IActionResult ApproveClaim()
        {
            return View();
        }


    }
}

