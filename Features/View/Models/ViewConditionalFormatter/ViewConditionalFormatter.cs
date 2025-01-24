using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class ViewConditionalFormatter {
        public int? Id { get; set; }

        [Required]
        public int ViewId { get; set; }
        public View View { get; set; }
        
        public string Name { get; set; }
        public List<ViewConditionalFormatterCondition> Conditions { get; set; }
        public string FormatType { get; set; }
        public string FormatValue { get; set; }
    }

    class ViewConditionalFormatDBConfiguration : IEntityTypeConfiguration<ViewConditionalFormatter> {
        public void Configure(EntityTypeBuilder<ViewConditionalFormatter> modelBuilder) {
            modelBuilder.HasMany(item => item.Conditions).WithOne(item => item.ViewConditionalFormatter).HasForeignKey(item => item.ViewConditionalFormatterId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}