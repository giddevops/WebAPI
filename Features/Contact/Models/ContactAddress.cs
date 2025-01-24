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
    public class ContactAddress
    {
        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public int ContactId { get; set; }
        public Contact Contact { get; set; }

        public int ContactAddressTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class ContactAddressDBConfiguration : IEntityTypeConfiguration<ContactAddress>
    {
        public void Configure(EntityTypeBuilder<ContactAddress> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ContactId, t.AddressId });

            modelBuilder
                .HasOne(contactAddress => contactAddress.Address);
                // .WithMany(address => address.ContactAddresses)
                // .HasForeignKey(companyPhoneNumber => companyPhoneNumber.AddressId);

            modelBuilder
                .HasOne(contactAddress => contactAddress.Contact)
                .WithMany(contact => contact.Addresses)
                .HasForeignKey(contactAddress => contactAddress.ContactId);
        }
    }
}
