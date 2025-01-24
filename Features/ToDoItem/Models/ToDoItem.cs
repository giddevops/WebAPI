using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class ToDoItem
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ToDoDateTime { get; set; }
        public int? CreatedById { get; set; }

        public string Notes { get; set; }

        public int? AssignedToId { get; set; }
        
        public int? ToDoTypeOptionId { get; set; }

        public bool? Completed { get; set; }
        
        public List<QuoteToDoItem> QuoteToDoItems { get; set; }

        public List<LeadToDoItem> LeadToDoItems { get; set; }

        public List<SalesOrderToDoItem> SalesOrderToDoItems { get; set; }

        public List<PurchaseOrderToDoItem> PurchaseOrderToDoItems { get; set; }

        public List<RMAToDoItem> RMAToDoItems { get; set; }
    }
    
    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class ToDoItemDBConfiguration : IEntityTypeConfiguration<ToDoItem>
    {
        public void Configure(EntityTypeBuilder<ToDoItem> modelBuilder)
        {
            // modelBuilder.HasOne(item => item.InReplyToContactLogItem)
            //     .WithMany(item => item.Replies)
            //     .HasForeignKey(item => item.InReplyToContactLogItemId)
            //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
