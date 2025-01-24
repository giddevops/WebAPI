using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class CompanyPortal {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? PortalId { get; set; }
        public Portal Portal { get; set; }


        public string Username { get; set; }
        public string Rating { get; set; }
        public decimal? PositiveFeedbackPercent { get; set; }
        public DateTime? MemberSince { get; set; }
    }

    class CompanyPortalDBConfiguration : IEntityTypeConfiguration<CompanyPortal> {
        public void Configure(EntityTypeBuilder<CompanyPortal> modelBuilder) {

            modelBuilder.HasOne(item => item.Company)
                .WithMany(item => item.Portals)
                .HasForeignKey(item => item.CompanyId);

            modelBuilder.HasOne(item => item.Portal)
                .WithMany()
                .HasForeignKey(item => item.PortalId);
        }
    }
}