using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerStation {
        public int? Id { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public Scanner Scanner { get; set; }
        public int? ScannerId { get; set; }

        // public int? GidLocationOptionId { get; set; }
        public int? GidSubLocationOptionId { get; set; }
        // public string ScannerType { get; set; }

        // public ScannerLabelType DefaultScannerLabelType { get; set; }
        // public int? DefaultScannerLabelTypeId { get; set; }

        public ScannerLabel DefaultScannerLabel { get; set; }
        public int? DefaultScannerLabelId { get; set; }

    }
    class ScannerStationDBConfiguration : IEntityTypeConfiguration<ScannerStation> {
        public void Configure(EntityTypeBuilder<ScannerStation> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasOne(item => item.DefaultScannerLabel).WithMany().HasForeignKey(item => item.DefaultScannerLabelId);
            // modelBuilder.HasOne(item => item.DefaultScannerLabelType).WithMany().HasForeignKey(item => item.DefaultScannerLabelTypeId);
            // modelBuilder.Property(item => item.ScannerStationDataType).HasConversion(new EnumToNumberConverter<ScannerStationDataType>());
            modelBuilder.HasOne(item => item.Scanner).WithOne(item => item.ScannerStation).HasForeignKey<ScannerStation>(item => item.ScannerId);
        }
    }
}