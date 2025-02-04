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
    public class EmployerMap
    {
        public EmployerMap(EntityTypeBuilder<Employer> entityBuilder)
        {
            entityBuilder.HasKey(x => x.Id);
            entityBuilder.Property( x => x.CompanyName)
                .IsRequired()
                .HasMaxLength(100);
            entityBuilder.Property(x => x.RegistrationNumber)
                .IsRequired()
                .HasMaxLength(12)
                .IsUnicode(false);
            entityBuilder.Property(x => x.ActiveStatus)
                .HasDefaultValue(true);

            entityBuilder.HasMany(x => x.Members)
                .WithOne(x => x.Employer)
                .HasForeignKey(x => x.EmployerId);

        }
    }
}
