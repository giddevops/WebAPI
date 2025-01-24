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
    public class QuoteChatMessage
    {
        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }

        public int? QuoteId { get; set; }
        public Quote Quote { get; set; }

        public int? QuoteChatMessageTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class QuoteChatMessageDBConfiguration : IEntityTypeConfiguration<QuoteChatMessage>
    {
        public void Configure(EntityTypeBuilder<QuoteChatMessage> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.QuoteId, t.ChatMessageId });

            modelBuilder
                .HasOne(item => item.Quote)
                .WithMany(item => item.ChatMessages)
                .HasForeignKey(item => item.QuoteId);

            modelBuilder
                .HasOne(item => item.ChatMessage)
                .WithMany(item => item.Quotes)
                .HasForeignKey(item => item.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
