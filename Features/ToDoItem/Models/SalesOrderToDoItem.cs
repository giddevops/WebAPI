using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class SalesOrderToDoItem
    {
        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        public int ToDoItemId { get; set; }
        public ToDoItem ToDoItem { get; set; }

    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class SalesOrderToDoItemDBConfiguration : IEntityTypeConfiguration<SalesOrderToDoItem>
    {
        public void Configure(EntityTypeBuilder<SalesOrderToDoItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ToDoItemId, t.SalesOrderId });

            modelBuilder
                .HasOne(item => item.SalesOrder)
                .WithMany(item => item.ToDoItems)
                .HasForeignKey(item => item.SalesOrderId);

            modelBuilder
                .HasOne(item => item.ToDoItem)
                .WithMany(item => item.SalesOrderToDoItems)
                .HasForeignKey(item => item.ToDoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
