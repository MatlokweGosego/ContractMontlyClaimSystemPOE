using ContractMontlyClaimSystemPOE.Models;

namespace ContractMontlyClaimSystemPOE.Services
{
    public interface IValidationService
    {
        (bool isValid, string errorMessage) ValidateClaim(Claim claim);
        (bool isValid, string errorMessage) ValidateApproval(Claim claim, string approverRole);
    }
}