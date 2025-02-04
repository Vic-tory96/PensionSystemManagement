using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PensionSystem.Domain.Entities;

namespace PensionSystem.Infrastructure.Extensions
{
    public class MonthlyContributionReportMap
    {
        public MonthlyContributionReportMap(EntityTypeBuilder<MonthlyContributionReport> entityBuilder)
        {
            entityBuilder.Property(m => m.FirstName)
             .HasMaxLength(50)
             .IsRequired();

            entityBuilder.Property(m => m.LastName)
                  .HasMaxLength(50)
                  .IsRequired();

            // Date configuration
            entityBuilder.Property(m => m.ReportDate)
                  .HasColumnType("datetime2");

            // Decimal precision for TotalContributions
            entityBuilder.Property(m => m.TotalContributions)
                  .HasColumnType("decimal(18,2)");

            // Unique index on both MemberId and ReportDate so that a member can have multiple report but only one report per month.
            entityBuilder.HasIndex(m => new { m.MemberId, m.ReportDate })
                .IsUnique();

        }
    }
}
