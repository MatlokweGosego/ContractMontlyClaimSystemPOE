using FluentValidation;
using global::ContractMontlyClaimSystemPOE.Models;

namespace ContractMontlyClaimSystemPOE.Validators
{
    public class UserValidator : AbstractValidator<User>
    {
        public UserValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Valid email address is required");

            RuleFor(x => x.FullNames)
                .NotEmpty().WithMessage("Full name is required")
                .Length(2, 100).WithMessage("Full name must be between 2 and 100 characters");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required")
                .Must(BeAValidRole).WithMessage("Role must be Lecturer, Coordinator, Manager, or HR");
        }

        private bool BeAValidRole(string role)
        {
            return role == "Lecturer" || role == "Coordinator" || role == "Manager" || role == "HR";
        }
    }
}
