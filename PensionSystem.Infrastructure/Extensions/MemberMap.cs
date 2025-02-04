using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PensionSystem.Domain.Entity;

namespace PensionSystem.Infrastructure.Extensions
{
    public class MemberMap
    {
        public MemberMap(EntityTypeBuilder<Member> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.Property(x => x.FirstName).IsRequired();
            entityBuilder.Property(x => x.LastName).IsRequired();
            entityBuilder.Property(x => x.Email).IsRequired();
            entityBuilder.Property(x => x.DateOfBirth).IsRequired();
            entityBuilder.Property(x => x.PhoneNumber);
            entityBuilder.Property(x => x.IsDeleleted).HasDefaultValue(false);
            entityBuilder.Property(x => x.PasswordHash).IsRequired();


            entityBuilder.HasMany(x => x.Contributions)
               .WithOne(x => x.Member)  // A contribution belongs to one member
               .HasForeignKey(x => x.MemberId)  // The foreign key is MemberId
               .OnDelete(DeleteBehavior.Cascade);

            entityBuilder.HasMany(x =>x.Benefits)
                .WithOne(x =>x.Member)
                .HasForeignKey(x => x.MemberId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
