using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Libraries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class AccessLogItem {

        public int? Id { get; set; }
        public int? UserId { get; set; }
        public string IPAddress { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTimeOffset? CreatedAtCST { get; set; }

        
        public string Url { get; set; }

    }
    class AccessLogItemDBConfiguration : IEntityTypeConfiguration<AccessLogItem> {
        public void Configure(EntityTypeBuilder<AccessLogItem> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasIndex(item => item.CreatedAt);
            modelBuilder.HasIndex(item => new { item.UserId, item.CreatedAt });
        }
    }
}