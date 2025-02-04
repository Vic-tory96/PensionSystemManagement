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
    public class BenefitMap
    {
        public BenefitMap(EntityTypeBuilder<Benefit> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);

            entityBuilder.Property(x => x.BenefitType)
           .IsRequired();  // Ensure that BenefitType is required

            entityBuilder.Property(x => x.CalculationDate)
                .IsRequired();  // Ensure that CalculationDate is required

            entityBuilder.Property(x => x.EligibilityStatus)
                .IsRequired();  // Ensure that EligibilityStatus is required


            entityBuilder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)")  // Ensure correct precision for amount
                .IsRequired();  // Ensure that Amount is required

            // Foreign key relationship with Member
            entityBuilder.HasOne(x => x.Member)
                .WithMany(m => m.Benefits)  // A Member can have many Benefits
                .HasForeignKey(x => x.MemberId)  // Foreign key for Member
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete to remove Benefits if Member is deleted

            // Relationship with TransactionHistory
            entityBuilder.HasMany(x => x.TransactionHistories)
                .WithOne(x => x.Benefit)  // Each TransactionHistory belongs to one Benefit
                .HasForeignKey(x => x.BenefitId)  // Foreign key for Benefit in TransactionHistory
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
