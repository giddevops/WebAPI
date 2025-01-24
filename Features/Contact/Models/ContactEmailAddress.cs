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
    public class ContactEmailAddress
    {
        public int? EmailAddressId { get; set; }
        public EmailAddress EmailAddress { get; set; }

        public int? ContactId { get; set; }
        public Contact Contact { get; set; }

        public int? ContactEmailAddressTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class ContactEmailAddressDBConfiguration : IEntityTypeConfiguration<ContactEmailAddress>
    {
        public void Configure(EntityTypeBuilder<ContactEmailAddress> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ContactId, t.EmailAddressId });

            modelBuilder
                .HasOne(contactEmailAddress => contactEmailAddress.EmailAddress);
                // .WithMany(emailAddress => emailAddress.ContactEmailAddresses)
                // .HasForeignKey(companyPhoneNumber => companyPhoneNumber.EmailAddressId);

            modelBuilder.HasIndex(item => item.ContactId);

            modelBuilder
                .HasOne(contactEmailAddress => contactEmailAddress.Contact)
                .WithMany(contact => contact.EmailAddresses)
                .HasForeignKey(contactEmailAddress => contactEmailAddress.ContactId);
        }
    }
}
