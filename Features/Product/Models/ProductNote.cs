using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and products
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class ProductNote
    {
        public int NoteId { get; set; }
        public Note Note { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and products many to many relationship
    /// </summary>
    class ProductNoteDBConfiguration : IEntityTypeConfiguration<ProductNote>
    {
        public void Configure(EntityTypeBuilder<ProductNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.ProductId, t.NoteId });

            modelBuilder
                .HasOne(productNote => productNote.Note)
                .WithMany(note => note.ProductNotes)
                .HasForeignKey(productNote => productNote.NoteId);

            modelBuilder
                .HasOne(productNote => productNote.Product)
                .WithMany(product => product.Notes)
                .HasForeignKey(productNote => productNote.ProductId);
        }
    }
}
