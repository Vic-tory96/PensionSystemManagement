using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Infrastructure.Helpers
{
    public class EmployerValidator : AbstractValidator<Employer>
    {
        public EmployerValidator()
        {
            RuleFor(e => e.CompanyName)
                .NotEmpty().WithMessage("Company name is required.")
                .MaximumLength(100).WithMessage("Company name cannot exceed 100 characters.");

            RuleFor(e => e.RegistrationNumber)
                .NotEmpty().WithMessage("Registration number is required.")
                .Matches(@"^[A-Za-z0-9]{8,12}$").WithMessage("Invalid registration number format.");

            RuleFor(e => e.ActiveStatus)
                .NotNull().WithMessage("Active status must be specified.");
        }
    }
}
