using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and purchaseOrders
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class PurchaseOrderEventLogEntry
    {
        public int EventLogEntryId { get; set; }
        public EventLogEntry EventLogEntry { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and purchaseOrders many to many relationship
    /// </summary>
    class PurchaseOrderEventLogEntryDBConfiguration : IEntityTypeConfiguration<PurchaseOrderEventLogEntry>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderEventLogEntry> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.PurchaseOrderId, t.EventLogEntryId });
            
            modelBuilder
                .HasOne(purchaseOrderEventLogEntry => purchaseOrderEventLogEntry.EventLogEntry)
                .WithMany()
                .HasForeignKey(purchaseOrderEventLogEntry => purchaseOrderEventLogEntry.EventLogEntryId);
            modelBuilder
                .HasOne(purchaseOrderEventLogEntry => purchaseOrderEventLogEntry.PurchaseOrder)
                .WithMany(purchaseOrder => purchaseOrder.EventLogEntries)
                .HasForeignKey(purchaseOrderEventLogEntry => purchaseOrderEventLogEntry.PurchaseOrderId);
        }
    }
}
