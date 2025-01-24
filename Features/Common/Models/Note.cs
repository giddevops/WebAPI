using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class Note
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        public User CreatedBy { get; set; }
        public int? CreatedById { get; set; }

        [Column(TypeName = "text")]
        public string Text { get; set; }

        public List<LeadNote> LeadNotes { get; set; }
        public List<QuoteNote> QuoteNotes { get; set; }
        public List<ContactNote> ContactNotes { get; set; }
        public List<CompanyNote> CompanyNotes { get; set; }
        public List<ProductNote> ProductNotes { get; set; }
        public List<SourceNote> SourceNotes { get; set; }
        public List<SalesOrderNote> SalesOrderNotes { get; set; }
        public List<PurchaseOrderNote> PurchaseOrderNotes { get; set; }
        public List<InventoryItemNote> InventoryItemNotes { get; set; }
        public List<RmaNote> RmaNotes { get; set;  }
    }

    /// <summary>
    /// set db configuration
    /// </summary>
    class NoteDBConfiguration : IEntityTypeConfiguration<Note>
    {
        public void Configure(EntityTypeBuilder<Note> modelBuilder)
        {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(l => l.CreatedBy)
                .WithMany()
                .HasForeignKey(l => l.CreatedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
