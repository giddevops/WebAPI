using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class SalesOrderLineItem
    {
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
        public int? LineItemConditionTypeId { get; set; }
        public LineItemConditionType LineItemConditionType { get; set; }

        public int? Quantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? DiscountPercent { get; set; }

        public int SalesOrderId { get; set; }

        public string CpuRequirements { get; set; }
        public int? CpuQuantity { get; set; }
        public int? CpuStockVerifiedOptionId { get; set; }
        public int? MemoryQuantity { get; set; }
        public int? MemoryStockVerifiedOptionId { get; set; }

        public decimal? DeclaredValue { get; set; }
        public int? CountryOfOriginId { get; set; }
        public Country CountryOfOrigin { get; set; }
        public string ScheduleB { get; set; }
        
        public int? LeadTimeRangeStart { get; set; }
        public int? LeadTimeRangeEnd { get; set; }
        public string LeadTimeRangeUnit { get; set; }

        public int? CpuProductId { get; set; }
        public int? MemoryProductId { get; set; }
        public int? FanHeatSinkProductId { get; set; }
        public decimal? AverageCost { get; set; }
        public decimal? ProjectedProfit { get; set; }

        public string CustomerNotes { get; set; }


        public int? OutgoingLineItemWarrantyOptionId { get; set; }
        public OutgoingLineItemWarrantyOption Warranty { get; set; }
        
        public int? OutgoingLineItemLeadTimeOptionId { get; set; }
        public OutgoingLineItemLeadTimeOption LeadTime { get; set; }
        
        public List<SalesOrderLineItemInventoryItem> InventoryItems { get; set; }
        public List<SalesOrderLineItemSource> Sources { get; set; }
        public List<RmaLineItem> RmaLineItems { get; set; }
        public List<InvoiceLineItem> InvoiceLineItems { get; set; }

        public async Task UpdateSalesOrderTotal(AppDBContext _context){
            var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).AsNoTracking().FirstOrDefaultAsync(item => item.Id == this.SalesOrderId);
     
            await salesOrder.UpdateBalance(_context);
            await salesOrder.UpdateTotal(_context);
        }
    }
    
    class SalesOrderLineItemDBConfiguration : IEntityTypeConfiguration<SalesOrderLineItem>
    {
        public void Configure(EntityTypeBuilder<SalesOrderLineItem> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasOne(item => item.Manufacturer)
                .WithMany()
                .HasForeignKey(item => item.ManufacturerId);
            modelBuilder.HasOne(item => item.Warranty)
                .WithMany()
                .HasForeignKey(item => item.OutgoingLineItemWarrantyOptionId);
            modelBuilder.HasOne(item => item.LineItemConditionType)
                .WithMany()
                .HasForeignKey(item => item.LineItemConditionTypeId);
            modelBuilder.HasOne(item => item.CountryOfOrigin).WithMany().HasForeignKey(item => item.CountryOfOriginId);
        }
    }
}