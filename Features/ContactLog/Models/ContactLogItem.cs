using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class ContactLogItem
    {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public DateTime? ContactDate { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string Notes { get; set; }

        public int? UserId { get; set; }
        public string ContactName { get; set; }
        public int? ContactMethodOptionId { get; set; }
        public int? ContactReasonOptionId { get; set; }

        public List<LeadContactLogItem> LeadContactLogItems { get; set; }
        public List<QuoteContactLogItem> QuoteContactLogItems { get; set; }
    }
    
    /// <summary>
    /// This sets up foreign keys in the database foreign keys
    /// </summary>
    class ContactLogItemDBConfiguration : IEntityTypeConfiguration<ContactLogItem>
    {
        public void Configure(EntityTypeBuilder<ContactLogItem> modelBuilder)
        {
            // modelBuilder.HasOne(item => item.InReplyToContactLogItem)
            //     .WithMany(item => item.Replies)
            //     .HasForeignKey(item => item.InReplyToContactLogItemId)
            //     .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
