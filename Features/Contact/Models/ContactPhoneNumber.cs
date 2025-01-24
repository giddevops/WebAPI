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
    public class ContactPhoneNumber
    {
        public int? PhoneNumberId { get; set; }
        public PhoneNumber PhoneNumber { get; set; }

        public int? ContactId { get; set; }
        public Contact Contact { get; set; }

        public int? ContactPhoneNumberTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database
    /// </summary>
    class ContactPhoneNumberDBConfiguration : IEntityTypeConfiguration<ContactPhoneNumber>
    {
        public void Configure(EntityTypeBuilder<ContactPhoneNumber> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ContactId, t.PhoneNumberId });

            modelBuilder
                .HasOne(contactPhoneNumber => contactPhoneNumber.PhoneNumber);
                // .WithMany(phoneNumber => phoneNumber.ContactPhoneNumbers)
                // .HasForeignKey(companyPhoneNumber => companyPhoneNumber.PhoneNumberId);

            modelBuilder.HasIndex(item => item.ContactId);
            
            modelBuilder
                .HasOne(contactPhoneNumber => contactPhoneNumber.Contact)
                .WithMany(contact => contact.PhoneNumbers)
                .HasForeignKey(contactPhoneNumber => contactPhoneNumber.ContactId);
        }
    }
}
