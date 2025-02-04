using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entities;
using PensionSystem.Domain.Entity;
using PensionSystem.Infrastructure.Extensions;

namespace PensionSystem.Infrastructure.DBContext
{
    public class PensionSystemContext : DbContext
    {
        public PensionSystemContext(DbContextOptions<PensionSystemContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Contribution> Contributions { get; set; }
        public DbSet<Benefit> Benefits { get; set; }
        public DbSet<TransactionHistory> TransactionHistories { get; set; }
        public DbSet<MonthlyContributionReport> MonthlyContributionReports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            new MemberMap(modelBuilder.Entity<Member>());
            new EmployerMap(modelBuilder.Entity<Employer>());
            new ContributionMap(modelBuilder.Entity<Contribution>());
            new BenefitMap(modelBuilder.Entity<Benefit>());
            new TransactionHistoryMap(modelBuilder.Entity<TransactionHistory>());
            new MonthlyContributionReportMap(modelBuilder.Entity<MonthlyContributionReport>());
        }

    }   
}
