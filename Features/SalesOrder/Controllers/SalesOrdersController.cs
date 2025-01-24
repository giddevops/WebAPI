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
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QuickBooks.Models;
using SelectPdf;

namespace WebApi.Features.Controllers {
    /// <summary>
    /// 
    /// </summary>
    [Produces("application/json")]
    [Route("SalesOrders")]
    public class SalesOrdersController : Controller {
        private readonly AppDBContext _context;
        // private readonly string SendgridApiKey;
        public IHostingEnvironment Environment;
        private IConverter PdfConverter;
        private IConfiguration _configuration;
        public string BaseGidSalesOrderUrl = "https://forms.gidindustrial.com/key/salesOrder/";
        public CreditCardTransactionsController creditCardTransactionsController;
        public ChatMessagesController chatMessagesController;

        //This is a helper service that allows rendering a razor file to raw html.  This is used to generate salesOrders
        private readonly ViewRender viewRenderer;

        public SalesOrdersController(AppDBContext context, IConfiguration config, ViewRender renderer, IHostingEnvironment env, IConverter converter, CreditCardTransactionsController transactionsController, ChatMessagesController messagesController) {
            _context = context;
            // SendgridApiKey = config.GetValue<string>("SENDGRID_API_KEY");
            _configuration = config;
            viewRenderer = renderer;
            Environment = env;
            PdfConverter = converter;
            creditCardTransactionsController = transactionsController;
            chatMessagesController = messagesController;
        }

        // GET: SalesOrders
        [HttpGet]
        public async Task<IActionResult> GetSalesOrders(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] Boolean fetchRelated = false,
            [FromQuery] int? contactId = null,
            [FromQuery] int? companyId = null,
            [FromQuery] bool withOutstandingBalanceOnly = false,
            [FromQuery] int? productId = null,
            [FromQuery] int? gidLocationOptionId = null,
            [FromQuery] int? salesPersonId = null,
            [FromQuery] int? salesOrderStatusOptionId = null,
            [FromQuery] string customerPurchaseOrderNumber = null,
            [FromQuery] int? productTypeId = null,
            [FromQuery] DateTime? createdAtStartDate = null,
            [FromQuery] DateTime? createdAtEndDate = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true,
            [FromQuery] bool totalsOnly = false,
            [FromQuery] bool hasBalance = false,
            [FromQuery] bool missingInvoice = false,
            [FromQuery] bool funded = false,
            [FromQuery] int? paymentTermId = null,
            [FromQuery] string companyName = null
        ) {
            var query = from salesOrder in _context.SalesOrders select salesOrder;
            if (String.IsNullOrWhiteSpace(searchString))
                searchString = null;

            if (fetchRelated) {
                query = query.Include(l => l.LineItems)
                    .ThenInclude(item => item.Product)
                        .ThenInclude(item => item.Manufacturer)
                            .ThenInclude(item => item.Invoices)
                                .ThenInclude(item => item.Company);
            }

            if (companyId != null)
                query = query.Where(l => l.CompanyId == companyId);
            if (contactId != null)
                query = query.Where(l => l.ContactId == contactId);
            if (productId != null)
                query = query.Where(l => l.LineItems.Any(li => li.ProductId == productId));
            if (salesPersonId != null)
                query = query.Where(item => item.SalesPersonId == salesPersonId);
            if (productTypeId != null)
                query = query.Where(item => item.LineItems.Any(pLineItem => pLineItem.Product.ProductTypeId == productTypeId));
            if (salesOrderStatusOptionId != null)
                query = query.Where(item => item.SalesOrderStatusOptionId == salesOrderStatusOptionId);
            if (createdAtStartDate != null)
                query = query.Where(l => l.CreatedAt >= createdAtStartDate);
            if (createdAtEndDate != null)
                query = query.Where(l => l.CreatedAt <= createdAtEndDate);
            if (!String.IsNullOrWhiteSpace(customerPurchaseOrderNumber))
                query = query.Where(item => EF.Functions.Like(item.CustomerPurchaseOrderNumber, customerPurchaseOrderNumber + "%"));
            if (!String.IsNullOrWhiteSpace(companyName))
                query = query.Where(item => EF.Functions.Like(item.Company.Name, companyName + "%"));
            if(gidLocationOptionId != null)
                query = query.Where(item => item.GidLocationOptionId == gidLocationOptionId);
            if (hasBalance)
                query = query.Where(item => item.Balance > 0);
            if (missingInvoice)
                query = query.Where(item => item.Invoices.Count == 0);
            if (funded)
                query = query.Where(item => item.Funded == true);
            if (paymentTermId != null)
                query = query.Where(item => item.PaymentTermId == paymentTermId);
            // if (withOutstandingBalanceOnly)
            //     query = query.Where(item => item.Bala)

            if (searchString == null)
            {
                switch (sortBy)
                {
                    case "Id":
                        query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                        break;
                    case "Company.Name":
                        query = sortAscending ? query.OrderBy(item => item.Company.Name) : query.OrderByDescending(item => item.Company.Name);
                        break;
                    case "SalesPerson.DisplayName":
                        query = sortAscending ? query.OrderBy(item => item.SalesPerson.DisplayName) : query.OrderByDescending(item => item.SalesPerson.DisplayName);
                        break;
                    case "CreatedAt":
                        query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                        break;
                    case "ShipDate":
                        query = sortAscending ? query.OrderBy(item => item.OutgoingShipments.FirstOrDefault().OutgoingShipment.ShippedAt) : query.OrderByDescending(item => item.OutgoingShipments.FirstOrDefault().OutgoingShipment.ShippedAt);
                        break;
                    case "PaymentTermId":
                        query = sortAscending ? query.OrderBy(item => item.PaymentTermId) : query.OrderByDescending(item => item.PaymentTermId);
                        break;
                    case "CustomerPONumber":
                        query = sortAscending ? query.OrderBy(item => item.CustomerPurchaseOrderNumber) : query.OrderByDescending(item => item.CustomerPurchaseOrderNumber);
                        break;
                    case "SalesOrderStatusOptionId":
                        query = sortAscending ? query.OrderBy(item => item.SalesOrderStatusOptionId) : query.OrderByDescending(item => item.SalesOrderStatusOptionId);
                        break;
                    case "Total":
                        query = sortAscending ? query.OrderBy(item => item.Total) : query.OrderByDescending(item => item.Total);
                        break;
                    case "Balance":
                        query = sortAscending ? query.OrderBy(item => item.Balance) : query.OrderByDescending(item => item.Balance);
                        break;
                    case "ShippedProfit":
                        query = sortAscending ? query.OrderBy(item => item.ShippedProfit) : query.OrderByDescending(item => item.ShippedProfit);
                        break;
                    case "PartNumber":
                        query = sortAscending ? query.OrderBy(item => item.LineItems.FirstOrDefault().Product.PartNumber) : query.OrderByDescending(item => item.LineItems.FirstOrDefault().Product.PartNumber);
                        break;
                    case "ExpectedDeliveryDate":
                        query = sortAscending ? query.OrderBy(item => item.ExpectedDeliveryDate) : query.OrderByDescending(item => item.ExpectedDeliveryDate);
                        break;
                    default:
                        query = query.OrderByDescending(item => item.CreatedAt);
                        break;
                }
            }

            if (searchString != null) {
                searchString = searchString.Trim();
                int searchStringNumber;
                if (Int32.TryParse(searchString, out searchStringNumber)) {

                } else {
                    searchStringNumber = 0;
                }
                //query = @"SELECT SalesOrders.* FROM SalesOrders"
                query = query.Where(item =>
                    item.Id == searchStringNumber ||
                    EF.Functions.Like(item.Company.Name, searchString + '%') ||
                    EF.Functions.Like(item.Contact.FirstName, searchString + '%') ||
                    EF.Functions.Like(item.Contact.LastName, searchString + '%') ||
                    EF.Functions.Like(item.Phone, searchString + '%') ||
                    EF.Functions.Like(item.Email, searchString + '%') ||
                    EF.Functions.Like(item.CustomerPurchaseOrderNumber, searchString + '%') ||
                    item.LineItems.Any(lLineItem =>
                        EF.Functions.Like(lLineItem.Product.PartNumber, searchString + '%')
                    )
                );
            }


            if (totalsOnly) {
                return Ok(new {
                    USD = new {
                        Total = await query.Where(item => item.CurrencyOptionId == 1).SumAsync(item => item.Total),
                        ShippedProfitTotal = await query.Where(item => item.CurrencyOptionId == 1).SumAsync(item => item.ShippedProfit)
                    },
                    EUR = new {
                        Total = await query.Where(item => item.CurrencyOptionId == 2).SumAsync(item => item.Total),
                        ShippedProfitTotal = await query.Where(item => item.CurrencyOptionId == 2).SumAsync(item => item.ShippedProfit)
                    },
                    NetProfit = await query.SumAsync(item => item.ProjectedProfit),
                    Balance = await query.SumAsync(item => item.Balance)
                });
            }

            if (String.IsNullOrWhiteSpace(searchString))
            {
                query = query
                    .Include(q => q.Company)
                    .Include(item => item.SalesPerson)
                    .Include(item => item.LineItems)
                        .ThenInclude(item => item.Product);
            }
            else
            {
                query = query
                    .Include(q => q.Company)
                    .Include(item => item.SalesPerson)
                    .Include(item => item.Invoices)
                        .ThenInclude(item => item.CashReceipts)
                            .ThenInclude(item => item.CashReceipt)
                    .Include(item => item.OutgoingShipments)
                        .ThenInclude(item => item.OutgoingShipment)
                    .Include(item => item.LineItems)
                        .ThenInclude(item => item.Product);
            }


            var items = await query.Skip(skip).Take(perPage).ToListAsync();
            // foreach (var item in items) {
            //     // item.OutstandingBalance = 
            //     var totalPrice = item.GetTotal();
            //     var totalPaid = item.Invoices.Sum(item2 => item2.CashReceipts.Sum(item3 => item3.Amount));
            //     item.Balance = totalPrice - totalPaid;
            // }

            var count = -1;
            if (String.IsNullOrWhiteSpace(searchString))
                count = await query.CountAsync();

            return Ok(new ListResult {
                Items = items,
                Count = count
            });
        }

