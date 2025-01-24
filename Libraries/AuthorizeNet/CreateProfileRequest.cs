
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.CreateProfileRequest {
    public class MerchantAuthentication {
        public string name { get; set; }
        public string transactionKey { get; set; }
    }

    public class CreditCard {
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public string cardCode { get; set; }
    }

    public class OpaqueData {
        public string dataDescriptor { get; set; }
        public string dataValue { get; set; }
    }

    public class CreditCardPayment {
        public CreditCard creditCard { get; set; }
    }
    public class OpaqueDataPayment {
        public OpaqueData opaqueData { get; set; }
    }

    public class PaymentProfiles {
        public string customerType { get; set; }
        public dynamic payment { get; set; }
    }

    public class Profile {
        public string merchantCustomerId { get; set; }
        public string description { get; set; }
        public string email { get; set; }
        public PaymentProfiles paymentProfiles { get; set; }
    }

    public class CreateCustomerProfileRequest {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public Profile profile { get; set; }
        public string validationMode { get; set; }
    }
}