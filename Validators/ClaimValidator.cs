using ContractMontlyClaimSystemPOE.Models;
using FluentValidation;

namespace ContractMontlyClaimSystemPOE.Validators
{
    public class ClaimValidator : AbstractValidator<Claim>
    {
        public ClaimValidator()
        {
            RuleFor(x => x.NumberOfHours)
                .GreaterThan(0).WithMessage("Number of hours must be greater than 0")
                .LessThanOrEqualTo(160).WithMessage("Cannot exceed 160 hours per month");

            RuleFor(x => x.AmountOfRate)
                .GreaterThan(0).WithMessage("Hourly rate must be greater than 0")
                .LessThanOrEqualTo(1000).WithMessage("Hourly rate cannot exceed R1000");

            RuleFor(x => x.NumberOfSessions)
                .GreaterThan(0).WithMessage("Number of sessions must be greater than 0");

            RuleFor(x => x.ModuleName)
                .NotEmpty().WithMessage("Module name is required")
                .Length(2, 100).WithMessage("Module name must be between 2 and 100 characters");

            RuleFor(x => x.FacultyName)
                .NotEmpty().WithMessage("Faculty name is required")
                .Length(2, 100).WithMessage("Faculty name must be between 2 and 100 characters");

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("Total amount must be greater than 0")
                .LessThanOrEqualTo(50000).WithMessage("Total amount cannot exceed R50,000");
        }
    }
}
