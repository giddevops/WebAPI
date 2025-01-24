using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class Contact
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Boolean? AutoInserted { get; set; }

        public List<CompanyContact> CompanyContacts { get; set; }
        public List<ContactAddress> Addresses { get; set; }
        public List<ContactEmailAddress> EmailAddresses { get; set; }
        public List<ContactPhoneNumber> PhoneNumbers { get; set; }
        public List<ContactNote> Notes { get; set; }
        public List<ContactAttachment> Attachments { get; set; }
        public List<ContactEventLogEntry> EventLogEntries { get; set; }

        public string GetDefaultEmailAddress(){
            if(this.EmailAddresses.Count == 0)
                return null;
            return this.EmailAddresses.First().EmailAddress.Address;
        }
    }

    /// <summary>
    /// set up createdAt, fluent api config
    /// </summary>
    class ContactDBConfiguration : IEntityTypeConfiguration<Contact>
    {
        public void Configure(EntityTypeBuilder<Contact> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            
            modelBuilder.HasIndex(item => item.FirstName);
            modelBuilder.HasIndex(item => item.LastName);
            modelBuilder.HasIndex(item => item.CreatedAt);
        }
    }
}
