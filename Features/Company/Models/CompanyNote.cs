using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table for attachments and companys
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class CompanyNote
    {
        public int NoteId { get; set; }
        public Note Note { get; set; }

        public int CompanyId { get; set; }
        public Company Company { get; set; }
    }

    /// <summary>
    /// This sets up foreign keys in the database connecting attachments and companys many to many relationship
    /// </summary>
    class CompanyNoteDBConfiguration : IEntityTypeConfiguration<CompanyNote>
    {
        public void Configure(EntityTypeBuilder<CompanyNote> modelBuilder)
        {
            modelBuilder
            .HasKey(t => new { t.CompanyId, t.NoteId });

            modelBuilder
                .HasOne(companyNote => companyNote.Note)
                .WithMany(note => note.CompanyNotes)
                .HasForeignKey(companyNote => companyNote.NoteId);

            modelBuilder
                .HasOne(companyNote => companyNote.Company)
                .WithMany(company => company.Notes)
                .HasForeignKey(companyNote => companyNote.CompanyId);
        }
    }
}
