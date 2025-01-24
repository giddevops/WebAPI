using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using GidIndustrial.Gideon.WebApi;
using DinkToPdf;
using Microsoft.AspNetCore.Hosting;
using DinkToPdf.Contracts;
using Microsoft.Extensions.Configuration;
using WebApi.Services;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using GidIndustrial.Gideon.WebApi.Libraries;
using QuickBooks.Models;
using GidIndustrial.Gideon.WebApi.Controllers;
using SelectPdf;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("PurchaseOrders")]
    public class PurchaseOrdersController : Controller {
        private readonly AppDBContext _context;
        public IHostingEnvironment Environment;
        private IConverter PdfConverter;
        private IConfiguration _configuration;
        //This is a helper service that allows rendering a razor file to raw html.  This is used to generate quotes
        private readonly ViewRender viewRenderer;

        public PurchaseOrdersController(AppDBContext context, IConfiguration config, ViewRender renderer, IHostingEnvironment env, IConverter converter) {
            _context = context;
            _configuration = config;
            viewRenderer = renderer;
            Environment = env;
            PdfConverter = converter;
        }

        // GET: PurchaseOrders
        [HttpGet]
        public ListResult GetPurchaseOrders(
            [FromQuery] int? productId,
            [FromQuery] int? companyId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] int? buyerId = null,
            [FromQuery] int? purchaseOrderStatusOptionId = null,
            [FromQuery] int? productTypeId = null,
            [FromQuery] int? contactId = null,
            [FromQuery] string trackingNumber = null,
            [FromQuery] DateTime? createdAtStartDate = null,
            [FromQuery] DateTime? createdAtEndDate = null,
            [FromQuery] string searchString = null,
            [FromQuery] bool noTracking = false,
            [FromQuery] string supplierName = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from purchaseOrder in _context.PurchaseOrders select purchaseOrder;

            if (companyId != null) {
                query = query.Where(item => item.SupplierId == companyId);
            }
            if (productId != null) {
                query = query.Where(item => item.LineItems.Any(lineItem => lineItem.ProductId == productId));
            }

            if (buyerId != null)
                query = query.Where(item => item.BuyerId == buyerId);
            if (productTypeId != null)
                query = query.Where(item => item.LineItems.Any(pLineItem => pLineItem.Product.ProductTypeId == productTypeId));
            if (purchaseOrderStatusOptionId != null)
                query = query.Where(item => item.PurchaseOrderStatusOptionId == purchaseOrderStatusOptionId);
            if (createdAtStartDate != null)
                query = query.Where(l => l.CreatedAt >= createdAtStartDate);
            if (createdAtEndDate != null)
                query = query.Where(l => l.CreatedAt <= createdAtEndDate);
            if (contactId != null)
                query = query.Where(item => item.ContactId == contactId);


            if (!String.IsNullOrWhiteSpace(trackingNumber))
            {
                query = query.Where(item => item.IncomingShipments.Any(item2 => item2.IncomingShipment.TrackingNumber.StartsWith(trackingNumber)));
            }

            if (!String.IsNullOrWhiteSpace(supplierName))
            { 
               query = query.Where(item => item.Supplier.Name.StartsWith(supplierName) || 
                item.Supplier.Portals.Any(item2 => item2.Username.StartsWith(supplierName)));
            }

            if(noTracking)
                query = query.Where(i => i.IncomingShipments == null || i.IncomingShipments.Any(item2 => 
                    (item2.IncomingShipment.TrackingNumber == null || item2.IncomingShipment.TrackingNumber == String.Empty)));


            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "Supplier.Name":
                    query = sortAscending ? query.OrderBy(item => item.Supplier.Name) : query.OrderByDescending(item => item.Supplier.Name);
                    break;
                case "PurchaseOrderStatusOptionId":
                    query = sortAscending ? query.OrderBy(item => item.PurchaseOrderStatusOptionId) : query.OrderByDescending(item => item.PurchaseOrderStatusOptionId);
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
                    item.Supplier.Name.StartsWith(searchString) ||
                    item.Contact.FirstName.StartsWith(searchString) ||
                    item.Contact.LastName.StartsWith(searchString) ||
                    item.Phone.StartsWith(searchString) ||
                    item.Email.StartsWith(searchString) ||
                    item.LineItems.Any(lLineItem => lLineItem.ProductName.StartsWith(searchString) || lLineItem.Product.PartNumber.StartsWith(searchString) || lLineItem.Description.Contains(searchString))
                );
            }

            query = query
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.Supplier);


            var count = -1;
            if (String.IsNullOrWhiteSpace(searchString))
                count = query.Count();

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = count
            };
        }

        // GET: PurchaseOrders/Search
        [HttpGet("Search")]
        public async Task<IActionResult> SearchPurchaseOrders(
            [FromQuery] int? query,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var dbQuery = from purchaseOrder in _context.PurchaseOrders select purchaseOrder;

            dbQuery = dbQuery.Where(purchaseOrder => purchaseOrder.Id == query);
            dbQuery = dbQuery
                .OrderByDescending(q => q.CreatedAt);

            var items = await dbQuery.Select(item => new {
                Id = item.Id,
                Name = item.Id
            }).ToListAsync();

            return Ok(items);
        }


        // GET: PurchaseOrders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrder([FromRoute] int? id, [FromQuery] bool? includeSalesOrderLineItemSources) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = from po in _context.PurchaseOrders select po;

            query = query
                .Include(po => po.LineItems)
                .Include(item => item.EventLogEntries)
                    .ThenInclude(item => item.EventLogEntry)
                .Include(item => item.Notes)
                    .ThenInclude(item => item.Note)
                .Include(item => item.ShippingAddress)
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment);

            if (includeSalesOrderLineItemSources == true) {
                query = query.Include(item => item.SalesOrderLineItemSources);
            }

            var purchaseOrder = await query.SingleOrDefaultAsync(m => m.Id == id);

            if (purchaseOrder == null) {
                return NotFound();
            }

            return Ok(purchaseOrder);
        }

        // GET: PurchaseOrders/5/LineItems
        [HttpGet("{id}/LineItems")]
        public async Task<IActionResult> GetPurchaseOrderLineItems([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            PurchaseOrder purchaseOrder = await _context.PurchaseOrders
                .Include(m => m.LineItems)
                    .ThenInclude(m => m.Product)
                .Include(m => m.IncomingShipments)
                    .ThenInclude(m => m.IncomingShipment)
                        .ThenInclude(m => m.InventoryItems)
                            .ThenInclude(m => m.InventoryItem)
                .FirstOrDefaultAsync(l => l.Id == id);

            return Ok(purchaseOrder);
        }

        [HttpGet("{id}/RelatedSalesOrders")]
        public async Task<IActionResult> GetRelatedSalesOrders(
            [FromRoute] int? id,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from salesOrder in _context.SalesOrders select salesOrder;

            query = query.Where(item =>
                item.LineItems.Any(soLineItem => soLineItem.Sources.Any(soliSource => soliSource.PurchaseOrderId == id)) ||
                item.PurchaseOrders.Any(soPurchaseOrders => soPurchaseOrders.PurchaseOrderId == id)
            );

            return Ok(new ListResult {
                Items = await query.Skip(skip).Take(perPage).ToListAsync(),
                Count = query.Count()
            });
        }

        /// <summary>
        /// This method generates a pdf of the purchaseOrders
        /// </summary>
        /// <param name="id">The id of the purchaseOrder</param>
        /// <returns></returns>
        [HttpGet("{id}/GeneratePurchaseOrderPdf")]
        public async Task GeneratePurchaseOrderPdf([FromRoute] int id) {

            var purchaseOrder = await _context.PurchaseOrders
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Product)
                        .ThenInclude(j => j.Manufacturer)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.LineItemConditionType)
                .Include(c => c.Contact)
                .Include(c => c.Supplier)
                    .ThenInclude(item => item.Addresses)
                        .ThenInclude(item => item.Address)
                .Include(item => item.ShippingAddress)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.BillingAddress)
                .Include(item => item.ShippingMethod)
                .Include(item => item.PaymentMethod)
                .Include(item => item.CurrencyOption)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (purchaseOrder == null) {
                Response.StatusCode = 400;
                return;
            }
            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            purchaseOrder.Preparer = currentUser;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await purchaseOrder.CheckIfIsGidEurope(_context);
            ViewData["LogoUrlNew"] = Environment.ContentRootPath + @"\Features\Common\Views\logo-footer.png";
            ViewData["LogoUrlNew2"] = Environment.ContentRootPath + @"\Features\Common\Views\logo-footer2.png";
            ViewData["Font1"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Regular.ttf";
            ViewData["Font2"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Medium.ttf";
            ViewData["Font3"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-SemiBold.ttf";
            ViewData["Font4"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Bold.ttf";

            // Render html
            string html = viewRenderer.Render("~/Features/PurchaseOrder/Views/PurchaseOrderPdfNew.cshtml", purchaseOrder, ViewData);

            // Create initial pdf
            HtmlToPdf converter = new HtmlToPdf();
            SelectPdf.PdfDocument pdfDocument = converter.ConvertHtmlString(html);

            // Remove last blank page
            if (pdfDocument.Pages.Count > 1 && pdfDocument.Pages[pdfDocument.Pages.Count - 1].ClientRectangle.Location.IsEmpty)
            {
                // pdfDocument.RemovePageAt(pdfDocument.Pages.Count - 1);
            }

            // Render t&c html
            string tcHtml = viewRenderer.Render("~/Features/Common/Views/TermsAndConditionsPdf.cshtml", purchaseOrder.GidLocationOption, ViewData);

            // Create initial pdf
            SelectPdf.PdfDocument pdfTCDocument = converter.ConvertHtmlString(tcHtml);

            // Create original appeneded doc
            SelectPdf.PdfDocument pdfDocumentAppended = new SelectPdf.PdfDocument();

            // Remove blank page if needed

            pdfDocumentAppended.Append(pdfDocument);
            pdfDocumentAppended.Append(pdfTCDocument);

            byte[] dataReturn = pdfDocumentAppended.Save();

            Response.ContentType = "application/pdf";
            await Response.Body.WriteAsync(dataReturn, 0, dataReturn.Length);

            //ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            //ViewData["IsGidEurope"] = await purchaseOrder.CheckIfIsGidEurope(_context);
            //string purchaseOrderHtml = viewRenderer.Render("~/Features/PurchaseOrder/Views/PurchaseOrderPdf.cshtml", purchaseOrder, ViewData);
            //var doc = new HtmlToPdfDocument() {
            //    GlobalSettings = {
            //        ColorMode = ColorMode.Color,
            //        Orientation = Orientation.Portrait,
            //        PaperSize = PaperKind.A4,
            //    },
            //    Objects = {
            //        new ObjectSettings() {
            //            PagesCount = true,
            //            HtmlContent = purchaseOrderHtml,
            //            WebSettings = { DefaultEncoding = "utf-8" },
            //            // HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
            //        }
            //    }
            //};
            //byte[] data = PdfConverter.Convert(doc);
            //// Response.Headers.ContentType = "application/pdf";
            //Response.ContentType = "application/pdf";
            //await Response.Body.WriteAsync(data, 0, data.Length);
            return;
        }

        [HttpPost("SendPurchaseOrder")]
        public async Task<IActionResult> SendPurchaseOrder([FromBody] SendPurchaseOrderData sendPurchaseOrderData) {
            var emailParameters = sendPurchaseOrderData.EmailParameters;
            var purchaseOrder = sendPurchaseOrderData.PurchaseOrder;
            SendGrid.Response response;
            if (emailParameters != null) {
                var errorMessages = emailParameters.getErrorMessage();
                if (errorMessages != null) {
                    return BadRequest(errorMessages);
                }

                var client = EmailGenerator.GetNewSendGridClient();
                if (emailParameters.Bcc == null) {
                    emailParameters.Bcc = "";
                }

                var apEmail = "ap@gidindustrial.com";
                if (!emailParameters.Bcc.Contains(apEmail) && !emailParameters.To.Contains(apEmail)) {
                    if(!string.IsNullOrWhiteSpace(emailParameters.Bcc)){
                        emailParameters.Bcc += ",";
                    }
                    emailParameters.Bcc += apEmail;
                }

                var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

                response = await client.SendEmailAsync(msg);

                int responseStatusCodeNumber = (int)response.StatusCode;
                if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                    _context.PurchaseOrderEventLogEntries.Add(new PurchaseOrderEventLogEntry {
                        PurchaseOrderId = (int)sendPurchaseOrderData.PurchaseOrder.Id,
                        EventLogEntry = new EventLogEntry {
                            Event = "Sent Purchase Order",
                            CreatedAt = DateTime.UtcNow,
                            OccurredAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                            UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                        }
                    });
                    await _context.SaveChangesAsync();
                } else {
                    return BadRequest(new {
                        Error = "Error sending email. Status code was wrong",
                        StatusCode = response.StatusCode,
                        Body = response.Body
                    });
                }
            } else {
                _context.PurchaseOrderEventLogEntries.Add(new PurchaseOrderEventLogEntry {
                    PurchaseOrderId = (int)sendPurchaseOrderData.PurchaseOrder.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Confirmed Purchase Order (without emailing)",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                await _context.SaveChangesAsync();
            }

            //Now create inventory items and shipments
            purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.LineItems)
                .Where(po => po.Id == sendPurchaseOrderData.PurchaseOrder.Id)
                .FirstOrDefaultAsync();

            var purchaseOrderTotalCost = purchaseOrder.GetTotal();
            var lineItemsTotalCost = purchaseOrder.LineItemsTotalCost();
            var nonLineItemsTotalCost = purchaseOrder.NonLineItemsTotalCost();

            var incomingShipmentInventoryItems = new List<IncomingShipmentInventoryItem> { };
            foreach (PurchaseOrderLineItem purchaseOrderLineItem in purchaseOrder.LineItems) {
                var lineItemCost = purchaseOrderLineItem.GetCostForAllUnits();
                var lineItemCostFraction = lineItemCost / lineItemsTotalCost;
                decimal totalCostPerUnit = 0;

                if (purchaseOrderLineItem.Quantity > 0) { //make sure not to divide by 0
                    totalCostPerUnit = (purchaseOrderLineItem.Cost ?? 0) + nonLineItemsTotalCost * lineItemCostFraction / (purchaseOrderLineItem.Quantity ?? 0);
                }

                for (int i = 0; i < purchaseOrderLineItem.Quantity; ++i) {
                    var inventoryItem = new InventoryItem {
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        CreatedAt = DateTime.UtcNow,
                        ProductId = purchaseOrderLineItem.ProductId,
                        CurrencyOptionId = purchaseOrder.CurrencyOptionId,
                        Description = purchaseOrderLineItem.Description,
                        InventoryItemStatusOptionId = InventoryItemStatusOption.Inbound,
                        PurchaseOrderLineItemId = purchaseOrderLineItem.Id,
                        UnitCost = purchaseOrderLineItem.Cost,
                        TotalCost = totalCostPerUnit
                    };
                    incomingShipmentInventoryItems.Add(new IncomingShipmentInventoryItem {
                        InventoryItem = inventoryItem
                    });
                }
            }

            IncomingShipment incomingShipment = new IncomingShipment {
                CreatedAt = DateTime.UtcNow,
                InventoryItems = incomingShipmentInventoryItems
            };

            purchaseOrder.IncomingShipments = new List<PurchaseOrderIncomingShipment>{
                new PurchaseOrderIncomingShipment{
                    IncomingShipment = incomingShipment
                }
            };

            purchaseOrder.Sent = true;
            purchaseOrder.SentAt = DateTime.UtcNow;

            //set status
            purchaseOrder.PurchaseOrderStatusOptionId = (await _context.PurchaseOrderStatusOptions.FirstOrDefaultAsync(item => item.Value == "Sent")).Id;

            _context.Entry(purchaseOrder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("{id}/Cancel")]
        public async Task<IActionResult> CancelPurchaseOrder([FromBody] SendCancelPurchaseOrderData sendCancelPurchaseOrderData) {
            // if (!ModelState.IsValid)
            // {
            //     return BadRequest(ModelState);
            // }

            if (sendCancelPurchaseOrderData.PurchaseOrder == null || sendCancelPurchaseOrderData.PurchaseOrder.Id == null) {
                return BadRequest("A purchase order is required");
            }

            //When cancelling, need to remove
            // 1. Incoming Shipments
            // 2. Inventory Items in incoming shipments
            var purchaseOrder = await _context.PurchaseOrders
                .Include(po => po.IncomingShipments)
                    .ThenInclude(pois => pois.IncomingShipment)
                        .ThenInclude(incomingShipment => incomingShipment.InventoryItems)
                            .ThenInclude(inventoryItems => inventoryItems.InventoryItem)
                .FirstOrDefaultAsync(po => po.Id == sendCancelPurchaseOrderData.PurchaseOrder.Id);

            if (purchaseOrder == null)
                return NotFound(new {
                    Error = "A purchase order with that Id was not found"
                });

            // var pdfBytes = this.GenerateCancelPurchaseOrderPdf((int)id);

            //Need to check if any of the items have received a serial number yet. If so, the purchase order cannot be undone
            foreach (PurchaseOrderIncomingShipment purchaseOrderIncomingShipment in purchaseOrder.IncomingShipments) {
                foreach (IncomingShipmentInventoryItem incomingShipmentInventoryItem in purchaseOrderIncomingShipment.IncomingShipment.InventoryItems) {
                    if (incomingShipmentInventoryItem.ReceivedAt != null) {
                        return BadRequest("You can't cancel this purchase order because we have already received some of the items.");
                    }
                }
            }

            foreach (PurchaseOrderIncomingShipment purchaseOrderIncomingShipment in purchaseOrder.IncomingShipments) {
                foreach (IncomingShipmentInventoryItem incomingShipmentInventoryItem in purchaseOrderIncomingShipment.IncomingShipment.InventoryItems) {
                    // _context.Entry(incomingShipmentInventoryItem.InventoryItem).State = EntityState.Deleted;
                    _context.Entry(incomingShipmentInventoryItem).State = EntityState.Deleted;
                    _context.Entry(incomingShipmentInventoryItem.InventoryItem).State = EntityState.Deleted;
                }
                _context.Entry(purchaseOrderIncomingShipment).State = EntityState.Deleted;
                _context.Entry(purchaseOrderIncomingShipment.IncomingShipment).State = EntityState.Deleted;
            }

            purchaseOrder.Sent = false;
            purchaseOrder.SentAt = null;
            purchaseOrder.PurchaseOrderStatusOptionId = (await _context.PurchaseOrderStatusOptions.FirstOrDefaultAsync(item => item.Value == "Canceled")).Id;

            var emailParameters = sendCancelPurchaseOrderData.EmailParameters;

            SendGrid.Response response;
            if (emailParameters != null) {

                var errorMessages = emailParameters.getErrorMessage();
                if (errorMessages != null) {
                    return BadRequest(errorMessages);
                }

                var apEmail = "ap@gidindustrial.com";
                if (emailParameters.Bcc == null) {
                    emailParameters.Bcc = "";
                }
                if (!emailParameters.Bcc.Contains(apEmail) && !emailParameters.To.Contains(apEmail)) {
                    if(!string.IsNullOrWhiteSpace(emailParameters.Bcc)){
                        emailParameters.Bcc += ",";
                    }
                    emailParameters.Bcc += apEmail;
                }

                var client = EmailGenerator.GetNewSendGridClient();
                var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);
                response = await client.SendEmailAsync(msg);

                int responseStatusCodeNumber = (int)response.StatusCode;
                if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                    _context.PurchaseOrderEventLogEntries.Add(new PurchaseOrderEventLogEntry {
                        PurchaseOrderId = (int)sendCancelPurchaseOrderData.PurchaseOrder.Id,
                        EventLogEntry = new EventLogEntry {
                            Event = "Cancelled Purchase Order",
                            CreatedAt = DateTime.UtcNow,
                            OccurredAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                            UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                        }
                    });
                } else {
                    return BadRequest(new {
                        Error = "Error sending email. Status code was wrong"
                    });
                }
            } else {
                _context.PurchaseOrderEventLogEntries.Add(new PurchaseOrderEventLogEntry {
                    PurchaseOrderId = (int)sendCancelPurchaseOrderData.PurchaseOrder.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Cancelled Purchase Order (without sending email)",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
            }

            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Ok(purchaseOrder);
        }

        /// <summary>
        /// This method generates a pdf of the purchaseOrders
        /// </summary>
        /// <param name="id">The id of the purchaseOrder</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateCancelPurchaseOrderPdf")]
        public async Task GenerateCancelPurchaseOrderPdf(int id) {

            var purchaseOrder = await _context.PurchaseOrders
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Product)
                .Include(c => c.Contact)
                .Include(c => c.Supplier)
                    .ThenInclude(item => item.Addresses)
                        .ThenInclude(item => item.Address)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.ShippingAddress)
                .Include(item => item.CurrencyOption)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (purchaseOrder == null) {
                throw new Exception("Purchase order was not found");
            }

            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            purchaseOrder.Preparer = currentUser;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await purchaseOrder.CheckIfIsGidEurope(_context);
            string purchaseOrderHtml = viewRenderer.Render("~/Features/PurchaseOrder/Views/CancelPurchaseOrderPdf.cshtml", purchaseOrder, ViewData);
            var doc = new HtmlToPdfDocument() {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = purchaseOrderHtml,
                        WebSettings = { DefaultEncoding = "utf-8" }
                    }
                }
            };
            byte[] data = PdfConverter.Convert(doc);

            Response.ContentType = "application/pdf";
            await Response.Body.WriteAsync(data, 0, data.Length);
            // return data;
        }

        // PUT: PurchaseOrders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrder([FromRoute] int? id, [FromBody] PurchaseOrder purchaseOrder) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != purchaseOrder.Id) {
                return BadRequest();
            }

            _context.Entry(purchaseOrder).State = EntityState.Modified;
            if (purchaseOrder.ShippingAddress != null && purchaseOrder.ShippingAddress.Id != null) {
                _context.Entry(purchaseOrder.ShippingAddress).State = EntityState.Modified;
            }

            var previousPurchaseOrder = await _context.PurchaseOrders.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            var updatedEvent = "Updated";
            if (previousPurchaseOrder.PurchaseOrderStatusOptionId != purchaseOrder.PurchaseOrderStatusOptionId)
            {
                var status = await _context.PurchaseOrderStatusOptions.FirstOrDefaultAsync(item => item.Id == purchaseOrder.PurchaseOrderStatusOptionId);
                updatedEvent += " Status - " + (status != null ? status.Value : "None");
            }

            // If expected ship date changed, we need to add a chat to the sales order
            if (purchaseOrder.ExpectedShipDate.HasValue && previousPurchaseOrder.ExpectedShipDate.HasValue 
                && previousPurchaseOrder.ExpectedShipDate.Value != purchaseOrder.ExpectedShipDate.Value)
            {
                try
                {
                    // Get reason
                    PurchaseOrderExpectedShipDateChangeReason reason = await _context.PurchaseOrderExpectedShipDateChangeReasons.FirstOrDefaultAsync(i => i.Id == purchaseOrder.ExpectedShipDateChangeReasonId);

                    var query = from salesOrder in _context.SalesOrders select salesOrder;

                    query = query.Where(item =>
                        item.LineItems.Any(soLineItem => soLineItem.Sources.Any(soliSource => soliSource.PurchaseOrderId == id)) ||
                        item.PurchaseOrders.Any(soPurchaseOrders => soPurchaseOrders.PurchaseOrderId == id)
                    );

                    var salesOrders = await query.ToListAsync();

                    ChatMessageControllerHelper helper = new ChatMessageControllerHelper();
                    UserControllerHelper userHelper = new UserControllerHelper();

                    foreach (SalesOrder salesOrder in salesOrders)
                    {
                        // Get sales person user
                        var salesPerson = await userHelper.GetUserById(salesOrder.SalesPersonId.Value, _context);

                        // Create chat message and post it
                        ChatMessage chatMessage = new ChatMessage();
                        chatMessage.CreatedAt = DateTime.Now;
                        chatMessage.CreatedById = 2;
                        chatMessage.Message = String.Format("@[{3}] Purchase Order {0} Expected Date Changed to {1} - {2}", id, purchaseOrder.ExpectedShipDate.Value.ToShortDateString(),
                            (!String.IsNullOrEmpty(reason.Value) ? "Reason: " + reason.Value : ""), salesPerson.DisplayName);

                        // Post chat message
                        var chatResult = await helper.PostChatMessageLocal(chatMessage, _context, this);

                        // Set chat message id
                        chatMessage.Id = ((ChatMessage)((CreatedAtActionResult)chatResult).Value).Id;

                        // Create sales order chat message
                        SalesOrderChatMessage salesOrderChatMessage = new SalesOrderChatMessage();
                        salesOrderChatMessage.ChatMessage = chatMessage;
                        salesOrderChatMessage.ChatMessageId = chatMessage.Id;
                        salesOrderChatMessage.SalesOrderId = salesOrder.Id;

                        var salesOrderChatMessageResult = await helper.PostSalesOrderChatMessageLocal(salesOrderChatMessage, _context, this);
                    }
                }
                catch (Exception ex)
                {
                    string e = ex.Message;
                }
                
            }

            //add event log entry
            _context.PurchaseOrderEventLogEntries.Add(new PurchaseOrderEventLogEntry
            {
                PurchaseOrder = purchaseOrder,
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
                if (!PurchaseOrderExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            await purchaseOrder.UpdateTotal(_context);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: PurchaseOrders
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrder([FromBody] PurchaseOrder purchaseOrder) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            purchaseOrder.CreatedAt = DateTime.UtcNow;
            purchaseOrder.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);

            _context.PurchaseOrders.Add(purchaseOrder);
            //add event log entry
            _context.PurchaseOrderEventLogEntries.Add(new PurchaseOrderEventLogEntry {
                PurchaseOrder = purchaseOrder,
                EventLogEntry = new EventLogEntry {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            await _context.SaveChangesAsync();
            await purchaseOrder.UpdateTotal(_context);
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetPurchaseOrder", new { id = purchaseOrder.Id }, purchaseOrder);
        }

        // DELETE: PurchaseOrders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrder([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var purchaseOrder = await _context.PurchaseOrders
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (purchaseOrder == null) {
                return NotFound();
            }

            foreach (var itemAttachment in purchaseOrder.Attachments.ToList()) {
                await itemAttachment.Attachment.Delete(_context, _configuration);
            }
            _context.PurchaseOrders.Remove(purchaseOrder);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Ok(purchaseOrder);
        }

        private bool PurchaseOrderExists(int? id) {
            return _context.PurchaseOrders.Any(e => e.Id == id);
        }
    }
}