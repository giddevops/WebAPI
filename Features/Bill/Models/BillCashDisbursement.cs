using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models
{
    public class BillCashDisbursement
    {
        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? BillId { get; set; }
        public Bill Bill { get; set; }

        public int? CashDisbursementId { get; set; }
        public CashDisbursement CashDisbursement { get; set; }

        public decimal Amount { get; set; }
    
    }

    class BillCashDisbursementDBConfiguration : IEntityTypeConfiguration<BillCashDisbursement>
    {
        public void Configure(EntityTypeBuilder<BillCashDisbursement> modelBuilder)
        {
            modelBuilder.HasKey(t => new { t.BillId, t.CashDisbursementId });

            modelBuilder
                .HasOne(item => item.CashDisbursement)
                .WithMany(item => item.Bills)
                .HasForeignKey(item => item.CashDisbursementId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .HasOne(item => item.Bill)
                .WithMany(item => item.CashDisbursements)
                .HasForeignKey(item => item.BillId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}