using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using System.Net;
using SendGrid;
using SendGrid.Helpers.Mail;
using WebApi.Services;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using DinkToPdf;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Rmas")]
    public class RmasController : Controller {
        private readonly AppDBContext _context;
        // private readonly string SendgridApiKey;
        public IHostingEnvironment Environment;
        private IConverter PdfConverter;
        private IConfiguration _configuration;
        //This is a helper service that allows rendering a razor file to raw html.  This is used to generate quotes
        private readonly ViewRender viewRenderer;

        public RmasController(AppDBContext context, IConfiguration config, ViewRender renderer, IHostingEnvironment env, IConverter converter) {
            _context = context;
            // SendgridApiKey = config.GetValue<string>("SENDGRID_API_KEY");
            _configuration = config;
            viewRenderer = renderer;
            Environment = env;
            PdfConverter = converter;
        }


        // GET: Rmas
        [HttpGet]
        public ListResult GetRmas(
            [FromQuery] int? id,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] int? salesPersonId = null,
            [FromQuery] int? rmaStatusOptionId = null,
            [FromQuery] int? rmaReasonOptionId = null,
            [FromQuery] int? rmaActionOptionId = null,
            [FromQuery] int? companyId = null,
            [FromQuery] int? contactId = null,
            [FromQuery] int? productId = null,
            [FromQuery] DateTime? createdAtStartDate = null,
            [FromQuery] DateTime? createdAtEndDate = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from rma in _context.Rmas select rma;
            query = query.Include(item => item.Company);

            // if(salesPersonId != null)
            //     query = query.Where(item => item.SalesPersonId == salesPersonId);
            if (rmaStatusOptionId != null)
                query = query.Where(item => item.RmaStatusOptionId == rmaStatusOptionId);
            if (rmaReasonOptionId != null)
                query = query.Where(item => item.RmaReasonOptionId == rmaReasonOptionId);
            if (rmaActionOptionId != null)
                query = query.Where(item => item.RmaActionOptionId == rmaActionOptionId);
            if (createdAtStartDate != null)
                query = query.Where(l => l.CreatedAt >= createdAtStartDate);
            if (createdAtEndDate != null)
                query = query.Where(l => l.CreatedAt <= createdAtEndDate);

                
            if (companyId != null)
                query = query.Where(l => l.CompanyId == companyId);
            //if (contactId != null)
            //    query = query.Where(l => l.ContactId == contactId);
            if (productId != null)
                query = query.Where(l => l.LineItems.Any(li => li.InventoryItem.ProductId == productId));
                
            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "Company.Name":
                    query = sortAscending ? query.OrderBy(item => item.Company.Name) : query.OrderByDescending(item => item.Company.Name);
                    break;
                case "RmaStatusOptionId":
                    query = sortAscending ? query.OrderBy(item => item.RmaStatusOption.Value) : query.OrderByDescending(item => item.RmaStatusOption.Value);
                    break;
                case "SalesOrderId":
                    query = sortAscending ? query.OrderBy(item => item.SalesOrderId) : query.OrderByDescending(item => item.SalesOrderId);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(searchString)) {
                searchString = searchString.Trim();
                int searchStringNumber;
                if (Int32.TryParse(searchString, out searchStringNumber)) {

                } else {
                    searchStringNumber = 0;
                }
                query = query.Where(item =>
                    item.Id == searchStringNumber ||
                    item.Company.Name.StartsWith(searchString) ||
                    item.LineItems.Any(lLineItem => lLineItem.InventoryItem.Product.PartNumber.StartsWith(searchString) || lLineItem.InventoryItem.SerialNumber.StartsWith(searchString))
                );
            }


            var count = -1;
            if (String.IsNullOrWhiteSpace(searchString))
                count = query.Count();


            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = count
            };
        }

        // GET: Rmas
        [HttpGet("Search")]
        public async Task<IActionResult> SearchRmas(
            [FromQuery] int? query,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var dbQuery = from rma in _context.Rmas select rma;

            dbQuery = dbQuery.Where(rma => rma.Id == query);


            dbQuery = dbQuery
                .OrderByDescending(q => q.CreatedAt);

            var items = await dbQuery.Select(item => new {
                Id = item.Id,
                Name = item.Id
            }).ToListAsync();

            return Ok(items);

            // return new ListResult
            // {
            //     Items = query.Skip(skip).Take(perPage),
            //     Count = query.Count()
            // };
        }

        // GET: Rmas/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRma([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var rma = await _context.Rmas
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .Include(item => item.EventLogEntries)
                    .ThenInclude(item => item.EventLogEntry)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (rma == null) {
                return NotFound();
            }

            return Ok(rma);
        }

        // PUT: Rmas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRma([FromRoute] int id, [FromBody] Rma rma) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != rma.Id) {
                return BadRequest();
            }

            var oldRma = await _context.Rmas.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (oldRma.SalesOrderId != rma.SalesOrderId) {
                return BadRequest(new {
                    Error = "You cannot change the sales order Id once an RMA is created.  Just make a new one."
                });
            }
            _context.Entry(rma).State = EntityState.Modified;


            var updatedEvent = "Updated";
            if (oldRma.RmaStatusOptionId != rma.RmaStatusOptionId)
            {
                var status = await _context.RmaStatusOptions.FirstOrDefaultAsync(item => item.Id == rma.RmaStatusOptionId);
                updatedEvent += " Status - " + (status != null ? status.Value : "None");
            }

            //add event log entry
            _context.RmaEventLogEntries.Add(new RmaEventLogEntry
            {
                Rma = rma,
                EventLogEntry = new EventLogEntry
                {
                    Event = updatedEvent,
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RmaExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // GET: Rmas/5/LineItems
        [HttpGet("{id}/LineItems")]
        public async Task<IActionResult> GetRmaLineItems([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            Rma rma = await _context.Rmas
                .Include(m => m.LineItems)
                    .ThenInclude(m => m.InventoryItem)
                        .ThenInclude(m => m.Product)
                .FirstOrDefaultAsync(l => l.Id == id);

            return Ok(rma.LineItems);
        }

        // POST: Rmas
        [HttpPost]
        public async Task<IActionResult> PostRma([FromBody] Rma rma) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            rma.CreatedAt = DateTime.UtcNow;
            rma.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);

            _context.Rmas.Add(rma);
            //add event log entry
            _context.RmaEventLogEntries.Add(new RmaEventLogEntry {
                Rma = rma,
                EventLogEntry = new EventLogEntry {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRma", new { id = rma.Id }, rma);
        }

        [HttpPost("CancelRma")]
        public async Task<IActionResult> CancelRma([FromBody] SendRmaData sendRmaData)
        {
            return Ok();
        }



            // POST: Rmas
            [HttpPost("SendRma")]
        public async Task<IActionResult> SendRma([FromBody] SendRmaData sendRmaData) {
            var emailParameters = sendRmaData.EmailParameters;
            var rma = sendRmaData.Rma;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null) {
                return BadRequest(errorMessages);
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                _context.RmaEventLogEntries.Add(new RmaEventLogEntry {
                    RmaId = (int)rma.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Sent RMA",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                await _context.SaveChangesAsync();
            } else {
                var body = await response.Body.ReadAsStringAsync();
                Console.WriteLine("Error sending email");
                Console.WriteLine(body);
                return BadRequest(new {
                    Error = "Error sending email. Status code was wrong"
                });
            }

            //Now create inventory items and shipments
            rma = await _context.Rmas
                .Include(item => item.LineItems)
                .Where(item => item.Id == rma.Id)
                .FirstOrDefaultAsync();

            var incomingShipmentInventoryItems = new List<IncomingShipmentInventoryItem> { };
            foreach (RmaLineItem rmaLineItem in rma.LineItems) {
                // for (int i = 0; i < rmaLineItem.Quantity; ++i)
                // {
                // var inventoryItem = new InventoryItem
                // {
                //     CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                //     CreatedAt = DateTime.UtcNow,
                //     ProductId = rmaLineItem.ProductId,
                //     Description = rmaLineItem.Description
                // };
                incomingShipmentInventoryItems.Add(new IncomingShipmentInventoryItem {
                    InventoryItemId = rmaLineItem.InventoryItemId
                });
                // }
            }

            IncomingShipment incomingShipment = new IncomingShipment {
                CreatedAt = DateTime.UtcNow,
                InventoryItems = incomingShipmentInventoryItems
            };

            rma.IncomingShipments = new List<RmaIncomingShipment>{
                new RmaIncomingShipment{
                    IncomingShipment = incomingShipment
                }
            };

            var sentStatus = await _context.RmaStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower() == "Sent");

            if (sentStatus != null)
                rma.RmaStatusOptionId = sentStatus.Id;

            // rma.Sent = true;
            rma.SentAt = DateTime.UtcNow;
            var newEventLogEntry = new RmaEventLogEntry {
                RmaId = (int)rma.Id,
                EventLogEntry = new EventLogEntry {
                    Event = "Sent RMA",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            };
            _context.Add(newEventLogEntry);
            _context.Entry(rma).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        [HttpGet("GenerateCancelRmaPdf/{id}")]
        public async Task GenerateCancelRmaPdf([FromRoute] int id)
        {
            await this.GenerateRmaPdf(id, true);
        }

        [HttpGet("GenerateRmaPdf/{id}")]
        public async Task GenerateRmaPdf([FromRoute] int id, bool cancel = false) {

            var rma = await _context.Rmas
                // .Include(q => q.Contact)
                .Include(q => q.LineItems)
                .Include(item => item.RmaActionOption)
                .Include(item => item.RmaReasonOption)
                .Include(item => item.Company)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.InventoryItem)
                        .ThenInclude(item => item.Product)
                            .ThenInclude(item => item.Manufacturer)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.SalesOrderLineItem)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.BillingAddress)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.ShippingAddress)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.CurrencyOption)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                // .Include(item => item)
                // .ThenInclude(l => l.Product)
                // .Include(q => q.SalesPerson)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (rma == null) {
                await Response.WriteAsync("Rma not found");
                Response.StatusCode = 400;
                return;
            }
            // if 

            rma.PreparedBy = await _context.Users.FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User));
            rma.CurrencyOption = rma.SalesOrder.CurrencyOption;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewBag.Cancelled = cancel;
            var templateUrl = "~/Features/Rma/Views/RmaPdf.cshtml";

            string rmaHtml = viewRenderer.Render(templateUrl, rma, ViewData);
            var doc = new HtmlToPdfDocument() {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = rmaHtml,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        // HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };
            byte[] data = PdfConverter.Convert(doc);
            // Response.Headers.ContentType = "application/pdf";
            Response.ContentType = "application/pdf";
            await Response.Body.WriteAsync(data, 0, data.Length);
            return;
        }


        // DELETE: Rmas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRma([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var rma = await _context.Rmas
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (rma == null) {
                return NotFound();
            }

            foreach (var itemAttachment in rma.Attachments.ToList()) {
                await itemAttachment.Attachment.Delete(_context, _configuration);
            }
            _context.Rmas.Remove(rma);
            await _context.SaveChangesAsync();

            return Ok(rma);
        }

        private bool RmaExists(int id) {
            return _context.Rmas.Any(e => e.Id == id);
        }
    }
}