using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ProductType {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Value { get; set; }
        public string Prefix { get; set; }
        public string Description { get; set; }

        public ProductType Parent { get; set; }
        public int? ParentProductTypeId { get; set; }
        public bool? IsPiecePart { get; set; }
        public bool? IsSerialized { get; set; }
        public bool? Kittable { get; set; }

        public int? MasterCategoryId { get; set; }

        public string MasterCategoryName { get; set; }

        public string MasterCategoryParents { get; set; }

        public string MasterCategoryComplete { 
            get 
            {
                if (MasterCategoryName != null)
                {
                    return MasterCategoryParents + " > " + MasterCategoryName;
                }
                else
                    return String.Empty;
            } 
        }

        public List<ProductTypeAlias> Aliases { get; set; }
        public List<ProductAttribute> Attributes { get; set; }

        public async Task<string> GetNextGidPartNumber(AppDBContext _context)
        {
            if (String.IsNullOrWhiteSpace(this.Prefix))
            {
                var productTypeId = this.Id.ToString();
                throw new Exception($"Error - the product type you selected doesn't have a prefix.  Go ahead and add a prefix to the product type first <a href='/product-types/{productTypeId}' target='_blank'>Click Here To Edit the Product Type</a>.");
            }

            var lastPart = await _context.Products
                .Where(item => item.GidPartNumber.StartsWith(this.Prefix + "-"))
                .OrderByDescending(item => item.GidPartNumber)
                .FirstOrDefaultAsync();

            var id = 1;
            if (lastPart != null)
            {
                var partNumber = Int32.Parse(lastPart.GidPartNumber.Split("-").Last());
                id = partNumber + 1;
            }
            return this.Prefix + "-" + id.ToString();
        }
    }
    class ProductTypeDBConfiguration : IEntityTypeConfiguration<ProductType> {
        public void Configure(EntityTypeBuilder<ProductType> modelBuilder) {
            modelBuilder.HasOne(item => item.Parent).WithMany().HasForeignKey(item => item.ParentProductTypeId);
            // modelBuilder.Property(item => item.ProductTypeDataType).HasConversion(new EnumToNumberConverter<ProductTypeDataType>());
            modelBuilder.HasIndex(item => item.Value);
            modelBuilder.HasIndex(item => item.Prefix);
        }
    }
}
