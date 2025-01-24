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
    public class QuoteNote
    {
        public int NoteId { get; set; }
        public Note Note { get; set; }

        public int QuoteId { get; set; }
        public Quote Quote { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and quotes many to many relationship
    /// </summary>
    class QuoteNoteDBConfiguration : IEntityTypeConfiguration<QuoteNote>
    {
        public void Configure(EntityTypeBuilder<QuoteNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.QuoteId, t.NoteId });

            modelBuilder
                .HasOne(quoteNote => quoteNote.Note)
                .WithMany(note => note.QuoteNotes)
                .HasForeignKey(quoteNote => quoteNote.NoteId);

            modelBuilder
                .HasOne(quoteNote => quoteNote.Quote)
                .WithMany(quote => quote.Notes)
                .HasForeignKey(quoteNote => quoteNote.QuoteId);
        }
    }
}
