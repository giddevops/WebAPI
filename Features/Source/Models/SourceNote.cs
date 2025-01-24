using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and sources
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class SourceNote
    {
        public int NoteId { get; set; }
        public Note Note { get; set; }

        public int SourceId { get; set; }
        public Source Source { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and sources many to many relationship
    /// </summary>
    class SourceNoteDBConfiguration : IEntityTypeConfiguration<SourceNote>
    {
        public void Configure(EntityTypeBuilder<SourceNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.SourceId, t.NoteId });

            modelBuilder
                .HasOne(sourceNote => sourceNote.Note)
                .WithMany(note => note.SourceNotes)
                .HasForeignKey(sourceNote => sourceNote.NoteId);

            modelBuilder
                .HasOne(sourceNote => sourceNote.Source)
                .WithMany(source => source.Notes)
                .HasForeignKey(sourceNote => sourceNote.SourceId);
        }
    }
}
