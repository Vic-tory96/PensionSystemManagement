using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PensionSystem.Domain.Entities;

namespace PensionSystem.Infrastructure.Helpers
{
    public class TransactionHistoryValidator : AbstractValidator<TransactionHistory>
    {
        public TransactionHistoryValidator()
        {
            RuleFor(x => x.BenefitId)
                .NotEmpty().WithMessage("BenefitId is required.");

            RuleFor(x => x.TransactionDate)
                .NotEmpty().WithMessage("TransactionDate is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("TransactionDate cannot be in the future.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.AmountBefore)
                .GreaterThanOrEqualTo(0).WithMessage("AmountBefore must be greater than or equal to zero.");

            RuleFor(x => x.AmountAfter)
                .GreaterThanOrEqualTo(0).WithMessage("AmountAfter must be greater than or equal to zero.");

            RuleFor(x => x.UserWhoPerformedAction)
                .NotEmpty().WithMessage("UserWhoPerformedAction is required.");
        }
    }

}
