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
    public class SalesOrderNote
    {
        public int? NoteId { get; set; }
        public Note Note { get; set; }

        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and salesOrders many to many relationship
    /// </summary>
    class SalesOrderNoteDBConfiguration : IEntityTypeConfiguration<SalesOrderNote>
    {
        public void Configure(EntityTypeBuilder<SalesOrderNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.SalesOrderId, t.NoteId });

            modelBuilder
                .HasOne(salesOrderNote => salesOrderNote.Note)
                .WithMany(note => note.SalesOrderNotes)
                .HasForeignKey(salesOrderNote => salesOrderNote.NoteId);

            modelBuilder
                .HasOne(salesOrderNote => salesOrderNote.SalesOrder)
                .WithMany(salesOrder => salesOrder.Notes)
                .HasForeignKey(salesOrderNote => salesOrderNote.SalesOrderId);
        }
    }
}
