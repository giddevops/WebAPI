using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class View {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public string Name { get; set; }
        public string ObjectName { get; set; }

        public int? UserGroupId { get; set; }
        public UserGroup UserGroup { get; set; }

        public string AlertEmails { get; set; }

        public bool Deactivated { get; set; }

        public List<ViewDisplayField> DisplayFields { get; set; }
        public List<ViewFilter> Filters { get; set; }
        public List<ViewConditionalFormatter> ConditionalFormatters { get; set; }

        public DateTime? LastViewCacheDate { get; set; }

    }

    class ViewDBConfiguration : IEntityTypeConfiguration<View> {
        public void Configure(EntityTypeBuilder<View> modelBuilder) {
            modelBuilder.HasMany(item => item.DisplayFields).WithOne(item => item.View).HasForeignKey(item => item.ViewId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasMany(item => item.Filters).WithOne(item => item.View).HasForeignKey(item => item.ViewId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasMany(item => item.ConditionalFormatters).WithOne(item => item.View).HasForeignKey(item => item.ViewId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasOne(item => item.UserGroup).WithMany().HasForeignKey(item => item.UserGroupId);

        }
    }
}