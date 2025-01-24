using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class LeadRoutingRuleCountry {
        public LeadRoutingRule LeadRoutingRule { get; set; }
        public int? LeadRoutingRuleId { get; set; }

        public Country Country { get; set; }
        public int? CountryId { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class LeadRoutingRuleCountryDBConfiguration : IEntityTypeConfiguration<LeadRoutingRuleCountry> {
        public void Configure(EntityTypeBuilder<LeadRoutingRuleCountry> modelBuilder) {
            modelBuilder.HasKey(item => new { item.LeadRoutingRuleId, item.CountryId });
            modelBuilder
                .HasOne(item => item.LeadRoutingRule)
                .WithMany(item => item.Countries)
                .HasForeignKey(item => item.LeadRoutingRuleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(item => item.Country)
                .WithMany()
                .HasForeignKey(item => item.CountryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}