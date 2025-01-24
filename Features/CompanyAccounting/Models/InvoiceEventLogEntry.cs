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
    public class InvoiceEventLogEntry
    {
        public int? EventLogEntryId { get; set; }
        public EventLogEntry EventLogEntry { get; set; }

        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and salesOrders many to many relationship
    /// </summary>
    class InvoiceEventLogEntryDBConfiguration : IEntityTypeConfiguration<InvoiceEventLogEntry>
    {
        public void Configure(EntityTypeBuilder<InvoiceEventLogEntry> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.InvoiceId, t.EventLogEntryId });

            modelBuilder
                .HasOne(item => item.EventLogEntry)
                .WithMany()
                .HasForeignKey(item => item.EventLogEntryId);

            modelBuilder
                .HasOne(item => item.Invoice)
                .WithMany(item => item.EventLogEntries)
                .HasForeignKey(item => item.InvoiceId);
        }
    }
}
