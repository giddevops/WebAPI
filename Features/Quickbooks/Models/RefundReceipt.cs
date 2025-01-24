using System;
using System.Collections.Generic;

namespace QuickBooks.Models {

    public class RefundReceiptLine {
        public string Id { get; set; }
        public int LineNum { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string DetailType { get; set; }
        public SalesItemLineDetail SalesItemLineDetail { get; set; }
        public SubTotalLineDetail SubTotalLineDetail { get; set; }
    }

    public class PaymentMethodRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class RefundReceipt {
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
        public List<QuickBooksCustomField> CustomField { get; set; }
        public string DocNumber { get; set; }
        public string TxnDate { get; set; }
        public List<RefundReceiptLine> Line { get; set; }
        public TxnTaxDetail TxnTaxDetail { get; set; }
        public CustomerRef CustomerRef { get; set; }
        public CustomerMemo CustomerMemo { get; set; }
        public Address BillAddr { get; set; }
        public Address ShipAddr { get; set; }
        public decimal TotalAmt { get; set; }
        public bool ApplyTaxAfterDiscount { get; set; }
        public string PrintStatus { get; set; }
        public QuickBooksBillEmail BillEmail { get; set; }
        public int Balance { get; set; }
        public PaymentMethodRef PaymentMethodRef { get; set; }
        public DepositToAccountRef DepositToAccountRef { get; set; }
    }

    public class RefundReceiptResponse {
        public RefundReceipt RefundReceipt { get; set; }
    }
    public class RefundReceiptDeleted {
        public string Status { get; set; }
    }
    public class RefundReceiptDeleteResponse {
        public RefundReceiptDeleted RefundReceipt { get; set; }
    }
}