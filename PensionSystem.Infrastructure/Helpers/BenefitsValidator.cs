using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PensionSystem.Domain.Entities;

namespace PensionSystem.Infrastructure.Helpers
{
    public class BenefitValidator : AbstractValidator<Benefit>
    {
        public BenefitValidator()
        {
            RuleFor(x => x.MemberId)
                .NotEmpty().WithMessage("MemberId is required.");

            RuleFor(x => x.BenefitType)
                .IsInEnum().WithMessage("BenefitType must be a valid value (Retirement, Disability, Survivor).");

            RuleFor(x => x.CalculationDate)
                .NotEmpty().WithMessage("CalculationDate is required.")
                .LessThanOrEqualTo(DateTime.Now).WithMessage("CalculationDate cannot be in the future.");

            RuleFor(x => x.EligibilityStatus)
                .IsInEnum().WithMessage("EligibilityStatus must be a valid value.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than zero.");

            // Victory don't forget to add rules for the TransactionHistories if needed, for example:
            //RuleForEach(x => x.TransactionHistories)
            //    .SetValidator(new TransactionHistoryValidator());  // This would be a separate validator for TransactionHistory if you want to validate its properties
        }
    }

}
