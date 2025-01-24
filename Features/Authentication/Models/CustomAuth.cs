
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Controllers;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi {
    public static class CustomAuthExtensions {
        public static AuthenticationBuilder AddCustomAuth(this AuthenticationBuilder builder, Action<CustomAuthOptions> configureOptions) {
            return builder.AddScheme<CustomAuthOptions, CustomAuthHandler>("Custom Scheme", "Custom Auth", configureOptions);
        }
    }
    public class CustomAuthOptions : AuthenticationSchemeOptions {
        public CustomAuthOptions() {

        }
    }
    internal class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions> {
        private readonly ILogger _logger;
        private readonly IServiceCollection _services;
        private readonly IWebHost _webhost;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public CustomAuthHandler(IOptionsMonitor<CustomAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, IServiceScopeFactory scopeFactory) : base(options, logger, encoder, clock) {
            // store custom services here...
            this._serviceScopeFactory = scopeFactory;
        }
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync() {
            // build the claims and put them in "Context"; you need to import the Microsoft.AspNetCore.Authentication package


            string gideonAuthenticationToken;
            Request.Cookies.TryGetValue("GideonAuthenticationToken", out gideonAuthenticationToken);
            if (String.IsNullOrWhiteSpace(gideonAuthenticationToken)) {
                return AuthenticateResult.Fail(new Exception("You are not logged in"));
            }

            // return AuthenticateResult.Success(new AuthenticationTicket());
            // return AuthenticateResult.NoResult();
            // create a ClaimsPrincipal from your header
            using (var scope = this._serviceScopeFactory.CreateScope()) {
                var provider = scope.ServiceProvider;
                    var createdAtEarliestTime = DateTime.UtcNow.AddMinutes(-AuthenticationController.TokenLifetimeMinutes);
                using (var dbContext = provider.GetRequiredService<AppDBContext>()) {
                    var authenticationToken = await dbContext.AuthenticationTokens.FirstOrDefaultAsync(item => item.Token == gideonAuthenticationToken && item.CreatedAt > createdAtEarliestTime);
                    if (authenticationToken == null) {
                        return AuthenticateResult.Fail(new Exception("You are not logged in"));
                    }
                    var oneMinuteAgo = DateTime.UtcNow.AddMinutes(-1); //if it has been more than a minute send a new token
                    // Console.WriteLine("Halfway is " + halfwayToExpirationTime + " Minuts are " + (-AuthenticationController.DefaultTokenLifetime / 2).ToString());
                    
                    if (authenticationToken.CreatedAt < oneMinuteAgo) {
                        // var newToken = AuthenticationController.GenerateNewAuthenticationToken(authenticationToken.UserId);
                        // dbContext.AuthenticationTokens.Remove(authenticationToken);
                        // dbContext.AuthenticationTokens.Add(newToken);
                        authenticationToken.CreatedAt = DateTime.UtcNow;
                        await dbContext.SaveChangesAsync();
                        Response.Cookies.Append("GideonAuthenticationToken", authenticationToken.Token, new CookieOptions {
                            HttpOnly = true,
                            Expires = authenticationToken.CreatedAt.Value.AddMinutes(AuthenticationController.TokenLifetimeMinutes)
                        });
                    }
                    var claims = new List<Claim>{
                        new Claim(CustomClaimTypes.UserId, authenticationToken.UserId.ToString()),
                    };
                    var user = await dbContext.Users
                        .Include(item => item.UserGroups)
                            .ThenInclude(item => item.UserGroup)
                        .SingleAsync(item => item.Id == authenticationToken.UserId);
                        
                    foreach(var userGroup in user.UserGroups){
                        claims.Add(new Claim("groups",userGroup.UserGroup.AzureId));
                    }

                    var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme.Name));
                    var ticket = new AuthenticationTicket(claimsPrincipal,
                        new AuthenticationProperties { IsPersistent = false },
                        Scheme.Name
                    );
                    return AuthenticateResult.Success(ticket);
                }
            }

            // return AuthenticateResult.Success(ticket);
            return AuthenticateResult.Fail(new Exception("You are not logged in"));
        }
    }
}