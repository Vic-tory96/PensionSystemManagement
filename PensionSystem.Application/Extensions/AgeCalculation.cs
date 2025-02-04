using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.Extensions
{
    public static class AgeCalculation
    {
        public static bool IsValidAge(DateTime dateOfBirth, int minAge = 18, int maxAge = 70)
        {
            int age = DateTime.Today.Year - dateOfBirth.Year;

            // Adjust age if birthday hasn't occurred yet this year
            if (dateOfBirth.Date > DateTime.Today.AddYears(-age))
                age--;

            return age >= minAge && age <= maxAge;
        }
    }
}
