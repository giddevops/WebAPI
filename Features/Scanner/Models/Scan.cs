using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Scan {
        public int? Id { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }

        [Required]
        public string ScannerSerialNumber { get; set; }

        public int? ScannerId { get; set; }
        public Scanner Scanner { get; set; }

        public bool Implied { get; set; }

        public int? ScannerLabelId { get; set; }
        public ScannerLabel ScannerLabel { get; set; }

        public int? EndScannerLabelId { get; set; }
        public ScannerLabel EndScannerLabel { get; set; }

        public int? ScanGroupId { get; set; }
        public ScanGroup ScanGroup { get; set; }

        public int? ScannerStationId { get; set; }

        public string ResultMessage { get; set; }
        public string ResultCode { get; set; }
    }
    class ScanDBConfiguration : IEntityTypeConfiguration<Scan> {
        public void Configure(EntityTypeBuilder<Scan> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasOne(item => item.ScannerLabel).WithMany().HasForeignKey(item => item.ScannerLabelId);
            modelBuilder.HasOne(item => item.EndScannerLabel).WithMany().HasForeignKey(item => item.EndScannerLabelId);
            modelBuilder.HasIndex(item => item.ScannerStationId);
            modelBuilder.HasOne(item => item.Scanner).WithMany().HasForeignKey(item => item.ScannerId).OnDelete(DeleteBehavior.SetNull);
        }
    }
}