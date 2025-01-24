using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class LeadRoutingRuleLeadWebsite {
        public LeadRoutingRule LeadRoutingRule { get; set; }
        public int? LeadRoutingRuleId { get; set; }

        public LeadWebsite LeadWebsite { get; set; }
        public int? LeadWebsiteId { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class LeadRoutingRuleLeadWebsiteDBConfiguration : IEntityTypeConfiguration<LeadRoutingRuleLeadWebsite> {
        public void Configure(EntityTypeBuilder<LeadRoutingRuleLeadWebsite> modelBuilder) {
            modelBuilder.HasKey(item => new { item.LeadRoutingRuleId, item.LeadWebsiteId });
            modelBuilder
                .HasOne(item => item.LeadRoutingRule)
                .WithMany(item => item.LeadWebsites)
                .HasForeignKey(item => item.LeadRoutingRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(item => item.LeadWebsite)
                .WithMany()
                .HasForeignKey(item => item.LeadWebsiteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}