        [HttpGet("GetCustomerUploadType")]
        public async Task<int> GetSalesOrderCustomerUploadType() {
            return (await _context.AttachmentTypes.FirstOrDefaultAsync(item => item.Value.ToLower() == "sales order customer upload")).Id;
        }


        [HttpGet("{id}/CheckIfHasCreditCard")]
        public async Task<IActionResult> CheckIfHasCreditCard([FromRoute] int id) {
            var salesOrder = await _context.SalesOrders
                .Include(item => item.CreditCard)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (salesOrder == null) {
                return NotFound("A sales order was not found with that Id");
            }
            if (salesOrder.CreditCard != null && !String.IsNullOrWhiteSpace(salesOrder.CreditCard.CardNumberEncrypted)) {
                return Ok(true);
            }
            return Ok(false);
        }

        [HttpPost("{id}/SaveCreditCard")]
        [RequirePermission("ManageBilling")]
        public async Task<IActionResult> SaveCreditCard([FromRoute] int? id, [FromBody] CreditCard creditCardData) {
            var salesOrder = await _context.SalesOrders
                .Include(item => item.CreditCard)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (salesOrder == null) {
                return BadRequest("There is no credit card for this sales order");
            }

            var creditCard = new CreditCard {
                NameOnCardEncrypted = Encryption.EncryptData(creditCardData.NameOnCard),
                CardNumberEncrypted = Encryption.EncryptData(creditCardData.CardNumber),
                SecurityCodeEncrypted = Encryption.EncryptData(creditCardData.SecurityCode),
                ExpirationMonthEncrypted = Encryption.EncryptData(creditCardData.ExpirationMonth),
                ExpirationYearEncrypted = Encryption.EncryptData(creditCardData.ExpirationYear),
            };
            if (salesOrder.CreditCard == null) {
                using (var transaction = _context.Database.BeginTransaction()) {
                    _context.CreditCards.Add(creditCard);
                    salesOrder.CreditCard = creditCard;
                    await _context.SaveChangesAsync();
                    transaction.Commit();
                }
            } else {
                _context.CreditCards.Remove(salesOrder.CreditCard);
                await _context.SaveChangesAsync();
                salesOrder.CreditCard = creditCard;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpGet("{Id}/PaymentStats")]
        public async Task<IActionResult> PaymentStats([FromRoute] int id) {
            var salesOrder = await _context.SalesOrders
                .Include(item => item.Invoices)
                    .ThenInclude(item => item.LineItems)
                .Include(item => item.Invoices)
                    .ThenInclude(item => item.CashReceipts)
                        .ThenInclude(item => item.CashReceipt)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == id);

            var relatedInventoryItems = await _context.InventoryItems
                .Where(item => item.SalesOrderLineItems.Any(item2 => item2.SalesOrderLineItem.SalesOrderId == id))
                .Include(item => item.SalesOrderLineItems)
                    .ThenInclude(item => item.SalesOrderLineItem)
                .ToListAsync();

            var inventoryItemsShipped = await _context.InventoryItems
                .Where(item => item.OutgoingShipmentBoxes.Any(oBox => oBox.OutgoingShipmentBox.OutgoingShipment.SalesOrderOutgoingShipment.SalesOrderId == id && oBox.OutgoingShipmentBox.OutgoingShipment.ShippedAt != null))
                .ToListAsync();

            decimal shippedLineItemsTotal = 0;
            foreach (var itemShipped in inventoryItemsShipped) {
                var relatedInventoryItem = relatedInventoryItems.FirstOrDefault(item => item.Id == itemShipped.Id);
                if (relatedInventoryItem != null) {
                    var itemPrice = relatedInventoryItem.SalesOrderLineItems[0].SalesOrderLineItem.Price;
                    shippedLineItemsTotal += itemPrice ?? 0;
                }
            }

            var salesOrderLineItemsTotal = salesOrder.LineItems.Sum(item => (item.Price ?? 0) * (item.Quantity ?? 0));
            decimal totalFraction = 1;
            if (salesOrderLineItemsTotal != 0)
                totalFraction = shippedLineItemsTotal / salesOrderLineItemsTotal;
            var salesOrderExtraFees = (salesOrder.SalesTax ?? 0) + (salesOrder.WireTransferFee ?? 0) + (salesOrder.ShippingAndHandlingFee ?? 0) + (salesOrder.ExpediteFee ?? 0) + (salesOrder.CreditCardFee ?? 0);

            if (salesOrder == null) {
                return NotFound("A sales order was not found with that Id");
            }

            var authorizedAmount = await _context.CreditCardTransactions.Where(item =>
                    item.SalesOrderId == id &&
                    item.Succeeded == true &&
                    (item.CreditCardTransactionType == CreditCardTransactionType.Authorization || item.CreditCardTransactionType == CreditCardTransactionType.AuthorizationAndCapture)
                    && item.VoidedAt == null
                )
                .SumAsync(item => item.Amount);

            var chargedAmount = await _context.CreditCardTransactions.Where(item =>
                    item.SalesOrderId == id &&
                    item.Succeeded == true &&
                    (item.CreditCardTransactionType == CreditCardTransactionType.Capture || item.CreditCardTransactionType == CreditCardTransactionType.AuthorizationAndCapture)
                )
                .SumAsync(item => item.Amount);
            var refundedAmount = await _context.CreditCardTransactions.Where(item =>
                    item.SalesOrderId == id &&
                    item.Succeeded == true &&
                    (item.CreditCardTransactionType == CreditCardTransactionType.Refund)
            ).SumAsync(item => item.Amount);

            var netChargedAmount = chargedAmount - refundedAmount;

            return Ok(new {
                ShippedTotal = Math.Round(shippedLineItemsTotal + salesOrderExtraFees * totalFraction, 2),
                AuthorizedAmount = authorizedAmount,
                ChargedAmount = chargedAmount,
                NetChargedAmount = netChargedAmount,
                Total = salesOrder.GetTotal(),
                InvoicesCount = salesOrder.Invoices.Count,
                InvoicedTotal = salesOrder.Invoices.Sum(item => item.GetTotal()),
                InvoicesRemainingBalance = salesOrder.Invoices.Sum(item => item.Balance)
            });
        }

