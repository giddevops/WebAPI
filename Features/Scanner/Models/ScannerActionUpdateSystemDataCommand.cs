using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerActionUpdateSystemDataCommand {
        public int? Id { get; set; }

        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? ScannerActionUpdateSystemDataId { get; set; }

        [Required]
        public string Type { get; set; }

        public string ObjectName { get; set; }
        public string ObjectField { get; set; }
        public string FieldType { get; set; }

        // The value can be a text constant
        public string TextValue { get; set; }

        //The value can be stored in another label.  If so, these variables are used to store the label type, and which variable within that label type contains the value.
        public ScannerLabelType ValueScannerLabelType { get; set; }
        public int? ValueScannerLabelTypeId { get; set; }
        public ScannerLabelTypeVariable ValueScannerLabelTypeVariable { get; set; }
        public int? ValueScannerLabelTypeVariableId { get; set; }


    }
     
    class ScannerActionUpdateSystemDataCommandDBConfiguration : IEntityTypeConfiguration<ScannerActionUpdateSystemDataCommand> {
        public void Configure(EntityTypeBuilder<ScannerActionUpdateSystemDataCommand> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasOne(item => item.ValueScannerLabelTypeVariable).WithMany().HasForeignKey(item => item.ValueScannerLabelTypeVariableId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ScannerActionUpdateSystemDataCommandType {
        public const string UPDATE = "UPDATE";
        public const string INSERT = "INSERT";
        public const string DELETE = "DELETE";
    }
}