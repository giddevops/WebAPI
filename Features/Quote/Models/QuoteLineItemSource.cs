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
    public class QuoteLineItemSource
    {
        public int QuoteLineItemId { get; set; }
        public QuoteLineItem QuoteLineItem { get; set; }

        public int SourceId { get; set; }
        public Source Source { get; set; }

        public int? Quantity { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database 
    /// </summary>
    class QuoteLineItemSourceDBConfiguration : IEntityTypeConfiguration<QuoteLineItemSource>
    {
        public void Configure(EntityTypeBuilder<QuoteLineItemSource> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.QuoteLineItemId, t.SourceId });

            modelBuilder.HasOne(quoteLineItemSource => quoteLineItemSource.QuoteLineItem)
                .WithMany(l => l.Sources)
                .HasForeignKey(quoteLineItemSource => quoteLineItemSource.QuoteLineItemId);
            
            modelBuilder.HasOne(QuoteLineItemSource=> QuoteLineItemSource.Source)
                .WithMany(item => item.QuoteLineItems)
                .HasForeignKey(quoteLIneItemSource => quoteLIneItemSource.SourceId);
        }
    }
}
