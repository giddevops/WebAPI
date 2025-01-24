using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ViewFilter {
        public int? Id { get; set; }

        [Required]
        public int ViewId { get; set; }
        public View View { get; set; }
        public string Name { get; set; }

        public string FieldName { get; set; }

        // [Column(TypeName = "NVARCHAR(MAX)")]
        public string ViewFilterCondition { get; set; }

        public bool UserViewable { get; set; }

        public string Value { get; set; }
        public string FieldType { get; set; }

        public bool ForAlertsOnly { get; set; }

        public int? OrGroupNumber { get; set; }

        // public List<ViewFilterDateValue> DateValues { get; set; }
        // public List<ViewFilterNumericValue> NumericValues { get; set; }
        // public List<ViewFilterStringValue> StringValues { get; set; }
        // public List<ViewFilterUserValue> UserValues { get; set; }
    }

    class ViewFilterDBConfiguration : IEntityTypeConfiguration<ViewFilter> {
        public void Configure(EntityTypeBuilder<ViewFilter> modelBuilder) {
            // modelBuilder.HasMany(item => item.DateValues).WithOne(item => item.ViewFilter).HasForeignKey(item => item.ViewFilterId);
            // modelBuilder.HasMany(item => item.NumericValues).WithOne(item => item.ViewFilter).HasForeignKey(item => item.ViewFilterId);
            // modelBuilder.HasMany(item => item.StringValues).WithOne(item => item.ViewFilter).HasForeignKey(item => item.ViewFilterId);
            // modelBuilder.HasMany(item => item.UserValues).WithOne(item => item.ViewFilter).HasForeignKey(item => item.ViewFilterId);
        }
    }

}