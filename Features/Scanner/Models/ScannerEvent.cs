// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace GidIndustrial.Gideon.WebApi.Models {
//     public class ScannerEvent {
//         public int? Id { get; set; }

//         [Required]
//         public DateTime? CreatedAt { get; set; }
//         [Required]
//         public int? CreatedById { get; set; }

//         public ScannerLabelType EventLabelType { get; set; }
//         public int? ScannerEventLabelTypeId { get; set; }

//         public List<ScannerEventScannerLabelType> DataLabelTypes { get; set; }

//         public int? ScannerId { get; set; }

//         public int? ScannerStationId { get; set; }

//         public ScannerActionUpdateLocation ScannerActionUpdateLocation { get; set; }
//         public ScannerActionRelatePieceParts ScannerActionRelatePieceParts { get; set; }
//         public ScannerActionUpdateWorkLog ScannerActionUpdateWorkLog { get; set; }
//         public ScannerActionUpdateSystemData ScannerActionUpdateSystemData { get; set; }

//         public bool IsUpdateLocationOnly(){
//             return (
//                 (this.ScannerActionUpdateLocation != null && this.ScannerActionUpdateLocation.Active) &&
//                 (this.ScannerActionUpdateWorkLog == null || !this.ScannerActionUpdateWorkLog.Active) &&
//                 (this.ScannerActionRelatePieceParts == null || !this.ScannerActionRelatePieceParts.Active) &&
//                 (this.ScannerActionUpdateSystemData == null || !this.ScannerActionUpdateSystemData.Active)
//             );
//         }
//     }
//     class ScannerEventDBConfiguration : IEntityTypeConfiguration<ScannerEvent> {
//         public void Configure(EntityTypeBuilder<ScannerEvent> modelBuilder) {
//             modelBuilder.HasOne(item => item.EventLabelType).WithMany().HasForeignKey(item => item.ScannerEventLabelTypeId).OnDelete(DeleteBehavior.Cascade);
//             modelBuilder.HasAlternateKey(item => item.ScannerEventLabelTypeId);

//             modelBuilder.HasOne(item => item.ScannerActionUpdateLocation).WithOne().HasForeignKey<ScannerActionUpdateLocation>(item => item.ScannerEventId).OnDelete(DeleteBehavior.Cascade);
//             modelBuilder.HasOne(item => item.ScannerActionRelatePieceParts).WithOne().HasForeignKey<ScannerActionRelatePieceParts>(item => item.ScannerEventId).OnDelete(DeleteBehavior.Cascade);
//             modelBuilder.HasOne(item => item.ScannerActionUpdateWorkLog).WithOne().HasForeignKey<ScannerActionUpdateWorkLog>(item => item.ScannerEventId).OnDelete(DeleteBehavior.Cascade);
//             modelBuilder.HasOne(item => item.ScannerActionUpdateSystemData).WithOne().HasForeignKey<ScannerActionUpdateSystemData>(item => item.ScannerEventId).OnDelete(DeleteBehavior.Cascade);
//         }
//     }
// }