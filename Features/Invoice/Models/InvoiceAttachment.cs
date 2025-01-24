using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and products
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class InvoiceAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }

    class InvoiceAttachmentDBConfiguration : IEntityTypeConfiguration<InvoiceAttachment>
    {
        public void Configure(EntityTypeBuilder<InvoiceAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.InvoiceId, t.AttachmentId });
            modelBuilder
                .HasOne(productAttachment => productAttachment.Attachment)
                .WithMany(attachment => attachment.InvoiceAttachments)
                .HasForeignKey(productAttachment => productAttachment.AttachmentId);

            modelBuilder
                .HasOne(productAttachment => productAttachment.Invoice)
                .WithMany(product => product.Attachments)
                .HasForeignKey(productAttachment => productAttachment.InvoiceId);
        }
    }
}
