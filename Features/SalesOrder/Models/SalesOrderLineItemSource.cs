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
    public class SalesOrderLineItemSource
    {
        public int? SalesOrderLineItemId { get; set; }
        public SalesOrderLineItem SalesOrderLineItem { get; set; }

        public int? SourceId { get; set; }
        public Source Source { get; set; }

        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        public int? Quantity { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database 
    /// </summary>
    class SalesOrderLineItemSourceDBConfiguration : IEntityTypeConfiguration<SalesOrderLineItemSource>
    {
        public void Configure(EntityTypeBuilder<SalesOrderLineItemSource> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.SalesOrderLineItemId, t.SourceId });

            modelBuilder.HasOne(salesOrderLineItemSource => salesOrderLineItemSource.SalesOrderLineItem)
                .WithMany(l => l.Sources)
                .HasForeignKey(salesOrderLineItemSource => salesOrderLineItemSource.SalesOrderLineItemId);
            
            modelBuilder.HasOne(SalesOrderLineItemSource=> SalesOrderLineItemSource.Source)
                .WithMany(item => item.SalesOrderLineItems)
                .HasForeignKey(salesOrderLineItemSource => salesOrderLineItemSource.SourceId);

            modelBuilder.HasOne(salesOrderLineItemSource => salesOrderLineItemSource.PurchaseOrder)
                .WithMany(l => l.SalesOrderLineItemSources)
                .HasForeignKey(salesOrderLineItemSource => salesOrderLineItemSource.PurchaseOrderId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.HasOne(SalesOrderLineItemSource=> SalesOrderLineItemSource.Source)
                .WithMany(item => item.SalesOrderLineItems)
                .HasForeignKey(salesOrderLineItemSource => salesOrderLineItemSource.SourceId);
        }
    }
}
