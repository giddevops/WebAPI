using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ProductAttribute {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int ProductTypeId { get; set; }
        public string Name { get; set; }

        public List<ProductAttributeValueOption> ValueOptions { get; set; }
    }

    class ProductAttributeDBConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> modelBuilder)
        {
            modelBuilder
                .HasMany(item => item.ValueOptions)
                .WithOne(item => item.ProductAttribute)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
