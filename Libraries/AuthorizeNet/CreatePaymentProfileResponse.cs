
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.CreatePaymentProfileResponse {
    public class Message {
        public string code { get; set; }
        public string text { get; set; }
    }

    public class Messages {
        public string resultCode { get; set; }
        public List<Message> message { get; set; }
    }

    public class CreatePaymentProfileResponse {
        public string customerProfileId { get; set; }
        public string customerPaymentProfileId { get; set; }
        public string validationDirectResponse { get; set; }
        public Messages messages { get; set; }
    }
}