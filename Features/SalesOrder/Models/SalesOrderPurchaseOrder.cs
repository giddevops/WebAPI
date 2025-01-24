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
    /// This class creates the joining table for SalesOrders and purchaseOrders
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class SalesOrderPurchaseOrder
    {
        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }
    
    /// <summary>
    /// This sets up foreign keys in the database connecting SalesOrders and purchaseOrders many to many relationship
    /// </summary>
    class SalesOrderPurchaseOrderDBConfiguration : IEntityTypeConfiguration<SalesOrderPurchaseOrder>
    {
        public void Configure(EntityTypeBuilder<SalesOrderPurchaseOrder> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.PurchaseOrderId, t.SalesOrderId });

            modelBuilder
                .HasOne(salesOrderPurchaseOrder => salesOrderPurchaseOrder.SalesOrder)
                .WithMany(SalesOrder => SalesOrder.PurchaseOrders)
                .HasForeignKey(salesOrderPurchaseOrder => salesOrderPurchaseOrder.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(salesOrderPurchaseOrder => salesOrderPurchaseOrder.PurchaseOrder)
                .WithMany(purchaseOrder => purchaseOrder.SalesOrders)
                .HasForeignKey(salesOrderPurchaseOrder => salesOrderPurchaseOrder.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
