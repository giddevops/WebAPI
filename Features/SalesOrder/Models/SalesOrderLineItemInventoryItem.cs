using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for SalesOrders and inventoryItems
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class SalesOrderLineItemInventoryItem
    {
        public int? SalesOrderLineItemId { get; set; }
        public SalesOrderLineItem SalesOrderLineItem { get; set; }

        public int? InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
    }
    
    /// <summary>
    /// This sets up foreign keys in the database connecting SalesOrders and inventoryItems many to many relationship
    /// </summary>
    class SalesOrderLineItemInventoryItemDBConfiguration : IEntityTypeConfiguration<SalesOrderLineItemInventoryItem>
    {
        public void Configure(EntityTypeBuilder<SalesOrderLineItemInventoryItem> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.InventoryItemId, t.SalesOrderLineItemId });

            modelBuilder
                .HasOne(item => item.SalesOrderLineItem)
                .WithMany(item => item.InventoryItems)
                .HasForeignKey(item => item.SalesOrderLineItemId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(item => item.InventoryItem)
                .WithMany(item => item.SalesOrderLineItems)
                .HasForeignKey(item => item.InventoryItemId);
        }
    }
}
