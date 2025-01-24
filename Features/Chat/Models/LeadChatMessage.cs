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
    public class LeadChatMessage
    {
        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }

        public int? LeadId { get; set; }
        public Lead Lead { get; set; }

        public int? LeadChatMessageTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class LeadChatMessageDBConfiguration : IEntityTypeConfiguration<LeadChatMessage>
    {
        public void Configure(EntityTypeBuilder<LeadChatMessage> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.LeadId, t.ChatMessageId });

            modelBuilder
                .HasOne(item => item.Lead)
                .WithMany(item => item.ChatMessages)
                .HasForeignKey(item => item.LeadId);

            modelBuilder
                .HasOne(item => item.ChatMessage)
                .WithMany(item => item.Leads)
                .HasForeignKey(item => item.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
