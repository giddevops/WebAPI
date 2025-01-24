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
    public class PurchaseOrderChatMessage
    {
        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }

        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        public int? PurchaseOrderChatMessageTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class PurchaseOrderChatMessageDBConfiguration : IEntityTypeConfiguration<PurchaseOrderChatMessage>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderChatMessage> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.PurchaseOrderId, t.ChatMessageId });

            modelBuilder
                .HasOne(item => item.PurchaseOrder)
                .WithMany(item => item.ChatMessages)
                .HasForeignKey(item => item.PurchaseOrderId);

            modelBuilder
                .HasOne(item => item.ChatMessage)
                .WithMany(item => item.PurchaseOrders)
                .HasForeignKey(item => item.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
