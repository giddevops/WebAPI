using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.GetPaymentProfilesRequest {
    public class MerchantAuthentication {
        public string name { get; set; }
        public string transactionKey { get; set; }
    }

    public class Sorting {
        public string orderBy { get; set; }
        public string orderDescending { get; set; }
    }

    public class Paging {
        public string limit { get; set; }
        public string offset { get; set; }
    }

    public class GetCustomerPaymentProfileListRequest {
        public MerchantAuthentication merchantAuthentication { get; set; }
        public Sorting sorting { get; set; }
        public Paging paging { get; set; }
    }

    public class GetPaymentProfilesRequest {
        public GetCustomerPaymentProfileListRequest getCustomerPaymentProfileListRequest { get; set; }
    }
}