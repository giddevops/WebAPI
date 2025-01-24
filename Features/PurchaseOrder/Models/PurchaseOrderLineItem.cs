using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class PurchaseOrderLineItem {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public string ProductName { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public string ManufacturerName { get; set; }
        public int? ManufacturerId { get; set; }
        public Company Manufacturer { get; set; }

        public string Description { get; set; }
        public int? LineItemServiceTypeId { get; set; }
        public int? LineItemConditionTypeId { get; set; }
        public LineItemConditionType LineItemConditionType { get; set; }

        public int? Quantity { get; set; }
        public decimal? Cost { get; set; }
        public decimal? DiscountPercent { get; set; }
        public int? WarrantyDuration { get; set; }
        public string WarrantyDurationUnit { get; set; }

        // [NotMapped]
        // public int? QuantityRemainingForRelateSalesOrder { 
        //     get{
        //         if(this.QuantityUsedForRelateSalesOrder == null)
        //             return 
        //     }
        // }

        public async Task SyncShipments(int? previousQuantity, PurchaseOrder purchaseOrder, int? userId, AppDBContext context)
        {

            if (previousQuantity == this.Quantity)
                return;

            purchaseOrder = await context.PurchaseOrders
                .Include(item => item.IncomingShipments)
                .FirstOrDefaultAsync(item => item.Id == this.PurchaseOrderId);


            var sentStatus = await context.PurchaseOrderStatusOptions.FirstOrDefaultAsync(item => item.Value == "Sent");
            if (purchaseOrder.PurchaseOrderStatusOptionId != sentStatus.Id && purchaseOrder.IncomingShipments.Count == 0)
                return;

            purchaseOrder = await context.PurchaseOrders
                .Include(item => item.LineItems)
                .Include(item => item.IncomingShipments)
                    .ThenInclude(item => item.IncomingShipment)
                        .ThenInclude(item => item.InventoryItems)
                            .ThenInclude(item => item.InventoryItem)
                .FirstOrDefaultAsync(item => item.Id == this.PurchaseOrderId);
            
            
            var purchaseOrderTotalCost = purchaseOrder.GetTotal();
            var lineItemsTotalCost = purchaseOrder.LineItemsTotalCost();
            var nonLineItemsTotalCost = purchaseOrder.NonLineItemsTotalCost();

            var total = 0;
            var unreceived = 0;
            foreach (var shipment in purchaseOrder.IncomingShipments)
            {
                total += shipment.IncomingShipment.InventoryItems.Where(item => item.InventoryItem.PurchaseOrderLineItemId == this.Id).Count();
                unreceived += shipment.IncomingShipment.InventoryItems.Where(item => item.InventoryItem.PurchaseOrderLineItemId == this.Id && item.ReceivedAt == null).Count();
            }

            var received = total - unreceived;
            if (received > this.Quantity)
            {
                throw new Exception("You need to unreceive some items before you can decrease the quantity.  Number received " + received + " is greater than line item quantity: " + this.Quantity);
            }

            // Need to delete items
            var numToRemove = total - this.Quantity;
            if (numToRemove > 0)
            {
                var numRemoved = 0;
                foreach (var shipment in purchaseOrder.IncomingShipments)
                {
                    for(int i = shipment.IncomingShipment.InventoryItems.Count - 1; i >= 0; --i)
                    {
                        if(shipment.IncomingShipment.InventoryItems[i].ReceivedAt == null)
                        {
                            context.InventoryItems.Remove(shipment.IncomingShipment.InventoryItems[i].InventoryItem);
                            shipment.IncomingShipment.InventoryItems.RemoveAt(i);
                            numRemoved += 1;
                            if (numRemoved == numToRemove)
                                break;
                        }
                    }
                    if (numRemoved == numToRemove)
                        break;
                }
            }

            // Add new items
            var numToAdd = this.Quantity - total;
            if(numToAdd > 0)
            {
                var lineItemCost = this.GetCostForAllUnits();
                var lineItemCostFraction = lineItemCost / lineItemsTotalCost;
                decimal totalCostPerUnit = 0;

                if (numToAdd > 0)
                { //make sure not to divide by 0
                    totalCostPerUnit = (this.Cost ?? 0) + nonLineItemsTotalCost * lineItemCostFraction / (this.Quantity ?? 0);
                }

                for (int i = 0; i < numToAdd; ++i)
                {
                    var inventoryItem = new InventoryItem
                    {
                        CreatedById = userId,
                        CreatedAt = DateTime.UtcNow,
                        ProductId = this.ProductId,
                        CurrencyOptionId = purchaseOrder.CurrencyOptionId,
                        Description = this.Description,
                        InventoryItemStatusOptionId = InventoryItemStatusOption.Inbound,
                        PurchaseOrderLineItemId = this.Id,
                        UnitCost = this.Cost,
                        TotalCost = totalCostPerUnit
                    };
                    var incomingShipment = purchaseOrder.IncomingShipments.Last();
                    if (incomingShipment == null)
                    {
                        incomingShipment = new PurchaseOrderIncomingShipment
                        {
                            IncomingShipment = new IncomingShipment { }
                        };
                        context.PurchaseOrderIncomingShipments.Add(incomingShipment);
                        purchaseOrder.IncomingShipments.Add(incomingShipment);
                    }
                    var incomingShipmentInventoryItem = new IncomingShipmentInventoryItem
                    {
                        InventoryItem = inventoryItem
                    };
                    context.IncomingShipmentInventoryItems.Add(incomingShipmentInventoryItem);
                    incomingShipment.IncomingShipment.InventoryItems.Add(incomingShipmentInventoryItem);
                }
            }
        }

        [NotMapped]
        public int QuantityUsedForRelateSalesOrder { get; set; }

        public decimal GetCostForAllUnits() {
            return (this.Cost ?? 0) * (100 - (this.DiscountPercent ?? 0)) / 100 * (this.Quantity ?? 0);
        }

        [NotMapped]
        public string WarrantyString {
            get {
                if (this.WarrantyDuration == null || this.WarrantyDuration == 0) {
                    return "";
                }
                if (String.IsNullOrWhiteSpace(this.WarrantyDurationUnit)) {
                    return "";
                }

                var durationString = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(this.WarrantyDurationUnit.ToLower());
                if (WarrantyDuration > 1)
                    durationString += "s";

                return WarrantyDuration.ToString() + " " + durationString;
            }
        }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        public List<BillLineItem> BillLineItems { get; set; }
        // public List<PurchaseOrderLineItemSource> Sources { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting SalesOrders and inventoryItems many to many relationship
    /// </summary>
    class PurchaseOrderLineItemDBConfiguration : IEntityTypeConfiguration<PurchaseOrderLineItem> {
        public void Configure(EntityTypeBuilder<PurchaseOrderLineItem> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(item => item.Manufacturer)
                .WithMany()
                .HasForeignKey(item => item.ManufacturerId);

            modelBuilder.HasOne(item => item.LineItemConditionType)
                .WithMany()
                .HasForeignKey(item => item.LineItemConditionTypeId);

            modelBuilder.HasOne(item => item.PurchaseOrder)
                .WithMany(item => item.LineItems)
                .HasForeignKey(item => item.PurchaseOrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
