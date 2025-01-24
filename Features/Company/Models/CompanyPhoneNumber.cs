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
    public class CompanyPhoneNumber
    {
        
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        //CompanyPhoneNumberType
        public int? CompanyPhoneNumberTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database
    /// </summary>
    class CompanyPhoneNumberDBConfiguration : IEntityTypeConfiguration<CompanyPhoneNumber>
    {
        public void Configure(EntityTypeBuilder<CompanyPhoneNumber> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.CompanyId, t.PhoneNumberId });

            modelBuilder.HasIndex(item => item.CompanyId);

            modelBuilder
                .HasOne(companyPhoneNumber => companyPhoneNumber.PhoneNumber)
                .WithMany(phoneNumber => phoneNumber.CompanyPhoneNumbers)
                .HasForeignKey(companyPhoneNumber => companyPhoneNumber.PhoneNumberId);

            modelBuilder
                .HasOne(companyPhoneNumber => companyPhoneNumber.Company)
                .WithMany(company => company.PhoneNumbers)
                .HasForeignKey(companyPhoneNumber => companyPhoneNumber.CompanyId);
        }
    }
}
