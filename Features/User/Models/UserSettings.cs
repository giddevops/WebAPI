// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations.Schema;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.EntityFrameworkCore.Metadata.Builders;

// namespace GidIndustrial.Gideon.WebApi.Models {
//     public class UserSettingsContainer {
//         public int? Id { get; set; }

//         public GidLocationOption DefaultGidLocationOption { get; set; }
//         public int? DefaultGidLocationOptionId { get; set; }
//     }

//     /// <summary>
//     /// set up createdAt, updatedAt and createdBy auto properties
//     /// </summary>
//     class UserSettingsContainerDBConfiguration : IEntityTypeConfiguration<UserSettingsContainer> {
//         public void Configure(EntityTypeBuilder<UserSettingsContainer> modelBuilder) {
//             modelBuilder.HasOne(item => item.DefaultGidLocationOption)
//             .WithMany()
//             .HasForeignKey(item => item.DefaultGidLocationOptionId);
//         }
//     }
// }