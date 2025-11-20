namespace ContractMontlyClaimSystemPOE.Validators
{
    public class ClaimValidator : AbstractValidator<Claim>
    {
        public ClaimValidator()
        {
            RuleFor(x => x.NumberOfHours).InclusiveBetween(1, 160);
            RuleFor(x => x.AmountOfRate).InclusiveBetween(50, 1000);
            RuleFor(x => x.ModuleName).NotEmpty().Length(2, 100);
            RuleFor(x => x.FacultyName).NotEmpty().Length(2, 100);
        }
    }
}
