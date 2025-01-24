using System;
using System.Collections.Generic;

namespace QuickBooks.Models {

    public class VendorRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class BankAccountRef {
        public string value { get; set; }
        public string name { get; set; }
    }

    public class CheckPayment {
        public BankAccountRef BankAccountRef { get; set; }
        public string PrintStatus { get; set; }
    }

    public class BillPaymentLine {
        public decimal Amount { get; set; }
        public List<QuickBooksLinkedTxn> LinkedTxn { get; set; }
    }

    public class BillPayment {
        public VendorRef VendorRef { get; set; }
        public string PayType { get; set; }
        public CheckPayment CheckPayment { get; set; }
        public decimal TotalAmt { get; set; }
        public string domain { get; set; }
        public bool sparse { get; set; }
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
        public string TxnDate { get; set; }
        public string PrivateNote { get; set; }
        public List<BillPaymentLine> Line { get; set; }
    }
    
    public class BillPaymentResponse {
        public BillPayment BillPayment { get; set; }
    }
    public class BillPaymentDeleted {
        public string Status { get; set; }
    }
    public class BillPaymentDeleteResponse {
        public BillPaymentDeleted BillPayment { get; set; }
    }
}