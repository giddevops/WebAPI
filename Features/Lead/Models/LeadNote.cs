using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and leads
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class LeadNote
    {
        public int? NoteId { get; set; }
        public Note Note { get; set; }

        public int? LeadId { get; set; }
        // public Lead Lead { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and leads many to many relationship
    /// </summary>
    class LeadNoteDBConfiguration : IEntityTypeConfiguration<LeadNote>
    {
        public void Configure(EntityTypeBuilder<LeadNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.LeadId, t.NoteId });

            modelBuilder
                .HasOne(leadNote => leadNote.Note)
                .WithMany(item => item.LeadNotes)
                .HasForeignKey(leadNote => leadNote.NoteId);

            // modelBuilder
            //     .HasOne(leadNote => leadNote.Lead)
            //     .WithMany(lead => lead.Notes)
            //     .HasForeignKey(leadNote => leadNote.LeadId);
        }
    }
}
