using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and quotes
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class QuoteAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int QuoteId { get; set; }
        public Quote Quote { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and quotes many to many relationship
    /// </summary>
    class QuoteAttachmentDBConfiguration : IEntityTypeConfiguration<QuoteAttachment>
    {
        public void Configure(EntityTypeBuilder<QuoteAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.QuoteId, t.AttachmentId });
            
            // modelBuilder
            //     .HasKey(quoteAttachment => quoteAttachment.AttachmentId)
            //     .HasRequired()

            modelBuilder
                .HasOne(quoteAttachment => quoteAttachment.Attachment)
                .WithMany(attachment => attachment.QuoteAttachments)
                .HasForeignKey(quoteAttachment => quoteAttachment.AttachmentId);
                // .HasAlternateKey
            
            // modelBuilder.
                // .HasForeignKey(quoteAttachment => quoteAttachment.AttachmentId);

            modelBuilder
                .HasOne(quoteAttachment => quoteAttachment.Quote)
                .WithMany(quote => quote.Attachments)
                .HasForeignKey(quoteAttachment => quoteAttachment.QuoteId);
        }
    }
}
