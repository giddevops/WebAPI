using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class InvoiceCredit {
        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int? CreditId { get; set; }
        public Credit Credit { get; set; }

        public decimal Amount { get; set; }
    }

    class InvoiceCreditDBConfiguration : IEntityTypeConfiguration<InvoiceCredit> {
        public void Configure(EntityTypeBuilder<InvoiceCredit> modelBuilder) {
            modelBuilder.HasKey(t => new { t.InvoiceId, t.CreditId });

            modelBuilder
                .HasOne(item => item.Credit)
                .WithMany(item => item.Invoices)
                .HasForeignKey(item => item.CreditId);

            modelBuilder
                .HasOne(item => item.Invoice)
                .WithMany(item => item.Credits)
                .HasForeignKey(item => item.InvoiceId);
        }
    }
}