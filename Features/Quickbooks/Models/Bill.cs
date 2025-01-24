using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace QuickBooks.Models {

    public class CurrencyRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class AccountRef {
        public string value { get; set; }
        public string name { get; set; }
    }


    public class AccountBasedExpenseLineDetail {
        public CustomerRef CustomerRef { get; set; }
        public AccountRef AccountRef { get; set; }
        public string BillableStatus { get; set; }
        public TaxCodeRef TaxCodeRef { get; set; }
        public decimal TaxInclusiveAmt { get; set; }
    }

    public class ItemBasedExpenseLineDetail {
        public ItemRef ItemRef { get; set; }
        public decimal UnitPrice { get; set; }
        public int Qty { get; set; }
    }

    public class BillLine {
        public string Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal LineNum { get; set; }
        public string DetailType { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public AccountBasedExpenseLineDetail AccountBasedExpenseLineDetail { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public ItemBasedExpenseLineDetail ItemBasedExpenseLineDetail { get; set; }
    }

    public class APAccountRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class Bill {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Status { get; set; }
        public TxnTaxDetail TxnTaxDetail { get; set; }
        public SalesTermRef SalesTermRef { get; set; }
        public string DueDate { get; set; }
        public int Balance { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public string DocNumber { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public CurrencyRef CurrencyRef { get; set; }
        public List<QuickBooksLinkedTxn> LinkedTxn { get; set; }
        public List<BillLine> Line { get; set; }
        public VendorRef VendorRef { get; set; }
        public APAccountRef APAccountRef { get; set; }
        public double TotalAmt { get; set; }
    }

    public class RootObject {
        public Bill Bill { get; set; }
        public DateTime time { get; set; }
    }

    public class BillResponse {
        public Bill Bill { get; set; }
    }
    public class BillDeleted {
        public string Status { get; set; }
    }
    public class BillDeleteResponse {
        public BillDeleted Bill { get; set; }
    }
}