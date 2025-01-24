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
    public class InvoiceChatMessage
    {
        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }

        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int? InvoiceChatMessageTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class InvoiceChatMessageDBConfiguration : IEntityTypeConfiguration<InvoiceChatMessage>
    {
        public void Configure(EntityTypeBuilder<InvoiceChatMessage> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.InvoiceId, t.ChatMessageId });

            modelBuilder
                .HasOne(item => item.Invoice)
                .WithMany(item => item.ChatMessages)
                .HasForeignKey(item => item.InvoiceId);

            modelBuilder
                .HasOne(item => item.ChatMessage)
                .WithMany(item => item.Invoices)
                .HasForeignKey(item => item.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
