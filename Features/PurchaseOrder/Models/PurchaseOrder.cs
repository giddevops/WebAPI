using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class PurchaseOrder {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        // public User Buyer { get; set; }
        public int? BuyerId { get; set; }

        public bool? Ncnr { get; set; }
        public bool? NeedsFunding { get; set; }

        public bool? Funded { get; set; }

        public int? SupplierId { get; set; }
        public Company Supplier { get; set; }

        public int? ContactId { get; set; }
        public Contact Contact { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }
        public int? PurchaseOrderStatusOptionId { get; set; }
        public int? PurchaseOrderReasonOptionId { get; set; }

        [NotMapped]
        public User Preparer { get; set; }

        [Required]
        public int? CurrencyOptionId { get; set; }
        public CurrencyOption CurrencyOption { get; set; }

        public int? GidLocationOptionId { get; set; }
        public GidLocationOption GidLocationOption { get; set; }


        // public string TrackingNumber { get; set; }
        public string ConfirmationNumber { get; set; }


        public decimal? SalesTax { get; set; }
        public decimal? WireTransferFee { get; set; }
        public decimal? ShippingAndHandlingFee { get; set; }
        public decimal? ExpediteFee { get; set; }

        public int? ShippingMethodId { get; set; }
        public ShippingMethod ShippingMethod { get; set; }

        public int? PurchaseOrderPaymentMethodId { get; set; }
        public PurchaseOrderPaymentMethod PaymentMethod { get; set; }

        public DateTime? ExpectedShipDate { get; set; }
        public DateTime? ExpectedArrivalDate { get; set; }
        public decimal Total { get; set; }
        public string FreightAccountNumber { get; set; }

        public int? ExpectedShipDateChangeReasonId { get; set; }
        public PurchaseOrderExpectedShipDateChangeReason ExpectedShipDateChangeReason { get; set; }
        public bool HasExpectedShipDate { get { return this.ExpectedShipDate != null; } }

        public bool? Priority { get; set; }

        // public Address SupplierAddress { get; set; }
        public Address ShippingAddress { get; set; }

        public bool? Sent { get; set; }
        public DateTime? SentAt { get; set; }

        public List<PurchaseOrderLineItem> LineItems { get; set; }
        public List<PurchaseOrderIncomingShipment> IncomingShipments { get; set; }
        public List<PurchaseOrderEventLogEntry> EventLogEntries { get; set; }
        public List<SalesOrderPurchaseOrder> SalesOrders { get; set; }
        public List<PurchaseOrderAttachment> Attachments { get; set; }
        public List<PurchaseOrderNote> Notes { get; set; }
        public List<PurchaseOrderChatMessage> ChatMessages { get; set; }
        public List<Bill> Bills { get; set; }

        public List<SalesOrderLineItemSource> SalesOrderLineItemSources { get; set; }


        public List<PurchaseOrderToDoItem> ToDoItems { get; set; }


        public async Task<bool> CheckIfIsGidEurope(AppDBContext _context)
        {
            var GidEuropeOption = await _context.GidLocationOptions.FirstOrDefaultAsync(item => item.Value.Contains("Europe"));
            if (GidEuropeOption == null) {
                throw new Exception("Unable to find GID Europe option");
            }
            return this.GidLocationOptionId == GidEuropeOption.Id;
        }

        // public async Task<bool> GetGidLocationOption(AppDBContext _context){
        //     return await _context.GidLocationOptions.FirstOrDefaultAsync(item => item.Id == this.GidLocationOptionId);
        // }

        public async Task UpdateTotal(AppDBContext _context) {
            if (this.LineItems == null) {
                this.LineItems = await _context.PurchaseOrderLineItems.Where(item => item.PurchaseOrderId == this.Id).ToListAsync();
            }
            this.Total = this.GetTotal();
        }

        public decimal LineItemsTotalCost() {
            var total = this.LineItems.Sum(item => item.GetCostForAllUnits());
            return total;
        }

        public decimal NonLineItemsTotalCost() {
            var total = this.SalesTax ?? 0;
            total += this.WireTransferFee ?? 0;
            total += this.ShippingAndHandlingFee ?? 0;
            total += this.ExpediteFee ?? 0;
            return total;
        }

        public decimal GetLineItemsTotal() {
            return this.LineItems.Sum(item => item.GetCostForAllUnits());
        }

        public decimal GetTotal() {
            var total = this.GetLineItemsTotal();
            total += this.SalesTax ?? 0;
            total += this.WireTransferFee ?? 0;
            total += this.ShippingAndHandlingFee ?? 0;
            total += this.ExpediteFee ?? 0;
            return total;
        }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class PurchaseOrderDBConfiguration : IEntityTypeConfiguration<PurchaseOrder> {
        public void Configure(EntityTypeBuilder<PurchaseOrder> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(item => item.ShippingMethod).WithMany().HasForeignKey(item => item.ShippingMethodId);

            modelBuilder.HasOne(po => po.Supplier)
                .WithMany()
                .HasForeignKey(po => po.SupplierId);

            modelBuilder.HasOne(item => item.CurrencyOption)
                .WithMany()
                .HasForeignKey(item => item.CurrencyOptionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasOne(item => item.PaymentMethod)
                .WithMany()
                .HasForeignKey(item => item.PurchaseOrderPaymentMethodId);

            modelBuilder.HasOne(item => item.GidLocationOption)
                .WithMany()
                .HasForeignKey(item => item.GidLocationOptionId);




            modelBuilder.HasIndex(item => item.Phone);
            modelBuilder.HasIndex(item => item.Email);
            modelBuilder.HasIndex(item => item.CreatedAt);
        }
    }
}