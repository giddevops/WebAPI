using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and rmas
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class RmaEventLogEntry
    {
        public int EventLogEntryId { get; set; }
        public EventLogEntry EventLogEntry { get; set; }

        public int RmaId { get; set; }
        public Rma Rma { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and rmas many to many relationship
    /// </summary>
    class RmaEventLogEntryDBConfiguration : IEntityTypeConfiguration<RmaEventLogEntry>
    {
        public void Configure(EntityTypeBuilder<RmaEventLogEntry> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.RmaId, t.EventLogEntryId });
            
            modelBuilder
                .HasOne(rmaEventLogEntry => rmaEventLogEntry.EventLogEntry)
                .WithMany()
                .HasForeignKey(rmaEventLogEntry => rmaEventLogEntry.EventLogEntryId);
            modelBuilder
                .HasOne(rmaEventLogEntry => rmaEventLogEntry.Rma)
                .WithMany(rma => rma.EventLogEntries)
                .HasForeignKey(rmaEventLogEntry => rmaEventLogEntry.RmaId);
        }
    }
}
