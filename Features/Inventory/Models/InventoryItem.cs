using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    enum InventoryItemLocationOptions {
        Receiving = 1,
        Shipped = 2,
        MainWarehouse = 3
    }
    public class InventoryItem {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public int? ManufacturerId { get; set; }
        [ForeignKey("ManufacturerId")]
        public Company Manufacturer { get; set; }

        public string Description { get; set; }
        // public bool? Serialized { get; set; }

        // public bool Committed { get; set; }

        public string SerialNumber { get; set; }
        public int? InventoryItemLocationOptionId { get; set; }
        public int? ProductConditionOptionId { get; set; }
        public int InventoryItemStatusOptionId { get; set; }

        public int? GidLocationOptionId { get; set; }

        public int? GidSubLocationOptionId { get; set; }
        public GidSubLocationOption GidSubLocationOption { get; set; }

        public int? PurchaseOrderLineItemId { get; set; }
        public PurchaseOrderLineItem PurchaseOrderLineItem { get; set; }

        public int? CurrencyOptionId { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }

        // public int? SalesOrderId { get; set; }

        public SalesOrderInventoryItem SalesOrderInventoryItem { get; set; }

        public List<IncomingShipmentInventoryItem> IncomingShipmentInventoryItems { get; set; }

        public List<InventoryItemEventLogEntry> EventLogEntries { get; set; }
        public List<InventoryItemNote> Notes { get; set; }
        public List<InventoryItemAttachment> Attachments { get; set; }
        public List<OutgoingShipmentBoxInventoryItem> OutgoingShipmentBoxes { get; set; }
        // public List<RmaLineItem

        // public int? ParentRelatedInventoryItemId { get; set; }
        public InventoryItemRelatedInventoryItem ParentRelatedInventoryItem { get; set; }

        public List<InventoryItemRelatedInventoryItem> ChildRelatedInventoryItems { get; set; }
        // public int? ParentInventoryInventoryItemRelatedInventoryItemId { get; set; }
        // public InventoryItemRelatedInventoryItem ParentInventoryItem { get; set; }

        public List<InventoryItem> ChildInventoryItems { get; set; }
        public List<SalesOrderLineItemInventoryItem> SalesOrderLineItems { get; set; }

        public void GenerateSerialNumber() {
            this.SerialNumber = $"{this.Id:0000000}";
        }

        public async Task<RmaIdOrSalesOrderId> GetDefaultSalesOrderIdOrRmaId(AppDBContext context) {
            var openSalesOrder = await context.SalesOrders
                .Where(item =>
                    //find sales orders where the inventory item is related to the sales order line item
                    item.LineItems.Any(item2 => item2.InventoryItems.Any(item3 => item3.InventoryItemId == this.Id)) &&
                    //but make sure it isn't in a box that has already been shipped
                    !item.OutgoingShipments.Any(item2 => item2.OutgoingShipment.ShippedAt != null && item2.OutgoingShipment.Boxes.Any(item3 => item3.InventoryItems.Any(item4 => item4.InventoryItemId == this.Id))))
                .FirstOrDefaultAsync();

            if (openSalesOrder != null) {
                return new RmaIdOrSalesOrderId{
                    SalesOrderId = openSalesOrder.Id
                };
            }

            var openStatus = await context.RmaStatusOptions.FirstOrDefaultAsync(item => item.Value == "Open");
            var openRma = await context.Rmas
                .Where(item => item.RmaStatusOptionId == openStatus.Id)
                .Where(item => item.LineItems.Any(item2 => item2.InventoryItemId == this.Id))
                .FirstOrDefaultAsync();

            if (openRma != null) {
                return new RmaIdOrSalesOrderId{
                    RmaId = openRma.Id
                };
            }
            return new RmaIdOrSalesOrderId{ };
        }
    }

    public class RmaIdOrSalesOrderId {
        public int? RmaId { get; set; }
        public int? SalesOrderId { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class InventoryItemDBConfiguration : IEntityTypeConfiguration<InventoryItem> {
        public void Configure(EntityTypeBuilder<InventoryItem> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasIndex(item => item.SerialNumber);
            modelBuilder.HasIndex(item => item.CreatedAt);


            modelBuilder.HasOne(item => item.PurchaseOrderLineItem)
                .WithMany()
                .HasForeignKey(item => item.PurchaseOrderLineItemId);

            modelBuilder.HasOne(item => item.GidSubLocationOption).WithMany().HasForeignKey(item => item.GidSubLocationOptionId);

            // modelBuilder.HasOne(ii => ii.ParentRelatedInventoryItem)
            //     .WithOne(item => item.ChildInventoryItem)
            //     .HasForeignKey()

            // modelBuilder.HasOne(ii => ii.Product)
            //     .WithMany()
            //     .HasForeignKey(ii => ii.ProductId);

            // modelBuilder.HasOne(ii => ii.ParentInventoryItem)
            //     .WithMany(ii => ii.ChildRelatedInventoryItems)
            //     .HasForeignKey(ii => ii.ParentInventoryItemId)
            //     .OnDelete(DeleteBehavior.Restrict);

            // modelBuilder.HasMany(ii => ii.ParentRelatedInventoryItems)
            //     .WithOne(iirii => iirii.ParentInventoryItem)
            //     .HasForeignKey(iirii => iirii.ParentInventoryItemId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // modelBuilder.HasMany(ii => ii.ChildRelatedInventoryItems)
            //     .WithOne(iirii => iirii.ChildInventoryItem)
            //     .HasForeignKey(iirii => iirii.ChildInventoryItemId)
            //     .OnDelete(DeleteBehavior.Cascade);                
        }
    }
}