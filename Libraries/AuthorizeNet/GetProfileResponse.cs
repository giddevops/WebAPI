
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.GetProfileResponse {
    public class CreditCard {
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public string cardType { get; set; }
        public string issuerNumber { get; set; }
        public bool isPaymentToken { get; set; }
    }

    public class Payment {
        public CreditCard creditCard { get; set; }
    }

    public class BillTo {
        public string firstName { get; set; }
        public string lastName { get; set; }
    }

    public class PaymentProfile {
        public bool defaultPaymentProfile { get; set; }
        public string customerPaymentProfileId { get; set; }
        public Payment payment { get; set; }
        public string customerType { get; set; }
        public BillTo billTo { get; set; }
    }

    public class Profile {
        public List<PaymentProfile> paymentProfiles { get; set; }
        public string customerProfileId { get; set; }
        public string merchantCustomerId { get; set; }
        public string description { get; set; }
        public string email { get; set; }
    }

    public class Message {
        public string code { get; set; }
        public string text { get; set; }
    }

    public class Messages {
        public string resultCode { get; set; }
        public List<Message> message { get; set; }
    }

    public class AuthorizeNetProfile {
        public Profile profile { get; set; }
        public List<string> subscriptionIds { get; set; }
        public Messages messages { get; set; }
    }
}