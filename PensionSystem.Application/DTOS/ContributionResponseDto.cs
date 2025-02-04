using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOS
{
    public class ContributionResponseDto
    {
        public string ReferenceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string ContributionType { get; set; } = string.Empty;
        public DateTime ContributionDate { get; set; }
    }
}
