using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PensionSystem.Application.DTOS
{
    public class ContributionStatementDto
    {
        public string MemberId { get; set; } = string.Empty;
        public decimal TotalMandatoryContributions { get; set; }
        public decimal TotalVoluntaryContributions { get; set; }
        public decimal TotalContributions { get; set; }
    }
}
