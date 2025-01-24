using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.GetPaymentProfilesResponse {
    public class Message {
        public string code { get; set; }
        public string text { get; set; }
    }

    public class Messages {
        public string resultCode { get; set; }
        public List<Message> message { get; set; }
    }

    public class BillTo {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class CreditCard {
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
    }

    public class Payment {
        public CreditCard creditCard { get; set; }
    }

    public class PaymentProfile {
        public string customerPaymentProfileId { get; set; }
        public string customerProfileId { get; set; }
        public BillTo billTo { get; set; }
        public Payment payment { get; set; }
    }

    public class PaymentProfiles {
        public PaymentProfile paymentProfile { get; set; }
    }

    public class GetCustomerPaymentProfileListResponse {
        public Messages messages { get; set; }
        public string totalNumInResultSet { get; set; }
        public PaymentProfiles paymentProfiles { get; set; }
    }

    public class GetPaymentProfilesResponse {
        public GetCustomerPaymentProfileListResponse getCustomerPaymentProfileListResponse { get; set; }
    }
}