using ContractMontlyClaimSystemPOE.Models;

namespace ContractMontlyClaimSystemPOE.Services
{
    public class ValidationService : IValidationService
    {
        public (bool isValid, string errorMessage) ValidateClaim(Claim claim)
        {
            if (claim.NumberOfHours <= 0) return (false, "Hours must be greater than 0");
            if (claim.NumberOfHours > 160) return (false, "Maximum 160 hours per month");
            if (claim.AmountOfRate <= 0) return (false, "Rate must be greater than 0");
            if (claim.AmountOfRate > 1000) return (false, "Maximum rate: R1000 per hour");
            if (claim.NumberOfSessions <= 0) return (false, "Sessions must be greater than 0");
            if (string.IsNullOrEmpty(claim.ModuleName)) return (false, "Module name required");
            if (string.IsNullOrEmpty(claim.FacultyName)) return (false, "Faculty name required");
            return (true, "Valid");
        }

        public (bool isValid, string errorMessage) ValidateApproval(Claim claim, string approverRole)
        {
            // Coordinator limitations
            if (approverRole == "Coordinator")
            {
                if (claim.NumberOfHours > 120)
                    return (false, "Claims over 120 hours require Manager approval");
                if (claim.AmountOfRate > 800)
                    return (false, "High rates (>R800) require Manager approval");
                if (claim.TotalAmount > 10000)
                    return (false, "Large amounts (>R10,000) require Manager approval");
            }

            // Manager can approve anything
            return (true, "Approval criteria met");
        }
    }
}