using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Credit {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? RmaId { get; set; }
        public Rma Rma { get; set; }

        public decimal Amount { get; set; }
        public decimal Balance { get; set; }

        public decimal CurrencyOptionId { get; set; }

        public string Note { get; set; }
        public DateTime? CreditedAt { get; set; }

        public int? CreditAccountId { get; set; }
        public CreditAccount CreditAccount { get; set; }

        public List<InvoiceCredit> Invoices { get; set; }

        public decimal GetBalance() {
            return this.Amount - this.Invoices.Sum(item => item.Amount);
        }
    }

    class CreditDBConfiguration : IEntityTypeConfiguration<Credit> {
        public void Configure(EntityTypeBuilder<Credit> modelBuilder) {
            modelBuilder.HasOne(item => item.Company)
                .WithMany(item => item.Credits)
                .HasForeignKey(item => item.CompanyId);

            modelBuilder.HasOne(item => item.Rma)
                .WithMany(item => item.Credits)
                .HasForeignKey(item => item.RmaId);

            modelBuilder.HasIndex(item => item.CurrencyOptionId);
        }
    }
}
