using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and leads
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class CompanyEventLogEntry
    {
        public int? EventLogEntryId { get; set; }
        public EventLogEntry EventLogEntry { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and leads many to many relationship
    /// </summary>
    class CompanyEventLogEntryDBConfiguration : IEntityTypeConfiguration<CompanyEventLogEntry>
    {
        public void Configure(EntityTypeBuilder<CompanyEventLogEntry> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.CompanyId, t.EventLogEntryId });

            modelBuilder
                .HasOne(leadEventLogEntry => leadEventLogEntry.EventLogEntry)
                .WithMany(logEntry => logEntry.CompanyEventLogEntries)
                .HasForeignKey(leadEventLogEntry => leadEventLogEntry.EventLogEntryId);

            modelBuilder
                .HasOne(leadEventLogEntry => leadEventLogEntry.Company)
                .WithMany(lead => lead.EventLogEntries)
                .HasForeignKey(leadEventLogEntry => leadEventLogEntry.CompanyId);
        }
    }
}
