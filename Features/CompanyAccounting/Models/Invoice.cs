using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Libraries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Invoice {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public Contact Contact { get; set; }
        public int? ContactId { get; set; }

        public int? SalesPersonId { get; set; }
        public User SalesPerson { get; set; }


        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        // public decimal? Total { get; set; }
        public decimal? Balance { get; set; }

        public DateTime? DateSent { get; set; }
        public DateTime? DateDue { get; set; }
        public bool? ShippedOnly { get; set; }

        public int? CurrencyOptionId { get; set; }
        public CurrencyOption CurrencyOption { get; set; }

        public int? GidLocationOptionId { get; set; }
        public GidLocationOption GidLocationOption { get; set; }

        public decimal ShippingAndHandlingFee { get; set; }

        public string ShippingAndHandlingDisplay
        {
            get
            {
                return ((ShippingAndHandlingFee > 0)
                    || (ShippingAndHandlingFee == 0 && (SalesOrder != null && (!String.IsNullOrEmpty(SalesOrder.FreightAccountNumber) && SalesOrder.FreightAccountNumber != "(PP&A)")))) ?
                    (CurrencyOption != null ? CurrencyOption.Symbol : String.Empty) + ShippingAndHandlingFee.ToString("N2") : "TBD";
            }
        }

        public decimal SalesTax { get; set; }
        public decimal WireTransferFee { get; set; }
        public decimal ExpediteFee { get; set; }
        // public decimal DiscountPercent { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public decimal Total { get; set; }

        public DateTime? SentAt { get; set; }
        public DateTime? CancelledAt { get; set; }

        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }

        public string CustomerPurchaseOrderNumber { get; set; }
        public string CustomerVatNumber { get; set; }

        public ShippingCarrier ShippingCarrier { get; set; }
        public int? ShippingCarrierId { get; set; }

        public ShippingCarrierShippingMethod ShippingCarrierShippingMethod { get; set; }
        public int? ShippingCarrierShippingMethodId { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public string QuickBooksId { get; set; }
        public string QuickBooksSyncToken { get; set; }

        [NotMapped]
        public User Preparer { get; set; }

        public List<InvoiceLineItem> LineItems { get; set; }
        public List<InvoiceAttachment> Attachments { get; set; }

        public List<InvoiceCashReceipt> CashReceipts { get; set; }
        public List<InvoiceCredit> Credits { get; set; }
        // public List<CreditCardCharge> CreditCardCharges { get; set; }
        public List<CreditCardTransaction> CreditCardTransactions { get; set; }
        public List<InvoiceChatMessage> ChatMessages { get; set; }
        public List<InvoiceEventLogEntry> EventLogEntries { get; set; }


        public int? PaymentTermId { get; set; }
        public PaymentTerm PaymentTerm { get; set; }


        public string DiscountType { get; set; }
        public decimal DiscountAmount { get; set; }

        public decimal CreditCardFee { get; set; }


        public decimal? GetLineItemsTotal() {
            return LineItems.Sum(item => item.DiscountPercent == null ? item.Price * item.Quantity : item.Price * (100 - item.DiscountPercent) / 100 * item.Quantity);
        }

        public decimal GetDiscountAmount() {
            if (this.DiscountAmount != 0) {
                if (this.DiscountType == DiscountTypes.PERCENT) {
                    return (this.GetLineItemsTotal().Value * this.DiscountAmount / 100);
                } else if (this.DiscountType == DiscountTypes.FIXED_AMOUNT) {
                    return this.DiscountAmount;
                } else {
                    return 0;
                }
            } else {
                return 0;
            }
        }

        public decimal? GetTotal() {
            var total = this.GetLineItemsTotal();

            total += ShippingAndHandlingFee;
            total += SalesTax;
            total += WireTransferFee;
            total += ExpediteFee;
            total += CreditCardFee;
            total -= this.GetDiscountAmount();

            return total;
        }

        public async Task<decimal?> GetBalance(AppDBContext _context) {
            var invoiceUntracked = await _context.Invoices.AsNoTracking()
                .Include(item => item.LineItems)
                .Include(item => item.CashReceipts)
                .Include(item => item.Credits)
                .FirstOrDefaultAsync(item => item.Id == this.Id);
            var amountPaid = invoiceUntracked.CashReceipts.Sum(item => item.Amount) + invoiceUntracked.Credits.Sum(item => item.Amount);
            return invoiceUntracked.GetTotal() - amountPaid;
        }

        public async Task<bool> CheckIfIsGidEurope(AppDBContext _context) {
            var GidEuropeOption = await _context.GidLocationOptions.FirstOrDefaultAsync(item => item.Value.Contains("Europe"));
            if (GidEuropeOption == null) {
                throw new Exception("Unable to find GID Europe option");
            }
            return this.GidLocationOptionId == GidEuropeOption.Id;
        }

        public async Task<QuickBooksSyncResult> EnsureLineItemsAndCompanyInQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (this.LineItems == null || this.LineItems.Count == 0)
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "There are no line items - aborting"
                };
            foreach (var lineItem in this.LineItems) {
                var result = await lineItem.Product.EnsureInQuickBooks(quickBooksConnector, _context);
                if (!result.Succeeded) {
                    return result;
                }
            }
            var status = await this.Company.EnsureCustomerInQuickBooks(quickBooksConnector, _context);
            if (!status.Succeeded)
                return status;
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = "Line items synced successfully"
            };
        }

        public async Task<QuickBooksSyncResult> DeleteFromQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (String.IsNullOrWhiteSpace(this.QuickBooksId))
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "This object has no quickbooks Id so it can't be deleted from quickbooks. This may mean it was never imported in the first place. It could also mean that there was an error when importing"
                };

            dynamic responseData;
            try {
                responseData = await quickBooksConnector.DeleteResource("invoice", new {
                    Id = this.QuickBooksId,
                    SyncToken = this.QuickBooksSyncToken
                });
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = $"Error deleting invoice id {this.Id}. The error message was {ex.Message}"
                };
            }

            var newInvoice = responseData.ToObject<QuickBooks.Models.InvoiceDeleteResponse>().Invoice;
            if (newInvoice.Status == "Deleted") {
                this.QuickBooksId = null;
                this.QuickBooksSyncToken = null;
                await _context.SaveChangesAsync();
            }
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = "The item was deleted from quickbooks successfully"
            };
        }

        //Quickbooks api uses a POST request for both Create and Update, so I use the same method to create/update
        public async Task<QuickBooksSyncResult> SyncWithQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context, bool ignoreCashReceipts = false) {
        /*
            if (this.CurrencyOptionId != 1)
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "CurrencyOptionId was not set to USD so not syncing"
                };
            if (this.GidLocationOptionId != 1) {
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "Not Syncing because GID location was not GID Industrial (USA)"
                };
            }
            if (this.SentAt == null) {
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "Not syncing because it hasn't been sent yet"
                };
            }
            var invoice = await _context.Invoices.AsNoTracking()
                .Include(item => item.Contact)
                    .ThenInclude(item => item.EmailAddresses)
                        .ThenInclude(item => item.EmailAddress)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.BillingAddress)
                .Include(item => item.ShippingAddress)
                .Include(item => item.ShippingCarrier)
                .Include(item => item.ShippingCarrierShippingMethod)
                .Include(item => item.Company)
                    .ThenInclude(item => item.EmailAddresses)
                        .ThenInclude(item => item.EmailAddress)
                .Include(item => item.SalesOrder)
                    .ThenInclude(item => item.SalesPerson)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.PaymentTerm)
                .FirstAsync(item => item.Id == this.Id);
            if (invoice.LineItems.Count == 0)
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "There are no line items - aborting"
                };

            await invoice.EnsureLineItemsAndCompanyInQuickBooks(quickBooksConnector, _context);

            var needToSyncCashReceipts = true;

            if (!String.IsNullOrWhiteSpace(invoice.QuickBooksId)) {
                try {
                    //first have to get the object to make sure have latest sync token
                    QuickBooks.Models.InvoiceResponse getResponseData = (await quickBooksConnector.GetResource("invoice", invoice.QuickBooksId))
                        .ToObject<QuickBooks.Models.InvoiceResponse>();
                    invoice.QuickBooksSyncToken = getResponseData.Invoice.SyncToken;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = "Error getting sync token for invoice from quickbooks: " + ex.Message,
                        AdditionalData = ex.StackTrace
                    };
                }
            }

            dynamic newInvoice;
            try {
                dynamic responseData = await quickBooksConnector.PostResource("invoice", await invoice.GetQuickBooksObject(_context));
                newInvoice = responseData.ToObject<QuickBooks.Models.InvoiceResponse>().Invoice;
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Error posting invoice to quickbooks: " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }

            try {
                this.QuickBooksId = newInvoice.Id;
                this.QuickBooksSyncToken = newInvoice.SyncToken;

                _context.Entry(this).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.Entry(this).State = EntityState.Detached;
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Invoice posted to quickbooks, but there was an error saving that fact to the Gideon Database. Future updates probably won't work. " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }
            if (needToSyncCashReceipts && !ignoreCashReceipts) {
                foreach (var invoiceCashReceipt in invoice.CashReceipts) {
                    try {
                        await invoiceCashReceipt.CashReceipt.SyncWithQuickBooks(quickBooksConnector, _context);
                    }
                    catch (Exception ex) {
                        return new QuickBooksSyncResult {
                            Succeeded = false,
                            Message = "The invoice posted to quickbooks successfully, but there was an error posting Cash Receipt with Id " + invoiceCashReceipt.CashReceiptId + ". The error: " + ex.Message,
                            AdditionalData = ex.StackTrace
                        };
                    }
                }
            }
            */
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = "The invoice was imported successfully"
            };
        }

        public async Task<QuickBooks.Models.QuickBooksInvoice> GetQuickBooksObject(AppDBContext _context) {
            var taxString = "NON";
            if (this.SalesTax > 0)
                taxString = "TAX";

            var emailToUse = this.Email;
            if (String.IsNullOrWhiteSpace(emailToUse)) {
                if (this.Contact != null && !String.IsNullOrWhiteSpace(this.Contact.GetDefaultEmailAddress())) {
                    emailToUse = this.Contact.GetDefaultEmailAddress();
                } else if (this.Company != null && !String.IsNullOrWhiteSpace(this.Company.GetDefaultEmailAddress())) {
                    emailToUse = this.Company.GetDefaultEmailAddress();
                }
            }

            var qbLineItems = new List<QuickBooks.Models.InvoiceLine>();
            var index = 0;
            foreach (var lineItem in this.LineItems) {
                ++index;
                var newQuickBooksLineItem = new QuickBooks.Models.InvoiceLine {
                    Amount = (lineItem.Quantity * lineItem.Price ?? 0), //* (1 - .01m * this.DiscountPercent),
                    Description = lineItem.Description,
                    LineNum = index,
                    DetailType = "SalesItemLineDetail",
                    SalesItemLineDetail = new QuickBooks.Models.SalesItemLineDetail {
                        ItemRef = new QuickBooks.Models.ItemRef {
                            value = lineItem.Product.QuickBooksId
                        },
                        UnitPrice = lineItem.Price ?? 0,
                        DiscountRate = lineItem.DiscountPercent ?? 0,
                        Qty = lineItem.Quantity ?? 0
                    }
                };
                // if (this.SalesTax > 0) {
                newQuickBooksLineItem.SalesItemLineDetail.TaxCodeRef = new QuickBooks.Models.TaxCodeRef {
                    value = taxString
                };
                // }
                qbLineItems.Add(newQuickBooksLineItem);
            }

            //now add shipping, expedite fee, wire transfer fee, and sales tax
            // Shipping
            var shippingLineItem = new QuickBooks.Models.InvoiceLine {
                Amount = this.ShippingAndHandlingFee,
                DetailType = "SalesItemLineDetail",
                SalesItemLineDetail = new QuickBooks.Models.SalesItemLineDetail {
                    ItemRef = new QuickBooks.Models.ItemRef {
                        value = "SHIPPING_ITEM_ID"
                    }
                }
            };
            // if (this.SalesTax > 0) {
            shippingLineItem.SalesItemLineDetail.TaxCodeRef = new QuickBooks.Models.TaxCodeRef {
                value = "NON"
            };
            // }
            qbLineItems.Add(shippingLineItem);

            // Wire Transfer
            var wireTransferLineItem = new QuickBooks.Models.InvoiceLine {
                Amount = this.WireTransferFee,
                DetailType = "SalesItemLineDetail",
                SalesItemLineDetail = new QuickBooks.Models.SalesItemLineDetail {
                    ItemRef = new QuickBooks.Models.ItemRef {
                        value = QuickBooksConnector.WireTransferProductId
                    },
                    UnitPrice = this.WireTransferFee,
                    Qty = 1
                }
            };
            // if (this.SalesTax > 0) {
            wireTransferLineItem.SalesItemLineDetail.TaxCodeRef = new QuickBooks.Models.TaxCodeRef {
                value = "NON"
            };
            // }
            qbLineItems.Add(wireTransferLineItem);
            // // Sales Tax
            // qbLineItems.Add(new QuickBooks.Models.InvoiceLine {
            //     Amount = this.SalesTax,
            //     DetailType = "SalesItemLineDetail",
            //     SalesItemLineDetail = new QuickBooks.Models.SalesItemLineDetail {
            //         ItemRef = new QuickBooks.Models.ItemRef {
            //             value = QuickBooksConnector.SalesTaxProductId
            //         },
            //         UnitPrice = this.SalesTax,
            //         Qty = 1
            //     }
            // });
            // Expedite Fee
            var expediteFeeLineIteme = new QuickBooks.Models.InvoiceLine {
                Amount = this.ExpediteFee,
                DetailType = "SalesItemLineDetail",
                SalesItemLineDetail = new QuickBooks.Models.SalesItemLineDetail {
                    ItemRef = new QuickBooks.Models.ItemRef {
                        value = QuickBooksConnector.ExpediteFeeProductId
                    },
                    UnitPrice = this.ExpediteFee,
                    Qty = 1
                }
            };
            // if (this.SalesTax > 0) {
            expediteFeeLineIteme.SalesItemLineDetail.TaxCodeRef = new QuickBooks.Models.TaxCodeRef {
                value = taxString
            };
            // }
            qbLineItems.Add(expediteFeeLineIteme);

            //Discount
            if (this.GetDiscountAmount() > 0) {
                var discountLineDetail = new QuickBooks.Models.DiscountLineDetail { };
                // if(this.DiscountType == DiscountTypes.PERCENT){
                //     discountLineDetail.PercentBased = true;
                //     discountLineDetail.DiscountPercent = this.DiscountAmount;
                // }
                qbLineItems.Add(new QuickBooks.Models.InvoiceLine {
                    DetailType = "DiscountLineDetail",
                    DiscountLineDetail = discountLineDetail,
                    Amount = this.GetDiscountAmount()
                });
            }

            //shippingcarrier are never filled in.. figure out
            QuickBooks.Models.QuickBooksShipMethodRef ShippingMethod = null;
            if (this.ShippingCarrier != null) {
                ShippingMethod = new QuickBooks.Models.QuickBooksShipMethodRef {
                    value = this.ShippingCarrier.Name
                };
                if (this.ShippingCarrierShippingMethod != null) {
                    ShippingMethod.value += " " + this.ShippingCarrierShippingMethod.Name;
                }
                if (ShippingMethod.value.Length > 30)
                    ShippingMethod.value = ShippingMethod.value.Substring(0, 30);
            }

            // Get the first shipment that has a 
            var salesOrderLineItemIds = this.LineItems.Select(item => item.SalesOrderLineItemId);
            var outgoingShipments = await _context.OutgoingShipments
                .OrderBy(item => item.ShippedAt)
                .Where(item => item.ShippedAt != null)
                .Where(
                    item => item.Boxes.Any(
                        box => box.InventoryItems.Any(
                            outgoingShipmentBoxInventoryItem => outgoingShipmentBoxInventoryItem.InventoryItem.SalesOrderLineItems.Any(
                                salesOrderLineItemInventoryItem => salesOrderLineItemIds.Contains(salesOrderLineItemInventoryItem.SalesOrderLineItemId)
                            )
                        )
                    )
                )
                .Include(item => item.Boxes)
                .ToListAsync();
            OutgoingShipment firstOutgoingShipment = null;
            if (outgoingShipments.Count > 0)
                firstOutgoingShipment = outgoingShipments[0];

            string trackingNumbers = null;
            if (firstOutgoingShipment != null && firstOutgoingShipment.Boxes.Count > 0) {
                trackingNumbers = String.Join(", ", firstOutgoingShipment.Boxes.Select(item => item.TrackingNumber));
                //maximum of 31 characters
                if (trackingNumbers.Length > 31) {
                    trackingNumbers = trackingNumbers.Substring(0, 28) + "...";
                }
            }

            //Add custom fields - 1) Sales Person, 2) Customer PO number 3) Sales Order #
            var customFields = new List<QuickBooks.Models.QuickBooksCustomField> { };
            if (this.SalesOrder != null) {
                if (this.SalesOrder.SalesPerson != null && this.SalesOrder.SalesPerson.DisplayName != null)
                    customFields.Add(new QuickBooks.Models.QuickBooksCustomField { DefinitionId = "1", StringValue = this.SalesOrder.SalesPerson.DisplayName, Type = "StringType" });
                customFields.Add(new QuickBooks.Models.QuickBooksCustomField { DefinitionId = "3", StringValue = this.SalesOrder.Id.ToString(), Type = "StringType" });
            }
            if (!String.IsNullOrWhiteSpace(this.CustomerPurchaseOrderNumber)) {
                customFields.Add(new QuickBooks.Models.QuickBooksCustomField { DefinitionId = "2", StringValue = this.CustomerPurchaseOrderNumber, Type = "StringType" });
            }

            var taxInfo = new QuickBooks.Models.TxnTaxDetail {
                TotalTax = this.SalesTax,
                TxnTaxCodeRef = new QuickBooks.Models.TxnTaxCodeRef {
                    value = "3"
                }
            };
            if (this.SalesTax == 0) {
                taxInfo.TxnTaxCodeRef = null;
                // taxInfo = null;
            }

            var newInvoice = new QuickBooks.Models.QuickBooksInvoice {
                Id = this.QuickBooksId,
                SyncToken = this.QuickBooksSyncToken,
                DocNumber = this.Id.ToString(),
                Line = qbLineItems,
                CustomerRef = new QuickBooks.Models.CustomerRef {
                    value = this.Company.QuickBooksCustomerId
                },
                TxnDate = this.CreatedAt.Value.ToString("yyyy-MM-dd") + "Z",
                TxnTaxDetail = taxInfo,
                BillAddr = new QuickBooks.Models.Address(this.BillingAddress),
                ShipAddr = new QuickBooks.Models.Address(this.ShippingAddress),
                BillEmail = new QuickBooks.Models.QuickBooksBillEmail {
                    Address = emailToUse
                },
                ShipMethodRef = ShippingMethod,
                ShipDate = firstOutgoingShipment != null ? firstOutgoingShipment.ShippedAt.Value.ToString("yyyy-MM-dd") + "Z" : null,
                TrackingNum = trackingNumbers,
                DueDate = DateDue.HasValue ? DateDue.Value.ToString("yyyy-MM-dd") + "Z" : null,
                CustomField = customFields
            };
            if (this.PaymentTerm != null && this.PaymentTerm.QuickBooksId != null) {
                newInvoice.SalesTermRef = new QuickBooks.Models.SalesTermRef {
                    value = this.PaymentTerm.QuickBooksId
                };
            }
            return newInvoice;
        }

        public static async Task<Invoice> CreateInvoiceFromSalesOrderId(AppDBContext _context, int salesOrderId, int? createdById, bool shippedOnly = false) {
            //create a new invoice
            var salesOrder = await _context.SalesOrders
                .Include(item => item.LineItems)
                .Include(item => item.BillingAddress)
                .Include(item => item.ShippingAddress)
                .FirstOrDefaultAsync(item => item.Id == salesOrderId);

            decimal shippingAndHandlingFee = salesOrder.ShippingAndHandlingFee ?? 0;
            decimal salesTax = salesOrder.SalesTax ?? 0;
            decimal wireTransferFee = salesOrder.WireTransferFee ?? 0;
            decimal expediteFee = salesOrder.ExpediteFee ?? 0;
            decimal creditCardFee = salesOrder.CreditCardFee ?? 0;

            var lineItemsToUse = salesOrder.LineItems;
            if (shippedOnly) {
                lineItemsToUse = await salesOrder.GetShippedLineItems(_context);
                //need to prorate tax, shipping, etc.
                //to do this, calculate total  cost divided by cost of items shipped
                var totalCost = salesOrder.LineItems.Sum(item => (item.Quantity ?? 0) * (item.Price ?? 0));
                var proratedCost = lineItemsToUse.Sum(item => (item.Quantity ?? 0) * (item.Price ?? 0));

                var costFraction = proratedCost / totalCost;

                shippingAndHandlingFee = Utilities.RoundMoney(shippingAndHandlingFee * costFraction);
                salesTax = Utilities.RoundMoney(salesTax * costFraction);
                wireTransferFee = Utilities.RoundMoney(wireTransferFee * costFraction);
                expediteFee = Utilities.RoundMoney(expediteFee * costFraction);
                creditCardFee = Utilities.RoundMoney(creditCardFee * costFraction);
            }

            var invoiceLineItems = lineItemsToUse.Select(item => {
                return new InvoiceLineItem {
                    CreatedAt = DateTime.UtcNow,
                    Description = item.Description,
                    DiscountPercent = item.DiscountPercent,
                    CreatedById = createdById,
                    Price = item.Price,
                    Quantity = item.Quantity,
                    ManufacturerName = item.ManufacturerName,
                    SalesOrderLineItemId = item.Id,
                    ProductId = item.ProductId
                };
            });

            var newInvoice = new Invoice {
                CreatedAt = DateTime.UtcNow,
                GidLocationOptionId = salesOrder.GidLocationOptionId,
                CurrencyOptionId = salesOrder.CurrencyOptionId,
                CompanyId = salesOrder.CompanyId,
                ContactId = salesOrder.ContactId,
                Email = salesOrder.Email,
                Phone = salesOrder.Phone,
                DiscountAmount = salesOrder.DiscountAmount,
                DiscountType = salesOrder.DiscountType,
                // CustomerPurchaseOrderNumber = salesOrder.CustomerPurchaseOrderNumber,
                LineItems = invoiceLineItems.ToList(),
                CashReceipts = new List<InvoiceCashReceipt> { },
                Credits = new List<InvoiceCredit> { },
                CustomerVatNumber = salesOrder.CustomerVatNumber,
                CustomerPurchaseOrderNumber = salesOrder.CustomerPurchaseOrderNumber,
                SalesOrderId = salesOrder.Id,
                CreatedById = createdById,
                WireTransferFee = wireTransferFee,
                ShippingAndHandlingFee = shippingAndHandlingFee,
                ExpediteFee = expediteFee,
                SalesTax = salesTax,
                CreditCardFee = creditCardFee,
                ShippingCarrierId = salesOrder.ShippingCarrierId,
                ShippingCarrierShippingMethodId = salesOrder.ShippingCarrierShippingMethodId,
                PaymentTermId = salesOrder.PaymentTermId
            };

            if (salesOrder.BillingAddress != null) {
                newInvoice.BillingAddress = new Address {
                    Address1 = salesOrder.BillingAddress.Address1,
                    Address2 = salesOrder.BillingAddress.Address2,
                    Address3 = salesOrder.BillingAddress.Address3,
                    City = salesOrder.BillingAddress.City,
                    State = salesOrder.BillingAddress.State,
                    ZipPostalCode = salesOrder.BillingAddress.ZipPostalCode,
                    CountryId = salesOrder.BillingAddress.CountryId,
                    Attention = salesOrder.BillingAddress.Attention,
                    Name = salesOrder.BillingAddress.Name,
                    PhoneNumber = salesOrder.BillingAddress.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };
            }
            if (salesOrder.ShippingAddress != null) {
                newInvoice.ShippingAddress = new Address {
                    Address1 = salesOrder.ShippingAddress.Address1,
                    Address2 = salesOrder.ShippingAddress.Address2,
                    Address3 = salesOrder.ShippingAddress.Address3,
                    City = salesOrder.ShippingAddress.City,
                    State = salesOrder.ShippingAddress.State,
                    ZipPostalCode = salesOrder.ShippingAddress.ZipPostalCode,
                    CountryId = salesOrder.ShippingAddress.CountryId,
                    Attention = salesOrder.ShippingAddress.Attention,
                    Name = salesOrder.ShippingAddress.Name,
                    PhoneNumber = salesOrder.ShippingAddress.PhoneNumber,
                    CreatedAt = DateTime.UtcNow
                };
            }
            return newInvoice;
        }


    }

    class InvoiceDBConfiguration : IEntityTypeConfiguration<Invoice> {
        public void Configure(EntityTypeBuilder<Invoice> modelBuilder) {
            modelBuilder.HasOne(item => item.Company)
                .WithMany(item => item.Invoices)
                .HasForeignKey(item => item.CompanyId);

            modelBuilder.HasOne(item => item.SalesOrder)
                .WithMany(item => item.Invoices)
                .HasForeignKey(item => item.SalesOrderId);

            modelBuilder.HasOne(item => item.SalesPerson)
                .WithMany()
                .HasForeignKey(item => item.SalesPersonId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.HasOne(item => item.CurrencyOption)
                .WithMany()
                .HasForeignKey(item => item.CurrencyOptionId);

            modelBuilder.HasOne(item => item.GidLocationOption)
                .WithMany()
                .HasForeignKey(item => item.GidLocationOptionId);

            modelBuilder.HasOne(item => item.Contact)
                .WithMany()
                .HasForeignKey(item => item.ContactId);

            modelBuilder.HasOne(item => item.ShippingCarrier)
                .WithMany()
                .HasForeignKey(item => item.ShippingCarrierId);

            modelBuilder.HasOne(item => item.ShippingCarrierShippingMethod)
                .WithMany()
                .HasForeignKey(item => item.ShippingCarrierShippingMethodId);

            modelBuilder.HasOne(item => item.PaymentTerm).WithMany().HasForeignKey(item => item.PaymentTermId);

            modelBuilder.HasIndex(item => item.Balance);
            modelBuilder.HasIndex(item => item.CreatedById);
            modelBuilder.HasIndex(item => item.SalesOrderId);
            modelBuilder.HasIndex(item => item.CreatedAt);
        }
    }
}
