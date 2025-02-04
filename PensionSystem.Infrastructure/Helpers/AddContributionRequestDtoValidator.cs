using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PensionSystem.Application.DTOS;

namespace PensionSystem.Infrastructure.Helpers
{
    public class AddContributionRequestDtoValidator : AbstractValidator<AddContributionRequestDto>
    {
        public AddContributionRequestDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("Email address is required.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
        }
    }
}
