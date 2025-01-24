using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Net;
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
    [Route("AccessLogItems")]
    public class AccessLogItemsController : Controller {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public AccessLogItemsController(AppDBContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        // GET: AccessLogItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccessLogItem([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var accessLogItem = await _context.AccessLogItems
                .SingleOrDefaultAsync(m => m.Id == id);

            if (accessLogItem == null) {
                return NotFound();
            }

            return Ok(accessLogItem);
        }

        [HttpGet]
        /// <summary>
        /// GET /AccessLogItems
        /// </summary>
        /// <returns></returns>
        /// [Http]
        public ListResult Index(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            IQueryable<AccessLogItem> query = from accessLogItem in _context.AccessLogItems select accessLogItem;

            query = query
                .OrderBy(l => l.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        [HttpPost]
        public async Task<IActionResult> AccessLogItem([FromBody] AccessLogItemRequestData data) {
            var userId = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            var local = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneInfo);

            
            _context.AccessLogItems.Add(new AccessLogItem {
                Url = data.Url,
                UserId = userId,
                CreatedAtCST = local,
                IPAddress = HttpContext.Connection.RemoteIpAddress.ToString()
            });
            await _context.SaveChangesAsync();
            return Ok();
        }

        private bool AccessLogItemExists(int id) {
            return _context.AccessLogItems.Any(e => e.Id == id);
        }

    }
    public class AccessLogItemRequestData {
        public string Url { get; set; }
    }
}