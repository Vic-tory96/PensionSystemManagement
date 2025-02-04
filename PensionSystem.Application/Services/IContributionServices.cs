using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PensionSystem.Application.DTOS;
using PensionSystem.Domain.Entities;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Application.Services
{
    public interface IContributionServices
    {
        Task AddMonthlyMandatoryContribution(Member member, decimal amount);
        Task AddVoluntaryContribution(Member member, decimal amount);

        Task<decimal> GetTotalContributionsByType(Member member, ContributionType type);
        Task<ContributionStatementDto> GenerateContributionStatement(Member member);

        Task<decimal> GetTotalContributions(Member member);
        Task<bool> HasMetMinimumContributionPeriod(Member member);
        Task<decimal> CalculateBenefit(Member member);
        Task<decimal?> CalculateMonthlyInterest(Member member);
        Task<decimal> GetTotalContributionsForMonth(Member member);
    }
}
