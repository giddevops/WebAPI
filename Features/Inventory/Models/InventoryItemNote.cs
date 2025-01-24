using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and inventoryItems
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class InventoryItemNote
    {
        public int? NoteId { get; set; }
        public Note Note { get; set; }

        public int? InventoryItemId { get; set; }
        // public InventoryItem InventoryItem { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and inventoryItems many to many relationship
    /// </summary>
    class InventoryItemNoteDBConfiguration : IEntityTypeConfiguration<InventoryItemNote>
    {
        public void Configure(EntityTypeBuilder<InventoryItemNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.InventoryItemId, t.NoteId });

            modelBuilder
                .HasOne(inventoryItemNote => inventoryItemNote.Note)
                .WithMany()
                .HasForeignKey(inventoryItemNote => inventoryItemNote.NoteId);

            // modelBuilder
            //     .HasOne(inventoryItemNote => inventoryItemNote.InventoryItem)
            //     .WithMany(inventoryItem => inventoryItem.Notes)
            //     .HasForeignKey(inventoryItemNote => inventoryItemNote.InventoryItemId);
        }
    }
}
