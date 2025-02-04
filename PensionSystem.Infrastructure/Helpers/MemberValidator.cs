using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FluentValidation;
using PensionSystem.Application.Extensions;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Infrastructure.Validation
{
    public class MemberValidator : AbstractValidator<Member>
    {
        public MemberValidator()
        {
            RuleFor(m => m.FirstName)
                .NotEmpty().WithMessage("First name is required.");

            RuleFor(m => m.LastName)
                    .NotEmpty().WithMessage("Last name is required.");

            RuleFor(m => m.DateOfBirth)
                    .NotEmpty().WithMessage("Date of Birth is required.")
                    .Must(dob => AgeCalculation.IsValidAge(dob))
                    .WithMessage("Age must be between 18 and 70 years.");

            RuleFor(m => m.Email)
                    .NotEmpty().WithMessage("Email is required.")
                    .EmailAddress().WithMessage("Invalid email format.");

            //RuleFor(m => m.PhoneNumber)
            //        .Matches(@"^\+?\d{10,15}$").WithMessage("Invalid phone number format.");
            RuleFor(m => m.PhoneNumber)
                    .NotEmpty().WithMessage("Phone number is required.")
                    .Matches(@"^(070|080|081|090|091)\d{8}$")
                   .WithMessage("Invalid phone number format. It should be 11 digits.");
        }


    }
    
}
