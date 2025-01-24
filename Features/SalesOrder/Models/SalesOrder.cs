using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Ganss.XSS;
using GidIndustrial.Gideon.WebApi.Libraries;
using LeadAutomation.Firefly.Exchange.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;
using QuickBooks.Models;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class SalesOrder {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public User SalesPerson { get; set; }
        public int? SalesPersonId { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }
        public int? ContactId { get; set; }
        public Contact Contact { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

        public int? SalesOrderPaymentMethodId { get; set; }
        public SalesOrderPaymentMethod PaymentMethod { get; set; }

        public int? ShippingMethodId { get; set; }

        [NotMapped]
        public User Preparer { get; set; }

        public int? SalesOrderStatusOptionId { get; set; }
        public int? QuoteId { get; set; }
        public Quote Quote { get; set; }
        public int? LeadId { get; set; }
        public int? LeadOriginOptionId { get; set; }

        public int? GidLocationOptionId { get; set; }
        public GidLocationOption GidLocationOption { get; set; }

        public int? CustomerTypeId { get; set; }

        [Required]
        public int? CurrencyOptionId { get; set; }
        public CurrencyOption CurrencyOption { get; set; }

        public string CustomerNotes { get; set; }

        public Address ShippingAddress { get; set; }
        public int? ShippingAddressId { get; set; }
        public Address BillingAddress { get; set; }
        public int? BillingAddressId { get; set; }

        public decimal? SalesTax { get; set; }
        public decimal? WireTransferFee { get; set; }
        public decimal? ShippingAndHandlingFee { get; set; }

        public string ShippingAndHandlingDisplay
        {
            get
            {
                return ((ShippingAndHandlingFee.HasValue && ShippingAndHandlingFee.Value > 0) 
                    || (ShippingAndHandlingFee.HasValue && ShippingAndHandlingFee.Value == 0 && (!String.IsNullOrEmpty(FreightAccountNumber) && FreightAccountNumber != "(PP&A)"))) ? 
                    (CurrencyOption != null ? CurrencyOption.Symbol : String.Empty) + ShippingAndHandlingFee.Value.ToString("N2") : "TBD";
            }
        }


        public decimal? ExpediteFee { get; set; }

        public string CustomerPurchaseOrderNumber { get; set; }
        public string CustomerVatNumber { get; set; }

        public DateTime? SentAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public CreditCard CreditCard { get; set; }

        public string PaypalEmailAddress { get; set; }

        public ShippingCarrier ShippingCarrier { get; set; }
        public int? ShippingCarrierId { get; set; }

        public ShippingCarrierShippingMethod ShippingCarrierShippingMethod { get; set; }
        public int? ShippingCarrierShippingMethodId { get; set; }
        public decimal Balance { get; set; }
        public decimal? ShippedProfit { get; set; }
        public decimal Total { get; set; }
        public decimal? ProjectedProfit { get; set; }
        public decimal? CreditCardFee { get; set; }
        public List<Repair> Repairs { get; set; }

        public int? ShippingTypeId { get; set; }
        public string ShippingAccountNumber { get; set; }
        public bool PartialShipAccepted { get; set; }
        public bool SaturdayDeliveryAccepted { get; set; }
        public string InternalReferenceNumber { get; set; }
        public bool? DropShipment { get; set; }

        public string FreightAccountNumber { get; set; }
        public int? OutgoingShipmentShippingTermOptionId { get; set; }
        public OutgoingShipmentShippingTermOption OutgoingShipmentShippingTermOption { get; set; }

        public bool? IsEbayOrder { get; set; }

        public List<SalesOrderPurchaseOrder> PurchaseOrders { get; set; }
        public List<SalesOrderLineItem> LineItems { get; set; }
        public List<SalesOrderOutgoingShipment> OutgoingShipments { get; set; }
        public List<SalesOrderInventoryItem> InventoryItems { get; set; }
        public List<SalesOrderNote> Notes { get; set; }
        public List<SalesOrderAttachment> Attachments { get; set; }
        public List<SalesOrderEventLogEntry> EventLogEntries { get; set; }
        public string BiosRequirements { get; set; }
        public List<SalesOrderChatMessage> ChatMessages { get; set; }
        public List<Invoice> Invoices { get; set; }
        public bool ReadyToPurchase { get; set; }
        public bool? PopulateAtGidDiscretion { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; }
        public int? PaymentTermId { get; set; }
        public PaymentTerm PaymentTerm { get; set; }
        public int? PreviousSalesOrderId { get; set; }
        public bool? Funded { get; set; }

        public List<CreditCardTransaction> CreditCardTransactions { get; set; }

        public List<SalesOrderToDoItem> ToDoItems { get; set; }

        public string PhoneNumberFormatted
        {
            get
            {
                if (!String.IsNullOrWhiteSpace(Phone))
                {
                    var number = Phone;
                    number = Regex.Replace(number, "/[^\\d]/g", "");
                    number = number.Replace("-", "").Replace("+", "").Replace(" ", "");

                    if (number.Length < 3)
                        return number;
                    else if (number.Length < 7)
                        return "(" + number.Substring(0, 3) + ") " + number.Substring(3);
                    else if (number.Length < 11)
                        return "(" + number.Substring(0, 3) + ") " + number.Substring(3, 3) + "-" + number.Substring(6);
                    else
                    {
                        var mainNumber = number.Substring(number.Length - 10); //get last 10 digits
                        var countryCode = number.Substring(0, number.Length - 10);
                        return "+" + countryCode + " (" + mainNumber.Substring(0, 3) + ") " + mainNumber.Substring(3, 3) + "-" + mainNumber.Substring(6);
                    }
                }

                return String.Empty;
            }
        }

        public static async Task SetBalanceForAllSalesOrders(AppDBContext dbContext, IServiceCollection services) {
            var dateTime = new DateTime(2020, 1, 1);
            var salesOrders = await dbContext.SalesOrders
                .Include(item => item.LineItems)
                .AsNoTracking()
                .Where(item => item.CreatedAt > dateTime)
                .ToListAsync();

            foreach (var salesOrder in salesOrders) {
                await salesOrder.UpdateTotal(dbContext);
                Console.WriteLine("Updated balance " + salesOrder.Id);
            }

            await dbContext.SaveChangesAsync();

            // var i = 0;
            // using (var scope = services.BuildServiceProvider().GetRequiredService<IServiceScopeFactory>().CreateScope()) {
            //     var provider = scope.ServiceProvider;
            //     var tasks = salesOrders.Select((item) => {
            //         return Task.Run(async () => {
            //             Console.WriteLine("Starting #" + ++i);
            //             using (var newDBContext = provider.GetRequiredService<AppDBContext>()) {
            //                 Console.WriteLine("getting salesOrder " + i);
            //                 var salesOrder = await dbContext.SalesOrders
            //                     .Include(thing => thing.LineItems)
            //                     .FirstOrDefaultAsync(so => so.Id == item.Id);
            //                 Console.WriteLine("got sales order");
            //                 salesOrder.Total = salesOrder.GetTotal();
            //                 await salesOrder.UpdateShippedProfit(dbContext);
            //                 Console.WriteLine("Updated shipped profit " + salesOrder.Id);
            //             }
            //         });
            //     });
            //     var arr = tasks.ToArray();
            //     Console.WriteLine("JJJJJJJJJJJJJJJJJJJJJJJJJJJJ");
            //     Task.WaitAll(arr);
            // }
            // Console.WriteLine("Done");

            // foreach (var salesOrder in salesOrders) {
            //     salesOrder.Total = salesOrder.GetTotal();
            //     await salesOrder.UpdateShippedProfit(dbContext);
            // }
            await dbContext.SaveChangesAsync();
        }

        public decimal? GetLineItemsTotalCost() {
            var values = this.LineItems.Select(item => (item.Quantity ?? 0) * (item.Price ?? 0));
            var sum = values.Sum();
            return sum;
        }
        public decimal? GetLineItemsTotalCostDB(AppDBContext _context) {
            var values = _context.SalesOrderLineItems
                .Where(item => item.SalesOrderId == this.Id)
                .Select(item => (item.Quantity ?? 0) * (item.Price ?? 0));
            var sum = values.Sum();
            return sum;
        }

        public async Task UpdateBalance(AppDBContext context) {
            var total = this.GetTotal();
            var cashReceiptsSum = await context.CashReceipts.Where(item => item.SalesOrderId == this.Id).SumAsync(item => item.Amount);
            var cashDisbursementsSum = await context.CashDisbursements.Where(item => item.SalesOrderId == this.Id).SumAsync(item => item.Amount);
            this.Balance = total - cashReceiptsSum + cashDisbursementsSum;
            context.Entry(this).State = EntityState.Modified;
            await context.SaveChangesAsync();
        }

        public decimal? GetLineItemsCommercialInvoiceTotalCost() {
            var values = this.LineItems.Select(item => {
                if (item.DeclaredValue != null && item.DeclaredValue != 0) {
                    return (item.Quantity ?? 0) * item.DeclaredValue;
                }
                return (item.Quantity ?? 0) * (item.Price ?? 0);
            });
            var sum = values.Sum();
            return sum;
        }

        public decimal GetTotal(AppDBContext context = null) {
            decimal total;
            if (context != null)
                total = (decimal)this.GetLineItemsTotalCostDB(context);
            else
                total = (decimal)this.GetLineItemsTotalCost();
            total -= this.GetDiscountAmount();
            total += this.SalesTax ?? 0;
            total += this.WireTransferFee ?? 0;
            total += this.ShippingAndHandlingFee ?? 0;
            total += this.ExpediteFee ?? 0;
            total += this.CreditCardFee ?? 0;
            return Math.Round(total, 2);
        }

        public decimal GetDiscountAmount() {
            if (this.DiscountAmount != 0) {
                if (this.DiscountType == DiscountTypes.PERCENT) {
                    return (this.GetLineItemsTotalCost().Value * this.DiscountAmount / 100);
                } else if (this.DiscountType == DiscountTypes.FIXED_AMOUNT) {
                    return this.DiscountAmount;
                } else {
                    return 0;
                }
            } else {
                return 0;
            }
        }

        // public decimal GetDiscountedTotal() {
        //     var total = (decimal)this.GetLineItemsTotalCost();

        //     total += this.SalesTax ?? 0;
        //     total += this.WireTransferFee ?? 0;
        //     total += this.ShippingAndHandlingFee ?? 0;
        //     total += this.ExpediteFee ?? 0;
        //     return total;
        // }

        public async Task<List<SalesOrderLineItem>> GetShippedLineItems(AppDBContext dbContext) {
            var salesOrder = await dbContext.SalesOrders
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.InventoryItems)
                        .ThenInclude(item => item.InventoryItem)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                        .ThenInclude(item => item.Manufacturer)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Sources)
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == this.Id);

            var lineItems = salesOrder.LineItems;
            var shippedStatus = await dbContext.InventoryItemStatusOptions.FirstOrDefaultAsync(item => item.Value == "Shipped");
            if (shippedStatus == null) {
                throw new Exception("Unable to find the 'Shipped' inventory item status id.");
            }
            lineItems.ForEach(lineItem => {
                lineItem.Quantity = lineItem.InventoryItems.Count(lineItemInventoryItem => lineItemInventoryItem.InventoryItem.InventoryItemStatusOptionId == shippedStatus.Id);
            });
            lineItems = lineItems.Where(item => item.Quantity > 0).ToList();
            return lineItems;
        }

        public async Task UpdateTotal(AppDBContext _context) {
            this.Total = this.GetTotal(_context);
            _context.Entry(this).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            _context.Entry(this).State = EntityState.Detached;
        }

        public async Task UpdateShippedProfit(AppDBContext _context) {
            //get outgingshiments
            var salesOrderWithOutgoingShipments = await _context.SalesOrders.AsNoTracking()
                .Include(item => item.OutgoingShipments)
                    .ThenInclude(item => item.OutgoingShipment)
                        .ThenInclude(item => item.Boxes)
                            .ThenInclude(item => item.InventoryItems)
                                .ThenInclude(item => item.InventoryItem)
                .FirstOrDefaultAsync(item => item.Id == this.Id);

            var salesOrderLineItems = (await _context.SalesOrders.AsNoTracking()
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.InventoryItems)
                .Where(item => item.Id == this.Id)
                .ToListAsync())
                .SelectMany(item => item.LineItems);

            var salesOrderLineItemInventoryItems = salesOrderLineItems
                .SelectMany(item => item.InventoryItems);

            var shippedShipments = salesOrderWithOutgoingShipments.OutgoingShipments.Where(item => item.OutgoingShipment.ShippedAt != null).Select(item => item.OutgoingShipment);

            decimal totalCost = 0;
            decimal totalRevenue = 0;
            var shippedInventoryItems = new List<InventoryItem> { };

            foreach (var shippedShipment in shippedShipments) {
                foreach (var box in shippedShipment.Boxes) {
                    foreach (var outgoingShipmentInventoryItem in box.InventoryItems) {
                        if (outgoingShipmentInventoryItem.InventoryItem.TotalCost == 0 || outgoingShipmentInventoryItem.InventoryItem.TotalCost == null) {
                            this.ShippedProfit = 0;
                            return;
                        }
                        totalCost += (decimal)outgoingShipmentInventoryItem.InventoryItem.TotalCost;
                        var matchingSalesOrderLineItemInventoryItem = salesOrderLineItemInventoryItems
                            .First(item => item.InventoryItemId == outgoingShipmentInventoryItem.InventoryItemId);
                        var matchingSalesOrderLineItem = salesOrderLineItems.First(item => item.Id == matchingSalesOrderLineItemInventoryItem.SalesOrderLineItemId);
                        if (matchingSalesOrderLineItem.Price == 0 || matchingSalesOrderLineItem.Price == null) {
                            this.ShippedProfit = 0;
                            return;
                        }
                        totalRevenue += (decimal)matchingSalesOrderLineItem.Price;
                        //shippedInventoryItems.Add(outgoingShipmentInventoryItem.InventoryItem);
                    }
                }
            }
            _context.Entry(this).State = EntityState.Detached;
            var salesOrder = await _context.SalesOrders.AsNoTracking().FirstOrDefaultAsync(item => item.Id == this.Id);
            salesOrder.ShippedProfit = totalRevenue - totalCost;
            await _context.SaveChangesAsync();
            _context.Entry(salesOrder).State = EntityState.Detached;
        }


        public async Task SendNewSalesOrderNotificationEmail(AppDBContext _context, Microsoft.AspNetCore.Http.HttpContext HttpContext) {
            //send email to saleperson, support and executive when a new sales order is created
            var to = "executive@gidindustrial.com,support@gidindustrial.com,orderstatus@gidindustrial.com";
            // to = "dane@turbobuilt.com";
            if (GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User) != this.SalesPersonId) {
                var salesPerson = await _context.Users.FirstOrDefaultAsync(item => item.Id == this.SalesPersonId);
                if (salesPerson != null) {
                    to += "," + salesPerson.Email;
                }
            }

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, new EmailGeneratorParameters {
                To = to,
                From = "leads@gidindustrial.com",
                Subject = "New Sales Order - ID # " + this.Id.ToString(),
                HtmlContent = $@"
<p>A new Sales Order was created: <a href='https://gideon.gidindustrial.com/sales-orders/{this.Id}'>Link</a></p>
<p>Company Name: {(this.Company != null ? this.Company.Name : "")}</p>
<p>Dollar Amount: ${(this.LineItems != null ? this.LineItems.Select(item => item.Quantity * item.Price).Sum().ToString() : "")}</p>"
            });
            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber >= 200 && responseStatusCodeNumber < 300) {
                Console.WriteLine("ERROR SENDING EMAIL THAT LEAD was assigned");
            }
        }

        public async Task<bool> CheckIfIsGidEurope(AppDBContext _context) {
            var GidEuropeOption = await _context.GidLocationOptions.FirstOrDefaultAsync(item => item.Value.Contains("Europe"));
            if (GidEuropeOption == null) {
                throw new Exception("Unable to find GID Europe option");
            }
            return this.GidLocationOptionId == GidEuropeOption.Id;
        }
        public static async Task<(SalesOrder, string)> CreateSalesOrderFromQuoteFormSubmission(AppDBContext context, QuoteFormSalesOrderSubmission quoteFormSalesOrderSubmission) {
            var quote = await context.Quotes.AsNoTracking()

                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Sources)
                .FirstOrDefaultAsync(item => item.Id == quoteFormSalesOrderSubmission.QuoteId);
            if (quote == null) {
                throw new Exception("Quote Id not found");
            }

            List<SalesOrderLineItem> salesOrderLineItems = quote.LineItems.Select(quoteLineItem => new SalesOrderLineItem {
                ProductId = quoteLineItem.ProductId,
                CreatedAt = DateTime.UtcNow,
                Description = quoteLineItem.Description,
                ProductName = quoteLineItem.ProductName,
                ManufacturerName = quoteLineItem.ManufacturerName,
                ManufacturerId = quoteLineItem.ManufacturerId,
                LineItemConditionTypeId = quoteLineItem.LineItemConditionTypeId,
                LineItemServiceTypeId = quoteLineItem.LineItemServiceTypeId,
                Quantity = quoteLineItem.Quantity,
                Price = quoteLineItem.Price,
                DiscountPercent = quoteLineItem.DiscountPercent,
                OutgoingLineItemWarrantyOptionId = quoteLineItem.OutgoingLineItemWarrantyOptionId,
                OutgoingLineItemLeadTimeOptionId = quoteLineItem.OutgoingLineItemLeadTimeOptionId,
                Sources = quoteLineItem.Sources.Select(quoteLineItemSource => new SalesOrderLineItemSource {
                    SourceId = quoteLineItemSource.SourceId
                }).ToList()
            }).ToList();

            var newSalesOrder = new SalesOrder();
            newSalesOrder.ContactId = quote.ContactId;
            newSalesOrder.CompanyId = quote.CompanyId;
            newSalesOrder.QuoteId = quote.Id;
            newSalesOrder.Phone = quote.Phone;
            newSalesOrder.Email = quote.Email;
            newSalesOrder.SalesPersonId = quote.SalesPersonId;
            newSalesOrder.LineItems = salesOrderLineItems;
            newSalesOrder.CustomerTypeId = quote.CustomerTypeId;
            newSalesOrder.SalesTax = quote.SalesTax;
            newSalesOrder.CreditCardFee = quote.CreditCardFee;
            newSalesOrder.WireTransferFee = quote.WireTransferFee;
            newSalesOrder.ShippingAndHandlingFee = quote.ShippingAndHandlingFee;
            newSalesOrder.ExpediteFee = quote.ExpediteFee;
            newSalesOrder.CurrencyOptionId = quote.CurrencyOptionId;
            newSalesOrder.GidLocationOptionId = quote.GidLocationOptionId;
            newSalesOrder.SalesOrderPaymentMethodId = quote.SalesOrderPaymentMethodId;
            newSalesOrder.PaymentTermId = quote.PaymentTermId;
            newSalesOrder.SalesOrderStatusOptionId = 1;

            newSalesOrder.BillingAddress = new Address {
                Name = quoteFormSalesOrderSubmission.BillingName,
                Attention = quoteFormSalesOrderSubmission.BillingAttention,
                Address1 = quoteFormSalesOrderSubmission.BillingAddress1,
                Address2 = quoteFormSalesOrderSubmission.BillingAddress2,
                Address3 = quoteFormSalesOrderSubmission.BillingAddress3,
                City = quoteFormSalesOrderSubmission.BillingCity,
                State = quoteFormSalesOrderSubmission.BillingState,
                ZipPostalCode = quoteFormSalesOrderSubmission.BillingPostalCode,
                CountryId = await Address.GetCountryIdFromCode(context, quoteFormSalesOrderSubmission.BillingCountryCode)
            };
            newSalesOrder.ShippingAddress = new Address {
                Name = quoteFormSalesOrderSubmission.ShippingName,
                Attention = quoteFormSalesOrderSubmission.ShippingAttention,
                Address1 = quoteFormSalesOrderSubmission.ShippingAddress1,
                Address2 = quoteFormSalesOrderSubmission.ShippingAddress2,
                Address3 = quoteFormSalesOrderSubmission.ShippingAddress3,
                City = quoteFormSalesOrderSubmission.ShippingCity,
                State = quoteFormSalesOrderSubmission.ShippingState,
                ZipPostalCode = quoteFormSalesOrderSubmission.ShippingPostalCode,
                CountryId = await Address.GetCountryIdFromCode(context, quoteFormSalesOrderSubmission.ShippingCountryCode)
            };
            newSalesOrder.ShippingCarrierId = quoteFormSalesOrderSubmission.ShippingCarrierId;
            newSalesOrder.ShippingCarrierShippingMethodId = quoteFormSalesOrderSubmission.ShippingCarrierShippingMethodId;
            newSalesOrder.ShippingTypeId = quoteFormSalesOrderSubmission.ShippingTypeId;
            newSalesOrder.FreightAccountNumber = quoteFormSalesOrderSubmission.ShippingAccountNumber;
            newSalesOrder.PartialShipAccepted = quoteFormSalesOrderSubmission.PartialShipAccepted;
            newSalesOrder.SaturdayDeliveryAccepted = quoteFormSalesOrderSubmission.SaturdayDeliveryAccepted;
            newSalesOrder.PaypalEmailAddress = quoteFormSalesOrderSubmission.PaypalEmail;
            newSalesOrder.SalesOrderPaymentMethodId = quoteFormSalesOrderSubmission.SalesOrderPaymentMethodId;
            newSalesOrder.PaymentTermId = quoteFormSalesOrderSubmission.PaymentTermId;

            newSalesOrder.InternalReferenceNumber = quoteFormSalesOrderSubmission.InternalReferenceNumber;
            newSalesOrder.CustomerPurchaseOrderNumber = quoteFormSalesOrderSubmission.InternalReferenceNumber;
            if (String.IsNullOrWhiteSpace(newSalesOrder.CustomerPurchaseOrderNumber))
                newSalesOrder.CustomerPurchaseOrderNumber = "Online";

            string paymentProfileId = null;

            if (newSalesOrder.CompanyId != null && !String.IsNullOrWhiteSpace(quoteFormSalesOrderSubmission.CreditCardNumber)) {
                //first check if they have a profile
                var company = await context.Companies.FirstOrDefaultAsync(item => item.Id == newSalesOrder.CompanyId);
                if (company != null) {
                    try {
                        if (quoteFormSalesOrderSubmission.CreditCardExpirationMonth.Length == 1)
                            quoteFormSalesOrderSubmission.CreditCardExpirationMonth = "0" + quoteFormSalesOrderSubmission.CreditCardExpirationMonth;
                        if (company.AuthorizeNetProfileId == null) {
                            var response =await company.CreateAuthorizeNetProfile(context, null, new Libraries.AuthorizeNet.CreateProfileRequest.CreditCard {
                                cardNumber = quoteFormSalesOrderSubmission.CreditCardNumber,
                                expirationDate = quoteFormSalesOrderSubmission.CreditCardExpirationYear + "-" + quoteFormSalesOrderSubmission.CreditCardExpirationMonth,
                                cardCode = quoteFormSalesOrderSubmission.CreditCardSecurityCode,
                            });
                            paymentProfileId = response.customerPaymentProfileIdList.First();
                        } else {
                            var response = await company.AddPaymentMethod(context, null, new Libraries.AuthorizeNet.CreatePaymentProfileRequest.CreditCard {
                                cardNumber = quoteFormSalesOrderSubmission.CreditCardNumber,
                                expirationDate = quoteFormSalesOrderSubmission.CreditCardExpirationYear + "-" + quoteFormSalesOrderSubmission.CreditCardExpirationMonth,
                                cardCode = quoteFormSalesOrderSubmission.CreditCardSecurityCode,
                            });
                            paymentProfileId = response.customerPaymentProfileId;
                        }

                        if (paymentProfileId != null)
                        {
                            context.PaymentProfileCodes.Add(new PaymentProfileCode() { CardCode = Encryption.EncryptData(quoteFormSalesOrderSubmission.CreditCardSecurityCode), PaymentProfileId = paymentProfileId });
                            await context.SaveChangesAsync();
                        }

                    } catch (Exception ex) {
                        System.Diagnostics.Trace.TraceError("Error creating authorize net profile from quote form " + ex.Message);
                        Console.WriteLine("Error creating profile with authorize.net  +++++++++++++++++++++++++++++++++");
                        Console.WriteLine(ex.Message);

                        var nameOnCardEncrypted = Encryption.EncryptData(quoteFormSalesOrderSubmission.CreditCardName);
                        var cardNumberEncrypted = Encryption.EncryptData(quoteFormSalesOrderSubmission.CreditCardNumber);
                        var securityCodeEncrypted = Encryption.EncryptData(quoteFormSalesOrderSubmission.CreditCardSecurityCode);
                        var expirationMonthEncrypted = Encryption.EncryptData(quoteFormSalesOrderSubmission.CreditCardExpirationMonth);
                        var exiprationYearEncrypted = Encryption.EncryptData(quoteFormSalesOrderSubmission.CreditCardExpirationYear);

                        newSalesOrder.CreditCard = new CreditCard {
                            NameOnCardEncrypted = nameOnCardEncrypted,
                            CardNumberEncrypted = cardNumberEncrypted,
                            SecurityCodeEncrypted = securityCodeEncrypted,
                            ExpirationMonthEncrypted = expirationMonthEncrypted,
                            ExpirationYearEncrypted = exiprationYearEncrypted,
                        };
                    }
                }
            }

            var sanitizer = new HtmlSanitizer();
            newSalesOrder.CustomerNotes = sanitizer.Sanitize(quoteFormSalesOrderSubmission.Notes);

            return (newSalesOrder, paymentProfileId);
        }
        public async Task<decimal?> GetNetProfit(AppDBContext context)
        {
            var salesOrder = await context.SalesOrders
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Sources)
                        .ThenInclude(item => item.Source)
                .Include(item => item.CurrencyOption)
                .FirstOrDefaultAsync(item => item.Id == this.Id);


            foreach (var lineItem in salesOrder.LineItems) {

                var sources = lineItem.Sources.Select(item => item.Source).Where(item => item.Cost > 0).OrderBy(item => item.Cost).ToList();

                var prices = new List<decimal> { };
                foreach (var source in sources)
                {
                    var cost = source.Cost;
                    var currency = await context.CurrencyOptions.FirstOrDefaultAsync(item => item.Id == source.CurrencyOptionId);
                    if (cost == null || currency == null)
                        continue;
                    cost = ExchangeRates.ConvertToUsd(cost.Value, currency.Value);
                    cost = ExchangeRates.ConvertFromUsd(cost.Value, salesOrder.CurrencyOption.Value);
                    for (var i = 0; i < source.Quantity; ++i)
                    {
                        prices.Add(cost.Value);
                    }
                }


                if (prices.Count == 0)
                    return 0;

                // add extra prices based on last price if not enough
                if(prices.Count < lineItem.Quantity)
                {
                    for(var i = prices.Count; i <= lineItem.Quantity; ++i)
                    {
                        prices.Add(prices.Last());
                    }
                }

                var price = lineItem.Price * (100 - (lineItem.DiscountPercent ?? 0)) / 100;
                var averageCost = Math.Round(prices.Average(), 2);
                var profit = (price - averageCost) * lineItem.Quantity;
                lineItem.AverageCost = averageCost;
                lineItem.ProjectedProfit = profit;
            }

            salesOrder.ProjectedProfit = salesOrder.LineItems.Select(item => item.ProjectedProfit ?? 0).Sum();

            await context.SaveChangesAsync();


            return salesOrder.ProjectedProfit;
        }
    }


    /// <summary>
    /// set up createdAt, updatedAt and createdBy auto properties
    /// </summary>
    class SalesOrderDBConfiguration : IEntityTypeConfiguration<SalesOrder> {
        public void Configure(EntityTypeBuilder<SalesOrder> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(so => so.Company)
                .WithMany(item => item.SalesOrders)
                .HasForeignKey(so => so.CompanyId);

            modelBuilder.HasOne(item => item.Quote)
                .WithMany(quote => quote.SalesOrders)
                .HasForeignKey(item => item.QuoteId);

            modelBuilder.HasOne(item => item.PaymentMethod)
                .WithMany()
                .HasForeignKey(item => item.SalesOrderPaymentMethodId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasOne(item => item.CurrencyOption)
                .WithMany()
                .HasForeignKey(item => item.CurrencyOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.HasOne(item => item.ShippingCarrier)
                .WithMany()
                .HasForeignKey(item => item.ShippingCarrierId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.HasOne(item => item.ShippingCarrierShippingMethod)
                .WithMany()
                .HasForeignKey(item => item.ShippingCarrierShippingMethodId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.HasOne(item => item.SalesPerson)
                .WithMany()
                .HasForeignKey(item => item.SalesPersonId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder.HasOne(item => item.GidLocationOption)
                .WithMany()
                .HasForeignKey(item => item.GidLocationOptionId);

            modelBuilder.HasOne(item => item.BillingAddress).WithMany().HasForeignKey(item => item.BillingAddressId);
            modelBuilder.HasOne(item => item.ShippingAddress).WithMany().HasForeignKey(item => item.ShippingAddressId);
            modelBuilder.HasOne(item => item.PaymentTerm).WithMany().HasForeignKey(item => item.PaymentTermId).OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasIndex(item => item.GidLocationOptionId);
            modelBuilder.HasIndex(item => item.Phone);
            modelBuilder.HasIndex(item => item.Email);
            modelBuilder.HasIndex(item => item.CreatedAt);
            modelBuilder.HasIndex(item => item.Total);
            modelBuilder.HasIndex(item => item.ProjectedProfit);
            modelBuilder.HasIndex(item => item.CustomerPurchaseOrderNumber);
        }
    }
}
