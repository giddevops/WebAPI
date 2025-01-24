using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class BillLineItem {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? BillId { get; set; }
        public Bill Bill { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public string ManufacturerName { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal DiscountPercent { get; set; }

        public int? GLAccountId { get; set; }

        public int? PurchaseOrderLineItemId { get; set; }
        public PurchaseOrderLineItem PurchaseOrderLineItem { get; set; }

        [NotMapped]
        public int? QuantityShipped { get; set; }

        public decimal GetExt() {
            return this.Price * this.Quantity * (100 - this.DiscountPercent) / 100;
        }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class BillLineItemDBConfiguration : IEntityTypeConfiguration<BillLineItem> {
        public void Configure(EntityTypeBuilder<BillLineItem> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(l => l.Product).WithMany(p => p.BillLineItems).HasForeignKey(l => l.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.Bill)
                .WithMany(item => item.LineItems)
                .HasForeignKey(item => item.BillId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.HasOne(item => item.PurchaseOrderLineItem)
                .WithMany(item => item.BillLineItems)
                .HasForeignKey(item => item.PurchaseOrderLineItemId)
                .OnDelete(DeleteBehavior.Restrict);

            //modelBuilder.HasOne(item => item.Manufacturer)
            //    .WithMany()
            //    .HasForeignKey(item => item.ManufacturerId);

            // modelBuilder.HasOne(item => item.Condition)
            //     .WithMany()
            //     .HasForeignKey(item => item.LineItemConditionTypeId);

            // modelBuilder.HasOne(item => item.Service)
            //     .WithMany()
            //     .HasForeignKey(item => item.LineItemServiceTypeId);

            // modelBuilder.HasOne(item => item.Warranty)
            //     .WithMany()
            //     .HasForeignKey(item => item.OutgoingLineItemWarrantyOptionId);

            // modelBuilder.HasOne(item => item.LeadTime)
            //     .WithMany()
            //     .HasForeignKey(item => item.OutgoingLineItemLeadTimeOptionId);
        }
    }
}
