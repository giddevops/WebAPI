using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class WorkLogItem {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }
        public int? PerformedById { get; set; }

        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }

        public int? InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }
        public int? RmaId { get; set; }
        public Rma Rma { get; set; }

        public string ActivityText { get; set; }
        public int? WorkLogItemActivityOptionId { get; set; }
        public WorkLogItemActivityOption WorkLogItemActivityOption { get; set; }
    }

    class WorkLogItemDBConfiguration : IEntityTypeConfiguration<WorkLogItem> {
        public void Configure(EntityTypeBuilder<WorkLogItem> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(item => item.InventoryItem).WithMany().HasForeignKey(item => item.InventoryItemId);
            modelBuilder.HasOne(item => item.SalesOrder).WithMany().HasForeignKey(item => item.SalesOrderId);
            modelBuilder.HasOne(item => item.Rma).WithMany().HasForeignKey(item => item.RmaId);
            modelBuilder.HasOne(item => item.WorkLogItemActivityOption).WithMany().HasForeignKey(item => item.WorkLogItemActivityOptionId);
        }
    }
}