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
    public class SalesOrderChatMessage
    {
        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }

        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        public int? SalesOrderChatMessageTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class SalesOrderChatMessageDBConfiguration : IEntityTypeConfiguration<SalesOrderChatMessage>
    {
        public void Configure(EntityTypeBuilder<SalesOrderChatMessage> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.SalesOrderId, t.ChatMessageId });

            modelBuilder
                .HasOne(item => item.SalesOrder)
                .WithMany(item => item.ChatMessages)
                .HasForeignKey(item => item.SalesOrderId);

            modelBuilder
                .HasOne(item => item.ChatMessage)
                .WithMany(item => item.SalesOrders)
                .HasForeignKey(item => item.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
