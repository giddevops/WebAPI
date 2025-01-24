using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and contacts
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class ContactAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and contacts many to many relationship
    /// </summary>
    class ContactAttachmentDBConfiguration : IEntityTypeConfiguration<ContactAttachment>
    {
        public void Configure(EntityTypeBuilder<ContactAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ContactId, t.AttachmentId });
            modelBuilder
                .HasOne(contactAttachment => contactAttachment.Attachment)
                .WithMany(attachment => attachment.ContactAttachments)
                .HasForeignKey(contactAttachment => contactAttachment.AttachmentId);

            modelBuilder
                .HasOne(contactAttachment => contactAttachment.Contact)
                .WithMany(contact => contact.Attachments)
                .HasForeignKey(contactAttachment => contactAttachment.ContactId);
        }
    }
}
