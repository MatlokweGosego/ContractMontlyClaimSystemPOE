using ContractMontlyClaimSystemPOE.Models;

namespace ContractMontlyClaimSystemPOE.Services
{
    public interface IClaimService
    {
        Task<int> SubmitClaim(Claim claim, IFormFile supportingDocument);
        Task<List<Claim>> GetClaimsByLecturer(int lecturerId);
        Task<List<Claim>> GetPendingClaims();
        Task<bool> ApproveClaim(int claimId, string approvedBy);
        Task<bool> RejectClaim(int claimId, string rejectedBy);
        Task<Claim> GetClaimById(int claimId);

        // NEW METHODS FOR AUTOMATION
        Task<(bool success, string message)> SubmitClaimWithValidation(Claim claim, IFormFile supportingDocument);
        Task<(bool success, string message)> ApproveClaimWithValidation(int claimId, string approvedBy);
        Task<List<Claim>> GetApprovedClaims(); // FOR HR FUNCTIONALITY
    }
}
