using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ViewCacheItem {
        public int? Id { get; set; }

        [Required]
        public int ViewId { get; set; }
        public View View { get; set; }

        public int ItemId { get; set; }
    }
    class ViewCacheItemDBConfiguration : IEntityTypeConfiguration<ViewCacheItem> {
        public void Configure(EntityTypeBuilder<ViewCacheItem> modelBuilder) {
            modelBuilder.HasOne(item => item.View).WithMany().HasForeignKey(item => item.ViewId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.HasIndex(item => new { item.ViewId, item.ItemId });
        }
    }
}