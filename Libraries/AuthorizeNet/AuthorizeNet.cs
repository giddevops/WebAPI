
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet {

    public class AuthorizeNetApiRequestor {
        public static string apiUrl = "https://apitest.authorize.net/xml/v1/request.api";
        static HttpClient httpClient;
        public static string merchantAuthenticationName = "6ASudd33Mb"; // sandbox credentials
        public static string transactionKey = "36z59V6Xs4naNd6J";
        public static string ValidationMode = "none";


        static AuthorizeNetApiRequestor() {
            httpClient = new HttpClient();
        }

        public static void SetLiveMode() {
            AuthorizeNetApiRequestor.apiUrl = "https://api.authorize.net/xml/v1/request.api";
            AuthorizeNetApiRequestor.merchantAuthenticationName = "6ASudd33Mb";
            AuthorizeNetApiRequestor.transactionKey = "36z59V6Xs4naNd6J";
            AuthorizeNetApiRequestor.ValidationMode = "none";
        }
        public static async Task<string> DoApiRequest(dynamic requestBody) {
            var response = await httpClient.PostAsync(apiUrl, new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json"));
            var responseString = await response.Content.ReadAsStringAsync();
            return responseString;
        }
        public static AuthorizeNetResults GetResult(dynamic responseBody) {
            AuthorizeNetResults results = new AuthorizeNetResults();
            if (responseBody.messages != null) {
                results.ResultCode = responseBody.messages.resultCode;
            }
            if (responseBody.messages.message != null) {
                results.ResultMessageCode = responseBody.messages.message[0].code;
                results.ResultMessageText = responseBody.messages.message[0].text;
            }
            return results;
        }
    }
    public class AuthorizeNetResults {
        public string ResultCode { get; set; }
        public string ResultMessageCode { get; set; }
        public string ResultMessageText { get; set; }
    }
}