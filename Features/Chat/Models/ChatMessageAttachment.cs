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
    public class ChatMessageAttachment
    {
        public int? AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int? ChatMessageId { get; set; }
        public ChatMessage ChatMessage { get; set; }

        public int? ChatMessageAttachmentTypeId { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class ChatMessageAttachmentDBConfiguration : IEntityTypeConfiguration<ChatMessageAttachment>
    {
        public void Configure(EntityTypeBuilder<ChatMessageAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ChatMessageId, t.AttachmentId });

            modelBuilder
                .HasOne(item => item.ChatMessage)
                .WithMany(item => item.Attachments)
                .HasForeignKey(item => item.ChatMessageId);

            modelBuilder
                .HasOne(item => item.Attachment)
                .WithMany()
                .HasForeignKey(item => item.AttachmentId);
        }
    }
}
