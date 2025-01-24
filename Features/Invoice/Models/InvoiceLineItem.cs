using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class InvoiceLineItem {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        // public string ProductName { get; set; }

        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public string ManufacturerName { get; set; }
        public string Description { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercent { get; set; }

        public int? SalesOrderLineItemId { get; set; }
        public SalesOrderLineItem SalesOrderLineItem { get; set; }

        public string QuickBooksId { get; set; }

        [NotMapped]
        public int? QuantityShipped { get; set; }

        // public int? ProductId { get; set; }
        // public Product Product { get; set; }

        // public string ManufacturerName { get; set; }
        // // public int? ManufacturerId { get; set; }
        // // public Company Manufacturer { get; set; }

        // public string Description { get; set; }

        // public int? LineItemServiceTypeId { get; set; }
        // public LineItemServiceType Service { get; set; }

        // public int? LineItemConditionTypeId { get; set; }
        // public LineItemConditionType Condition { get; set; }

        // public int? OutgoingLineItemWarrantyOptionId { get; set; }
        // public OutgoingLineItemWarrantyOption Warranty { get; set; }

        // public int? OutgoingLineItemLeadTimeOptionId { get; set; }
        // public OutgoingLineItemLeadTimeOption LeadTime { get; set; }

        // public int? Quantity { get; set; }
        // public decimal? Price { get; set; }
        // public decimal? DiscountPercent { get; set; }


        // public int InvoiceId { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class InvoiceLineItemDBConfiguration : IEntityTypeConfiguration<InvoiceLineItem> {
        public void Configure(EntityTypeBuilder<InvoiceLineItem> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(l => l.Product).WithMany(p => p.InvoiceLineItems).HasForeignKey(l => l.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.Invoice)
                .WithMany(item => item.LineItems)
                .HasForeignKey(item => item.InvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.HasOne(item => item.SalesOrderLineItem)
                .WithMany(item => item.InvoiceLineItems)
                .HasForeignKey(item => item.SalesOrderLineItemId)
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
