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
    public class CompanyAddress
    {

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }

        public int CompanyAddressTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class CompanyAddressDBConfiguration : IEntityTypeConfiguration<CompanyAddress>
    {
        public void Configure(EntityTypeBuilder<CompanyAddress> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.CompanyId, t.AddressId });

            modelBuilder
                .HasOne(companyAddress => companyAddress.Address);
                // .WithMany(address => address.CompanyAddresses)
                // .HasForeignKey(companyAddress => companyAddress.AddressId);

            modelBuilder
                .HasOne(companyAddress => companyAddress.Company)
                .WithMany(company => company.Addresses)
                .HasForeignKey(companyAddress => companyAddress.CompanyId);
        }
    }
}
