using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class OutgoingShipmentBoxInventoryItem
    {
        // public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        // public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int OutgoingShipmentBoxId { get; set; }
        public OutgoingShipmentBox OutgoingShipmentBox { get; set; }
        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class OutgoingShipmentBoxInventoryItemDBConfiguration : IEntityTypeConfiguration<OutgoingShipmentBoxInventoryItem>
    {
        public void Configure(EntityTypeBuilder<OutgoingShipmentBoxInventoryItem> modelBuilder)
        {
            modelBuilder
                .HasKey(t => new { t.OutgoingShipmentBoxId, t.InventoryItemId });

            modelBuilder
                .HasOne(outgoingShipmentBoxInventoryItem => outgoingShipmentBoxInventoryItem.OutgoingShipmentBox)
                .WithMany(OutgoingShipmentBox => OutgoingShipmentBox.InventoryItems)
                .HasForeignKey(outgoingShipmentBoxInventoryItem => outgoingShipmentBoxInventoryItem.OutgoingShipmentBoxId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(outgoingShipmentBoxInventoryItem => outgoingShipmentBoxInventoryItem.InventoryItem)
                .WithMany(item => item.OutgoingShipmentBoxes)
                .HasForeignKey(outgoingShipmentBoxInventoryItem => outgoingShipmentBoxInventoryItem.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}