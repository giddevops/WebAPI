using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and rmas
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class RmaAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int RmaId { get; set; }
        public Rma Rma { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and rmas many to many relationship
    /// </summary>
    class RmaAttachmentDBConfiguration : IEntityTypeConfiguration<RmaAttachment>
    {
        public void Configure(EntityTypeBuilder<RmaAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.RmaId, t.AttachmentId });
            
            // modelBuilder
            //     .HasKey(rmaAttachment => rmaAttachment.AttachmentId)
            //     .HasRequired()

            modelBuilder
                .HasOne(rmaAttachment => rmaAttachment.Attachment)
                .WithMany(attachment => attachment.RmaAttachments)
                .HasForeignKey(rmaAttachment => rmaAttachment.AttachmentId);
                
            modelBuilder
                .HasOne(rmaAttachment => rmaAttachment.Rma)
                .WithMany(rma => rma.Attachments)
                .HasForeignKey(rmaAttachment => rmaAttachment.RmaId);
        }
    }
}
