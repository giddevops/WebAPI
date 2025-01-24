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
    public class RmaNote
    {
        public int NoteId { get; set; }
        public Note Note { get; set; }

        public int RmaId { get; set; }
        public Rma Rma { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and quotes many to many relationship
    /// </summary>
    class RmaNoteDBConfiguration : IEntityTypeConfiguration<RmaNote>
    {
        public void Configure(EntityTypeBuilder<RmaNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.RmaId, t.NoteId });

            modelBuilder
                .HasOne(rmaNote => rmaNote.Note)
                .WithMany(note => note.RmaNotes)
                .HasForeignKey(rmaNote => rmaNote.NoteId);

            modelBuilder
                .HasOne(rmaNote => rmaNote.Rma)
                .WithMany(rma => rma.Notes)
                .HasForeignKey(rmaNote => rmaNote.RmaId);
        }
    }
}
