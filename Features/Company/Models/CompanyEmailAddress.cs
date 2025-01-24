using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class CompanyEmailAddress
    {
        
        public int? EmailAddressId { get; set; }
        public EmailAddress EmailAddress { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? CompanyEmailAddressTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class CompanyEmailAddressDBConfiguration : IEntityTypeConfiguration<CompanyEmailAddress>
    {
        public void Configure(EntityTypeBuilder<CompanyEmailAddress> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.CompanyId, t.EmailAddressId });

            modelBuilder.HasIndex(item => item.CompanyId);

            modelBuilder
                .HasOne(companyEmailAddress => companyEmailAddress.EmailAddress);
                // .WithMany(emailAddress => emailAddress.CompanyEmailAddresses)
                // .HasForeignKey(companyAddress => companyAddress.EmailAddressId);

            modelBuilder
                .HasOne(companyEmailAddress => companyEmailAddress.Company)
                .WithMany(company => company.EmailAddresses)
                .HasForeignKey(companyEmailAddress => companyEmailAddress.CompanyId);
        }
    }
}
