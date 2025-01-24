using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ChatMessageUserMention {
        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }
        public int? UserId { get; set; }
        public User User { get; set; }
        public bool Read { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class ChatMessageUserMentionDBConfiguration : IEntityTypeConfiguration<ChatMessageUserMention> {
        public void Configure(EntityTypeBuilder<ChatMessageUserMention> modelBuilder) {
            modelBuilder.HasKey(item => new { item.UserId, item.ChatMessageId });
            modelBuilder.HasOne(item => item.User)
                .WithMany()
                .HasForeignKey(item => item.UserId);

            modelBuilder.HasOne(item => item.ChatMessage)
                .WithMany()
                .HasForeignKey(item => item.ChatMessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
