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
    public class LeadContactLogItem
    {
        public int? LeadId { get; set; }
        public Lead Lead { get; set; }

        public int ContactLogItemId { get; set; }
        public ContactLogItem ContactLogItem { get; set; }

    }

    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class LeadContactLogItemDBConfiguration : IEntityTypeConfiguration<LeadContactLogItem>
    {
        public void Configure(EntityTypeBuilder<LeadContactLogItem> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.ContactLogItemId, t.LeadId });

            modelBuilder
                .HasOne(item => item.Lead)
                .WithMany(item => item.ContactLogItems)
                .HasForeignKey(item => item.LeadId);

            modelBuilder
                .HasOne(item => item.ContactLogItem)
                .WithMany(item => item.LeadContactLogItems)
                .HasForeignKey(item => item.ContactLogItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
