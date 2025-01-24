using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class LeadRoutingRuleProductType {
        public LeadRoutingRule LeadRoutingRule { get; set; }
        public int? LeadRoutingRuleId { get; set; }

        public ProductType ProductType { get; set; }
        public int? ProductTypeId { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class LeadRoutingRuleProductTypeDBConfiguration : IEntityTypeConfiguration<LeadRoutingRuleProductType> {
        public void Configure(EntityTypeBuilder<LeadRoutingRuleProductType> modelBuilder) {
            modelBuilder.HasKey(item => new { item.LeadRoutingRuleId, item.ProductTypeId });
            modelBuilder
                .HasOne(item => item.LeadRoutingRule)
                .WithMany(item => item.ProductTypes)
                .HasForeignKey(item => item.LeadRoutingRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(item => item.ProductType)
                .WithMany()
                .HasForeignKey(item => item.ProductTypeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}