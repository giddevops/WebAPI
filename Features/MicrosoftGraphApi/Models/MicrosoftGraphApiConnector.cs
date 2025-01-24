using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class MicrosoftGraphApiConnector {
        public static readonly HttpClient client = new HttpClient();
        public static string accessToken;
        public static long? lastTokenFetched = 0;
        public static IConfiguration configuration { get; set; }

        public static async Task<string> DoGraphRequest(string url) {
            if(MicrosoftGraphApiConnector.configuration == null){
                throw new Exception("The configuration on the graph connector is null");
            }
            //in order to get the information on user groups, have to access the Microsoft Graph API.
            //The appsettings.json file stores a "client secret". To connect to the API you have to use that client secret to request a token
            //THen you can use that token to actually request the data you want. Seems kind of roundabout but that's what you have to do
            var currentTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            HttpResponseMessage response;
            string responseBody;
            if (MicrosoftGraphApiConnector.lastTokenFetched == null || currentTimestamp - MicrosoftGraphApiConnector.lastTokenFetched > 300) {
                string clientId = configuration.GetValue<string>("AzureAd:ClientId");
                string tenantId = configuration.GetValue<string>("AzureAd:TenantId");
                string clientSecret = configuration.GetValue<string>("AzureAd:ClientSecret");
                string scope = "https://graph.microsoft.com/.default";
                string grantType = "client_credentials";

                var oauthUrl = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

                // HttpWebRequest request = (HttpWebRequest)WebRequest.Create(oauthUrl);
                // request.Method = "POST";
                var bodyParams = new Dictionary<string, string>{
                    { "client_id", clientId },
                    { "client_secret", clientSecret },
                    { "grant_type", grantType },
                    { "scope", scope }
                };
                var body = new FormUrlEncodedContent(bodyParams);
                response = await MicrosoftGraphApiConnector.client.PostAsync(oauthUrl, body);
                responseBody = await response.Content.ReadAsStringAsync();
                TokenResponseBody responseBodyParsed = JsonConvert.DeserializeObject<TokenResponseBody>(responseBody);
                MicrosoftGraphApiConnector.lastTokenFetched = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                MicrosoftGraphApiConnector.accessToken = responseBodyParsed.access_token;
            }

            MicrosoftGraphApiConnector.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MicrosoftGraphApiConnector.accessToken);
            response = await client.GetAsync(url);
            responseBody = await response.Content.ReadAsStringAsync();
            if((int)response.StatusCode < 200 || (int)response.StatusCode >= 300){
                throw new Exception("Error getting requrest " + responseBody);
            }

            return responseBody;
        }
    }
    public class TokenResponseBody {
        public string token_type;
        public int expires_in;
        public int ext_expires_in;
        public string access_token;
    }
}
