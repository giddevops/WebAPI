using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace QuickBooks.Models {

    public class DepositToAccountRef {
        public string value { get; set; }
    }

    public class ValueObj {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class Any {
        public string name { get; set; }
        public string declaredType { get; set; }
        public string scope { get; set; }
        public ValueObj value { get; set; }
        public bool nil { get; set; }
        public bool globalScope { get; set; }
        public bool typeSubstituted { get; set; }
    }

    public class LineEx {
        public List<Any> any { get; set; }
    }

    public class PaymentLine {
        public decimal Amount { get; set; }
        public List<QuickBooksLinkedTxn> LinkedTxn { get; set; }
        public LineEx LineEx { get; set; }
    }

    public class QuickBooksPayment {
        [JsonIgnore]
        public int? RecordId { get; set; }

        [NotMapped]
        public CustomerRef CustomerRef { get; set; }
        [NotMapped]
        public DepositToAccountRef DepositToAccountRef { get; set; }
        [NotMapped]
        public PaymentMethodRef PaymentMethodRef { get; set; }
        [NotMapped]
        public QuickBooksMetaData MetaData { get; set; }
        [NotMapped]
        public List<PaymentLine> Line { get; set; }

        public decimal UnappliedAmt { get; set; }
        public bool ProcessPayment { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public decimal TotalAmt { get; set; }
        public string TxnDate { get; set; }
        public string PaymentRefNum { get; set; }
    }

    public class PaymentResponse {
        public QuickBooksPayment Payment { get; set; }
    }
    public class PaymentDeleted {
        public string Status { get; set; }
    }
    public class PaymentDeleteResponse {
        public PaymentDeleted Payment { get; set; }
    }
    public class PaymentQueryResponseContainer {
        public PaymentQueryResponse QueryResponse { get; set; }
    }
    public class PaymentQueryResponse {
        public List<QuickBooksPayment> Payment { get; set; }
    }
    public class QuickBooksPaymentDBConfiguration : IEntityTypeConfiguration<QuickBooksPayment> {
        public void Configure(EntityTypeBuilder<QuickBooksPayment> modelBuilder) {
            modelBuilder.HasKey(item => item.RecordId);
            modelBuilder.HasAlternateKey(item => item.Id);
        }
    }
}