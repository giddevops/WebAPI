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
    /// </summary>
    public class LeadLineItemSource
    {
        public int? LeadLineItemId { get; set; }
        public LeadLineItem LeadLineItem { get; set; }

        public int? SourceId { get; set; }
        public Source Source { get; set; }
        
        public int? Quantity { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database 
    /// </summary>
    class LeadLineItemSourceDBConfiguration : IEntityTypeConfiguration<LeadLineItemSource>
    {
        public void Configure(EntityTypeBuilder<LeadLineItemSource> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.LeadLineItemId, t.SourceId });

            modelBuilder.HasOne(leadLineItemSource => leadLineItemSource.LeadLineItem)
                .WithMany(l => l.Sources)
                .HasForeignKey(leadLineItemSource => leadLineItemSource.LeadLineItemId);
            
            modelBuilder.HasOne(LeadLineItemSource=> LeadLineItemSource.Source)
                .WithMany()
                .HasForeignKey(leadLIneItemSource => leadLIneItemSource.SourceId);
        }
    }
}