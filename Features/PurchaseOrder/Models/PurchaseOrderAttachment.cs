using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and purchaseOrders
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class PurchaseOrderAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and purchaseOrders many to many relationship
    /// </summary>
    class PurchaseOrderAttachmentDBConfiguration : IEntityTypeConfiguration<PurchaseOrderAttachment>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.PurchaseOrderId, t.AttachmentId });
            modelBuilder
                .HasOne(purchaseOrderAttachment => purchaseOrderAttachment.Attachment)
                .WithMany()
                .HasForeignKey(purchaseOrderAttachment => purchaseOrderAttachment.AttachmentId);

            modelBuilder
                .HasOne(purchaseOrderAttachment => purchaseOrderAttachment.PurchaseOrder)
                .WithMany(purchaseOrder => purchaseOrder.Attachments)
                .HasForeignKey(purchaseOrderAttachment => purchaseOrderAttachment.PurchaseOrderId);
        }
    }
}
