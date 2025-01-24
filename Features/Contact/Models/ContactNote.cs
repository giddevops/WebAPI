using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and contacts
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class ContactNote
    {
        public int NoteId { get; set; }
        public Note Note { get; set; }

        public int ContactId { get; set; }
        public Contact Contact { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and contacts many to many relationship
    /// </summary>
    class ContactNoteDBConfiguration : IEntityTypeConfiguration<ContactNote>
    {
        public void Configure(EntityTypeBuilder<ContactNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.ContactId, t.NoteId });

            modelBuilder
                .HasOne(contactNote => contactNote.Note)
                .WithMany(note => note.ContactNotes)
                .HasForeignKey(contactNote => contactNote.NoteId);

            modelBuilder
                .HasOne(contactNote => contactNote.Contact)
                .WithMany(contact => contact.Notes)
                .HasForeignKey(contactNote => contactNote.ContactId);
        }
    }
}
