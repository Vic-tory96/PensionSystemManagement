using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Domain.Entities
{
    public class TransactionHistory : BaseEntity
    {
        public string BenefitId { get; set; }  
        public Benefit Benefit { get; set; }  

        public DateTime TransactionDate { get; set; }  // Date of the transaction/update
        public string Description { get; set; }  // Description of the transaction (e.g., "Benefit Amount Updated")
        public decimal AmountBefore { get; set; }  // Amount before the transaction/update
        public decimal AmountAfter { get; set; }  // Amount after the transaction/update
        public string UserWhoPerformedAction { get; set; }
    }
}
