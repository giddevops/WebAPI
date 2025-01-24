using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class LeadLineItem
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public string ProductName { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        // public int? Product { get; set; } 
        public string ManufacturerName { get; set; }
        public int? CompanyId { get; set; }
        public string Description { get; set; }
        public int? LineItemServiceTypeId { get; set; }
        public int? Quantity { get; set; }
        public int? Order { get; set; } 

        public int LeadId { get; set; }

        public List<LeadLineItemSource> Sources { get; set; }
    }
    
    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class LeadLineItemDBConfiguration : IEntityTypeConfiguration<LeadLineItem>
    {
        public void Configure(EntityTypeBuilder<LeadLineItem> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(item => item.Product)
                .WithMany()
                .HasForeignKey(item => item.ProductId);

            modelBuilder.HasIndex(item => item.ProductName);

            // modelBuilder.HasOne(l => l.Product);
        }
    }
}