        [HttpGet("DoUpload")]
        public async Task<IActionResult> DoUpload() {

            //variables upi need to set
            var salesOrderId = 5;
            var fileName = HttpUtility.UrlEncode("file101.txt");
            var fileBytes = await System.IO.File.ReadAllBytesAsync(@"C:\Users\me\Documents\file.txt");
            var contentType = HttpUtility.UrlEncode("text/plain");
            var apiKey = "[U9CGdR(nJAWgx5t!U.R!Mku>(xj2f;THs_T6Ua|%dTH':uyQT*>_aH_@/#(sU";
            var apiUrl = "https://gideon-api.gidindustrial.com";

            //first get the right attachment type
            WebClient getTypeClient = new WebClient();
            getTypeClient.Headers.Add("API-Key", apiKey);
            var getTypeBody = await getTypeClient.DownloadStringTaskAsync($"{apiUrl}/SalesOrders/GetCustomerUploadType");
            int attachmentTypeId = Int32.Parse(getTypeBody);

            //now get upload link
            WebClient webClient = new WebClient();
            webClient.Headers.Add("API-Key", apiKey);
            webClient.QueryString.Add("fileSize", fileBytes.Length.ToString());
            webClient.QueryString.Add("attachmentTypeId", attachmentTypeId.ToString());
            webClient.QueryString.Add("fileName", fileName);
            webClient.QueryString.Add("contentType", contentType);
            webClient.QueryString.Add("salesOrderId", salesOrderId.ToString());
            string result = await webClient.DownloadStringTaskAsync($"{apiUrl}/Attachments/UploadAuthorization");
            webClient.Dispose();
            var AttachmentType = new { Id = 0, UploadUrl = "" };
            var newAttachment = JsonConvert.DeserializeAnonymousType(result, AttachmentType);

            //now upload file
            WebClient uploadClient = new WebClient();
            uploadClient.Headers.Add("x-ms-blob-type", "BlockBlob");
            uploadClient.Headers.Add("x-ms-blob-content-type", contentType);
            var uploadResult = await uploadClient.UploadDataTaskAsync(newAttachment.UploadUrl, HttpMethods.Put, fileBytes);
            uploadClient.Dispose();

            //now confirm that the upload succeeded
            WebClient confirmationClient = new WebClient();
            confirmationClient.Headers.Add("API-Key", apiKey);
            var url = "Attachments/ConfirmUploadSuccess/" + newAttachment.Id;
            string confirmationResult = await confirmationClient.DownloadStringTaskAsync($"{apiUrl}/Attachments/ConfirmUploadSuccess/" + newAttachment.Id);
            confirmationClient.Dispose();

            return Ok();
        }

