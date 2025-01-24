using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class OutgoingShipment {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public DateTime? ShippedAt { get; set; }
        public Address ShippingAddress { get; set; }

        public int? ShippingCarrierId { get; set; }
        public ShippingCarrier ShippingCarrier { get; set; }
        public int? ShippingCarrierShippingMethodId { get; set; }
        public ShippingCarrierShippingMethod ShippingCarrierShippingMethod { get; set; }

        public int? OutgoingShipmentShippingTermOptionId { get; set; }
        public OutgoingShipmentShippingTermOption OutgoingShipmentShippingTermOption { get; set; }

        public DateTime? ExpectedArrival { get; set; }
        public DateTime? ActualArrival { get; set; }

        [NotMapped]
        public User Preparer { get; set; }

        public string PackingSlipSpecialNotes { get; set; }
        public string CommercialInvoiceSpecialInstructions { get; set; }
        public string LicenseRequired { get; set; }
        public string ECCN { get; set; }

        public List<OutgoingShipmentShipmentTrackingEvent> TrackingEvents { get; set; }
        public SalesOrderOutgoingShipment SalesOrderOutgoingShipment { get; set; }
        public RmaOutgoingShipment RmaOutgoingShipment { get; set; }


        public List<OutgoingShipmentBox> Boxes { get; set; }

        public List<SalesOrderLineItem> GetSalesOrderLineItems() {
            var inventoryItems = this.Boxes.SelectMany(item => item.InventoryItems.Select(item2 => item2.InventoryItem));
            var salesOrderLineItemIds = inventoryItems.SelectMany(item => item.SalesOrderLineItems.Select(item2 => item2.SalesOrderLineItemId));
            var salesOrderLineItemCounts = salesOrderLineItemIds.GroupBy(
                item => item,
                (key, result) => new { SalesOrderLineItemId = key, Count = result.Count() }
            ).ToDictionary(item => item.SalesOrderLineItemId, item => item.Count);
            var salesOrderLineItems = this.SalesOrderOutgoingShipment.SalesOrder.LineItems;
            salesOrderLineItems.ForEach(item => item.Quantity = salesOrderLineItemCounts[item.Id]);
            return salesOrderLineItems;
        }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class OutgoingShipmentDBConfiguration : IEntityTypeConfiguration<OutgoingShipment> {
        public void Configure(EntityTypeBuilder<OutgoingShipment> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(item => item.OutgoingShipmentShippingTermOption)
            .WithMany()
            .HasForeignKey(item => item.OutgoingShipmentShippingTermOptionId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasMany(item => item.Boxes)
                .WithOne(item => item.OutgoingShipment)
                .HasForeignKey(item => item.OutgoingShipmentId);

            modelBuilder.HasOne(item => item.ShippingCarrier)
                .WithMany()
                .HasForeignKey(item => item.ShippingCarrierId);

            modelBuilder.HasOne(item => item.ShippingCarrierShippingMethod)
                .WithMany()
                .HasForeignKey(item => item.ShippingCarrierShippingMethodId);
        }
    }
}