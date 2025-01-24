using System;
using System.Collections.Generic;


//This corresponds to SalesOrderPaymentMethod
namespace QuickBooks.Models {
    public class PaymentMethod {
        public string Id { get; set; }
        public string SyncToken { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Type { get; set; }
        public QuickBooksMetaData MetaData { get; set; }
    }
    public class PaymentMethodResponse {
        public PaymentMethod PaymentMethod { get; set; }
    }
    public class PaymentMethodQueryResponseContainer {
        public PaymentMethodQueryResponse QueryResponse { get; set; }
    }
    public class PaymentMethodQueryResponse {
        public List<PaymentMethod> PaymentMethod { get; set; }
    }
}