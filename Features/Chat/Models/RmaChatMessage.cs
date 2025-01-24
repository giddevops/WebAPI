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
    public class RmaChatMessage
    {
        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }

        public int? RmaId { get; set; }
        public Rma Rma { get; set; }

        public int? RmaChatMessageTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class RmaChatMessageDBConfiguration : IEntityTypeConfiguration<RmaChatMessage>
    {
        public void Configure(EntityTypeBuilder<RmaChatMessage> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.RmaId, t.ChatMessageId });

            modelBuilder
                .HasOne(item => item.Rma)
                .WithMany(item => item.ChatMessages)
                .HasForeignKey(item => item.RmaId);

            modelBuilder
                .HasOne(item => item.ChatMessage)
                .WithMany(item => item.Rmas)
                .HasForeignKey(item => item.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
