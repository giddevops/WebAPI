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
    public class SalesOrderInventoryItem
    {
        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        public int? InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
    }
    
    /// <summary>
    /// This sets up foreign keys in the database connecting SalesOrders and inventoryItems many to many relationship
    /// </summary>
    class SalesOrderInventoryItemDBConfiguration : IEntityTypeConfiguration<SalesOrderInventoryItem>
    {
        public void Configure(EntityTypeBuilder<SalesOrderInventoryItem> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.InventoryItemId, t.SalesOrderId });

            modelBuilder
                .HasOne(salesOrderInventoryItem => salesOrderInventoryItem.SalesOrder)
                .WithMany(SalesOrder => SalesOrder.InventoryItems)
                .HasForeignKey(salesOrderInventoryItem => salesOrderInventoryItem.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(salesOrderInventoryItem => salesOrderInventoryItem.InventoryItem)
                .WithOne(inventoryItem => inventoryItem.SalesOrderInventoryItem)
                // .HasForeignKey(salesOrderInventoryItem => salesOrderInventoryItem.InventoryItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
