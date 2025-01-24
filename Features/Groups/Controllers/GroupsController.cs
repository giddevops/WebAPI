using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Groups")]
    public class GroupsController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration configuration;
        public static readonly HttpClient client = new HttpClient();
        public static string accessToken;
        public static long? lastTokenFetched = 0;

        public GroupsController(AppDBContext context, IConfiguration appConfig) {
            this._context = context;
            this.configuration = appConfig;
        }

        // GET: 
        [HttpGet("ShortSelectOptions")]
        public IEnumerable<dynamic> GetEmailTemplateTypes() {
            return _context.UserGroups.OrderBy(item => item.Name).Select(item => new {
                Id = item.Id,
                Value = item.Name.Replace(" Security Group", "")
            });
        }

        // GET: Group
        /// <summary>
        /// This fetches the groups from azure active directory so that groups can be assigned.
        /// </summary>
        /// <returns></returns>
        [RequirePermission("ManagePermissions")]
        public async Task<string> GetAccessManagementGroups() {
            return await MicrosoftGraphApiConnector.DoGraphRequest("https://graph.microsoft.com/v1.0/groups?$filter=securityEnabled eq true");
        }
    }


    // public class GraphAuthProvider
    // {
    //     private readonly IMemoryCache _memoryCache;
    //     private TokenCache _userTokenCache;

    //     // Properties used to get and manage an access token.
    //     private readonly string _appId;
    //     private readonly ClientCredential _credential;
    //     private readonly string[] _scopes;
    //     private readonly string _redirectUri;
    //     public GraphAuthProvider(IMemoryCache memoryCache, IConfiguration configuration)
    //     {
    //         var azureOptions = new AzureAdOptions();
    //         configuration.Bind("AzureAd", azureOptions);

    //         _appId = azureOptions.ClientId;
    //         _credential = new ClientCredential(azureOptions.ClientSecret);
    //         _scopes = azureOptions.GraphScopes.Split(new[] { ' ' });
    //         _redirectUri = azureOptions.BaseUrl + azureOptions.CallbackPath;

    //         _memoryCache = memoryCache;
    //     }
    // }
}