using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Listings")]
    public class ListingsController : Controller {
        private readonly AppDBContext _context;
        private static readonly HttpClient HttpClient;
        private IConfiguration _configuration;
        
        static ListingsController() {
            HttpClient = new HttpClient();
        }

        public ListingsController(AppDBContext context, IConfiguration appConfig) {
            _context = context;
            this._configuration = appConfig;
        }

        // GET: ListingsContainers
        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string search) {
            
            var createdById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            var url = "https://sourcery-api.gidindustrial.com/v1/source?term=" + WebUtility.UrlEncode(search);

            var request = new HttpRequestMessage {
                RequestUri = new Uri(url),
                Method = HttpMethod.Get
            };
            request.Headers.Add("API-Key", _configuration.GetValue<string>("ListingsApiKey"));

            var response = await ListingsController.HttpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            if ((int)response.StatusCode != 200) {
                return BadRequest(new {
                    Error = responseBody,
                    StatusCode = response.StatusCode
                });
            }

            return Content(responseBody);
        }
    }
}