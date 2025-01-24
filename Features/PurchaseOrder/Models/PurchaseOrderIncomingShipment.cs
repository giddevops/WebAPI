using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class PurchaseOrderIncomingShipment {
        // public int? Id { get; set; }
        // public DateTime? CreatedAt { get; set; }
        // public DateTime? UpdatedAt { get; set; }
        // public int? CreatedById { get; set; }

        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        public int? IncomingShipmentId { get; set; }
        public IncomingShipment IncomingShipment { get; set; }
    }

    class PurchaseOrderIncomingShipmentDBConfiguration : IEntityTypeConfiguration<PurchaseOrderIncomingShipment> {
        public void Configure(EntityTypeBuilder<PurchaseOrderIncomingShipment> modelBuilder) {
            modelBuilder.HasKey(t => new { t.PurchaseOrderId, t.IncomingShipmentId });
            modelBuilder
                .HasOne(purchaseOrderIncomingShipment => purchaseOrderIncomingShipment.IncomingShipment)
                .WithMany(incomingShipment => incomingShipment.PurchaseOrders)
                .HasForeignKey(purchaseOrderIncomingShipment => purchaseOrderIncomingShipment.IncomingShipmentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(purchaseOrderIncomingShipment => purchaseOrderIncomingShipment.PurchaseOrder)
                .WithMany(purchaseOrder => purchaseOrder.IncomingShipments)
                .HasForeignKey(purchaseOrderIncomingShipment => purchaseOrderIncomingShipment.PurchaseOrderId);
        }
    }
}
