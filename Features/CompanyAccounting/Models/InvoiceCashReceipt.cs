using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class InvoiceCashReceipt {
        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int? CashReceiptId { get; set; }
        public CashReceipt CashReceipt { get; set; }

        public decimal Amount { get; set; }

    }

    class InvoiceCashReceiptDBConfiguration : IEntityTypeConfiguration<InvoiceCashReceipt> {
        public void Configure(EntityTypeBuilder<InvoiceCashReceipt> modelBuilder) {
            modelBuilder.Property(item => item.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            modelBuilder.HasKey(t => new { t.InvoiceId, t.CashReceiptId });

            modelBuilder
                .HasOne(item => item.CashReceipt)
                .WithMany(item => item.Invoices)
                .HasForeignKey(item => item.CashReceiptId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(item => item.Invoice)
                .WithMany(item => item.CashReceipts)
                .HasForeignKey(item => item.InvoiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}