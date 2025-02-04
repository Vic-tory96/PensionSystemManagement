using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Domain.Entities
{
    public class Contribution : BaseEntity
    {
        public string MemberId { get; set; }
        public Member Member { get; set; }

        public ContributionType ContributionType { get; set; }  

        public decimal Amount { get; set; }
        public decimal Interest { get; set; } // New property
        public DateTime ContributionDate { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
