using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and leads
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class LeadAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int LeadId { get; set; }
        public Lead Lead { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and leads many to many relationship
    /// </summary>
    class LeadAttachmentDBConfiguration : IEntityTypeConfiguration<LeadAttachment>
    {
        public void Configure(EntityTypeBuilder<LeadAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.LeadId, t.AttachmentId });
            modelBuilder
                .HasOne(leadAttachment => leadAttachment.Attachment)
                .WithMany(attachment => attachment.LeadAttachments)
                .HasForeignKey(leadAttachment => leadAttachment.AttachmentId);

            modelBuilder
                .HasOne(leadAttachment => leadAttachment.Lead)
                .WithMany(lead => lead.Attachments)
                .HasForeignKey(leadAttachment => leadAttachment.LeadId);
        }
    }
}
