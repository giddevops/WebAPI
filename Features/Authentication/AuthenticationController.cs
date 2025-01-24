using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Controllers {
    [Produces("application/json")]
    [Route("Authentication")]
    public class AuthenticationController : Controller {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;
        private static RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider();
        public static int TokenLifetimeMinutes = 60; //dont set to less than 60 ortherwise the person will be logged out of gideon but still logged into azure. In that case, the next person can log in again without a password.

        /// <summary>
        /// This is how long the token will last. After it's halfway to expiration, the token will be replaced with a new one.
        /// </summary>
        /// <param name="tokenLifetime"></param>
        public static void SetTokenLifetimeMinutes(int tokenLifetime) {
            TokenLifetimeMinutes = tokenLifetime;
        }


        public AuthenticationController(AppDBContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        [HttpGet("TokenLifetimeMinutes")]
        public IActionResult GetTokenLifetimeMinutes() {
            return Ok(AuthenticationController.TokenLifetimeMinutes);
        }

        [HttpGet("BarcodeScanApiKey")]
        public IActionResult BarcodeScanApiKey(){
            return Ok(_configuration.GetValue<string>("BarcodeApiKey"));
        }

        [AllowAnonymous]
        [HttpGet("SignIn")]
        /// <summary>
        /// Note, do NOT change the name of this controller or method without also whiteListing it here  "Gideon\WebApi\Extensions\AzureAdAuthenticationBuilderExtensions.cs" in the handler: "options.Events.OnRedirectToIdentityProvider".  Otherwise users will just get a 401 when trying to login!  This method actually calls the RediretToIdentityProvider, so the redirect hook will get called, even though [AllowAnonymous] is set
        /// </summary>
        /// <returns></returns>
        public void SignIn() {
            var redirectUrl = Url.Action(nameof(AuthenticationController.AzureOpenIdAuthenticateSuccess), "Authentication", null, Request.Scheme);
            // return Challenge(
            //     new AuthenticationProperties { RedirectUri = redirectUrl },
            //     OpenIdConnectDefaults.AuthenticationScheme);
            string clientId = _configuration.GetValue<string>("AzureAd:ClientId");
            string tenantId = _configuration.GetValue<string>("AzureAd:TenantId");
            string clientSecret = _configuration.GetValue<string>("AzureAd:ClientSecret");
            // string scope = "https://graph.microsoft.com/.default";
            // string grantType = "client_credentials";

            var url = $"https://login.microsoftonline.com/{tenantId}/oauth2/authorize";
            url += "?client_id=" + clientId;
            url += "&response_type=id_token";
            url += "&redirect_uri=" + WebUtility.UrlEncode(redirectUrl);
            url += "&response_mode=form_post";
            url += "&scope=openid";
            // url += "&state="; this prevents an obscure for of XSRF where an attacker can get the person logged into the attacker's account instead of the person's own account. The idea is that maybe then the person will enter important info like a bank account number, thinking that it is their account. Well, now they have actually entered their bank account number into the bad guy's account.  This isn't really an issue in our system, so I'm leaving this blank.
            url += "&nonce=" + AzureLoginAttempt.CreateLoginAttempt(); //this returns a nonce that will be passed into the login attempt. It expires after 10 minutes, so that if the user starts, but doesn't finish login in 10 minutes, they will have to restart.
            HttpContext.Response.Redirect(url);
        }

        [AllowAnonymous]
        [HttpPost("AzureOpenIdAuthenticateSuccess")]
        public async Task<IActionResult> AzureOpenIdAuthenticateSuccess([FromForm]OpenIdConnectResponse responseData) {
            if (!String.IsNullOrWhiteSpace(responseData.error)) {
                return BadRequest("Microsoft said they ran into a problem logging you in. If this persists, contact support <br>\n" + responseData.error + " <br>\n " + responseData.error_description);
            }

            string clientId = _configuration.GetValue<string>("AzureAd:ClientId");
            string tenantId = _configuration.GetValue<string>("AzureAd:TenantId");

            string stsDiscoveryEndpoint = "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration";
            ConfigurationManager<OpenIdConnectConfiguration> configManager = new ConfigurationManager<OpenIdConnectConfiguration>(stsDiscoveryEndpoint, new OpenIdConnectConfigurationRetriever());
            OpenIdConnectConfiguration config = configManager.GetConfigurationAsync().Result;

            TokenValidationParameters validationParameters = new TokenValidationParameters {
                ValidAudience = clientId,
                ValidateAudience = true,
                ValidIssuer = $"https://sts.windows.net/{tenantId}/",
                ValidateIssuer = true,
                IssuerSigningKeys = config.SigningKeys,
                ValidateLifetime = true
            };
            JwtSecurityTokenHandler tokendHandler = new JwtSecurityTokenHandler();
            SecurityToken jwt;
            var loginData = tokendHandler.ValidateToken(responseData.id_token, validationParameters, out jwt);
            if (loginData == null && jwt == null) {
                return BadRequest("You there was an error validating the JWT");
            }

            //make sure login has valid nonce
            if (!AzureLoginAttempt.CheckIfLoginAttemptValid(loginData.Claims.FirstOrDefault(item => item.Type == "nonce").Value)) {
                return BadRequest("Error - you waited too long to finish logging in. You need to restart");
            }

            //now get user and set cookie
            var userAzureId = loginData.Claims.FirstOrDefault(item => item.Type == "http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            var userAzureIdGuid = new Guid(userAzureId);
            var user = await _context.Users.FirstOrDefaultAsync(item => item.AzureObjectId == userAzureIdGuid);
            if (user == null) {
                return BadRequest("Login successful, but your account is not a part of Gideon");
            }

            var authToken = new AuthenticationToken {
                CreatedAt = DateTime.UtcNow,
                Token = AuthenticationController.GenerateSecureToken(),
                UserId = user.Id
            };
            await _context.AddAsync(authToken);
            await AuthenticationController.ClearExpiredLogins(_context);
            await _context.SaveChangesAsync();

            Response.Cookies.Append("GideonAuthenticationToken", authToken.Token, new CookieOptions {
                HttpOnly = true,
                Expires = authToken.CreatedAt.Value.AddMinutes(AuthenticationController.TokenLifetimeMinutes)
            });

            //make sure that the user is in the database
            return View("~/Features/User/Views/SignInSuccess.cshtml");
        }

        public static async Task ClearExpiredLogins(AppDBContext _context) {
            var earliestCreateDate = DateTime.UtcNow.AddMinutes(-AuthenticationController.TokenLifetimeMinutes);
            var expiredLogins = _context.AuthenticationTokens.Where(item => item.CreatedAt < earliestCreateDate);
            _context.AuthenticationTokens.RemoveRange(expiredLogins);
            await _context.SaveChangesAsync();
        }

        public static AuthenticationToken GenerateNewAuthenticationToken(int? userId) {
            return new AuthenticationToken {
                CreatedAt = DateTime.UtcNow,
                Token = AuthenticationController.GenerateSecureToken(),
                UserId = userId
            };
        }
        public static string GenerateSecureToken() {
            var bytes = new byte[32];
            rngCsp.GetBytes(bytes);

            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }

        [AllowAnonymous]
        [HttpGet("SignOutSuccess")]
        public IActionResult SignOutSuccess() {
            return View("~/Features/User/Views/SignOutSuccess.cshtml");
        }



        /// <summary>
        /// Sign out. Note - this has the same issue as SignIn. Read comment for that method if you want to change the name of this function
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("SignOut")]
        public async Task SignOut() {
            var callbackUrl = Url.Action(nameof(AuthenticationController.SignOutSuccess), "Authentication", values: null, protocol: Request.Scheme);
            var url = $"https://login.microsoftonline.com/common/oauth2/logout";
            url += "?post_logout_redirect_uri=" + WebUtility.UrlEncode(callbackUrl);

            //expire cookie
            string gideonAuthenticationToken;
            Request.Cookies.TryGetValue("GideonAuthenticationToken", out gideonAuthenticationToken);
            if (!String.IsNullOrWhiteSpace(gideonAuthenticationToken)) {
                var createdAtEarliestTime = DateTime.UtcNow.AddMinutes(-AuthenticationController.TokenLifetimeMinutes);
                var authenticationToken = await _context.AuthenticationTokens.FirstOrDefaultAsync(item => item.Token == gideonAuthenticationToken && item.CreatedAt > createdAtEarliestTime);
                if (authenticationToken != null) {
                    _context.AuthenticationTokens.Remove(authenticationToken);
                }
            }
            Response.Cookies.Delete("GideonAuthenticationToken");
            HttpContext.Response.Redirect(url);
        }

        [HttpGet("SignedOut")]
        public IActionResult SignedOut() {
            if (User.Identity.IsAuthenticated) {
                // Redirect to home page if the user is authenticated.
                return Redirect("/");
            }

            return View();
        }
    }
}