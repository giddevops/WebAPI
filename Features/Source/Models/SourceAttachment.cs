using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and sources
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class SourceAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int SourceId { get; set; }
        public Source Source { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and sources many to many relationship
    /// </summary>
    class SourceAttachmentDBConfiguration : IEntityTypeConfiguration<SourceAttachment>
    {
        public void Configure(EntityTypeBuilder<SourceAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.SourceId, t.AttachmentId });
            modelBuilder
                .HasOne(sourceAttachment => sourceAttachment.Attachment)
                .WithMany(attachment => attachment.SourceAttachments)
                .HasForeignKey(sourceAttachment => sourceAttachment.AttachmentId);

            modelBuilder
                .HasOne(sourceAttachment => sourceAttachment.Source)
                .WithMany(source => source.Attachments)
                .HasForeignKey(sourceAttachment => sourceAttachment.SourceId);
        }
    }
}
