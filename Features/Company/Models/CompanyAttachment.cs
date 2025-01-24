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
    public class CompanyAttachment
    {
        public int AttachmentId { get; set; }
        public Attachment Attachment { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and companys many to many relationship
    /// </summary>
    class CompanyAttachmentDBConfiguration : IEntityTypeConfiguration<CompanyAttachment>
    {
        public void Configure(EntityTypeBuilder<CompanyAttachment> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.CompanyId, t.AttachmentId });
            modelBuilder
                .HasOne(companyAttachment => companyAttachment.Attachment)
                .WithMany(attachment => attachment.CompanyAttachments)
                .HasForeignKey(companyAttachment => companyAttachment.AttachmentId);

            modelBuilder
                .HasOne(companyAttachment => companyAttachment.Company)
                .WithMany(company => company.Attachments)
                .HasForeignKey(companyAttachment => companyAttachment.CompanyId);
        }
    }
}
