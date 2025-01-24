using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ProductKitItem {

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? ParentProductId { get; set; }
        public Product ParentProduct { get; set; }
        public int? ChildProductId { get; set; }
        public Product ChildProduct { get; set; }
        public int? Quantity { get; set; }
    }

    class ProductKitItemDBConfiguration : IEntityTypeConfiguration<ProductKitItem>
    {
        public void Configure(EntityTypeBuilder<ProductKitItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ParentProductId, t.ChildProductId });
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder
                .HasOne(ProductKitItem => ProductKitItem.ParentProduct)
                .WithMany(product => product.ProductKitItems)
                .HasForeignKey(ProductKitItem => ProductKitItem.ParentProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(item => item.ChildProduct)
                .WithMany()
                .HasForeignKey(item => item.ChildProductId);
        }
    }
}
