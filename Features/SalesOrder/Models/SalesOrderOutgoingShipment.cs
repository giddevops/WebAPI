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
    public class SalesOrderOutgoingShipment
    {
        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        public int? OutgoingShipmentId { get; set; }
        public OutgoingShipment OutgoingShipment { get; set; }
    }
    
    /// <summary>
    /// This sets up foreign keys in the database connecting SalesOrders and inventoryItems many to many relationship
    /// </summary>
    class SalesOrderOutgoingShipmentDBConfiguration : IEntityTypeConfiguration<SalesOrderOutgoingShipment>
    {
        public void Configure(EntityTypeBuilder<SalesOrderOutgoingShipment> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.OutgoingShipmentId, t.SalesOrderId });

            modelBuilder
                .HasOne(salesOrderOutgoingShipment => salesOrderOutgoingShipment.SalesOrder)
                .WithMany(SalesOrder => SalesOrder.OutgoingShipments)
                .HasForeignKey(salesOrderOutgoingShipment => salesOrderOutgoingShipment.SalesOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(salesOrderOutgoingShipment => salesOrderOutgoingShipment.OutgoingShipment)
                .WithOne(item => item.SalesOrderOutgoingShipment);
                // .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
