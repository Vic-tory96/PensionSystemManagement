using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Domain.Entities
{
    public class Benefit : BaseEntity
    {
        public string MemberId { get; set; }  
        public Member Member { get; set; }  
        public BenefitType BenefitType { get; set; } 
        public DateTime CalculationDate { get; set; }
        public EligiblityStatus EligibilityStatus { get; set; }  
        public decimal Amount { get; set; }
        public List<TransactionHistory> TransactionHistories { get; set; }
    }
}
