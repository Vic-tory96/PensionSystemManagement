using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Domain.Entities
{
    public class MonthlyContributionReport : BaseEntity
    {
        public string MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public decimal TotalContributions { get; set; }
        public bool IsEligibleForBenefit { get; set; }
        public DateTime ReportDate { get; set; }
    }

}
