using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class Source
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? ProductId { get; set; }
        public Product Product { get; set; }

        public int? SupplierId { get; set; }
        public Company Supplier { get; set; }

        public Contact Contact { get; set; }
        public int? ContactId { get; set; }

        public bool? Ncnr { get; set; }

        public string ProductSourceWebPage { get; set; }
        public int? ProductConditionOptionId { get; set; }
        public decimal? Cost { get; set; }
        public int? Quantity { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public int? SourceLeadTimeOptionId { get; set; }
        public decimal? ShippingCost { get; set; }
        public int? PortalId { get; set; }


        public string ListingId { get; set; }

        public int? WarrantyDuration { get; set; }
        public string WarrantyDurationUnit { get; set; }

        public List<SourceAttachment> Attachments { get; set; }
        public List<SourceNote> Notes { get; set; }

        public List<QuoteLineItemSource> QuoteLineItems { get; set; }
        public List<SalesOrderLineItemSource> SalesOrderLineItems { get; set; }

        public int? LeadTimeRangeStart { get; set; }
        public int? LeadTimeRangeEnd { get; set; }
        public string LeadTimeRangeUnit { get; set; }

        public int? CurrencyOptionId { get; set; }
        public CurrencyOption Currency { get; set; }

        //these are added via a custom join query
        [NotMapped]
        public List<Lead> Leads { get; set; }
    }

    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class SourceDBConfiguration : IEntityTypeConfiguration<Source>
    {
        public void Configure(EntityTypeBuilder<Source> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasOne(s => s.Supplier)
                .WithMany()
                .HasForeignKey(s => s.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasIndex(item => item.CreatedAt);
            modelBuilder.HasIndex(item => item.CreatedById);

        }
    }
}