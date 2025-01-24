using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for Companys and attachments
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class CompanyContact
    {
        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? ContactId { get; set; }
        public Contact Contact { get; set; }

        //this is just for when saving sometimes have this as a string. In that case, need to add it to the list of relationship types
        [NotMapped]
        public string RelationshipTypeString { get; set; }

        public int CompanyContactRelationshipTypeId { get; set; }
    }
    
    /// <summary>
    /// This sets up foreign keys in the database connecting Companys and attachments many to many relationship
    /// </summary>
    class CompanyContactDBConfiguration : IEntityTypeConfiguration<CompanyContact>
    {
        public void Configure(EntityTypeBuilder<CompanyContact> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.ContactId, t.CompanyId });

            modelBuilder
                .HasOne(companyContact => companyContact.Company)
                .WithMany(Company => Company.CompanyContacts)
                .HasForeignKey(companyContact => companyContact.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(companyContact => companyContact.Contact)
                .WithMany(attachment => attachment.CompanyContacts)
                .HasForeignKey(companyContact => companyContact.ContactId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
