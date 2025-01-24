using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using DinkToPdf;
using WebApi.Services;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;
using GidIndustrial.Gideon.WebApi.Libraries;
using SelectPdf;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Invoices")]
    public class InvoicesController : Controller {
        private readonly AppDBContext _context;
        private readonly ViewRender viewRenderer;
        private IConverter PdfConverter;
        private IHostingEnvironment Environment;
        private QuickBooksConnector _quickBooksConnector;

        public InvoicesController(AppDBContext context, ViewRender renderer, IConverter converter, IHostingEnvironment env, QuickBooksConnector quickBooksConnector) {
            _context = context;
            viewRenderer = renderer;
            PdfConverter = converter;
            Environment = env;
            _quickBooksConnector = quickBooksConnector;
        }

        [HttpGet("CreateQuickBooksInvoice/Do")]
        public async Task<IActionResult> CreateQuickBooksInvoice() {
            //return Ok(await _quickBooksConnector.PostResource("customer", new {
            //    GivenName = "Bobby The First",
            //    PrimaryEmailAddr = new {
            //        Address = "bobby@jones.com"
            //    }
            //}));
            return Ok(new object());
        }

        // GET: Invoices
        [HttpGet]
        public async Task<IActionResult> GetInvoices(
            [FromQuery] int? companyId,
            [FromQuery] int? salesOrderId,
            [FromQuery] bool? unpaidOnly,
            [FromQuery] int? currencyOptionId,
            [FromQuery] int? gidLocationOptionId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true,
            [FromQuery] bool totalsOnly = false
        ) {
            var query = from invoice in _context.Invoices select invoice;

            if (companyId != null)
                query = query.Where(item => item.CompanyId == companyId);
            if (unpaidOnly == true)
                query = query.Where(item => item.Balance > 0);
            if (currencyOptionId != null)
                query = query.Where(item => item.CurrencyOptionId == currencyOptionId);
            if (gidLocationOptionId != null)
                query = query.Where(item => item.GidLocationOptionId == gidLocationOptionId);
            if (salesOrderId != null)
                query = query.Where(item => item.SalesOrderId == salesOrderId);

            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "SalesOrderId":
                    query = sortAscending ? query.OrderBy(item => item.SalesOrderId) : query.OrderByDescending(item => item.SalesOrderId);
                    break;
                // case "Amount":
                //     query = sortAscending ? query.OrderBy(item => item.) : query.OrderByDescending(item => item.);
                //     break;
                case "Balance":
                    query = sortAscending ? query.OrderBy(item => item.Balance) : query.OrderByDescending(item => item.Balance);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                case "CreatedById":
                    query = sortAscending ? query.OrderBy(item => item.CreatedById) : query.OrderByDescending(item => item.CreatedById);
                    break;
                case "DateDue":
                    query = sortAscending ? query.OrderBy(item => item.DateDue) : query.OrderByDescending(item => item.DateDue);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            query = query
                .Include(item => item.BillingAddress)
                .Include(item => item.ShippingAddress)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.Company);

            if (totalsOnly)
            {
                return Ok(new
                {
                    USD = new
                    {
                        Total = await query.Where(item => item.CurrencyOptionId == 1).SumAsync(item => item.Total),
                        Balance = await query.Where(item => item.CurrencyOptionId == 1).SumAsync(item => item.Balance)
                    },
                    EUR = new
                    {
                        Total = await query.Where(item => item.CurrencyOptionId == 2).SumAsync(item => item.Total),
                        Balance = await query.Where(item => item.CurrencyOptionId == 2).SumAsync(item => item.Balance)
                    }
                    
                });
            }

            return Ok(new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            });
        }

        // GET: Invoices/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var invoice = await _context.Invoices
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .Include(item => item.BillingAddress)
                .Include(item => item.Contact)
                .Include(q => q.EventLogEntries)
                    .ThenInclude(q => q.EventLogEntry)
                .Include(item => item.ShippingAddress)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (invoice == null) {
                return NotFound();
            }

            return Ok(invoice);
        }

        // GET: Invoices/5/LineItems
        [HttpGet("{id}/LineItems")]
        public async Task<IActionResult> GetInvoiceLineItems([FromRoute] int id, [FromQuery] bool? includeInventoryItems) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = from so in _context.Invoices select so;

            query = query
                .Include(m => m.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.SalesOrderLineItem)
                        .ThenInclude(item => item.InventoryItems)
                            .ThenInclude(item => item.InventoryItem)
                .Include(m => m.Company);

            // var lineItems = salesOrder.LineItems;
            // var shippedStatus = await _context.InventoryItemStatusOptions.FirstOrDefaultAsync(item => item.Value == "Shipped");
            // if(shippedStatus == null){
            //     return BadRequest(new {
            //         Error = "Unable to find the 'Shipped' inventory item status id."
            //     });
            // }
            // lineItems.ForEach(lineItem => {
            //     lineItem.Quantity = lineItem.InventoryItems.Count(lineItemInventoryItem => lineItemInventoryItem.InventoryItem.InventoryItemStatusOptionId == shippedStatus.Id);
            // });


            Invoice salesOrder = await query
                .FirstOrDefaultAsync(l => l.Id == id);

            var shippedStatus = await _context.InventoryItemStatusOptions.FirstOrDefaultAsync(item => item.Value == "Shipped");
            if (shippedStatus == null) {
                return BadRequest(new {
                    Error = "Unable to find the 'Shipped' inventory item status id."
                });
            }
            salesOrder.LineItems.ForEach(lineItem => {
                lineItem.QuantityShipped = lineItem.SalesOrderLineItem.InventoryItems.Count(lineItemInventoryItem => lineItemInventoryItem.InventoryItem.InventoryItemStatusOptionId == shippedStatus.Id);
            });

            return Ok(salesOrder.LineItems);
        }

        [HttpGet("{id}/InvoiceCredits")]
        public ListResult GetInvoiceCredits([FromRoute] int id, [FromQuery] int perPage = 100, [FromQuery] int skip = 0) {
            var query = _context.InvoiceCredits
                .Include(item => item.Credit)
                .Where(item => item.InvoiceId == id)
                .OrderByDescending(item => item.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }


        [HttpGet("{id}/InvoiceCashReceipts")]
        public ListResult InvoiceCashReceipts([FromRoute] int id, [FromQuery] int perPage = 100, [FromQuery] int skip = 0) {
            var query = _context.InvoiceCashReceipts
                .Include(item => item.CashReceipt)
                .Where(item => item.InvoiceId == id)
                .OrderByDescending(item => item.CashReceipt.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // PUT: Invoices/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice([FromRoute] int id, [FromBody] Invoice invoice) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (id != invoice.Id) {
                return BadRequest();
            }

            var originalInvoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(item => item.Id == invoice.Id);
            if (originalInvoice == null) {
                return NotFound("An invoice was not found with that Id");
            }
            bool canChangeQuickBooksId = false;
            if (GidIndustrial.Gideon.WebApi.Models.User.HasPermission(_context, User, "ManageBilling")) {
                canChangeQuickBooksId = true;
            }
            // var user = await _context.Users
            //     .Include(item => item.Permissions)
            //     .FirstOrDefaultAsync(item => item.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(User));

            if (String.IsNullOrWhiteSpace(invoice.QuickBooksId) || !canChangeQuickBooksId) {
                invoice.QuickBooksId = originalInvoice.QuickBooksId;
            }
            invoice.QuickBooksSyncToken = originalInvoice.QuickBooksSyncToken;

            if (invoice.ShippingAddress != null && invoice.ShippingAddress.Id != null)
                _context.Entry(invoice.ShippingAddress).State = EntityState.Modified;
            else if (invoice.ShippingAddress != null)
                _context.Entry(invoice.ShippingAddress).State = EntityState.Added;

            if (invoice.BillingAddress != null && invoice.BillingAddress.Id != null)
                _context.Entry(invoice.BillingAddress).State = EntityState.Modified;
            else if (invoice.BillingAddress != null)
                _context.Entry(invoice.BillingAddress).State = EntityState.Added;


            _context.Entry(invoice).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!InvoiceExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            var newInvoice = await _context.Invoices.AsNoTracking()
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.Company)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .FirstOrDefaultAsync(item => item.Id == id);
            newInvoice.Balance = await newInvoice.GetBalance(_context);
            invoice.Balance = newInvoice.Balance;
            await _context.SaveChangesAsync();

            _context.Entry(invoice).State = EntityState.Detached;

            if (!String.IsNullOrWhiteSpace(invoice.QuickBooksId)) {
                var result = await invoice.SyncWithQuickBooks(_quickBooksConnector, _context);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(newInvoice);
        }

        // POST: Invoices
        [HttpPost]
        public async Task<IActionResult> PostInvoice([FromBody] Invoice invoice) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            invoice.CreatedAt = DateTime.UtcNow;
            invoice.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
            if (invoice.LineItems != null)
                invoice.Balance = await invoice.GetBalance(_context);
            await _context.SaveChangesAsync();

            //now post invoice to quickbooks
            invoice = await _context.Invoices.AsNoTracking()
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.BillingAddress)
                .Include(item => item.ShippingAddress)
                .FirstOrDefaultAsync(item => item.Id == invoice.Id);

            return CreatedAtAction("GetInvoice", new { id = invoice.Id }, invoice);
        }

        // DELETE: Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var invoice = await _context.Invoices
                .Include(item => item.Credits)
                .Include(item => item.CashReceipts)
                // .Include(item => item.Attachments)
                //     .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (invoice == null) {
                return NotFound();
            }

            if (invoice.Credits.Count > 0 || invoice.CashReceipts.Count > 0) {
                return BadRequest("You can't delete this invoice until you remove all Cash Receipts and Credits applied towards it");
            }

            // foreach(var itemAttachment in invoice.Attachments.ToList()){
            //     await itemAttachment.Attachment.Delete(_context, _configuration);
            // }
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            if (!String.IsNullOrWhiteSpace(invoice.QuickBooksId)) {
                var result = await invoice.DeleteFromQuickBooks(_quickBooksConnector, _context);
                if (!result.Succeeded)
                    return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return Ok(invoice);
        }

        private bool InvoiceExists(int id) {
            return _context.Invoices.Any(e => e.Id == id);
        }


        /// <summary>
        /// This method generates a pdf of the invoices
        /// </summary>
        /// <param name="id">The id of the invoice</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateInvoicePdf")]
        public async Task GenerateInvoicePdf([FromRoute] int id) {
            var invoice = await _context.Invoices
                .Include(item => item.LineItems)
                    .ThenInclude(l => l.Product)
                        .ThenInclude(j => j.Manufacturer)
                .Include(item => item.LineItems)
                    .ThenInclude(l => l.SalesOrderLineItem)
                        .ThenInclude(j => j.Warranty)
                .Include(item => item.LineItems)
                    .ThenInclude(l => l.SalesOrderLineItem)
                        .ThenInclude(j => j.LineItemConditionType)
                .Include(item => item.LineItems)
                    .ThenInclude(l => l.SalesOrderLineItem)
                        .ThenInclude(j => j.LeadTime)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.PaymentMethod)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.OutgoingShipments)
                        .ThenInclude(item => item.OutgoingShipment)
                            .ThenInclude(item => item.Boxes)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.ShippingCarrier)
                .Include(item => item.ShippingCarrierShippingMethod)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.PaymentMethod)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.OutgoingShipments)
                        .ThenInclude(item => item.OutgoingShipment)
                            .ThenInclude(item => item.ShippingCarrier)
                .Include(item => item.Company)
                .Include(item => item.ShippingAddress)
                .Include(item => item.BillingAddress)
                .Include(item => item.PaymentTerm)
                // .Include(item => item.LineItems)
                //     .ThenInclude(l => l.Warranty)
                // .Include(item => item.LineItems)
                //     .ThenInclude(l => l.LineItemConditionType)
                // .Include(item => item.LineItems)
                //     .ThenInclude(item => item.LeadTime)
                // .Include(c => c.Contact)
                // .Include(item => item.PaymentMethod)
                // .Include(item => item.BillingAddress)
                //     .ThenInclude(item => item.Country)
                // .Include(item => item.ShippingAddress)
                //     .ThenInclude(item => item.Country)
                // .Include(item => item.ShippingCarrier)
                // .Include(item => item.ShippingCarrierShippingMethod)
                .Include(item => item.CurrencyOption)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.SalesPerson)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (invoice == null) {
                Response.StatusCode = 400;
                return;
            }
            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            invoice.Preparer = currentUser;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await invoice.CheckIfIsGidEurope(_context);
            ViewData["LogoUrlNew"] = Environment.ContentRootPath + @"\Features\Common\Views\logo-footer.png";
            ViewData["LogoUrlNew2"] = Environment.ContentRootPath + @"\Features\Common\Views\logo-footer2.png";
            ViewData["Font1"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Regular.ttf";
            ViewData["Font2"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Medium.ttf";
            ViewData["Font3"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-SemiBold.ttf";
            ViewData["Font4"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Bold.ttf";

            // Render html
            string html = viewRenderer.Render("~/Features/Invoice/Views/InvoicePdfNew.cshtml", invoice, ViewData);

            

            // Create initial pdf
            HtmlToPdf converter = new HtmlToPdf();
            SelectPdf.PdfDocument pdfDocument = converter.ConvertHtmlString(html);

            // Remove last blank page
            if (pdfDocument.Pages.Count > 1 && pdfDocument.Pages[pdfDocument.Pages.Count - 1].ClientRectangle.Location.IsEmpty)
            {
                // pdfDocument.RemovePageAt(pdfDocument.Pages.Count - 1);
            }

            // Render t&c html
            string tcHtml = viewRenderer.Render("~/Features/Common/Views/TermsAndConditionsPdf.cshtml", invoice.GidLocationOption, ViewData);

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
            //ViewData["IsGidEurope"] = await invoice.CheckIfIsGidEurope(_context);
            //string invoiceHtml = viewRenderer.Render("~/Features/Invoice/Views/InvoicePdf.cshtml", invoice, ViewData);
            //var doc = new HtmlToPdfDocument() {
            //    GlobalSettings = {
            //        ColorMode = ColorMode.Color,
            //        Orientation = Orientation.Portrait,
            //        PaperSize = PaperKind.A4,
            //    },
            //    Objects = {
            //        new ObjectSettings() {
            //            PagesCount = true,
            //            HtmlContent = invoiceHtml,
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

        // POST: Invoices
        [HttpPost("SendInvoice")]
        public async Task<IActionResult> SendInvoice([FromBody] SendInvoiceData sendInvoiceData) {
            var emailParameters = sendInvoiceData.EmailParameters;
            var invoice = sendInvoiceData.Invoice;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null) {
                return BadRequest(errorMessages);
            }

            invoice.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);
            msg.AddCc("ar@gidindustrial.com");

            var response = await client.SendEmailAsync(msg);

            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                _context.InvoiceEventLogEntries.Add(new InvoiceEventLogEntry {
                    InvoiceId = (int)sendInvoiceData.Invoice.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Sent Invoice",
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

            // var confirmedInvoiceStatusOption = await _context.InvoiceStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower().Contains("confirmed"));
            // if (confirmedInvoiceStatusOption != null) {
            //     invoice.InvoiceStatusOptionId = confirmedInvoiceStatusOption.Id;
            // } else {
            //     return BadRequest(new {
            //         Error = "Error finding sales order status option"
            //     });
            // }

            invoice.SentAt = DateTime.UtcNow;
            invoice.CancelledAt = null;

            _context.Entry(invoice).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            _context.Entry(invoice).State = EntityState.Detached;


            var newInvoice = await _context.Invoices.AsNoTracking()
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.Company)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .FirstOrDefaultAsync(item => item.Id == invoice.Id);

            var result = await newInvoice.SyncWithQuickBooks(_quickBooksConnector, _context);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, result);

            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        [HttpGet("CreateInvoiceFromSalesOrderId")]
        public async Task<IActionResult> Invoice([FromQuery] int salesOrderId, [FromQuery] bool shippedOnly = false){
            var salesOrder = await _context.SalesOrders.FirstOrDefaultAsync(item => item.Id == salesOrderId);
            if(salesOrder == null){
                return BadRequest("A sales order was not found with that Id");
            }
            var userId = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            var newInvoice = await GidIndustrial.Gideon.WebApi.Models.Invoice.CreateInvoiceFromSalesOrderId(_context, salesOrderId, userId, shippedOnly);
            return Ok(newInvoice);
        }

        /// <summary>
        /// This method generates a pdf of the invoices
        /// </summary>
        /// <param name="id">The id of the invoice</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateCancelInvoicePdf")]
        public async Task GenerateCancelInvoicePdf([FromRoute] int id) {
            var invoice = await _context.Invoices
                .Include(item => item.LineItems)
                    .ThenInclude(l => l.Product)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.PaymentMethod)
                // .Include(item => item.LineItems)
                //     .ThenInclude(l => l.Warranty)
                // .Include(item => item.LineItems)
                //     .ThenInclude(l => l.LineItemConditionType)
                // .Include(item => item.LineItems)
                //     .ThenInclude(item => item.LeadTime)
                // .Include(c => c.Contact)
                // .Include(item => item.PaymentMethod)
                // .Include(item => item.BillingAddress)
                //     .ThenInclude(item => item.Country)
                // .Include(item => item.ShippingAddress)
                //     .ThenInclude(item => item.Country)
                // .Include(item => item.ShippingCarrier)
                // .Include(item => item.ShippingCarrierShippingMethod)
                .Include(item => item.CurrencyOption)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.SalesPerson)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (invoice == null) {
                Response.StatusCode = 400;
                return;
            }
            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            invoice.Preparer = currentUser;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await invoice.CheckIfIsGidEurope(_context);
            string invoiceHtml = viewRenderer.Render("~/Features/Invoice/Views/CancelInvoicePdf.cshtml", invoice, ViewData);
            var doc = new HtmlToPdfDocument() {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = invoiceHtml,
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

        [HttpPost("CancelInvoice")]
        public async Task<IActionResult> CancelInvoice([FromBody] SendInvoiceData cancelInvoiceData) {
            var emailParameters = cancelInvoiceData.EmailParameters;
            var invoice = cancelInvoiceData.Invoice;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null) {
                return BadRequest(errorMessages);
            }

            invoice.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

            var response = await client.SendEmailAsync(msg);

            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                _context.InvoiceEventLogEntries.Add(new InvoiceEventLogEntry {
                    InvoiceId = (int)cancelInvoiceData.Invoice.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Sent Invoice Cancellation",
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

            // var confirmedInvoiceStatusOption = await _context.InvoiceStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower().Contains("confirmed"));
            // if (confirmedInvoiceStatusOption != null) {
            //     invoice.InvoiceStatusOptionId = confirmedInvoiceStatusOption.Id;
            // } else {
            //     return BadRequest(new {
            //         Error = "Error finding sales order status option"
            //     });
            // }

            invoice.SentAt = null;
            invoice.CancelledAt = DateTime.UtcNow;

            _context.Entry(invoice).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        [HttpGet("{id}/ResyncWithQuickBooks")]
        public async Task<IActionResult> ResyncWithQuickBooks([FromRoute] int id) {
            //var invoice = await _context.Invoices.FirstOrDefaultAsync(item => item.Id == id);
            //if (invoice == null) {
            //    return NotFound("An invoice with that Id doesn't exist");
            //}
            //if (invoice.SentAt == null) {
            //    return BadRequest("This invoice cannot be synced until it has been sent");
            //}

            //var result = await invoice.SyncWithQuickBooks(_quickBooksConnector, _context);
            //if (result.Succeeded)
            //    return Ok(invoice);
            return StatusCode(StatusCodes.Status200OK);

        }

        [HttpGet("{id}/MarkSent")]
        public async Task<IActionResult> MarkSent([FromRoute] int id) {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(item => item.Id == id);
            if (invoice == null)
                return NotFound("An invoice was not found with that Id: " + id);

            invoice.CancelledAt = null;
            invoice.SentAt = DateTime.UtcNow;

            _context.InvoiceEventLogEntries.Add(new InvoiceEventLogEntry {
                InvoiceId = id,
                EventLogEntry = new EventLogEntry {
                    Event = "Invoice Marked As Sent Without Sending",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            await _context.SaveChangesAsync();

            var newInvoice = await _context.Invoices.AsNoTracking()
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.Company)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .FirstOrDefaultAsync(item => item.Id == invoice.Id);

            _context.Entry(invoice).State = EntityState.Detached;

            var result = await newInvoice.SyncWithQuickBooks(_quickBooksConnector, _context);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, result);

            return Ok(invoice);
        }
    }
}