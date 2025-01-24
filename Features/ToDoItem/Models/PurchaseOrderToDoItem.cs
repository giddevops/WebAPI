using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class PurchaseOrderToDoItem
    {
        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        public int ToDoItemId { get; set; }
        public ToDoItem ToDoItem { get; set; }

    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class PurchaseOrderToDoItemDBConfiguration : IEntityTypeConfiguration<PurchaseOrderToDoItem>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderToDoItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ToDoItemId, t.PurchaseOrderId });

            modelBuilder
                .HasOne(item => item.PurchaseOrder)
                .WithMany(item => item.ToDoItems)
                .HasForeignKey(item => item.PurchaseOrderId);

            modelBuilder
                .HasOne(item => item.ToDoItem)
                .WithMany(item => item.PurchaseOrderToDoItems)
                .HasForeignKey(item => item.ToDoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
