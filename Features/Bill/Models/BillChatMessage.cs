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
    public class BillChatMessage
    {
        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }

        public int? BillId { get; set; }
        public Bill Bill { get; set; }

        public int? BillChatMessageTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class BillChatMessageDBConfiguration : IEntityTypeConfiguration<BillChatMessage>
    {
        public void Configure(EntityTypeBuilder<BillChatMessage> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.BillId, t.ChatMessageId });

            modelBuilder
                .HasOne(item => item.Bill)
                .WithMany(item => item.ChatMessages)
                .HasForeignKey(item => item.BillId);

            modelBuilder
                .HasOne(item => item.ChatMessage)
                .WithMany(item => item.Bills)
                .HasForeignKey(item => item.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
