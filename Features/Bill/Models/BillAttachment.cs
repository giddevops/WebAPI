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
    public class BillAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int BillId { get; set; }
        public Bill Bill { get; set; }
    }

    class BillAttachmentDBConfiguration : IEntityTypeConfiguration<BillAttachment>
    {
        public void Configure(EntityTypeBuilder<BillAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.BillId, t.AttachmentId });
            modelBuilder
                .HasOne(productAttachment => productAttachment.Attachment)
                .WithMany(attachment => attachment.BillAttachments)
                .HasForeignKey(productAttachment => productAttachment.AttachmentId);

            modelBuilder
                .HasOne(productAttachment => productAttachment.Bill)
                .WithMany(product => product.Attachments)
                .HasForeignKey(productAttachment => productAttachment.BillId);
        }
    }
}
