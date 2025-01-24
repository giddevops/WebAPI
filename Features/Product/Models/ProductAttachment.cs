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
    public class ProductAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and products many to many relationship
    /// </summary>
    class ProductAttachmentDBConfiguration : IEntityTypeConfiguration<ProductAttachment>
    {
        public void Configure(EntityTypeBuilder<ProductAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ProductId, t.AttachmentId });
            modelBuilder
                .HasOne(productAttachment => productAttachment.Attachment)
                .WithMany(attachment => attachment.ProductAttachments)
                .HasForeignKey(productAttachment => productAttachment.AttachmentId);

            modelBuilder
                .HasOne(productAttachment => productAttachment.Product)
                .WithMany(product => product.Attachments)
                .HasForeignKey(productAttachment => productAttachment.ProductId);
        }
    }
}
