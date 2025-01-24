using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and purchaseOrders
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class PurchaseOrderNote
    {
        public int NoteId { get; set; }
        public Note Note { get; set; }

        public int PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and purchaseOrders many to many relationship
    /// </summary>
    class PurchaseOrderNoteDBConfiguration : IEntityTypeConfiguration<PurchaseOrderNote>
    {
        public void Configure(EntityTypeBuilder<PurchaseOrderNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.PurchaseOrderId, t.NoteId });

            modelBuilder
                .HasOne(purchaseOrderNote => purchaseOrderNote.Note)
                .WithMany(note => note.PurchaseOrderNotes)
                .HasForeignKey(purchaseOrderNote => purchaseOrderNote.NoteId);

            modelBuilder
                .HasOne(item => item.PurchaseOrder)
                .WithMany(purchaseOrder => purchaseOrder.Notes)
                .HasForeignKey(item => item.PurchaseOrderId);
        }
    }
}
