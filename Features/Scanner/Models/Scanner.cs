using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Scanner {
        public int? Id { get; set; }
        [Required]
        public DateTime? CreatedAt { get; set; }

        public int? CreatedById { get; set; }

        public string ModelNumber { get; set; }
        public string SerialNumber { get; set; }
        public string Type { get; set; }

        public ScannerStation ScannerStation { get; set; }

    }
    class ScannerDBConfiguration : IEntityTypeConfiguration<Scanner> {
        public void Configure(EntityTypeBuilder<Scanner> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasIndex(item => item.SerialNumber);
            modelBuilder.HasIndex(item => new { item.SerialNumber, item.ModelNumber });
        }
    }
}