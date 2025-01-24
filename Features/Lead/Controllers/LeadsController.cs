 using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using GidIndustrial.Gideon.WebApi.Libraries;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights;
using Dapper;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Leads")]
    public class LeadsController : Controller {
        private readonly AppDBContext _context;
        private readonly IConfiguration _configuration;

        public LeadsController(AppDBContext context, IConfiguration config) {
            _context = context;
            _configuration = config;
        }

        // GET: Leads
        [HttpGet]
        public async Task<ListResult> GetLeads(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] int? salesPersonId = null,
            [FromQuery] int? leadOriginOptionId = null,
            [FromQuery] int? leadStatusOptionId = null,
            [FromQuery] DateTime? createdAtStartDate = null,
            [FromQuery] DateTime? createdAtEndDate = null,
            // [FromQuery] int utcOffset = 0,
            [FromQuery] bool? todayOnly = null,
            [FromQuery] int? productTypeId = null,
            [FromQuery] int? contactId = null,
            [FromQuery] int? companyId = null,
            [FromQuery] int? productId = null,
            [FromQuery] int? id = null,
            [FromQuery] Boolean fetchRelated = false,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true,
            [FromQuery] string companyName = null
        ) {
            IQueryable<Lead> query = from lead in _context.Leads select lead;

            //if it's just a regular listing of leads
            if (String.IsNullOrWhiteSpace(searchString)) {
                if (salesPersonId != null && salesPersonId != 0)
                    query = query.Where(l => l.SalesPersonId == salesPersonId);
                if (leadOriginOptionId != null && leadOriginOptionId != 0)
                    query = query.Where(l => l.LeadOriginOptionId == leadOriginOptionId);
                if (leadStatusOptionId != null && leadStatusOptionId != 0)
                    query = query.Where(l => l.LeadStatusOptionId == leadStatusOptionId);
                if (createdAtStartDate != null)
                    query = query.Where(l => l.CreatedAt >= createdAtStartDate);
                if (createdAtEndDate != null)
                    query = query.Where(l => l.CreatedAt <= createdAtEndDate);
                if (companyId != null)
                    query = query.Where(l => l.CompanyId == companyId);
                if (contactId != null)
                    query = query.Where(l => l.ContactId == contactId);
                if (productId != null)
                    query = query.Where(l => l.LineItems.Any(li => li.ProductId == productId));
                if (!String.IsNullOrWhiteSpace(companyName))
                    query = query.Where(l => EF.Functions.Like(l.CompanyName, companyName + "%"));
                if (id != null)
                    query = query.Where(item => item.Id == id);

                query = query
                    .Include(item => item.Company)
                    .Include(l => l.SalesPerson)
                    .Include(l => l.Contact)
                    .Include(l => l.LineItems)
                        .ThenInclude(item => item.Product);

                switch (sortBy) {
                    case "Id":
                        query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                        break;
                    case "CompanyName":
                        query = sortAscending ? query.OrderBy(item => item.CompanyName) : query.OrderByDescending(item => item.CompanyName);
                        break;
                    case "SalesPerson.Name":
                        query = sortAscending ? query.OrderBy(item => item.SalesPerson.DisplayName) : query.OrderByDescending(item => item.SalesPerson.DisplayName);
                        break;
                    case "Date":
                        query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                        break;
                    case "Origin":
                        query = sortAscending ? query.OrderBy(item => item.OriginText) : query.OrderByDescending(item => item.OriginText);
                        break;
                    case "FullName":
                        query = sortAscending ? query.OrderBy(item => item.FullName) : query.OrderByDescending(item => item.FullName);
                        break;
                    case "Status":
                        query = sortAscending ? query.OrderBy(item => item.LeadStatusOptionId) : query.OrderByDescending(item => item.LeadStatusOptionId);
                        break;
                    case "PartNumber":
                        query = sortAscending ? query.OrderBy(item => item.PartNumber) : query.OrderByDescending(item => item.PartNumber);
                        break;
                    case "Emoji":
                        query = sortAscending ? query.OrderBy(item => item.Emoji) : query.OrderByDescending(item => item.Emoji);
                        break;
                    default:
                        query = query.OrderByDescending(item => item.CreatedAt);
                        break;
                }


                var count = -1;
                if (String.IsNullOrWhiteSpace(searchString))
                    count = query.Count();

                return new ListResult {
                    Items = query.Skip(skip).Take(perPage),
                    Count = count
                };
            } else { // if this is the big search page
                int searchStringNumber;
                searchString = searchString.Trim();
                if (Int32.TryParse(searchString, out searchStringNumber) == false) {
                    searchStringNumber = 0;
                }
                var searchStringPercent = searchString + "%";

                //                 var queryBaseText = @"
                // LEFT JOIN Contact ON Lead.ContactId = Contact.Id
                // LEFT JOIN Company ON Lead.CompanyId = Company.Id
                // LEFT JOIN LeadLineItem ON Lead.Id = LeadLineItem.LeadId
                // LEFT JOIN Product ON LeadLineItem.ProductId = Product.Id
                // WHERE
                // Lead.Id = {0}
                // OR Lead.CompanyName LIKE {1}
                // OR Company.Name LIKE {1}
                // OR Contact.FirstName LIKE {1}
                // OR Contact.LastName LIKE {1}
                // OR Phone LIKE {1}
                // OR Email LIKE {1}
                // OR LeadLineItem.ProductName LIKE {1}
                // OR Product.PartNumber LIKE {1}";

                //                 var queryText = "SELECT Lead.* FROM Lead " + queryBaseText + " ORDER BY Lead.CreatedAt DESC";
                //                 queryText += " OFFSET {2} ROWS FETCH NEXT {3} ROWS ONLY";
                //                 query = query.FromSql(queryText, searchStringNumber, searchStringPercent, skip, perPage);
                //                 var matchingLeads = await query.ToListAsync();
                //                 var Ids = matchingLeads.Select(item => item.Id);
                //                 var newQuery = _context.Leads
                //                     .Include(item => item.LineItems)
                //                     .Where(item => Ids.Contains(item.Id));


                //                 var countText = "SELECT Count(*) as Id FROM Lead " + queryBaseText;
                //                 var countQuery = _context.Leads.FromSql(countText, searchStringNumber, searchStringPercent).Select(item => item.Id);
                //                 var count = await countQuery.FirstAsync();

                //                 return new ListResult {
                //                     Items = await newQuery.ToListAsync(),
                //                     Count = count
                //                 };


                var querySting = @"SELECT DISTINCT
Lead.Id, Lead.CompanyName, Lead.CompanyId, Lead.SalesPersonId, Lead.CreatedAt, Lead.OriginText, Lead.LeadOriginOptionId, Lead.FullName, Lead.ContactId, Lead.LeadStatusOptionId, Lead.Emoji, Lead.LALeadNumber,
[User].DisplayName as SalesPersonName
FROM Lead
                LEFT JOIN Company ON Lead.CompanyId = Company.Id AND Company.Name LIKE @searchStringLike
                LEFT JOIN Contact ON Lead.ContactId = Contact.Id AND (Contact.FirstName LIKE @searchStringLike OR Contact.LastName LIKE @searchStringLike)
                LEFT JOIN LeadLineItem ON Lead.Id = LeadLineItem.LeadId
                LEFT JOIN Product on LeadLineItem.ProductId = Product.Id and Product.PartNumber LIKE @searchStringLike
                LEFT JOIN [User] on [User].Id = Lead.SalesPersonId
                WHERE Lead.CompanyName LIKE @searchStringLike
                OR Company.Name LIKE @searchStringLike
                OR Contact.FirstName LIKE @searchStringLike
                OR Contact.LastName LIKE @searchStringLike
                OR Lead.FullName LIKE @searchStringLike
                OR Lead.Phone LIKE @searchStringLike
                OR Lead.Email LIKE @searchStringLike
                OR Product.PartNumber LIKE @searchStringLike
                OR Lead.Id = @searchStringNumber
                ORDER BY Lead.Id DESC
                OFFSET @skip ROWS FETCH NEXT @perPage ROWS ONLY;
                ";
                var connection = _context.Database.GetDbConnection();
                var result = connection.Query<dynamic>(querySting, new
                {
                    @searchStringLike = searchStringPercent,
                    searchStringNumber = searchStringNumber,
                    skip = skip,
                    perPage = perPage
                });
                return new ListResult
                {
                    Items = result,
                    Count = -1
                };



                //query = query.Where(item =>
                //    item.Id == searchStringNumber ||
                //    item.LALeadNumber == searchStringNumber ||
                //    EF.Functions.Like(item.CompanyName, searchString + '%') ||
                //    EF.Functions.Like(item.Company.Name, searchString + '%') ||
                //    EF.Functions.Like(item.Contact.FirstName, searchString + '%') ||
                //    EF.Functions.Like(item.Contact.LastName, searchString + '%') ||
                //    EF.Functions.Like(item.Phone, searchString + '%') ||
                //    EF.Functions.Like(item.Email, searchString + '%') ||
                //    item.LineItems.Any(lLineItem => lLineItem.ProductName.StartsWith(searchString)
                //        || EF.Functions.Like(lLineItem.Product.PartNumber, searchString + '%')
                //      || lLineItem.Description.Contains(searchString)
                //    )
                //);

                //return new ListResult {
                //    Items = result,
                //    Count = -1
                //};
            }
        }

        // GET: Leads/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<dynamic> Search([FromQuery] string query) {
            return _context.Leads
                .Where(lead => lead.Id == int.Parse(query))
                .Select(lead => new {
                    Id = lead.Id,
                    Name = lead.Id
                });
        }

        //// GET: Leads/GetName/{id}=...
        //[HttpGet("GetLeadLANumber/{id}")]
        //public string GetName([FromRoute] int id)
        //{
        //    return _context.Leads
        //        .Where(c => c.Id == id)
        //        .Select(c => new
        //        {
        //            Id = c.Id,
        //            Name = c.LALeadNumber
        //            // Name = c.LALeadNumber
        //        }).FirstOrDefault().Name;
        //}

        // GET: Leads/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLead([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var lead = await _context.Leads
                .Include(m => m.Notes)
                    .ThenInclude(l => l.Note)
                .Include(m => m.Attachments)
                    .ThenInclude(l => l.Attachment)
                .Include(m => m.LineItems)
                    .ThenInclude(l => l.Product)
                        .ThenInclude(k => k.Manufacturer)
                .Include(m => m.LineItems)
                    .ThenInclude(l => l.Product)
                        .ThenInclude(k => k.ProductType)
                .Include(m => m.LineItems)
                    .ThenInclude(l => l.Sources)
                        .ThenInclude(k => k.Source)
                            .ThenInclude(j => j.Supplier)
                .Include(m => m.Quotes)
                .Include(m => m.EventLogEntries)
                    .ThenInclude(m => m.EventLogEntry)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (lead == null) {
                return NotFound();
            }

            // Fix lead product names
            foreach (LeadLineItem li in lead.LineItems)
            {
                if (li.ProductName == null && (li.Product != null && li.Product.PartNumber != null))
                    li.ProductName = li.Product.PartNumber;

                if (li.Product == null)
                    li.Product = new Product();

                if(li.Product.ProductType == null)
                    li.Product.ProductType = new ProductType();
            }

            // Sort line items
            lead.SortLineItems();

            return Ok(lead);
        }

        // GET: Leads/5/LineItems
        [HttpGet("{id}/LineItems")]
        public async Task<IActionResult> GetLeadLineItems([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            Lead lead = await _context.Leads
                .Include(m => m.LineItems)
                    .ThenInclude(li => li.Sources)
                        .ThenInclude(item => item.Source)
                 .Include(item => item.LineItems)
                     .ThenInclude(item => item.Product)
                        .ThenInclude(k => k.Manufacturer)
                .Include(m => m.Company)
                .FirstOrDefaultAsync(l => l.Id == id);

            // Fix lead product names
            foreach (LeadLineItem li in lead.LineItems)
            {
                if (li.ProductName == null && (li.Product != null && li.Product.PartNumber != null))
                    li.ProductName = li.Product.PartNumber;
            }

            // Sort line items
            lead.SortLineItems();

            return Ok(lead.LineItems);
        }

        // PUT: Leads/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLead([FromRoute] int id, [FromBody] Lead lead) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != lead.Id) {
                return BadRequest();
            }

            var previousLead = await _context.Leads.AsNoTracking().Include(item => item.SalesPerson).FirstOrDefaultAsync(item => item.Id == id);

            await lead.ComputeStatus(_context);
            _context.Entry(lead).State = EntityState.Modified;

            var updatedEvent = "Updated";
            if(previousLead.LeadStatusOptionId != lead.LeadStatusOptionId)
            {
                var status = await _context.LeadStatusOptions.FirstOrDefaultAsync(item => item.Id == lead.LeadStatusOptionId);
                updatedEvent += " Status - " + (status != null ? status.Value : "None");
            }

            //add event log entry
            _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                Lead = lead,
                EventLogEntry = new EventLogEntry {
                    Event = updatedEvent,
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            if (previousLead.SalesPersonId != lead.SalesPersonId) {
                var previousSalesPersonName = previousLead.SalesPerson != null ? previousLead.SalesPerson.DisplayName : "";
                var currentSalesPersonName = "";
                if (lead.SalesPersonId == null) {
                    currentSalesPersonName = "Nobody";
                } else {
                    var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == lead.SalesPersonId);
                    if (user == null) {
                        currentSalesPersonName = "Nobody";
                    } else {
                        currentSalesPersonName = user.DisplayName;
                    }
                }
                var newEventLogEntry = new EventLogEntry {
                    Event = $"Changed Sales Person from {previousSalesPersonName} to {currentSalesPersonName}",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                };
                _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                    Lead = lead,
                    EventLogEntry = newEventLogEntry
                });
            }
            //If the status has been changed to asian sourcing, send emma an email
            var asianSourcingStatus = await _context.LeadStatusOptions.FirstOrDefaultAsync(item => item.Value == "Asian Sourcing");
            if (asianSourcingStatus != null) {
                if (lead.LeadStatusOptionId == asianSourcingStatus.Id && previousLead.LeadStatusOptionId != asianSourcingStatus.Id) {
                    var userId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
                    if (userId != 36) { // don't send if she assigned to herself
                        var emma = await _context.Users.FirstOrDefaultAsync(item => item.Id == 36);

                        var client = EmailGenerator.GetNewSendGridClient();
                        var msg = await EmailGenerator.GenerateEmail(_context, new EmailGeneratorParameters {
                            To = emma.Email,
                            From = "gideonalerts@gidindustrial.com",
                            Subject = $"Lead #{lead.Id} Changed to Asian Sourcing Status",
                            HtmlContent = $@"
<p>Lead #{lead.Id} was changed to status 'Asian Sourcing'
<p>Click on the following link to see it</p>
<p><a href='https://gideon.gidindustrial.com/leads/{lead.Id}'>Lead</a></p>"
                        });
                        var response = await client.SendEmailAsync(msg);
                        int responseStatusCodeNumber = (int)response.StatusCode;
                        if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                            _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                                LeadId = lead.Id,
                                EventLogEntry = new EventLogEntry {
                                    Event = "Sent Emma a message about asian sourcing",
                                    CreatedAt = DateTime.UtcNow,
                                    OccurredAt = DateTime.UtcNow,
                                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                                }
                            });
                            await _context.SaveChangesAsync();
                        } else {
                            return BadRequest(new {
                                Error = "Error sending email. Status code was wrong"
                            });
                        }
                    }
                }
            }

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!LeadExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return Ok(lead);
        }

        /// <summary>
        /// POST: Leads - create a lead
        /// </summary>
        /// <param name="lead"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostLead([FromBody] Lead lead) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            lead.CreatedAt = DateTime.UtcNow;
            lead.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            await lead.ComputeStatus(_context);

            _context.Leads.Add(lead);
            //add event log entry
            _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                Lead = lead,
                EventLogEntry = new EventLogEntry {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLead", new { id = lead.Id }, lead);
        }

        [HttpPost("FromWebsite")]
        public async Task<IActionResult> PostLeadFromWebsite([FromBody] LeadAutomation.Pigeon.Exchange.Entities.LALead websiteLead) {

            
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var lead = await Lead.ParseWebsiteLead(websiteLead, _context);
            
            lead.LeadOriginOptionId = (await _context.LeadOriginOptions.FirstOrDefaultAsync(loo => loo.Value.ToLower() == "web")).Id;
            lead.LeadStatusOptionId = (await _context.LeadStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower() == "open")).Id;
            //now need to relate lead line items
            await lead.CreateLineItemsFromLALead(_context, websiteLead);
            await lead.ComputeStatus(_context);

            
            lead.CreatedById = 0;
            _context.Leads.Add(lead);
            //add event log entry
            _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                Lead = lead,
                EventLogEntry = new EventLogEntry {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = 0,
                    UserId = 0
                }
            });
            
            await _context.SaveChangesAsync();

            
            await lead.CalculateLeadRouting(_context);
            await _context.SaveChangesAsync();
            
            await lead.SendAutoResponse(_context);

            return CreatedAtAction("GetLead", new { id = lead.Id }, lead);
            

            //return CreatedAtAction("GetLead", new { id = 0 }, lead);
        }


        // POST: Leads/SendLeadNoBid
        [HttpPost("SendLeadNoBid")]
        public async Task<IActionResult> SendLead([FromBody] SendLeadNoBidData sendLeadData) {
            var emailParameters = sendLeadData.EmailParameters;
            var quote = sendLeadData.Lead;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null) {
                return BadRequest(errorMessages);
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);


            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                    LeadId = sendLeadData.Lead.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Sent Lead",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                await _context.SaveChangesAsync();
            } else {
                return BadRequest(new {
                    Error = "Error sending email. Status code was wrong"
                });
            }
            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        // POST: Leads/SendLeadRepair
        [HttpPost("SendLeadRepair")]
        public async Task<IActionResult> SendLead([FromBody] SendLeadRepairData sendLeadData)
        {
            var emailParameters = sendLeadData.EmailParameters;
            var quote = sendLeadData.Lead;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null)
            {
                return BadRequest(errorMessages);
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);


            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300)
            {
                _context.LeadEventLogEntries.Add(new LeadEventLogEntry
                {
                    LeadId = sendLeadData.Lead.Id,
                    EventLogEntry = new EventLogEntry
                    {
                        Event = "Sent Repair Lead",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                await _context.SaveChangesAsync();
            }
            else
            {
                return BadRequest(new
                {
                    Error = "Error sending email. Status code was wrong"
                });
            }
            return Ok(new
            {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }


        // POST: Leads/SendLeadProductUnavailable
        [HttpPost("SendLeadProductUnavailable")]
        public async Task<IActionResult> SendLead([FromBody] SendLeadProductUnavailableData sendLeadData) {
            var emailParameters = sendLeadData.EmailParameters;
            var quote = sendLeadData.Lead;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null) {
                return BadRequest(errorMessages);
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                    LeadId = sendLeadData.Lead.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Sent Lead",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                await _context.SaveChangesAsync();
            } else {
                return BadRequest(new {
                    Error = "Error sending email. Status code was wrong"
                });
            }
            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        // DELETE: Leads/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLead([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var lead = await _context.Leads
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .Include(item => item.ContactLogItems)
                    .ThenInclude(item => item.ContactLogItem)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (lead == null) {
                return NotFound();
            }

            foreach (var itemAttachment in lead.Attachments.ToList()) {
                await itemAttachment.Attachment.Delete(_context, _configuration);
            }
            _context.ContactLogItems.RemoveRange(lead.ContactLogItems.Select(item => item.ContactLogItem));

            _context.Leads.Remove(lead);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(lead);
        }

        private bool LeadExists(int id) {
            return _context.Leads.Any(e => e.Id == id);
        }
    }
}