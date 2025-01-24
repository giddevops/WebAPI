using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class QuoteToDoItem
    {
        public int? QuoteId { get; set; }
        public Quote Quote { get; set; }

        public int ToDoItemId { get; set; }
        public ToDoItem ToDoItem { get; set; }

    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class QuoteToDoItemDBConfiguration : IEntityTypeConfiguration<QuoteToDoItem>
    {
        public void Configure(EntityTypeBuilder<QuoteToDoItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ToDoItemId, t.QuoteId });

            modelBuilder
                .HasOne(item => item.Quote)
                .WithMany(item => item.ToDoItems)
                .HasForeignKey(item => item.QuoteId);

            modelBuilder
                .HasOne(item => item.ToDoItem)
                .WithMany(item => item.QuoteToDoItems)
                .HasForeignKey(item => item.ToDoItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
