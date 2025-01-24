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
    public class InventoryItemRelatedInventoryItem
    {
        public DateTime? CreatedAt { get; set; }
        public int? ParentInventoryItemId { get; set; }
        public InventoryItem ParentInventoryItem { get; set; }

        public int? ChildInventoryItemId { get; set; }
        public InventoryItem ChildInventoryItem { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and inventoryItems many to many relationship
    /// </summary>
    class InventoryItemRelatedInventoryItemDBConfiguration : IEntityTypeConfiguration<InventoryItemRelatedInventoryItem>
    {
        public void Configure(EntityTypeBuilder<InventoryItemRelatedInventoryItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ParentInventoryItemId, t.ChildInventoryItemId });

            modelBuilder
                .HasOne(inventoryItemEventLogEntry => inventoryItemEventLogEntry.ParentInventoryItem)
                .WithMany(item => item.ChildRelatedInventoryItems)
                .HasForeignKey(item => item.ParentInventoryItemId)
                // .HasK(item => item.ParentRelatedInventoryItemId)
                // .HasForeignKey(inventoryItemEventLogEntry => inventoryItemEventLogEntry.ParentInventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(item => item.ChildInventoryItem)
                .WithOne(ii => ii.ParentRelatedInventoryItem)
                // .HasForeignKey(item => item.ChildInventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
