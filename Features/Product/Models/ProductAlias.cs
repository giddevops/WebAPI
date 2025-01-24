using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ProductAlias {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }

        [Required]
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public string PartNumber { get; set; }
        public string PartNumberNoSpecialChars
        {
            get
            {
                return this.PartNumber != null ? this.PartNumber.Trim().Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "") : null;
            }
            private set { }
        }
        public string ManufacturerName { get; set; }
        
        public ProductAliasType ProductAliasType { get; set; }
        public int? ProductAliasTypeId { get; set; }
    }
    
    class ProductAliasDbConfiguration : IEntityTypeConfiguration<ProductAlias> {
        public void Configure(EntityTypeBuilder<ProductAlias> modelBuilder) {
                
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasIndex(item => item.PartNumber);
            modelBuilder.HasIndex(item => item.ManufacturerName);
            modelBuilder.HasIndex(item => item.CreatedAt);
            modelBuilder.HasIndex(item => item.PartNumberNoSpecialChars);
        }
    }
}