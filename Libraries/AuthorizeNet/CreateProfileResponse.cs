
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.CreateCustomerResponse {
    public class Message {
        public string code { get; set; }
        public string text { get; set; }
    }

    public class Messages {
        public string resultCode { get; set; }
        public List<Message> message { get; set; }
    }

    public class CreateCustomerResponse {
        public string customerProfileId { get; set; }
        public List<string> customerPaymentProfileIdList { get; set; }
        public List<object> customerShippingAddressIdList { get; set; }
        public List<string> validationDirectResponseList { get; set; }
        public Messages messages { get; set; }
    }
}