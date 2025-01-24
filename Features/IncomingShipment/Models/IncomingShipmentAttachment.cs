using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and companys
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class IncomingShipmentAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int IncomingShipmentId { get; set; }
        public IncomingShipment IncomingShipment { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and companys many to many relationship
    /// </summary>
    class IncomingShipmentAttachmentDBConfiguration : IEntityTypeConfiguration<IncomingShipmentAttachment>
    {
        public void Configure(EntityTypeBuilder<IncomingShipmentAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.IncomingShipmentId, t.AttachmentId });
            modelBuilder
                .HasOne(companyAttachment => companyAttachment.Attachment)
                .WithMany(attachment => attachment.IncomingShipmentAttachments)
                .HasForeignKey(companyAttachment => companyAttachment.AttachmentId);

            modelBuilder
                .HasOne(companyAttachment => companyAttachment.IncomingShipment)
                .WithMany(company => company.Attachments)
                .HasForeignKey(companyAttachment => companyAttachment.IncomingShipmentId);
        }
    }
}
