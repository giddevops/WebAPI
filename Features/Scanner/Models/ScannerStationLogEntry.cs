using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerStationLogEntry {
        public int? Id { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }
        public int? UserId { get; set; }

        public int? ScannerId { get; set; }

        public int? ScannerLabelId { get; set; }


    }
    class ScannerStationLogEntryDBConfiguration : IEntityTypeConfiguration<ScannerStationLogEntry> {
        public void Configure(EntityTypeBuilder<ScannerStationLogEntry> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}