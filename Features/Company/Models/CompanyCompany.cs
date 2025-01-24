using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models

{
	public class CompanyCompany
	{
        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? RelatedCompanyId { get; set; }
        public Company RelatedCompany { get; set; }

        //this is just for when saving sometimes have this as a string. In that case, need to add it to the list of relationship types
        [NotMapped]
        public string RelationshipTypeString { get; set; }

        public int CompanyCompanyRelationshipTypeId { get; set; }

    }

    /// <summary>
    /// This sets up foreign keys in the database connecting Companys and attachments many to many relationship
    /// </summary>
    class CompanyCompanyDBConfiguration : IEntityTypeConfiguration<CompanyCompany>
    {
        public void Configure(EntityTypeBuilder<CompanyCompany> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.CompanyId, t.RelatedCompanyId});

            modelBuilder
                .HasOne(companyCompany => companyCompany.Company)
                .WithMany(Company => Company.CompanyCompanies)
                .HasForeignKey(companyCompany => companyCompany.CompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(companyCompany => companyCompany.RelatedCompany)
                .WithMany(Company => Company.CompanyRelatedCompanies)
                .HasForeignKey(companyCompany => companyCompany.RelatedCompanyId);
        }
    }
}
