using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ScannerLabelVariableValue {
        public int? Id { get; set; }

        [Required]
        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        [Required]
        public int? ScannerLabelTypeVariableId { get; set; }
        public int? ScannerLabelId { get; set; }
        public string Value { get; set; }

    }
    
    class ScannerLabelVariableValueDBConfiguration : IEntityTypeConfiguration<ScannerLabelVariableValue> {
        public void Configure(EntityTypeBuilder<ScannerLabelVariableValue> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        }
    }
}