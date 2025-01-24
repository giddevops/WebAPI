using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class WorkLogItemActivityOption {

        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string Value { get; set; }
        public bool Locked { get; set; }

    }

    class WorkLogItemActivityOptionDBConfiguration : IEntityTypeConfiguration<WorkLogItemActivityOption> {
        public void Configure(EntityTypeBuilder<WorkLogItemActivityOption> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        }
    }
}
