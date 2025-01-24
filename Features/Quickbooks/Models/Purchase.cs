using System;
using System.Collections.Generic;

namespace QuickBooks.Models {

    public class PurchaseAccountBasedExpenseLineDetail {
        public TaxCodeRef TaxCodeRef { get; set; }
        public AccountRef AccountRef { get; set; }
        public string BillableStatus { get; set; }
    }

    public class PurchaseLine {
        public string DetailType { get; set; }
        public decimal Amount { get; set; }
        public string Id { get; set; }
        public PurchaseAccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
    }

    public class Purchase {
        public string SyncToken { get; set; }
        public string domain { get; set; }
        public string TxnDate { get; set; }
        public decimal TotalAmt { get; set; }
        public string PaymentType { get; set; }
        public bool sparse { get; set; }
        public List<PurchaseLine> Line { get; set; }
        public AccountRef AccountRef { get; set; }
        public List<object> CustomField { get; set; }
        public string Id { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
    }
    
    public class PurchaseResponse {
        public Purchase Purchase { get; set; }
    }
    public class PurchaseDeleted {
        public string Status { get; set; }
    }
    public class PurchaseDeleteResponse { 
        public PurchaseDeleted Purchase { get; set;}
    }
}