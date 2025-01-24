using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class RMAToDoItem
    {
        public int? RMAId { get; set; }
        public Rma RMA{ get; set; }

        public int ToDoItemId { get; set; }
        public ToDoItem ToDoItem { get; set; }

    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class RMAToDoItemDBConfiguration : IEntityTypeConfiguration<RMAToDoItem>
    {
        public void Configure(EntityTypeBuilder<RMAToDoItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ToDoItemId, t.RMAId });

            modelBuilder
                .HasOne(item => item.RMA)
                .WithMany(item => item.ToDoItems)
                .HasForeignKey(item => item.RMAId);

            modelBuilder
                .HasOne(item => item.ToDoItem)
                .WithMany(item => item.RMAToDoItems)
                .HasForeignKey(item => item.ToDoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
