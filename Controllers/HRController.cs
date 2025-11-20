using Microsoft.AspNetCore.Mvc;

namespace ContractMontlyClaimSystemPOE.Controllers
{
    public class HRController : Controller
    {
        public async Task<IActionResult> Dashboard()
        {
            // HR dashboard with statistics
            return View();
        }

        public async Task<IActionResult> ApprovedClaims()
        {
            var claims = await GetApprovedClaimsForPayment();
            return View(claims);
        }

        public async Task<IActionResult> GenerateInvoice(int claimId)
        {
            var invoice = await GenerateInvoiceData(claimId);
            return View(invoice);
        }

        public async Task<IActionResult> ManageLecturers()
        {
            var lecturers = await GetAllLecturers();
            return View(lecturers);
        }
    }
}
