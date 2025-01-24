using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Ganss.XSS;
using System.Net.Http;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;

namespace WebApi.Features.Controllers {
    public class QuickBooksTokenResponse {
        public string access_token;
        public int expires_in;
        public string refresh_token;
        public string token_type;
        public int X_refresh_token_expires_in;
    }


    [Produces("application/json")]
    [Route("QuickBooks")]
    [RequirePermission("ManageBilling")]
    public class QuickBooksController : Controller {
        private readonly AppDBContext _context;
        public static readonly HttpClient client = new HttpClient();
        private QuickBooksConnector _quickBooksConnector;
        private readonly IHostingEnvironment _environment;


        public QuickBooksController(AppDBContext context, QuickBooksConnector quickBooksConnector, IHostingEnvironment environment) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
            _environment = environment;
        }

        [HttpGet("OauthRedirect")]
        public async Task<IActionResult> OauthRedirect([FromQuery] string code, [FromQuery] string realmId, [FromQuery] string error) {
            try {
            /* HIDE ALL QB INTEGRATION
                if (_environment.IsDevelopment()) {
                    // QuickBooksConnector.EnableLiveMode();
                } else {
                    QuickBooksConnector.EnableLiveMode();
                }
                var oauthUrl = $"https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer";
                // var clientId = "Q0exRgwiP75EwfxmQuIga9OWlTf6lv6YWF8ZyBNzlRbO6iYMNp";
                // var clientSecret = "LKYEPkWY7UcG0ZI3Nsij49q2BgeX8y4aCCWvBKtV";

                var bodyParams = new Dictionary<string, string>{
                    { "code", code },
                    { "redirect_uri", QuickBooksConnector.RedirectUrl },
                    { "grant_type", "authorization_code" }
                };
                var content = new FormUrlEncodedContent(bodyParams);
                var authorizationHeader = System.Convert.ToBase64String(System.Text.UTF8Encoding.UTF8.GetBytes(QuickBooksConnector.ClientId + ":" + QuickBooksConnector.ClientSecret));
                var request = new HttpRequestMessage {
                    RequestUri = new Uri(oauthUrl),
                    Method = HttpMethod.Post,
                    Content = content
                };
                request.Headers.Add("Authorization", "Basic " + authorizationHeader);
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                QuickBooksTokenResponse responseBodyParsed = JsonConvert.DeserializeObject<QuickBooksTokenResponse>(responseBody);

                if (String.IsNullOrWhiteSpace(responseBodyParsed.refresh_token)) {
                    return BadRequest("Error getting token. Refresh token was null.  Response was " + responseBody);
                }
                QuickBooksConnector.AccessToken = responseBodyParsed.access_token;
                QuickBooksConnector.AccessTokenExpires = DateTime.UtcNow.AddSeconds(responseBodyParsed.expires_in - 15);

                var quickBooksApiInfo = new QuickBooksApiInfo {
                    RefreshToken = responseBodyParsed.refresh_token,
                    RefreshTokenExpiresAt = DateTime.UtcNow.AddSeconds(responseBodyParsed.X_refresh_token_expires_in).AddSeconds(-15),
                    LastUpdated = DateTime.UtcNow,
                    RealmId = realmId
                };
                _quickBooksConnector.SetQuickBooksApiInfo(quickBooksApiInfo);

                var quickBooksApiInfoString = JsonConvert.SerializeObject(quickBooksApiInfo);
                var quickBooksConfig = await _context.Configs.FirstOrDefaultAsync(item => item.Id == 1);
                quickBooksConfig.Data = Encryption.EncryptData(quickBooksApiInfoString);
                await _context.SaveChangesAsync();
                */
                return Ok("done");

            }
            catch (Exception e) {
                return BadRequest(e.Message + e.StackTrace);
            }
        }
    }
}