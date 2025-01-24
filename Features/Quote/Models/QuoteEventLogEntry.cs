using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and quotes
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class QuoteEventLogEntry
    {
        public int EventLogEntryId { get; set; }
        public EventLogEntry EventLogEntry { get; set; }

        public int QuoteId { get; set; }
        public Quote Quote { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and quotes many to many relationship
    /// </summary>
    class QuoteEventLogEntryDBConfiguration : IEntityTypeConfiguration<QuoteEventLogEntry>
    {
        public void Configure(EntityTypeBuilder<QuoteEventLogEntry> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.QuoteId, t.EventLogEntryId });
            
            modelBuilder
                .HasOne(quoteEventLogEntry => quoteEventLogEntry.EventLogEntry)
                .WithMany(eventLogEntry => eventLogEntry.QuoteEventLogEntries)
                .HasForeignKey(quoteEventLogEntry => quoteEventLogEntry.EventLogEntryId);
            modelBuilder
                .HasOne(quoteEventLogEntry => quoteEventLogEntry.Quote)
                .WithMany(quote => quote.EventLogEntries)
                .HasForeignKey(quoteEventLogEntry => quoteEventLogEntry.QuoteId);
        }
    }
}
