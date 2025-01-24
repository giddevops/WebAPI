using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
// Microsoft.EntityFrameworkCore.Storage.ValueConversion

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerLabelTypeVariable {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public string Name { get; set; }

        [Column(TypeName = "nvarchar(24)")]
        public ScannerLabelTypeVariableDataType ScannerLabelTypeVariableDataType { get; set; }

        public int? ScannerLabelTypeId { get; set; }

        public string ObjectName { get; set; }
        public string ObjectField { get; set; }
        public string FieldType { get; set; }
        // public string Value { get; set; }
    }

    class ScannerLabelTypeVariableDBConfiguration : IEntityTypeConfiguration<ScannerLabelTypeVariable> {
        public void Configure(EntityTypeBuilder<ScannerLabelTypeVariable> modelBuilder) {
            // modelBuilder.Property(item => item.ScannerLabelTypeVariableDataType).HasConversion(new EnumToNumberConverter<ScannerLabelTypeVariableDataType>());
        }
    }

    public enum ScannerLabelTypeVariableDataType {
        NUMBER = 1,
        STRING = 2,
        DATE = 3,
        URL = 4,
        OBJECT_REFERENCE = 5
    }
}