
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.DBContext;
namespace PensionSystem.Infrastructure.Helpers
{
    public class ContributionValidator : AbstractValidator<Contribution>
    {
        private readonly PensionSystemContext _context;
        public ContributionValidator(PensionSystemContext context)
        {
            _context = context;

            RuleFor(c => c.MemberId)
                .NotEmpty().WithMessage("Member ID is required.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.ReferenceNumber)
                .Matches(@"^[A-Za-z0-9]{10}$").WithMessage("Reference number must be alphanumeric and 10 characters long.");

            RuleFor(x => x.ContributionType)
                .IsInEnum().WithMessage("Contribution type must be either Monthly or Voluntary.");

            RuleFor(c => c.ContributionDate)
            .NotEmpty().WithMessage("Contribution date is required.")
            .MustAsync(IsValidContributionDate)
            .WithMessage("Invalid contribution date. Only one monthly contribution is allowed per calendar month.");
        }

        private async Task<bool> IsValidContributionDate(Contribution contribution, DateTime date, CancellationToken cancellationToken)
        {
            if (contribution.ContributionType == ContributionType.Monthly)
            {
                var existingContribution = await _context.Contributions
                    .AnyAsync(c => c.MemberId == contribution.MemberId &&
                                   c.ContributionType == ContributionType.Monthly &&
                                   c.ContributionDate.Year == date.Year &&
                                   c.ContributionDate.Month == date.Month,
                              cancellationToken);

                return !existingContribution;
            }
            return true; // Voluntary Contribution
        }

    }
}
