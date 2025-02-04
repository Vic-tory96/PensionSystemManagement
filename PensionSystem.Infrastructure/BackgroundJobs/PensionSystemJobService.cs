using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using PensionSystem.Application.DTOS;
using PensionSystem.Application.Services;
using PensionSystem.Domain.Entities;
using PensionSystem.Infrastructure.DBContext;

namespace PensionSystem.Infrastructure.BackgroundJobs
{
    public class PensionSystemJobService
    {
        private readonly IContributionServices _contributionService;
        private readonly IMemberService _memberService;
        private readonly PensionSystemContext _context;
        private readonly ILogger<PensionSystemJobService> _logger;
        public PensionSystemJobService(IContributionServices contributionService, IMemberService memberService, PensionSystemContext context, ILogger<PensionSystemJobService> logger)
        {
            _contributionService = contributionService;
            _memberService = memberService;
            _context = context;
            _logger = logger;
        }

        public async Task BenefitEligibilityUpdate()
        {
            _logger.LogInformation("Benefit Eligibility Update job started at {Time}", DateTime.UtcNow);
            var members = await _memberService.GetAllMember();
            foreach (var member in members)
            {
                var isEligible = await _contributionService.HasMetMinimumContributionPeriod(member);
                member.IsEligibleForBenefit = isEligible;
                _logger.LogInformation("Processed Member ID {MemberId}: Eligibility - {Eligibility}", member.Id, isEligible);
            }
            await _memberService.SaveChangesAsync();
            _logger.LogInformation("Benefit Eligibility Update job completed at {Time}", DateTime.UtcNow);
        }

        public async Task GenerateMemberStatements()
        {
            _logger.LogInformation("Generate Member Statements job started at {Time}", DateTime.UtcNow);
            var members = await _memberService.GetAllMember();
            foreach (var member in members)
            {
                await _contributionService.GenerateContributionStatement(member);
                _logger.LogInformation("Generated statement for Member ID {MemberId}", member.Id);
            }
            _logger.LogInformation("Generate Member Statements job completed at {Time}", DateTime.UtcNow);
        }

        public async Task MonthlyInterestCalculation()
        {
            _logger.LogInformation("Monthly Interest Calculation job started at {Time}", DateTime.UtcNow);
            var members = await _memberService.GetAllMember();
            foreach (var member in members)
            {
                await _contributionService.CalculateMonthlyInterest(member);
                _logger.LogInformation("Calculated interest for Member ID {MemberId}", member.Id);
            }
            _logger.LogInformation("Monthly Interest Calculation job completed at {Time}", DateTime.UtcNow);
        }
        public async Task MonthlyContributionValidationReport()
        {
            _logger.LogInformation("Monthly Contribution Validation Report job started at {Time}", DateTime.UtcNow);
            var members = await _memberService.GetAllMember();
            var report = new List<MemberContributionReportDto>();

            foreach (var member in members)
            {
                var hasMetEligibility = await _contributionService.HasMetMinimumContributionPeriod(member);

                decimal totalContributions = 0;
                if (hasMetEligibility)
                {
                    totalContributions = await _contributionService.GetTotalContributionsForMonth(member);
                }

                report.Add(new MemberContributionReportDto
                {
                    MemberId = member.Id,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    TotalContributions = totalContributions,
                    IsEligibleForBenefit = hasMetEligibility
                });
                _logger.LogInformation("Added report entry for Member ID {MemberId}, Total Contributions: {TotalContributions}, Eligibility: {Eligibility}",
                 member.Id, totalContributions, hasMetEligibility);
            }

            await SaveMonthlyContributionReport(report);
            _logger.LogInformation("Monthly Contribution Validation Report job completed at {Time}", DateTime.UtcNow);
        }
        private async Task SaveMonthlyContributionReport(List<MemberContributionReportDto> report)
        {
            try
            {
                var reportEntities = report.Select(r => new MonthlyContributionReport
                {
                    MemberId = r.MemberId,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    TotalContributions = r.TotalContributions,
                    IsEligibleForBenefit = r.IsEligibleForBenefit,
                    ReportDate = DateTime.UtcNow
                }).ToList();

                await _context.MonthlyContributionReports.AddRangeAsync(reportEntities);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the Monthly Contribution Report.");
                throw; // Re-throw to let Hangfire capture it too
            }
        }

        //private async Task SaveMonthlyContributionReport(List<MemberContributionReportDto> report)
        //{
        //    var reportEntities = report.Select(r => new MonthlyContributionReport
        //    {
        //        MemberId = r.MemberId,
        //        FirstName = r.FirstName,
        //        LastName = r.LastName,
        //        TotalContributions = r.TotalContributions,
        //        IsEligibleForBenefit = r.IsEligibleForBenefit,
        //        ReportDate = DateTime.UtcNow
        //    }).ToList();

        //    await _context.MonthlyContributionReports.AddRangeAsync(reportEntities);
        //    await _context.SaveChangesAsync();
        //}


    }
}
