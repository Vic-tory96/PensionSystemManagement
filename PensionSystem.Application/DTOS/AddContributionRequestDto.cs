using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOS
{
    public class AddContributionRequestDto
    {
        public decimal Amount { get; set; }
        public string Email { get; set; } 
    }
}
