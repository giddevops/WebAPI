using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public enum LeadRoutingRuleIncludeOptions {
        All = 1,
        OnlySpecified = 2,
        AllExceptSpecified = 3,
    }

    public class LeadRoutingRule {
        public int? Id { get; set; }
        public int? UserId { get; set; }

        public int? ProductTypeIncludeOptionId { get; set; }
        public int? LeadWebsiteIncludeOptionId { get; set; }
        public int? CountryIncludeOptionId { get; set; }
        // public int? testId;
        // public int? bobohead;

        public List<LeadRoutingRuleProductType> ProductTypes { get; set; }
        public List<LeadRoutingRuleLeadWebsite> LeadWebsites { get; set; }
        public List<LeadRoutingRuleLineItemServiceType> LineItemServiceTypes { get; set; }
        public List<LeadRoutingRuleCompanyName> CompanyNames { get; set; }
        public int? SBCDailyLimit { get; set; }
        public int? DailyLeadLimit { get; set; }
        public List<LeadRoutingRuleCountry> Countries { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class LeadRoutingRuleDBConfiguration : IEntityTypeConfiguration<LeadRoutingRule> {
        public void Configure(EntityTypeBuilder<LeadRoutingRule> modelBuilder) {
            modelBuilder.HasMany(item => item.CompanyNames).WithOne(item => item.LeadRoutingRule).HasForeignKey(item => item.LeadRoutingRuleId).OnDelete(DeleteBehavior.Cascade);
            // modelBuilder.Property(item => item.ProductTypeIncludeOptionId)
            // .HasColumnName("ProductTypeIncludeOptionId");
            // modelBuilder.Property(item => item.LeadWebsiteIncludeOptionId)
            // .HasColumnName("LeadWebsiteIncludeOptionId");
            // modelBuilder.Property(item => item.CountryIncludeOptionId)
            // .HasColumnName("CountryIncludeOptionId");
        }
    }
}