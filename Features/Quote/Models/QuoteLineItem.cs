using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class QuoteLineItem {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public string ProductName { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public string DisplayPartNumber { get; set; }

        public string ManufacturerName { get; set; }
        public int? ManufacturerId { get; set; }
        public Company Manufacturer { get; set; }

        public string Description { get; set; }
        public int? LineItemServiceTypeId { get; set; }
        public LineItemServiceType Service { get; set; }

        public int? LineItemConditionTypeId { get; set; }
        public LineItemConditionType Condition { get; set; }

        public int? OutgoingLineItemWarrantyOptionId { get; set; }
        public OutgoingLineItemWarrantyOption Warranty { get; set; }

        public int? OutgoingLineItemLeadTimeOptionId { get; set; }
        public OutgoingLineItemLeadTimeOption LeadTime { get; set; }

        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercent { get; set; }

        public int? QuoteId { get; set; }
        public Quote Quote { get; set; }
        public List<QuoteLineItemSource> Sources { get; set; }
        public decimal? AverageCost { get; set; }
        public int? Order { get; set; }

        public decimal GetTotal(){
            var price = this.Price ?? 0;
            var quantity = this.Quantity ?? 0;
            var discountPercent = this.DiscountPercent ?? 0;
            return price*quantity*(100-discountPercent)/100;
        }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class QuoteLineItemDBConfiguration : IEntityTypeConfiguration<QuoteLineItem> {
        public void Configure(EntityTypeBuilder<QuoteLineItem> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(l => l.Product).WithMany(p => p.QuoteLineItems).HasForeignKey(l => l.ProductId).OnDelete(DeleteBehavior.Restrict);
            modelBuilder.HasOne(item => item.Manufacturer)
                .WithMany()
                .HasForeignKey(item => item.ManufacturerId);

            modelBuilder.HasOne(item => item.Condition)
                .WithMany()
                .HasForeignKey(item => item.LineItemConditionTypeId);

            modelBuilder.HasOne(item => item.Service)
                .WithMany()
                .HasForeignKey(item => item.LineItemServiceTypeId);

            modelBuilder.HasOne(item => item.Warranty)
                .WithMany()
                .HasForeignKey(item => item.OutgoingLineItemWarrantyOptionId);

            modelBuilder.HasOne(item => item.LeadTime)
                .WithMany()
                .HasForeignKey(item => item.OutgoingLineItemLeadTimeOptionId);

            modelBuilder.HasOne(item => item.Quote)
                .WithMany(item => item.LineItems)
                .HasForeignKey(item => item.QuoteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
