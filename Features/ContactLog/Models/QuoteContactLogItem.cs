using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    /// <summary>
    /// This class creates the joining table
    /// This is necessary because EF Core won't build it automatically otherwise
    /// </summary>
    public class QuoteContactLogItem
    {
        public int? QuoteId { get; set; }
        public Quote Quote { get; set; }

        public int ContactLogItemId { get; set; }
        public ContactLogItem ContactLogItem { get; set; }

    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class QuoteContactLogItemDBConfiguration : IEntityTypeConfiguration<QuoteContactLogItem>
    {
        public void Configure(EntityTypeBuilder<QuoteContactLogItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ContactLogItemId, t.QuoteId });

            modelBuilder
                .HasOne(item => item.Quote)
                .WithMany(item => item.ContactLogItems)
                .HasForeignKey(item => item.QuoteId);

            modelBuilder
                .HasOne(item => item.ContactLogItem)
                .WithMany(item => item.QuoteContactLogItems)
                .HasForeignKey(item => item.ContactLogItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
