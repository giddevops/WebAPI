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
    public class SalesOrderEventLogEntry
    {
        public int? EventLogEntryId { get; set; }
        public EventLogEntry EventLogEntry { get; set; }

        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and salesOrders many to many relationship
    /// </summary>
    class SalesOrderEventLogEntryDBConfiguration : IEntityTypeConfiguration<SalesOrderEventLogEntry>
    {
        public void Configure(EntityTypeBuilder<SalesOrderEventLogEntry> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.SalesOrderId, t.EventLogEntryId });

            modelBuilder
                .HasOne(salesOrderEventLogEntry => salesOrderEventLogEntry.EventLogEntry)
                .WithMany(logEntry => logEntry.SalesOrderEventLogEntries)
                .HasForeignKey(salesOrderEventLogEntry => salesOrderEventLogEntry.EventLogEntryId);

            modelBuilder
                .HasOne(salesOrderEventLogEntry => salesOrderEventLogEntry.SalesOrder)
                .WithMany(salesOrder => salesOrder.EventLogEntries)
                .HasForeignKey(salesOrderEventLogEntry => salesOrderEventLogEntry.SalesOrderId);
        }
    }
}
