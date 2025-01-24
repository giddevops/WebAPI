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
    [Route("Users")]
    public class UsersController : Controller {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public UsersController(AppDBContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        [RequirePermission("EditUsers")]
        [HttpGet("RefreshAzureAD")]
        public IActionResult GetAzureAd() {
            GidIndustrial.Gideon.WebApi.Models.User.GetAllUsers(_context);
            return Ok();
        }

        // PUT: Users/5
        [RequirePermission("EditUsers")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser([FromRoute] int id, [FromBody] User user) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != user.Id) {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!UserExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// GET /Users/Me
        /// </summary>
        /// <returns></returns>
        [HttpGet("Me")]
        public async Task<User> Me() {
            int? userId = Gideon.WebApi.Models.User.GetId(HttpContext.User);
            var user = await _context.Users
                .Include(item => item.DefaultGidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                        .ThenInclude(item => item.Country)
                .Include(item => item.DefaultGidLocationOption)
                    .ThenInclude(item => item.DefaultShippingAddress)
                .Include(item => item.LeadRoutingRules)
                .Include(item => item.UserGroups)
                    .ThenInclude(item => item.UserGroup)
                .FirstOrDefaultAsync(u => u.Id == userId);
            user.Permissions = new List<string> { };

            List<Permission> allPermissions = await _context.Permissions.ToListAsync();
            List<string> userGroups = user.UserGroups.Select(item => item.UserGroup.AzureId).ToList<string>();
            
            foreach (var permission in allPermissions) {
                List<string> allowedGroups = JsonConvert.DeserializeObject<List<string>>(permission.AllowedGroups);
                var matchingGroups = allowedGroups.Intersect(userGroups);
                if (matchingGroups.Count() > 0) {
                    user.Permissions.Add(permission.Name);
                }
            }

            return user;
        }

        // GET: Users/SearchByName
        [HttpGet("SearchByName")]
        public ListResult SearchByName(
            [FromQuery] string searchString,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            IQueryable<User> query = from user in _context.Users select user;

            query = query.Where(item => item.DisplayName.StartsWith(searchString, StringComparison.OrdinalIgnoreCase));

            query = query.OrderByDescending(l => l.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: Users/Me/SetMostRecentFilterUsed
        [HttpPut("Me/SetMostRecentFilterUsed")]
        public async Task<IActionResult> SetMostRecentFilterUsed(
            [FromQuery] int? mostRecentLeadFilterId,
            [FromQuery] int? mostRecentQuoteFilterId,
            [FromQuery] int? mostRecentSalesOrderFilterId,
            [FromQuery] int? mostRecentInvoiceFilterId,
            [FromQuery] int? mostRecentPurchaseOrderFilterId,
            [FromQuery] int? mostRecentRmaFilterId,
            [FromQuery] int? mostRecentInventoryItemFilterId,
            [FromQuery] int? mostRecentProductFilterId,
            [FromQuery] int? mostRecentSourceFilterId,
            [FromQuery] int? mostRecentCompanyFilterId,
            [FromQuery] int? mostRecentContactFilterId,
            [FromQuery] int? mostRecentBillFilterId
        ) {
            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == Models.User.GetId(User));
            if (user == null) {
                return BadRequest(new {
                    Error = "Unable to find your user info"
                });
            }
            var validInput = false;
            if (mostRecentLeadFilterId != null) {
                if (mostRecentLeadFilterId == -1)
                    user.MostRecentLeadFilterId = null;
                else
                    user.MostRecentLeadFilterId = mostRecentLeadFilterId;
                validInput = true;
            }
            if (mostRecentQuoteFilterId != null) {
                if (mostRecentQuoteFilterId == -1)
                    user.MostRecentQuoteFilterId = null;
                else
                    user.MostRecentQuoteFilterId = mostRecentQuoteFilterId;
                validInput = true;
            }
            if (mostRecentSalesOrderFilterId != null) {
                if (mostRecentSalesOrderFilterId == -1)
                    user.MostRecentSalesOrderFilterId = null;
                else
                    user.MostRecentSalesOrderFilterId = mostRecentSalesOrderFilterId;
                validInput = true;
            }
            if (mostRecentInvoiceFilterId != null) {
                if (mostRecentInvoiceFilterId == -1)
                    user.MostRecentInvoiceFilterId = null;
                else
                    user.MostRecentInvoiceFilterId = mostRecentInvoiceFilterId;
                validInput = true;
            }
            if (mostRecentPurchaseOrderFilterId != null) {
                if (mostRecentPurchaseOrderFilterId == -1)
                    user.MostRecentPurchaseOrderFilterId = null;
                else
                    user.MostRecentPurchaseOrderFilterId = mostRecentPurchaseOrderFilterId;
                validInput = true;
            }
            if (mostRecentRmaFilterId != null) {
                if (mostRecentRmaFilterId == -1)
                    user.MostRecentRmaFilterId = null;
                else
                    user.MostRecentRmaFilterId = mostRecentRmaFilterId;
                validInput = true;
            }
            if (mostRecentInventoryItemFilterId != null) {
                if (mostRecentInventoryItemFilterId == -1)
                    user.MostRecentInventoryItemFilterId = null;
                else
                    user.MostRecentInventoryItemFilterId = mostRecentInventoryItemFilterId;
                validInput = true;
            }
            if (mostRecentProductFilterId != null) {
                if (mostRecentProductFilterId == -1)
                    user.MostRecentProductFilterId = null;
                else
                    user.MostRecentProductFilterId = mostRecentProductFilterId;
                validInput = true;
            }
            if (mostRecentSourceFilterId != null) {
                if (mostRecentSourceFilterId == -1)
                    user.MostRecentSourceFilterId = null;
                else
                    user.MostRecentSourceFilterId = mostRecentSourceFilterId;
                validInput = true;
            }
            if (mostRecentCompanyFilterId != null) {
                if (mostRecentCompanyFilterId == -1)
                    user.MostRecentCompanyFilterId = null;
                else
                    user.MostRecentCompanyFilterId = mostRecentCompanyFilterId;
                validInput = true;
            }
            if (mostRecentContactFilterId != null) {
                if (mostRecentContactFilterId == -1)
                    user.MostRecentContactFilterId = null;
                else
                    user.MostRecentContactFilterId = mostRecentContactFilterId;
                validInput = true;
            }
            if (mostRecentBillFilterId != null) {
                if (mostRecentBillFilterId == -1)
                    user.MostRecentBillFilterId = null;
                else
                    user.MostRecentBillFilterId = mostRecentBillFilterId;
                validInput = true;
            }
            if (!validInput) {
                return BadRequest(new {
                    Error = "querystring didn't contain any recognized parameters"
                });
            }
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        // GET: Users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            UserControllerHelper helper = new UserControllerHelper();

            var user = await helper.GetUserById(id, _context);

            if (user == null) {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpGet]
        /// <summary>
        /// GET /Users
        /// </summary>
        /// <returns></returns>
        /// [Http]
        public ListResult Index(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            IQueryable<User> query = from user in _context.Users select user;

            query = query
                .Include(item => item.DefaultGidLocationOption)
                .OrderBy(l => l.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        [HttpGet("SalesSelectStubOptions")]
        /// <summary>
        /// GET /Users/SalesSelectStubOptions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserSelectStub> SalesSelectStubOptions([FromQuery] List<string> Roles) {
            return _context.Users
                .Where(item => item.UserGroups.Any(group => group.UserGroup.AzureId == "83e054ae-eee1-4c07-9613-a5b2087134f6"))
                .Select(u => new UserSelectStub {
                    Id = u.Id,
                    Value = u.DisplayName
                }).OrderBy(item => item.Value).ToList();
        }


        [HttpGet("SalesAndSupportSelectStubOptions")]
        /// <summary>
        /// GET /Users/SalesSelectStubOptions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserSelectStub> SalesAndSupportSelectStubOptions([FromQuery] List<string> Roles) {
            return _context.Users
                .Where(item => item.UserGroups.Any(group =>
                    group.UserGroup.AzureId == "83e054ae-eee1-4c07-9613-a5b2087134f6" //sales security group
                    || group.UserGroup.AzureId == "58e7e4f0-8057-40c9-a14f-8b9d02da17d4")) //support security group
                .Select(u => new UserSelectStub {
                    Id = u.Id,
                    Value = u.DisplayName
                }).OrderBy(item => item.Value).ToList();
        }

        [HttpGet("SelectOptions")]
        /// <summary>
        /// GET /Users/SalesSelectStubOptions
        /// </summary>
        /// <returns></returns>
        public IEnumerable<UserSelectStub> SelectOptions([FromQuery] List<string> Roles) {
            return _context.Users
                .Select(u => new UserSelectStub {
                    Id = u.Id,
                    Value = u.DisplayName
                }).ToList();
        }

        [HttpGet("SignedOut")]
        public IActionResult SignedOut() {
            if (User.Identity.IsAuthenticated) {
                // Redirect to home page if the user is authenticated.
                return Redirect("/");
            }

            return View();
        }


        private bool UserExists(int id) {
            return _context.Users.Any(e => e.Id == id);
        }

        // [HttpGet("AccessDenied")]
        // public IActionResult AccessDenied() {
        //     return View();
        // }
    }
}