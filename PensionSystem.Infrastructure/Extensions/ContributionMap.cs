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
    public class ContributionMap 
    {
        public ContributionMap(EntityTypeBuilder<Contribution> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x => x.Amount)
            .IsRequired()
            .HasColumnType("decimal(18,2)");  // Adjust decimal type precision as needed

            entityBuilder.Property(x => x.ContributionDate)
                .IsRequired();

            // Decimal precision for TotalContributions
            entityBuilder.Property(m => m.Interest)
                  .HasColumnType("decimal(18,2)");

            entityBuilder.Property(x => x.ReferenceNumber)
                .IsRequired()
                .HasMaxLength(10);  // Or any length you need

            
           
        }

    }
}
