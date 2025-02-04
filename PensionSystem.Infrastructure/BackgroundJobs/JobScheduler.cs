
using Hangfire;

namespace PensionSystem.Infrastructure.BackgroundJobs
{
    public class JobScheduler
    {
        private readonly PensionSystemJobService _pensionJobsService;

        public JobScheduler(PensionSystemJobService pensionJobsService)
        {
            _pensionJobsService = pensionJobsService;
        }

        public void ScheduleRecurringJobs()
        {
            RecurringJob.AddOrUpdate(
                recurringJobId: "BenefitEligibilityUpdate",
                methodCall: () => _pensionJobsService.BenefitEligibilityUpdate(),
                cronExpression: Cron.Monthly
            );

            RecurringJob.AddOrUpdate(
                recurringJobId: "GenerateMemberStatements",
                methodCall: () => _pensionJobsService.GenerateMemberStatements(),
                cronExpression: Cron.Monthly
            );

            RecurringJob.AddOrUpdate(
                recurringJobId: "MonthlyInterestCalculation",
                methodCall: () => _pensionJobsService.MonthlyInterestCalculation(),
                cronExpression: Cron.Monthly
            );

            RecurringJob.AddOrUpdate(
                recurringJobId: "MonthlyContributionValidationReport",
                methodCall: () => _pensionJobsService.MonthlyContributionValidationReport(),
                cronExpression: Cron.Monthly
            );
        }
    }

}
