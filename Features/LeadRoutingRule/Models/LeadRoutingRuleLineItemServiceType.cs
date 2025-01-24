using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class LeadRoutingRuleLineItemServiceType {
        public LeadRoutingRule LeadRoutingRule { get; set; }
        public int? LeadRoutingRuleId { get; set; }

        public LineItemServiceType LineItemServiceType { get; set; }
        public int? LineItemServiceTypeId { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class LeadRoutingRuleLineItemServiceTypeDBConfiguration : IEntityTypeConfiguration<LeadRoutingRuleLineItemServiceType> {
        public void Configure(EntityTypeBuilder<LeadRoutingRuleLineItemServiceType> modelBuilder) {
            modelBuilder.HasKey(item => new { item.LeadRoutingRuleId, item.LineItemServiceTypeId });
            modelBuilder
                .HasOne(item => item.LeadRoutingRule)
                .WithMany(item => item.LineItemServiceTypes)
                .HasForeignKey(item => item.LeadRoutingRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(item => item.LineItemServiceType)
                .WithMany(item => item.LeadRoutingRules)
                .HasForeignKey(item => item.LineItemServiceTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}