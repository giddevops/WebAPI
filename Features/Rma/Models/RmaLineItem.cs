using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class RmaLineItem
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public int RmaId { get; set; }

        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }

        public decimal? Price { get; set; }
        public int? SalesOrderLineItemId { get; set; }
        public SalesOrderLineItem SalesOrderLineItem { get; set; }
    }

    public class RmaLineItemGrouped{
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public int SalesOrderLineItemId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    class RmaLineItemDBConfiguration : IEntityTypeConfiguration<RmaLineItem>
    {
        public void Configure(EntityTypeBuilder<RmaLineItem> modelBuilder)
        {
            modelBuilder.HasOne(item => item.InventoryItem).WithMany().HasForeignKey(item => item.InventoryItemId);
            modelBuilder.HasOne(item => item.SalesOrderLineItem).WithMany(item => item.RmaLineItems).HasForeignKey(item => item.SalesOrderLineItemId);
        }
    }
}
