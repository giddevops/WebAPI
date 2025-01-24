using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class EndScannerLabel {
        public int? Id { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }
        public Guid? BarcodeGuid { get; set; }
        public int? StartScannerLabelId { get; set; }
        public ScannerLabel StartScannerLabel { get; set; }
    }
    class EndScannerLabelDBConfiguration : IEntityTypeConfiguration<EndScannerLabel> {
        public void Configure(EntityTypeBuilder<EndScannerLabel> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasIndex(item => item.BarcodeGuid);
        }
    }
}