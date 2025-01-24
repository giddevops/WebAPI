using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and inventoryItems
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class InventoryItemAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and inventoryItems many to many relationship
    /// </summary>
    class InventoryItemAttachmentDBConfiguration : IEntityTypeConfiguration<InventoryItemAttachment>
    {
        public void Configure(EntityTypeBuilder<InventoryItemAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.InventoryItemId, t.AttachmentId });
            modelBuilder
                .HasOne(inventoryItemAttachment => inventoryItemAttachment.Attachment)
                .WithMany(attachment => attachment.InventoryItemAttachments)
                .HasForeignKey(inventoryItemAttachment => inventoryItemAttachment.AttachmentId);

            modelBuilder
                .HasOne(inventoryItemAttachment => inventoryItemAttachment.InventoryItem)
                .WithMany(inventoryItem => inventoryItem.Attachments)
                .HasForeignKey(inventoryItemAttachment => inventoryItemAttachment.InventoryItemId);
        }
    }
}
