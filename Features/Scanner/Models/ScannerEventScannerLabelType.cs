// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace GidIndustrial.Gideon.WebApi.Models {
//     public class ScannerEventScannerLabelType {
//         public ScannerEvent ScannerEvent { get; set; }
//         public int? ScannerEventId { get; set; }

//         public ScannerLabelType ScannerLabelType { get; set; }
//         public int? ScannerLabelTypeId { get; set; }
//     }
//     class ScannerEventScannerLabelTypeDBConfiguration : IEntityTypeConfiguration<ScannerEventScannerLabelType> {
//         public void Configure(EntityTypeBuilder<ScannerEventScannerLabelType> modelBuilder) {
//             modelBuilder.HasKey(item => new { item.ScannerEventId, item.ScannerLabelTypeId });
//             modelBuilder.HasOne(item => item.ScannerEvent).WithMany(item => item.DataLabelTypes).HasForeignKey(item => item.ScannerEventId).OnDelete(DeleteBehavior.Cascade);
//             modelBuilder.HasOne(item => item.ScannerLabelType).WithMany().HasForeignKey(item => item.ScannerLabelTypeId).OnDelete(DeleteBehavior.Restrict);
//         }
//     }
// }