        // GET: SalesOrders/Search
        [HttpGet("Search")]
        public async Task<IActionResult> SearchSalesOrders(
            [FromQuery] int? query,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var dbQuery = from salesOrder in _context.SalesOrders select salesOrder;

            dbQuery = dbQuery.Where(salesOrder => salesOrder.Id == query);

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

        // GET: SalesOrders/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrder([FromRoute] int id, [FromQuery] bool fetchRelated) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrder = await _context.SalesOrders
                .Include(m => m.Notes)
                    .ThenInclude(l => l.Note)
                .Include(m => m.BillingAddress)
                .Include(m => m.ShippingAddress)
                .Include(m => m.Quote)
                .Include(item => item.CurrencyOption)
                // .Include(item => item.OutgoingShipments)
                //     .ThenInclude(item => item.OutgoingShipment)
                //     .ThenInclude(item => item.Boxes)
                //.Include(q => q.Addresses)
                //    .ThenInclude(a => a.Address)
                // .Include(item => item.PurchaseOrders)
                //     .ThenInclude(item => item.PurchaseOrder)
                .Include(item => item.Repairs)
                .Include(q => q.Attachments)
                    .ThenInclude(q => q.Attachment)
                .Include(item => item.Notes)
                    .ThenInclude(item => item.Note)
                 // .Include(q => q.LineItems)
                 //     .ThenInclude(q => q.Product)
                 .Include(q => q.LineItems)
                    .ThenInclude(li => li.Sources)
                        .ThenInclude(item => item.Source)
                            .ThenInclude(item => item.Currency)
                .Include(q => q.LineItems)
                    .ThenInclude(li => li.Product)
                        .ThenInclude(j => j.Manufacturer)
                .Include(q => q.LineItems)
                    .ThenInclude(li => li.Product)
                        .ThenInclude(j => j.ProductType)
                .Include(q => q.EventLogEntries)
                    .ThenInclude(q => q.EventLogEntry)
                .SingleOrDefaultAsync(m => m.Id == id);


            if (salesOrder == null) {
                return NotFound();
            }

            // Fix line items
            foreach (SalesOrderLineItem li in salesOrder.LineItems)
            {
                if (li.Product == null)
                    li.Product = new Product();

                if (li.Product.ProductType == null)
                    li.Product.ProductType = new ProductType();

                if (li.Product.Manufacturer == null)
                    li.Product.Manufacturer = new Company();
            }

            return Ok(salesOrder);
        }

        [HttpGet("{id}/Repairs")]
        public async Task<IActionResult> Repairs([FromRoute] int id) {
            return Ok(await _context.Repairs.Where(item => item.SalesOrderId == id).ToListAsync());
        }

