
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.CreatePaymentProfileRequest {
    public class MerchantAuthentication {
        public string name { get; set; }
        public string transactionKey { get; set; }
    }

    public class BillTo {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string address { get; set; }
        public string city { get; set; }
        public string state { get; set; }
        public string zip { get; set; }
        public string country { get; set; }
        public string phoneNumber { get; set; }
    }

    public class CreditCard {
        public string cardNumber { get; set; }
        public string expirationDate { get; set; }
        public string cardCode { get; set; }
    }

    public class OpaqueDataPayment {
        // public CreditCard creditCard { get; set; }
        public OpaqueData opaqueData { get; set; }
    }
    
    public class CreditCardPayment {
        public CreditCard creditCard { get; set; }
    }

    public class OpaqueData {
        public string dataDescriptor { get; set; }
        public string dataValue { get; set; }
    }

    public class PaymentProfile {
        public BillTo billTo { get; set; }
        public dynamic payment { get; set; }
        public bool defaultPaymentProfile { get; set; }
    }

    public class CreateCustomerPaymentProfileRequest {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public string customerProfileId { get; set; }
        public PaymentProfile paymentProfile { get; set; }
        public string validationMode { get; set; }
    }

    public class CreatePaymentProfileRequest {
        public CreateCustomerPaymentProfileRequest createCustomerPaymentProfileRequest { get; set; }
    }
}