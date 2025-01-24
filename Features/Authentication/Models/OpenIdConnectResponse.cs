
namespace GidIndustrial.Gideon.WebApi {
    public class OpenIdConnectResponse{
        public string id_token { get; set; }
        public string state { get; set; }
        public string error { get; set; }
        public string error_description { get; set; }
    }
}