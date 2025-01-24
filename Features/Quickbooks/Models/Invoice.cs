using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace QuickBooks.Models {

    public class QuickBooksCustomField {
        public string DefinitionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string StringValue { get; set; }
    }

    public class QuickBooksLinkedTxn {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }

    public class ItemRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class TaxCodeRef {
        public string value { get; set; }
    }

    public class SalesItemLineDetail {
        public ItemRef ItemRef { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountRate { get; set; }
        public int Qty { get; set; }
        public TaxCodeRef TaxCodeRef { get; set; }
    }

    public class DiscountLineDetail {
        public ClassRef ClassRef { get; set; }
        public TaxCodeRef TaxCodeRef { get; set; }
        public DiscountAccountRef DiscountAccountRef { get; set; }
        public bool PercentBased { get; set; }
        public decimal? DiscountPercent { get; set; }
    }

    public class SubTotalLineDetail {
    }
    public class DiscountAccountRef {
        public string value;
        public string name;
    }

    public class ClassRef {
        public string value;
        public string name;
    }

    public class InvoiceLine {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string DetailType { get; set; }
        public SalesItemLineDetail SalesItemLineDetail { get; set; }
        public DiscountLineDetail DiscountLineDetail { get; set; }
        public SubTotalLineDetail SubTotalLineDetail { get; set; }
    }

    public class TxnTaxCodeRef {
        public string value { get; set; }
    }

    public class QuickBooksTaxRateRef {
        public string value { get; set; }
    }

    public class QuickBooksTaxLineDetail {
        public QuickBooksTaxRateRef TaxRateRef { get; set; }
        public bool PercentBased { get; set; }
        public int TaxPercent { get; set; }
        public double NetAmountTaxable { get; set; }
    }

    public class QuickBooksTaxLine {
        public double Amount { get; set; }
        public string DetailType { get; set; }
        public QuickBooksTaxLineDetail TaxLineDetail { get; set; }
    }

    public class TxnTaxDetail {
        public TxnTaxCodeRef TxnTaxCodeRef { get; set; }
        public decimal TotalTax { get; set; }
        public List<QuickBooksTaxLine> TaxLine { get; set; }
    }

    public class CustomerRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CustomerMemo {
        public string value { get; set; }
    }

    public class ShipAddr {
        public string Id { get; set; }
        public string Line1 { get; set; }
        public string City { get; set; }
        public string CountrySubDivisionCode { get; set; }
        public string PostalCode { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
    }

    public class SalesTermRef {
        public string value { get; set; }
    }

    public class QuickBooksBillEmail {
        public string Address { get; set; }
    }
    public class QuickBooksInvoice {
        [JsonIgnore]
        public int? RecordId { get; set; }

        public int Deposit { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        [NotMapped]
        public QuickBooksMetaData MetaData { get; set; }
        [NotMapped]
        public List<QuickBooksCustomField> CustomField { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        [NotMapped]
        public List<QuickBooksLinkedTxn> LinkedTxn { get; set; }
        [NotMapped]
        public List<InvoiceLine> Line { get; set; }
        [NotMapped]
        public TxnTaxDetail TxnTaxDetail { get; set; }
        [NotMapped]
        public CustomerRef CustomerRef { get; set; }
        [NotMapped]
        public CustomerMemo CustomerMemo { get; set; }
        [NotMapped]
        public Address BillAddr { get; set; }
        [NotMapped]
        public Address ShipAddr { get; set; }
        [NotMapped]
        public SalesTermRef SalesTermRef { get; set; }
        public string DueDate { get; set; }
        public string ShipDate { get; set; }
        public double TotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public string EmailStatus { get; set; }
        [NotMapped]
        public QuickBooksBillEmail BillEmail { get; set; }
        public double Balance { get; set; }
        [NotMapped]
        public QuickBooksShipMethodRef ShipMethodRef { get; set; }
        public string TrackingNum { get; set; }
    }
    public class InvoiceResponse {
        public QuickBooksInvoice Invoice { get; set; }
    }
    public class InvoiceQueryResponseContainer {
        public InvoiceQueryResponse QueryResponse { get; set; }
    }
    public class InvoiceQueryResponse {
        public List<QuickBooksInvoice> Invoice { get; set; }
    }

    public class QuickBooksShipMethodRef {
        public string value { get; set; }
    }

    public class InvoiceDeleted {
        public string Status { get; set; }
    }
    public class InvoiceDeleteResponse {
        public InvoiceDeleted Invoice { get; set; }
    }
    public class QuickBooksInvoiceDBConfiguration : IEntityTypeConfiguration<QuickBooksInvoice> {
        public void Configure(EntityTypeBuilder<QuickBooksInvoice> modelBuilder) {
            modelBuilder.HasKey(item => item.RecordId);
            modelBuilder.HasAlternateKey(item => item.Id);
        }
    }
}