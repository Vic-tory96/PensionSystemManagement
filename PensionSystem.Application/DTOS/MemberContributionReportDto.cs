using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOS
{
    public class MemberContributionReportDto
    {
        public string MemberId { get; set; } = Guid.NewGuid().ToString();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsEligibleForBenefit { get; set; }
        public Decimal TotalContributions { get; set; }
    }
}
