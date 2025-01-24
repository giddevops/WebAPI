using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class IncomingShipmentInventoryItem
    {
        public int? IncomingShipmentId { get; set; }
        public IncomingShipment IncomingShipment { get; set; }

        public int? InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public DateTime? ReceivedAt { get; set; }
        public int? ReceivedById { get; set; }
    }
    
    class IncomingShipmentInventoryItemDBConfiguration : IEntityTypeConfiguration<IncomingShipmentInventoryItem>
    {
        public void Configure(EntityTypeBuilder<IncomingShipmentInventoryItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.IncomingShipmentId, t.InventoryItemId });

            modelBuilder
                .HasOne(incomingShipmentInventoryItem => incomingShipmentInventoryItem.InventoryItem)
                .WithMany(inventoryItem => inventoryItem.IncomingShipmentInventoryItems)
                .HasForeignKey(IncomingShipmentInventoryItem => IncomingShipmentInventoryItem.InventoryItemId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(incomingShipmentInventoryItem => incomingShipmentInventoryItem.IncomingShipment)
                .WithMany(incomingShipment => incomingShipment.InventoryItems)
                .HasForeignKey(incomingShipmentInventoryItem => incomingShipmentInventoryItem.IncomingShipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}