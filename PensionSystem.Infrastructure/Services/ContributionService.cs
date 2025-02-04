
using Microsoft.EntityFrameworkCore;
using PensionSystem.Application.DTOS;
using PensionSystem.Application.Services;
using PensionSystem.Domain.Entities;
using PensionSystem.Domain.Entity;
using PensionSystem.Infrastructure.DBContext;

namespace PensionSystem.Infrastructure.Services
{
    public class ContributionService : IContributionServices
    {
        private readonly PensionSystemContext _context;
        public ContributionService(PensionSystemContext context)
        {
            _context = context;
        }
        public async Task AddMonthlyMandatoryContribution(Member member, decimal amount)
        {
            var exitingContribution = await _context.Contributions
                .Where(x => x.MemberId == member.Id
                && x.ContributionDate.Month == DateTime.Now.Month
                && x.ContributionType == ContributionType.Monthly).FirstOrDefaultAsync();

            if (exitingContribution != null)
            {
                throw new InvalidOperationException("Only one mandatory contribution allowed per month.");
            }

            var contribution = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Monthly,
                Amount = amount,
                ReferenceNumber = GenerateReferenceNumber()
            };
            _context.Contributions.Add(contribution);
            await _context.SaveChangesAsync();
        }

        private string GenerateReferenceNumber()
        {
            var guid = Guid.NewGuid().ToString("N"); // "N" format removes hyphens
            return guid.Substring(0, 10).ToUpper(); // Truncate to 10 characters and convert to uppercase
        }

        public async Task AddVoluntaryContribution(Member member, decimal amount)
        {
            var contribution = new Contribution
            {
                MemberId = member.Id,
                ContributionDate = DateTime.Now,
                ContributionType = ContributionType.Voluntary,
                Amount = amount,
                ReferenceNumber = GenerateReferenceNumber()
            };
            _context.Contributions.Add(contribution);
            await _context.SaveChangesAsync();
        }

        public async Task<decimal> GetTotalContributionsByType(Member member, ContributionType type)
        {
            return await _context.Contributions
                .Where(c => c.MemberId == member.Id && c.ContributionType == type)
                .SumAsync(c => c.Amount);  // This sums the Amount for the given type of contribution
        }

        public async Task<decimal> GetTotalContributionsForMonth(Member member)
        {
            return await _context.Contributions
                .Where(c => c.MemberId == member.Id
                            && c.ContributionDate.Month == DateTime.Now.Month
                            && c.ContributionDate.Year == DateTime.Now.Year)  // Ensures contributions are only from the current month
                .SumAsync(c => c.Amount);
        }

        public async Task<ContributionStatementDto> GenerateContributionStatement(Member member)
        {
            var mandatoryContributions = await GetTotalContributionsByType(member, ContributionType.Monthly);
            var voluntaryContributions = await GetTotalContributionsByType(member, ContributionType.Voluntary);

            return new ContributionStatementDto
            {
                MemberId = member.Id,
                TotalMandatoryContributions = mandatoryContributions,
                TotalVoluntaryContributions = voluntaryContributions,
                TotalContributions = mandatoryContributions + voluntaryContributions
            };
        }

        // Method to calculate total contributions for member
        public async Task<decimal> GetTotalContributions(Member member)
        {
            // Calculate the total contributions for the member
            return await _context.Contributions
                .Where(c => c.MemberId == member.Id)
                .SumAsync(c => c.Amount);
        }

        // Method to check if the member has met the minimum contribution period
        public async Task<bool> HasMetMinimumContributionPeriod(Member member)
        {
            var contributionStartDate = member.Contributions
                .Where(c => c.MemberId == member.Id)
                .OrderBy(c => c.ContributionDate)
                .FirstOrDefault();

            // Check if the member has made contributions for at least 1 year
            return contributionStartDate != null && DateTime.Now.Year - contributionStartDate.ContributionDate.Year >= 1;
        }

        // Method to calculate the benefit based on total contributions
        public async Task<decimal> CalculateBenefit(Member member)
        {
            // Check if the member meets the minimum contribution period
            bool hasMetMinimumPeriod = await HasMetMinimumContributionPeriod(member);
            if (!hasMetMinimumPeriod)
            {
                throw new InvalidOperationException("Member is not eligible for benefits due to insufficient contribution period.");
            }

            // Calculate total contributions
            var totalContributions = await GetTotalContributions(member);

            // Calculate benefit based on total contributions
            return totalContributions * 0.05m; // Example: 5% of total contributions
        }

        // Come back to this ....
        public async Task<decimal?> CalculateMonthlyInterest(Member member) 
        {
            const decimal monthlyInterestRate = 0.01m; // 1% monthly interest rate
            var startOfMonth = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Get all contributions for the member in the current month
            var monthlyContributions = await _context.Contributions
                .Where(c => c.MemberId == member.Id && c.ContributionDate >= startOfMonth && c.ContributionDate <= endOfMonth)
                .ToListAsync();

            if (!monthlyContributions.Any())
                return null ; // No contributions made this month

            // Calculate the total contributions for the month
            var totalContributions = monthlyContributions.Sum(c => c.Amount);

            // Calculate total interest for the month
            var totalInterest = totalContributions * monthlyInterestRate;

            // Add the interest to the member's latest contribution for record-keeping
            var latestContribution = monthlyContributions
                .OrderByDescending(c => c.ContributionDate)
                .FirstOrDefault();

            if (latestContribution != null)
            {
                latestContribution.Interest += totalInterest;
            }

            await _context.SaveChangesAsync(); // Persist changes
            return totalInterest;
        }


        //public async Task CalculateMonthlyInterest(Member member)
        //{
        //    //Using Simple Interest fomula : Interest = Principal * Rate * Time
        //    //Where: Principal = Total Contibutions, 
        //    //       Rate = Monthly Interest Rate(e.g 1% - 0.01) 
        //    //       Time = 1 month(since it's monthly interest)

        //    const decimal monthlyInterestRate = 0.01m; // 1% monthly interest rate

        //    var contributions = await _context.Contributions
        //        .Where(c => c.MemberId == member.Id)
        //        .ToListAsync();

        //    foreach (var contribution in contributions)
        //    {
        //        // Calculate interest for each contribution
        //        var interest = contribution.Amount * monthlyInterestRate;
        //        contribution.Interest += interest; // Accumulate interest
        //    }

        //    await _context.SaveChangesAsync(); // Persist changes
        //}



    }
}
