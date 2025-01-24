using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class LeadToDoItem
    {
        public int? LeadId { get; set; }
        public Lead Lead { get; set; }

        public int ToDoItemId { get; set; }
        public ToDoItem ToDoItem { get; set; }

    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class LeadToDoItemDBConfiguration : IEntityTypeConfiguration<LeadToDoItem>
    {
        public void Configure(EntityTypeBuilder<LeadToDoItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ToDoItemId, t.LeadId });

            modelBuilder
                .HasOne(item => item.Lead)
                .WithMany(item => item.ToDoItems)
                .HasForeignKey(item => item.LeadId);

            modelBuilder
                .HasOne(item => item.ToDoItem)
                .WithMany(item => item.LeadToDoItems)
                .HasForeignKey(item => item.ToDoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
