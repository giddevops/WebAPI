using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and salesOrders
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class SalesOrderAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and salesOrders many to many relationship
    /// </summary>
    class SalesOrderAttachmentDBConfiguration : IEntityTypeConfiguration<SalesOrderAttachment>
    {
        public void Configure(EntityTypeBuilder<SalesOrderAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.SalesOrderId, t.AttachmentId });
            modelBuilder
                .HasOne(salesOrderAttachment => salesOrderAttachment.Attachment)
                .WithMany(attachment => attachment.SalesOrderAttachments)
                .HasForeignKey(salesOrderAttachment => salesOrderAttachment.AttachmentId);

            modelBuilder
                .HasOne(salesOrderAttachment => salesOrderAttachment.SalesOrder)
                .WithMany(salesOrder => salesOrder.Attachments)
                .HasForeignKey(salesOrderAttachment => salesOrderAttachment.SalesOrderId);
        }
    }
}
