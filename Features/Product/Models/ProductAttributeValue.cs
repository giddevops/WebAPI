using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ProductAttributeValue {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int? ProductAttributeValueOptionId { get; set; }
        public ProductAttributeValueOption ProductAttributeValueOption { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
    }

    class ProductAttributeValueDBConfiguration : IEntityTypeConfiguration<ProductAttributeValue>
    {
        public void Configure(EntityTypeBuilder<ProductAttributeValue> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ProductId, t.ProductAttributeValueOptionId });
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder
                .HasOne(productAttributeValue => productAttributeValue.Product)
                .WithMany(product => product.AttributeValues)
                .HasForeignKey(productAttributeValue => productAttributeValue.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder
                .HasOne(item => item.ProductAttributeValueOption)
                .WithMany()
                .HasForeignKey(item => item.ProductAttributeValueOptionId);
        }
    }
}
