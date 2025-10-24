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
    }
}