        // get purchase orders associated with a sales order
        [HttpGet("{id}/PurchaseOrders")]
        public async Task<IActionResult> GetPurchaseOrders([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrder = await _context.SalesOrders
                .Include(item => item.PurchaseOrders)
                    .ThenInclude(item => item.PurchaseOrder)
                        .ThenInclude(item => item.LineItems)
                            .ThenInclude(item => item.Product)
                .Include(item => item.PurchaseOrders)
                    .ThenInclude(item => item.PurchaseOrder)
                        .ThenInclude(item => item.LineItems)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (salesOrder == null) {
                return NotFound();
            }

            return Ok(salesOrder.PurchaseOrders);
        }

        [HttpGet("{id}/GetCreditCard")]
        [RequirePermission("ManageBilling")]
        public async Task<IActionResult> GetCreditCard(
            [FromRoute] int? id,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var salesOrder = await _context.SalesOrders
                .Include(item => item.CreditCard)
                .FirstOrDefaultAsync(item => item.Id == id);

            if (salesOrder.CreditCard == null) {
                return BadRequest("There is no credit card for this sales order");
            }

            var creditCard = new CreditCard {
                NameOnCard = Encryption.DecryptString(salesOrder.CreditCard.NameOnCardEncrypted),
                CardNumber = Encryption.DecryptString(salesOrder.CreditCard.CardNumberEncrypted),
                SecurityCode = Encryption.DecryptString(salesOrder.CreditCard.SecurityCodeEncrypted),
                ExpirationMonth = Encryption.DecryptString(salesOrder.CreditCard.ExpirationMonthEncrypted),
                ExpirationYear = Encryption.DecryptString(salesOrder.CreditCard.ExpirationYearEncrypted),
            };
            return Ok(creditCard);
        }

        /// <summary>
        /// This method generates a pdf of the salesOrders
        /// </summary>
        /// <param name="id">The id of the salesOrder</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateSalesOrderPdf")]
        public async Task GenerateSalesOrderPdf([FromRoute] int id) {
            var salesOrder = await _context.SalesOrders
                .Include(item => item.PaymentMethod)
                .Include(item => item.Company)
                .Include(item => item.PaymentTerm)
                .Include(item => item.OutgoingShipmentShippingTermOption)
                .Include(item => item.LineItems)
                    .ThenInclude(l => l.Product)
                        .ThenInclude(j => j.Manufacturer)
                .Include(item => item.LineItems)
                    .ThenInclude(l => l.Warranty)
                .Include(item => item.LineItems)
                    .ThenInclude(l => l.LineItemConditionType)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.LeadTime)
                .Include(c => c.Contact)
                .Include(item => item.PaymentMethod)
                .Include(item => item.BillingAddress)
                    .ThenInclude(item => item.Country)
                .Include(item => item.ShippingAddress)
                    .ThenInclude(item => item.Country)
                .Include(item => item.ShippingCarrier)
                .Include(item => item.ShippingCarrierShippingMethod)
                .Include(item => item.CurrencyOption)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.SalesPerson)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (salesOrder == null) {
                Response.StatusCode = 400;
                return;
            }
            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            salesOrder.Preparer = currentUser;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await salesOrder.CheckIfIsGidEurope(_context);
            ViewData["LogoUrlNew"] = Environment.ContentRootPath + @"\Features\Common\Views\logo-footer.png";
            ViewData["LogoUrlNew2"] = Environment.ContentRootPath + @"\Features\Common\Views\logo-footer2.png";
            ViewData["Font1"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Regular.ttf";
            ViewData["Font2"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Medium.ttf";
            ViewData["Font3"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-SemiBold.ttf";
            ViewData["Font4"] = Environment.ContentRootPath + @"\Features\Common\Views\Montserrat-Bold.ttf";

            // Render html
            string html = viewRenderer.Render("~/Features/SalesOrder/Views/SalesOrderPdfNew.cshtml", salesOrder, ViewData);

            // Create initial pdf
            HtmlToPdf converter = new HtmlToPdf();
            SelectPdf.PdfDocument pdfDocument = converter.ConvertHtmlString(html);

            // Remove last blank page
            if (pdfDocument.Pages.Count > 1 && pdfDocument.Pages[pdfDocument.Pages.Count - 1].ClientRectangle.Location.IsEmpty)
            {
               // pdfDocument.RemovePageAt(pdfDocument.Pages.Count - 1);
            }

            // Render t&c html
            string tcHtml = viewRenderer.Render("~/Features/Common/Views/TermsAndConditionsPdf.cshtml", salesOrder.GidLocationOption, ViewData);

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

            //string salesOrderHtml = viewRenderer.Render("~/Features/SalesOrder/Views/SalesOrderPdf.cshtml", salesOrder, ViewData);
            //var doc = new HtmlToPdfDocument() {
            //    GlobalSettings = {
            //        ColorMode = ColorMode.Color,
            //        Orientation = Orientation.Portrait,
            //        PaperSize = PaperKind.A4,
            //    },
            //    Objects = {
            //        new ObjectSettings() {
            //            PagesCount = true,
            //            HtmlContent = salesOrderHtml,
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

        // POST: SalesOrders
        [HttpPost("SendSalesOrder")]
        public async Task<IActionResult> SendSalesOrder([FromBody] SendSalesOrderData sendSalesOrderData) {
            var emailParameters = sendSalesOrderData.EmailParameters;
            var salesOrder = sendSalesOrderData.SalesOrder;

            var errorMessages = emailParameters.getErrorMessage();
            if (errorMessages != null) {
                return BadRequest(errorMessages);
            }

            salesOrder.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, emailParameters);

            var response = await client.SendEmailAsync(msg);

            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                _context.SalesOrderEventLogEntries.Add(new SalesOrderEventLogEntry {
                    SalesOrderId = (int)sendSalesOrderData.SalesOrder.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Sent Sales Order",
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

            var confirmedSalesOrderStatusOption = await _context.SalesOrderStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower().Contains("confirmed"));
            if (confirmedSalesOrderStatusOption != null) {
                salesOrder.SalesOrderStatusOptionId = confirmedSalesOrderStatusOption.Id;
            } else {
                return BadRequest(new {
                    Error = "Error finding sales order status option"
                });
            }

            salesOrder.SentAt = DateTime.UtcNow;
            salesOrder.CancelledAt = null;

            _context.Entry(salesOrder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        /// <summary>
        /// This method generates a pdf of the salesOrders
        /// </summary>
        /// <param name="id">The id of the salesOrder</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateCancelSalesOrderPdf")]
        public async Task GenerateCancelSalesOrderPdf([FromRoute] int id) {

            var salesOrder = await _context.SalesOrders
                .Include(item => item.PaymentMethod)
                .Include(item => item.PaymentTerm)
                .Include(q => q.LineItems)
                    .ThenInclude(l => l.Product)
                .Include(c => c.Contact)
                .Include(item => item.PaymentMethod)
                .Include(item => item.ShippingCarrier)
                .Include(item => item.ShippingCarrierShippingMethod)
                .Include(item => item.BillingAddress)
                    .ThenInclude(item => item.Country)
                .Include(item => item.ShippingAddress)
                    .ThenInclude(item => item.Country)
                .Include(item => item.SalesPerson)
                .Include(item => item.ShippingCarrier)
                .Include(item => item.GidLocationOption)
                    .ThenInclude(item => item.MainAddress)
                .Include(item => item.ShippingCarrierShippingMethod)
                .Include(item => item.CurrencyOption)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (salesOrder == null) {
                Response.StatusCode = 400;
                return;
            }

            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            salesOrder.Preparer = currentUser;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await salesOrder.CheckIfIsGidEurope(_context);
            string salesOrderHtml = viewRenderer.Render("~/Features/SalesOrder/Views/CancelSalesOrderPdf.cshtml", salesOrder, ViewData);
            var doc = new HtmlToPdfDocument() {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = salesOrderHtml,
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

        // POST: SalesOrders
        [HttpPost("SendCancelSalesOrder")]
        public async Task<IActionResult> SendCancelSalesOrder([FromBody] SendSalesOrderData sendSalesOrderData) {
            var emailParameters = sendSalesOrderData.EmailParameters;
            var salesOrder = sendSalesOrderData.SalesOrder;

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
                    SalesOrderId = (int)sendSalesOrderData.SalesOrder.Id,
                    EventLogEntry = new EventLogEntry {
                        Event = "Sent Cancel Sales Order",
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

            var cancelledSalesOrderStatusOption = await _context.SalesOrderStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower().Contains("cancelled"));
            if (cancelledSalesOrderStatusOption != null) {
                salesOrder.SalesOrderStatusOptionId = cancelledSalesOrderStatusOption.Id;
            } else {
                return BadRequest(new {
                    Error = "Error finding sales order status option"
                });
            }

            salesOrder.CancelledAt = DateTime.UtcNow;

            _context.Entry(salesOrder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(new {
                StatusCode = response.StatusCode,
                Body = response.Body
            });
        }

        // GET: SalesOrders/5/Company
        [HttpGet("{id}/GetCompany")]
        public async Task<IActionResult> GetSalesOrderCompany([FromRoute] int id) {
            var salesOrder = await _context.SalesOrders
                .Include(item => item.Company)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (salesOrder == null) {
                return NotFound(new {
                    Error = "The sales order was not found with that Id"
                });
            }
            return Ok(salesOrder.Company);
        }

        // GET: SalesOrders/5/LineItems
        [HttpGet("{id}/LineItems")]
        public async Task<IActionResult> GetSalesOrderLineItems([FromRoute] int id, [FromQuery] bool? includeInventoryItems) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = from so in _context.SalesOrders select so;

            query = query
                .Include(m => m.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(m => m.Company);

            if (includeInventoryItems == true) {
                query = query
                .Include(m => m.LineItems)
                    .ThenInclude(item => item.InventoryItems)
                        .ThenInclude(item => item.InventoryItem)
                            .ThenInclude(item => item.Product)
                .Include(m => m.LineItems)
                    .ThenInclude(item => item.Sources)
                        .ThenInclude(item => item.PurchaseOrder);
            }
            SalesOrder salesOrder = await query
                .FirstOrDefaultAsync(l => l.Id == id);

            return Ok(salesOrder.LineItems);
        }

        [HttpGet("{id}/ShippedLineItems")]
        public async Task<IActionResult> GetShippedSalesOrderLineItems([FromRoute] int Id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrder = await _context.SalesOrders.FirstOrDefaultAsync(item => item.Id == Id);
            if (salesOrder == null) {
                return NotFound(new {
                    Error = "A sales order was not found with that Id"
                });
            }

            var shippedLineItems = await salesOrder.GetShippedLineItems(_context);

            return Ok(shippedLineItems);

            // var query = from so in _context.SalesOrders select so;

            // query = query
            // .Include(item => item.LineItems)
            //     .ThenInclude(item => item.InventoryItems)
            //         .ThenInclude(item => item.InventoryItem)
            // .Include(item => item.LineItems)
            //     .ThenInclude(item => item.Product)
            //         .ThenInclude(item => item.Manufacturer)
            // .Include(item => item.LineItems)
            //     .ThenInclude(item => item.Sources);

            // var lineItems = salesOrder.LineItems;
            // var shippedStatus = await _context.InventoryItemStatusOptions.FirstOrDefaultAsync(item => item.Value == "Shipped");
            // if (shippedStatus == null) {
            //     return BadRequest(new {
            //         Error = "Unable to find the 'Shipped' inventory item status id."
            //     });
            // }
            // lineItems.ForEach(lineItem => {
            //     lineItem.Quantity = lineItem.InventoryItems.Count(lineItemInventoryItem => lineItemInventoryItem.InventoryItem.InventoryItemStatusOptionId == shippedStatus.Id);
            // });
            // lineItems = lineItems.Where(item => item.Quantity > 0).ToList();
            // return Ok(lineItems);
        }

        //When a shipment is marked as sent, need to update status to reflect if it is partialy sent, or if all shipments have been sents
        [HttpGet("{id}/RefreshStatus")]
        public async Task<IActionResult> RefreshStatus([FromRoute] int id) {
            SalesOrder salesOrder = await _context.SalesOrders
                .Include(item => item.LineItems)
                .Include(item => item.OutgoingShipments)
                    .ThenInclude(item => item.OutgoingShipment)
                        .ThenInclude(item => item.Boxes)
                            .ThenInclude(item => item.InventoryItems)
                .FirstOrDefaultAsync(item => item.Id == id);

            // to find total number of items to send, select the quantity from each line item, and then add them all together with the aggregate function
            int numberOfItemsToSend = salesOrder.LineItems
                .Select(item => item.Quantity ?? 0)
                .Sum();

            // to get the number of items that have been shipped, get all shipments that have already been shipped and count the number of inventory items in their boxes
            int numberOfInventoryItemsSent = salesOrder.OutgoingShipments
                .Where(item => item.OutgoingShipment.ShippedAt != null) //get shipments that have been shipped
                .SelectMany(item => item.OutgoingShipment.Boxes.Select(box => box.InventoryItems.Count)) //get the number of inventory items in the shipment's boxes 
                .Sum(); //sum the quantites

            if (numberOfItemsToSend == 0 || numberOfInventoryItemsSent == 0) {    //no shipment sent
                if (salesOrder.SentAt != null) {
                    var salesOrderStatusOption = await _context.SalesOrderStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower().Contains("confirmed"));
                    if (salesOrderStatusOption != null) {
                        salesOrder.SalesOrderStatusOptionId = salesOrderStatusOption.Id;
                    }
                }
            } else if (numberOfInventoryItemsSent != numberOfItemsToSend) {       //sent partially
                var salesOrderStatusOption = await _context.SalesOrderStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower().Contains("partially shipped"));
                if (salesOrderStatusOption != null) {
                    salesOrder.SalesOrderStatusOptionId = salesOrderStatusOption.Id;
                }
            } else if (numberOfInventoryItemsSent == numberOfItemsToSend) {       //fully sent
                var salesOrderStatusOption = await _context.SalesOrderStatusOptions.FirstOrDefaultAsync(item => item.Value.ToLower().Contains("shipment complete"));
                if (salesOrderStatusOption != null) {
                    salesOrder.SalesOrderStatusOptionId = salesOrderStatusOption.Id;
                }
            }

            _context.Entry(salesOrder).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(salesOrder);
        }

        // PUT: SalesOrders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrder([FromRoute] int id, [FromBody] SalesOrder salesOrder) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != salesOrder.Id) {
                return BadRequest();
            }

            var previousSalesOrder = await _context.SalesOrders.AsNoTracking().Include(item => item.SalesPerson).FirstOrDefaultAsync(item => item.Id == id);

            if (salesOrder.ShippingAddress != null && salesOrder.ShippingAddress.Id != null)
                _context.Entry(salesOrder.ShippingAddress).State = EntityState.Modified;
            else if (salesOrder.ShippingAddress != null)
                _context.Entry(salesOrder.ShippingAddress).State = EntityState.Added;

            if (salesOrder.BillingAddress != null && salesOrder.BillingAddress.Id != null)
                _context.Entry(salesOrder.BillingAddress).State = EntityState.Modified;
            else if (salesOrder.BillingAddress != null)
                _context.Entry(salesOrder.BillingAddress).State = EntityState.Added;

            _context.Entry(salesOrder).State = EntityState.Modified;

            var updatedEvent = "Updated";
            if (previousSalesOrder.SalesOrderStatusOptionId != salesOrder.SalesOrderStatusOptionId)
            {
                var status = await _context.SalesOrderStatusOptions.FirstOrDefaultAsync(item => item.Id == salesOrder.SalesOrderStatusOptionId);
                updatedEvent += " Status - " + (status != null ? status.Value : "None");
            }


            if (previousSalesOrder.SalesPersonId != salesOrder.SalesPersonId) {
                var previousSalesPersonName = previousSalesOrder.SalesPerson != null ? previousSalesOrder.SalesPerson.DisplayName : "";
                var currentSalesPersonName = "";
                if (salesOrder.SalesPersonId == null) {
                    currentSalesPersonName = "Nobody";
                } else {
                    var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == salesOrder.SalesPersonId);
                    if (user == null) {
                        currentSalesPersonName = "Nobody";
                    } else {
                        currentSalesPersonName = user.DisplayName;
                    }
                }
                updatedEvent += " " + $"Changed Sales Person from {previousSalesPersonName} to {currentSalesPersonName}";
            }

            var newEventLogEntry = new EventLogEntry
            {
                Event = updatedEvent,
                CreatedAt = DateTime.UtcNow,
                OccurredAt = DateTime.UtcNow,
                CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
            };
            _context.SalesOrderEventLogEntries.Add(new SalesOrderEventLogEntry
            {
                SalesOrder = salesOrder,
                EventLogEntry = newEventLogEntry
            });

            using (var transaction = _context.Database.BeginTransaction()) {
                await _context.SaveChangesAsync();
                await salesOrder.UpdateTotal(_context);
                await salesOrder.UpdateShippedProfit(_context);
                await salesOrder.UpdateBalance(_context);
                transaction.Commit();
            }


            return NoContent();
        }

        // POST: SalesOrders
        [HttpPost]
        public async Task<IActionResult> PostSalesOrder([FromBody] SalesOrder salesOrder) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            salesOrder.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            _context.SalesOrders.Add(salesOrder);

            //add to salesOrder event log entry the fact that it was created and who created it
            _context.SalesOrderEventLogEntries.Add(new SalesOrderEventLogEntry {
                SalesOrder = salesOrder,
                EventLogEntry = new EventLogEntry {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            //if this salesOrder is coming from a quote, add to quote event log entries table
            if (salesOrder.QuoteId != null && salesOrder.QuoteId != 0) {
                _context.QuoteEventLogEntries.Add(new QuoteEventLogEntry {
                    QuoteId = (int)salesOrder.QuoteId,
                    EventLogEntry = new EventLogEntry {
                        Event = "Converted to SalesOrder",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                //set quote status to Converted
                var quote = await _context.Quotes.FirstAsync(item => item.Id == salesOrder.QuoteId);
                var convertedStatus = await _context.QuoteStatusOptions.FirstAsync(item => item.Value == "Converted");
                quote.QuoteStatusOptionId = convertedStatus.Id;
                _context.Entry(quote).State = EntityState.Modified;
            }

            //if this salesOrder is coming from a lead, add to lead event log entries table
            if (salesOrder.LeadId != null && salesOrder.LeadId != 0) {
                _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                    LeadId = salesOrder.LeadId,
                    EventLogEntry = new EventLogEntry {
                        Event = "Converted to SalesOrder",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
            }

            await _context.SaveChangesAsync();

            salesOrder = await _context.SalesOrders
                .Include(item => item.Company)
                .Include(item => item.LineItems)
                .FirstOrDefaultAsync(item => item.Id == salesOrder.Id); ;

            if (!Environment.IsDevelopment())
                await salesOrder.SendNewSalesOrderNotificationEmail(_context, HttpContext);

            await salesOrder.UpdateTotal(_context);
            await salesOrder.UpdateBalance(_context);

            if (salesOrder.ExpediteFee != null && salesOrder.ExpediteFee > 0)
            {
                SalesOrderExpediteAlert(salesOrder);
            }

            return CreatedAtAction("GetSalesOrder", new { id = salesOrder.Id }, salesOrder);
        }

        /// <summary>
        /// Sends expedite chat alert
        /// </summary>
        /// <param name="salesOrder"></param>
        private async void SalesOrderExpediteAlert(SalesOrder salesOrder)
        {
            var salesPerson = await _context.Users.FirstAsync(item => item.Id == salesOrder.SalesPersonId);
            var madeline = await _context.Users.FirstAsync(item => item.DisplayName == "Madeline Devine");
            var emma = await _context.Users.FirstAsync(item => item.DisplayName == "Emma Chang");

            await chatMessagesController.PostChatMessage(new ChatMessage
            {
                SalesOrders = new List<SalesOrderChatMessage> { new SalesOrderChatMessage { SalesOrderId = salesOrder.Id } },
                Message = $"@[{salesPerson.FirstName} {salesPerson.LastName}] @[{madeline.FirstName} {madeline.LastName}] @[{emma.FirstName} {emma.LastName}], EXPEDITE order entered. Please proceed.",
                CreatedById = 2
            });
        }


        // POST: SalesOrders/FromQuoteForm
        [HttpPost("FromQuoteForm")]
        public async Task<IActionResult> PostSalesOrderFromQuoteForm([FromBody] QuoteFormSalesOrderSubmission quoteFormSalesOrderSubmission) {

            var (salesOrder, paymentProfileId) = await SalesOrder.CreateSalesOrderFromQuoteFormSubmission(_context, quoteFormSalesOrderSubmission);


            salesOrder.CreatedById = null;
            _context.SalesOrders.Add(salesOrder);

            //add to salesOrder event log entry the fact that it was created and who created it
            _context.SalesOrderEventLogEntries.Add(new SalesOrderEventLogEntry {
                SalesOrder = salesOrder,
                EventLogEntry = new EventLogEntry {
                    Event = "Created From Online Quote Form",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            //if this salesOrder is coming from a lead, add to lead event log entries table
            if (salesOrder.LeadId != null && salesOrder.LeadId != 0) {
                _context.LeadEventLogEntries.Add(new LeadEventLogEntry {
                    LeadId = salesOrder.LeadId,
                    EventLogEntry = new EventLogEntry {
                        Event = "Converted to Sales Order",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
            }
            //if this salesOrder is coming from a quote, add to quote event log entries table
            if (salesOrder.QuoteId != null && salesOrder.QuoteId != 0) {
                _context.QuoteEventLogEntries.Add(new QuoteEventLogEntry {
                    QuoteId = (int)salesOrder.QuoteId,
                    EventLogEntry = new EventLogEntry {
                        Event = "Converted to Sales Order",
                        CreatedAt = DateTime.UtcNow,
                        OccurredAt = DateTime.UtcNow,
                        CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                        UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                    }
                });
                //set quote status to Converted
                var quote = await _context.Quotes.FirstOrDefaultAsync(item => item.Id == salesOrder.QuoteId);
                if (quote != null) {
                    var convertedStatus = await _context.QuoteStatusOptions.FirstOrDefaultAsync(item => item.Value == "Converted");
                    if (convertedStatus != null) {
                        quote.QuoteStatusOptionId = convertedStatus.Id;
                        _context.Entry(quote).State = EntityState.Modified;
                    }
                }
            }

            var wireTransferFeeOption = await _context.SalesOrderPaymentMethods.FirstOrDefaultAsync(item => item.Value == "Wire Transfer");
            if (wireTransferFeeOption != null && salesOrder.CurrencyOptionId != null) {
                if (salesOrder.SalesOrderPaymentMethodId == wireTransferFeeOption.Id) {
                    var currency = await _context.CurrencyOptions.FirstOrDefaultAsync(item => item.Id == salesOrder.CurrencyOptionId);
                    if (currency != null) {
                        salesOrder.WireTransferFee = currency.WireTransferFee;
                    }
                }
            }

            var creditCardFeeOption = await _context.SalesOrderPaymentMethods.FirstOrDefaultAsync(item => item.Value == "Wire Transfer");
            if (creditCardFeeOption != null && salesOrder.CurrencyOptionId != null)
            {
                if (salesOrder.SalesOrderPaymentMethodId == creditCardFeeOption.Id)
                {
                    var currency = await _context.CurrencyOptions.FirstOrDefaultAsync(item => item.Id == salesOrder.CurrencyOptionId);
                    if (currency != null)
                    {
                        salesOrder.CreditCardFee = currency.CreditCardFee;
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                string t = "";
            }


            salesOrder = await _context.SalesOrders
                .Include(item => item.Company)
                .Include(item => item.LineItems)
                .FirstOrDefaultAsync(item => item.Id == salesOrder.Id);

            await salesOrder.UpdateTotal(_context);
            await _context.SaveChangesAsync();


            if (!Environment.IsDevelopment())
                await salesOrder.SendNewSalesOrderNotificationEmail(_context, HttpContext);

            var creditCardMethod = await _context.SalesOrderPaymentMethods.FirstOrDefaultAsync(val => val.Value == "Credit Card");
            if (!String.IsNullOrWhiteSpace(paymentProfileId))
            {
                // charge credit card
                var authorizationSucceeded = false;
                try
                {
                    var response = await creditCardTransactionsController.PostCreditCardTransaction(new CreditCardTransaction
                    {
                        Amount = Decimal.Round(salesOrder.Total, 2),
                        CreditCardTransactionType = CreditCardTransactionType.Authorization,
                        CompanyId = salesOrder.CompanyId,
                        PaymentProfileId = paymentProfileId,
                        SalesOrderId = salesOrder.Id
                    });
                    if (response.GetType() == typeof(CreatedAtActionResult))
                    {
                        if(((response as CreatedAtActionResult).Value as CreditCardTransaction).Succeeded){
                            authorizationSucceeded = true;
                        }
                    }
                } catch (Exception ex)
                {

                }
                try
                {
                    if (!authorizationSucceeded)
                    {
                        var salesPerson = await _context.Users.FirstAsync(item => item.Id == salesOrder.SalesPersonId);
                        await chatMessagesController.PostChatMessage(new ChatMessage
                        {
                            SalesOrders = new List<SalesOrderChatMessage> { new SalesOrderChatMessage { SalesOrderId = salesOrder.Id } },
                            Message = $"@[{salesPerson.FirstName} {salesPerson.LastName}], Credit card has failed auto authorization.  Please contact customer.",
                            CreatedById = 47
                        });
                    }
                    else
                    {
                        var salesPerson = await _context.Users.FirstAsync(item => item.Id == salesOrder.SalesPersonId);;
                        var madeline = await _context.Users.FirstAsync(item => item.DisplayName == "Madeline Devine");
                        var emma = await _context.Users.FirstAsync(item => item.DisplayName == "Emma Chang");
                        await chatMessagesController.PostChatMessage(new ChatMessage
                        {
                            SalesOrders = new List<SalesOrderChatMessage> { new SalesOrderChatMessage { SalesOrderId = salesOrder.Id } },
                            Message = $"@[{salesPerson.FirstName} {salesPerson.LastName}] @[{madeline.FirstName} {madeline.LastName}] @[{emma.FirstName} {emma.LastName}], credit card auto-authorized successfully!  Good to go.",
                            CreatedById = 2
                        });

                        if (salesOrder.ExpediteFee != null && salesOrder.ExpediteFee > 0)
                        {
                            await chatMessagesController.PostChatMessage(new ChatMessage
                            {
                                SalesOrders = new List<SalesOrderChatMessage> { new SalesOrderChatMessage { SalesOrderId = salesOrder.Id } },
                                Message = $"@[{salesPerson.FirstName} {salesPerson.LastName}] @[{madeline.FirstName} {madeline.LastName}] @[{emma.FirstName} {emma.LastName}], EXPEDITE order entered. Please proceed.",
                                CreatedById = 2
                            });
                        }
                    }
                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return CreatedAtAction("GetSalesOrder", new { id = salesOrder.Id }, salesOrder);
        }

        // DELETE: SalesOrders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrder([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrder = await _context.SalesOrders
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.InventoryItems)
                        .ThenInclude(item => item.InventoryItem)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Sources)
                        .ThenInclude(item => item.Source)
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (salesOrder == null) {
                return NotFound();
            }

            if (salesOrder.SentAt != null) {
                return BadRequest(new {
                    Error = "You can't delete a sales order once it has been sent"
                });
            }

            foreach (var itemAttachment in salesOrder.Attachments.ToList()) {
                await itemAttachment.Attachment.Delete(_context, _configuration);
            }

            _context.SalesOrders.Remove(salesOrder);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(salesOrder);
        }

        private bool SalesOrderExists(int id) {
            return _context.SalesOrders.Any(e => e.Id == id);
        }
    }
}