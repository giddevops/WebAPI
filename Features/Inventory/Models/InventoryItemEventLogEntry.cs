using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and inventoryItems
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class InventoryItemEventLogEntry
    {
        public int? EventLogEntryId { get; set; }
        public EventLogEntry EventLogEntry { get; set; }

        public int? InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and inventoryItems many to many relationship
    /// </summary>
    class InventoryItemEventLogEntryDBConfiguration : IEntityTypeConfiguration<InventoryItemEventLogEntry>
    {
        public void Configure(EntityTypeBuilder<InventoryItemEventLogEntry> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.InventoryItemId, t.EventLogEntryId });

            modelBuilder
                .HasOne(inventoryItemEventLogEntry => inventoryItemEventLogEntry.EventLogEntry)
                .WithMany()
                .HasForeignKey(inventoryItemEventLogEntry => inventoryItemEventLogEntry.EventLogEntryId);

            modelBuilder
                .HasOne(inventoryItemEventLogEntry => inventoryItemEventLogEntry.InventoryItem)
                .WithMany(inventoryItem => inventoryItem.EventLogEntries)
                .HasForeignKey(inventoryItemEventLogEntry => inventoryItemEventLogEntry.InventoryItemId);
        }
    }
}
