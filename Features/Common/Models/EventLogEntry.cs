using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class EventLogEntry
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }


        public DateTime OccurredAt { get; set; }
        public string Event { get; set; }
        public int? UserId { get; set; }
        // public HistoryActionType ActionType { get; set; }
        // public User Modified { get; set; }


        public List<LeadEventLogEntry> LeadEventLogEntries { get; set; }
        public List<QuoteEventLogEntry> QuoteEventLogEntries { get; set; }
        public List<CompanyEventLogEntry> CompanyEventLogEntries { get; set; }
        public List<ContactEventLogEntry> ContactEventLogEntries { get; set; }
        public List<SalesOrderEventLogEntry> SalesOrderEventLogEntries { get; set; }
        public List<InventoryItemEventLogEntry> InventoryItemEventLogEntries { get; set; }

    }
    /// <summary>
    /// Setup 
    /// </summary>
    class EventLogEntryDBConfiguration : IEntityTypeConfiguration<EventLogEntry>
    {
        public void Configure(EntityTypeBuilder<EventLogEntry> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

        }
    }
}
