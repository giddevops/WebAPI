using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using DinkToPdf;
using WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using DinkToPdf.Contracts;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using GidIndustrial.Gideon.WebApi.Libraries;

namespace WebApi.Features.Controllers {
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("Repairs")]
    public class RepairsController : Controller {
        private readonly AppDBContext _context;
        // private readonly string SendgridApiKey;
        public IHostingEnvironment Environment;
        private IConverter PdfConverter;
        private IConfiguration _configuration;
        public string BaseGidQuoteUrl = "https://forms.gidindustrial.com/key/repair/";

        //This is a helper service that allows rendering a razor file to raw html.  This is used to generate repairs
        private readonly ViewRender viewRenderer;

        public RepairsController(AppDBContext context, IConfiguration config, ViewRender renderer, IHostingEnvironment env, IConverter converter) {
            _context = context;
            _configuration = config;
            viewRenderer = renderer;
            Environment = env;
            PdfConverter = converter;
        }

        // GET: Repairs
        [HttpGet]
        public ListResult GetRepairs(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] DateTime? createdAtStartDate = null,
            [FromQuery] DateTime? createdAtEndDate = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true

        ) {
            var query = from repair in _context.Repairs select repair;

            switch (sortBy) {
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }


            if (!String.IsNullOrWhiteSpace(searchString)) {
                // int searchStringNumber;
                // if (Int32.TryParse(searchString, out searchStringNumber)) {

                // } else {
                //     searchStringNumber = 0;
                // }
                // query = query.Where(item =>
                //     item.Id == searchStringNumber ||
                //     EF.Functions.Like(item.Company.Name, searchString + '%') ||
                //     EF.Functions.Like(item.Contact.FirstName, searchString + '%') ||
                //     EF.Functions.Like(item.Contact.LastName, searchString + '%') ||
                //     EF.Functions.Like(item.Phone, searchString + '%') ||
                //     EF.Functions.Like(item.Email, searchString + '%') ||
                //     item.LineItems.Any(lLineItem =>
                //         EF.Functions.Like(lLineItem.ProductName, searchString + '%') ||
                //         EF.Functions.Like(lLineItem.Product.PartNumber, searchString + '%') ||
                //         lLineItem.Description.Contains(searchString)
                //     )
                // );
            }

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: Repairs/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuote([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = from q in _context.Repairs select q;

            var repair = await query
                .SingleOrDefaultAsync(m => m.Id == id);

            if (repair == null) {
                return NotFound();
            }

            return Ok(repair);
        }

        /// <summary>
        /// This method generates a pdf of the repair
        /// </summary>
        /// <param name="id">The id of the repair</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateRepairAuthorizationPdf")]
        public async Task GenerateRepairAuthorizationPdf([FromRoute] int id) {

            var repair = await _context.Repairs
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.LineItems)
                        .ThenInclude(item => item.Product)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.Contact)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.GidLocationOption)
                        .ThenInclude(item => item.MainAddress)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.SalesPerson)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.CurrencyOption)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (repair == null) {
                Response.StatusCode = 400;
                return;
            }

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await repair.SalesOrder.CheckIfIsGidEurope(_context);

            string quoteHtml = viewRenderer.Render("~/Features/Repair/Views/RepairAuthorizationPdf.cshtml", repair, ViewData);
            var doc = new HtmlToPdfDocument() {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = quoteHtml,
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

        // PUT: Repairs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRepair([FromRoute] int id, [FromBody] Repair repair) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != repair.Id) {
                return BadRequest();
            }

            _context.Entry(repair).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!RepairExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Repairs
        [HttpPost]
        public async Task<IActionResult> PostRepair([FromBody] Repair repair) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (repair.SalesOrderId == null) {
                return BadRequest("SalesOrderId must not be null");
            }

            repair.CreatedAt = DateTime.UtcNow;
            repair.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            _context.Repairs.Add(repair);

            //add to repair event log entry the fact that it was created and who created it
            _context.SalesOrderEventLogEntries.Add(new SalesOrderEventLogEntry {
                SalesOrderId = repair.SalesOrderId,
                EventLogEntry = new EventLogEntry {
                    Event = "Created Repair",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });


            using (var transaction = _context.Database.BeginTransaction()) {
                _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Repair ON");
                _context.SaveChanges();
                _context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT dbo.Repair OFF");
                transaction.Commit();
            }


            return CreatedAtAction("GetRepair", new { id = repair.Id }, repair);
        }

        // POST: Repairs/SendRepairAuthorization
        [HttpPost("SendRepairAuthorization")]
        public async Task<IActionResult> SendRepairAuthorization([FromBody] SendRepairAuthorizationData sendRepairAuthorizationData) {
            var emailParameters = sendRepairAuthorizationData.EmailParameters;
            var repair = await _context.Repairs
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.LineItems)
                .FirstOrDefaultAsync(item => item.Id == sendRepairAuthorizationData.Repair.Id);

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null) {
                return BadRequest(errorMessages);
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                _context.SalesOrderEventLogEntries.Add(new SalesOrderEventLogEntry {
                    SalesOrderId = repair.SalesOrder.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Sent Repair Authorization",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                repair.RepairAuthorizationAttachmentId = sendRepairAuthorizationData.EmailParameters.Attachments.First().Id;
                repair.DateIssued = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            } else {
                return BadRequest(new {
                    Error = "Error sending email. Status code was wrong"
                });
            }

            //Now create inventory items and shipments
            var incomingShipmentInventoryItems = new List<IncomingShipmentInventoryItem> { };
            foreach (var lineItem in repair.SalesOrder.LineItems) {
                if (lineItem.LineItemServiceTypeId == LineItemServiceType.Repair) {
                    for (var i = 0; i < lineItem.Quantity; ++i) {
                        var inventoryItem = new InventoryItem {
                            ProductId = lineItem.ProductId,
                            CreatedAt = DateTime.UtcNow,
                            UpdatedAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                            InventoryItemStatusOptionId = InventoryItemStatusOption.Inbound,
                            SalesOrderLineItems = new List<SalesOrderLineItemInventoryItem>{
                                new SalesOrderLineItemInventoryItem{
                                    SalesOrderLineItemId = lineItem.Id
                                }
                            },
                        };
                        incomingShipmentInventoryItems.Add(new IncomingShipmentInventoryItem {
                            InventoryItem = inventoryItem
                        });
                    }
                }
            }
            IncomingShipment incomingShipment = new IncomingShipment {
                CreatedAt = DateTime.UtcNow,
                InventoryItems = incomingShipmentInventoryItems
            };
            repair.IncomingShipments = new List<RepairIncomingShipment>{
                new RepairIncomingShipment{
                    IncomingShipment = incomingShipment
                }
            };
            await _context.SaveChangesAsync();

            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        [HttpGet("{id}/CancelRepairAuthorization")]
        public async Task<IActionResult> CancelRepairAuthorization([FromRoute] int id) {
            var repair = await _context.Repairs
                .Include(item => item.IncomingShipments)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.LineItems)
                        .ThenInclude(item => item.InventoryItems)
                            .ThenInclude(item => item.InventoryItem)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (repair == null) {
                return NotFound("A repair with that Id doesn't exist");
            }
            //now need to add inventory items
            foreach (var lineItem in repair.SalesOrder.LineItems) {
                if (lineItem.LineItemServiceTypeId == LineItemServiceType.Repair) {
                    var inventoryItemsToRemove = new List<InventoryItem> { };
                    //The incoming shipment inventory item will be deleted through ON DELETE CASCADE
                    foreach (var salesOrderLineItemInventoryItem in lineItem.InventoryItems) {
                        inventoryItemsToRemove.Add(salesOrderLineItemInventoryItem.InventoryItem);
                    }
                    _context.RemoveRange(inventoryItemsToRemove);
                    _context.RemoveRange(lineItem.InventoryItems);
                }
            }
            _context.RemoveRange(repair.IncomingShipments);
            repair.RepairAuthorizationAttachmentId = null;
            repair.DateIssued = null;

            await _context.SaveChangesAsync();

            return Ok();
        }


        // DELETE: Repairs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuote([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var repair = await _context.Repairs
                .SingleOrDefaultAsync(item => item.Id == id);
            if (repair == null) {
                return NotFound();
            }

            // foreach (var quoteAttachment in repair.Attachments.ToList()) {
            //     await quoteAttachment.Attachment.Delete(_context, _configuration);
            // }

            _context.Repairs.Remove(repair);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(repair);
        }

        private bool RepairExists(int id) {
            return _context.Repairs.Any(e => e.Id == id);
        }
    }
}