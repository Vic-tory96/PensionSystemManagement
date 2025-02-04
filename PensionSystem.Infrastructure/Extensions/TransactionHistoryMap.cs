using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using PensionSystem.Domain.Entities;

namespace PensionSystem.Infrastructure.Extensions
{
    public class TransactionHistoryMap
    {
        public TransactionHistoryMap(EntityTypeBuilder<TransactionHistory> entityBuilder)
        {
            // Primary key
            entityBuilder.HasKey(x => x.Id);

            // Properties configuration
            entityBuilder.Property(x => x.TransactionDate)
                .IsRequired();  // Ensure that TransactionDate is required

            entityBuilder.Property(x => x.Description)
                .IsRequired()  // Ensure that Description is required
                .HasMaxLength(500);  // Set a maximum length for Description (adjust as necessary)

            entityBuilder.Property(x => x.AmountBefore)
                .HasColumnType("decimal(18,2)")  // Ensure correct precision for AmountBefore
                .IsRequired();  // Ensure that AmountBefore is required

            entityBuilder.Property(x => x.AmountAfter)
                .HasColumnType("decimal(18,2)")  // Ensure correct precision for AmountAfter
                .IsRequired();  // Ensure that AmountAfter is required

            entityBuilder.Property(x => x.UserWhoPerformedAction)
                .HasMaxLength(100);  // Set a maximum length for UserWhoPerformedAction (adjust as necessary)

            // Foreign key relationship with Benefit
            entityBuilder.HasOne(x => x.Benefit)
                .WithMany(b => b.TransactionHistories)  // A Benefit can have many TransactionHistories
                .HasForeignKey(x => x.BenefitId)  // Foreign key for Benefit
                .OnDelete(DeleteBehavior.Cascade);  // Cascade delete: if Benefit is deleted, related TransactionHistories are deleted
        }
    }
}
