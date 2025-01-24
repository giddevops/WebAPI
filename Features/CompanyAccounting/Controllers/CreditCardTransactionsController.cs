using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("CreditCardTransactions")]
    public class CreditCardTransactionsController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration _configuration;
        private QuickBooksConnector _quickBooksConnector;

        public CreditCardTransactionsController(AppDBContext context, IConfiguration config, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _configuration = config;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: CreditCardTransactions
        [HttpGet]
        public async Task<IActionResult> GetCreditCardTransactions(
            [FromQuery] int? companyId,
            [FromQuery] bool? notCaptured,
            [FromQuery] bool? refundable,
            [FromQuery] int? salesOrderId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            if (companyId == null) {
                return BadRequest(new {
                    Error = "CompanyId Cannot Be Null"
                });
            }

            var query = from creditCardCharge in _context.CreditCardTransactions select creditCardCharge;

            if (notCaptured == true) {
                query = query
                    .Where(item => item.CreatedAt > DateTime.UtcNow.AddDays(-30))
                    .Where(item => item.NotCaptured == true);
            }
            if (refundable == true) {
                query = query
                    .Where(item => item.CreditCardTransactionType == CreditCardTransactionType.AuthorizationAndCapture || item.CreditCardTransactionType == CreditCardTransactionType.Capture)
                    .Where(item => item.Succeeded == true)
                    .Where(item => item.VoidedAt == null)
                    .Where(item => item.CreatedAt < DateTime.Now.Date)
                    .Where(item => item.FullyRefunded != true);
            }
            if (salesOrderId != null) {
                query = query.Where(item => item.SalesOrderId == salesOrderId);
            }

            query = query
                .Where(item => item.CompanyId == companyId)
                .OrderByDescending(item => item.CreatedAt);

            return Ok(new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            });
        }

        // GET: CreditCardTransactions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCreditCardTransaction([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var creditCardCharge = await _context.CreditCardTransactions.SingleOrDefaultAsync(m => m.Id == id);

            if (creditCardCharge == null) {
                return NotFound();
            }

            return Ok(creditCardCharge);
        }

        // GET: VoidCreditCardTransaction
        [RequirePermission("ManageBilling")]
        [HttpPost("{id}/Void")]
        public async Task<IActionResult> VoidCreditCardTransaction([FromRoute] int id) {
            var transaction = await _context.CreditCardTransactions.FirstOrDefaultAsync(item => item.Id == id);
            if (transaction == null) {
                return NotFound(new {
                    Error = "The Credit card transaction was not found with that id"
                });
            }
            var requestData = new {
                createTransactionRequest = new {
                    merchantAuthentication = new {
                        name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                        transactionKey = AuthorizeNetApiRequestor.transactionKey
                    },
                    transactionRequest = new {
                        transactionType = "voidTransaction",
                        refTransId = transaction.TransactionId,
                    }
                }
            };
            var response = await GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.AuthorizeNetApiRequestor.DoApiRequest(requestData);
            dynamic responseBody = Newtonsoft.Json.Linq.JObject.Parse(response);

            var results = new Dictionary<string, string> { };

            if (responseBody.messages != null && responseBody.messages.resultCode == "Error") {
                results["ResultCode"] = responseBody.messages.resultCode;
                results["ResultMessageCode"] = responseBody.messages.message[0].code;
                results["ResultMessageText"] = responseBody.messages.message[0].text;

                if (responseBody.transactionResponse != null && responseBody.transactionResponse.errors != null) {
                    results["TransactionErrorCode"] = responseBody.transactionResponse.errors[0].errorCode;
                    results["ErrorMessage"] = responseBody.transactionResponse.errors[0].errorText;
                } else {
                    results["TransactionErrorCode"] = responseBody.transactionResponse.errors[0].errorCode;
                    results["ErrorMessage"] = responseBody.transactionResponse.errors[0].errorText;
                }
                return BadRequest(results);
            }

            //remove any cash receipts/disbursements that were associated with this
            //I'm using the controllers instead of the database directly so that they get removed from quickbooks as well
            var invoiceCashReceiptsController = new InvoiceCashReceiptsController(_context, _quickBooksConnector);
            var cashReceiptsController = new CashReceiptsController(_context, _quickBooksConnector);
            var cashReceipts = await _context.CashReceipts
                .Include(item => item.Invoices)
                .Where(item => item.CreditCardTransactionId == transaction.Id).ToListAsync();
            foreach (var cashReceipt in cashReceipts) {
                foreach (var invoiceCashReceipt in cashReceipt.Invoices)
                    await invoiceCashReceiptsController.DeleteInvoiceCashReceipt((int)invoiceCashReceipt.InvoiceId, (int)cashReceipt.Id);
                await cashReceiptsController.DeleteCashReceipt((int)cashReceipt.Id);
            }

            var billCashDisbursementsController = new BillCashDisbursementsController(_context, _quickBooksConnector);
            var cashDisbursementsController = new CashDisbursementsController(_context, _quickBooksConnector);
            var cashDisbursements = await _context.CashDisbursements
                .Include(item => item.Bills)
                .Where(item => item.CreditCardTransactionId == transaction.Id).ToListAsync();
            
           foreach (var cashDisbursement in cashDisbursements) {
                foreach (var billCashDisbursement in cashDisbursement.Bills)
                    await billCashDisbursementsController.DeleteBillCashDisbursement((int)billCashDisbursement.BillId, (int)cashDisbursement.Id);
                await cashDisbursementsController.DeleteCashDisbursement((int)cashDisbursement.Id);
            }


            transaction.VoidedAt = DateTime.UtcNow;
            transaction.ResultCode += ", Voided";
            transaction.OverallStatus += ", Voided";
            _context.Entry(transaction).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(transaction);
        }

        // POST: CreditCardTransactions
        [RequirePermission("ManageBilling")]
        [HttpPost]
        public async Task<IActionResult> PostCreditCardTransaction([FromBody] CreditCardTransaction creditCardTransaction) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var company = await _context.Companies.AsNoTracking().FirstOrDefaultAsync(item => item.Id == creditCardTransaction.CompanyId);

            if (company == null) {
                return BadRequest("A company was not found with Id #" + creditCardTransaction.CompanyId);
            }

            var type = creditCardTransaction.CreditCardTransactionType;

            dynamic requestData = null;

            CreditCardTransaction relatedCreditCardTransaction = null;
            if (creditCardTransaction.RelatedTransactionId != null) {
                relatedCreditCardTransaction = await _context.CreditCardTransactions.FirstOrDefaultAsync(item => item.TransactionId == creditCardTransaction.RelatedTransactionId);
            }

            creditCardTransaction.Amount = Decimal.Round(creditCardTransaction.Amount, 2);

            // Get card code
            var cardCode = _context.PaymentProfileCodes.Where(i => i.PaymentProfileId == creditCardTransaction.PaymentProfileId).Select(j => j.CardCode).FirstOrDefault();

            // Decrypt cardCode if it exists
            if (cardCode != null)
                cardCode = Encryption.DecryptString(cardCode);

            if (type == CreditCardTransactionType.Authorization) {

                if (cardCode != null)
                {
                    requestData = new
                    {
                        createTransactionRequest = new
                        {
                            merchantAuthentication = new
                            {
                                name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                                transactionKey = AuthorizeNetApiRequestor.transactionKey
                            },
                            transactionRequest = new
                            {
                                transactionType = "authOnlyTransaction",
                                amount = creditCardTransaction.Amount,
                                profile = new
                                {
                                    customerProfileId = company.AuthorizeNetProfileId,
                                    paymentProfile = new
                                    {
                                        paymentProfileId = creditCardTransaction.PaymentProfileId,
                                        cardCode = cardCode
                                    }
                                }
                            }
                        }
                    };
                }
                else
                {
                    requestData = new
                    {
                        createTransactionRequest = new
                        {
                            merchantAuthentication = new
                            {
                                name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                                transactionKey = AuthorizeNetApiRequestor.transactionKey
                            },
                            transactionRequest = new
                            {
                                transactionType = "authOnlyTransaction",
                                amount = creditCardTransaction.Amount,
                                profile = new
                                {
                                    customerProfileId = company.AuthorizeNetProfileId,
                                    paymentProfile = new
                                    {
                                        paymentProfileId = creditCardTransaction.PaymentProfileId
                                    }
                                }
                            }
                        }
                    };
                }



            } else if (type == CreditCardTransactionType.Capture) {
                requestData = new {
                    createTransactionRequest = new {
                        merchantAuthentication = new {
                            name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                            transactionKey = AuthorizeNetApiRequestor.transactionKey
                        },
                        transactionRequest = new {
                            transactionType = "priorAuthCaptureTransaction",
                            amount = creditCardTransaction.Amount,
                            refTransId = creditCardTransaction.RelatedTransactionId
                        }
                    }
                };
            } else if (type == CreditCardTransactionType.AuthorizationAndCapture) {
                if (cardCode != null)
                {
                    requestData = new
                    {
                        createTransactionRequest = new
                        {
                            merchantAuthentication = new
                            {
                                name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                                transactionKey = AuthorizeNetApiRequestor.transactionKey
                            },
                            transactionRequest = new
                            {
                                transactionType = "authCaptureTransaction",
                                amount = creditCardTransaction.Amount,
                                profile = new
                                {
                                    customerProfileId = company.AuthorizeNetProfileId,
                                    paymentProfile = new
                                    {
                                        paymentProfileId = creditCardTransaction.PaymentProfileId,
                                        cardCode = cardCode
                                    }
                                }
                            }
                        }
                    };
                }
                else
                {
                    requestData = new
                    {
                        createTransactionRequest = new
                        {
                            merchantAuthentication = new
                            {
                                name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                                transactionKey = AuthorizeNetApiRequestor.transactionKey
                            },
                            transactionRequest = new
                            {
                                transactionType = "authCaptureTransaction",
                                amount = creditCardTransaction.Amount,
                                profile = new
                                {
                                    customerProfileId = company.AuthorizeNetProfileId,
                                    paymentProfile = new
                                    {
                                        paymentProfileId = creditCardTransaction.PaymentProfileId
                                    }
                                }
                            }
                        }
                    };
                }


                
            } else if (type == CreditCardTransactionType.Refund) {
                //have to have the last 4 digits of the credit card number to do a refund.
                var transactionDetails = await relatedCreditCardTransaction.FetchTransactionDetails(_configuration);
                if (transactionDetails.messages.resultCode == "Error") {
                    creditCardTransaction.ResultCode = transactionDetails.messages.resultCode;
                    creditCardTransaction.ResultMessageCode = transactionDetails.messages.message[0].code;
                    creditCardTransaction.ResultMessageText = transactionDetails.messages.message[0].text;
                    creditCardTransaction.TransactionErrorCode = creditCardTransaction.ResultMessageCode;
                    creditCardTransaction.ErrorMessage = creditCardTransaction.ResultMessageText;
                    creditCardTransaction.OverallStatus = "Error, Not Completed";
                    creditCardTransaction.Succeeded = false;
                    _context.CreditCardTransactions.Add(creditCardTransaction);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetCreditCardTransaction", new {
                        id = creditCardTransaction.Id
                    }, creditCardTransaction);
                }
                var last4 = transactionDetails.transaction.payment.creditCard.cardNumber;
                requestData = new {
                    createTransactionRequest = new {
                        merchantAuthentication = new {
                            name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                            transactionKey = AuthorizeNetApiRequestor.transactionKey
                        },
                        transactionRequest = new {
                            transactionType = "refundTransaction",
                            amount = creditCardTransaction.Amount,
                            payment = new {
                                creditCard = new {
                                    cardNumber = last4,
                                    expirationDate = "XXXX"
                                }
                            },
                            refTransId = creditCardTransaction.RelatedTransactionId,
                        }
                    }
                };
            } else {
                return BadRequest(new {
                    Error = "That is not yet implemented"
                });
            }

            var response = await GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.AuthorizeNetApiRequestor.DoApiRequest(requestData);
            dynamic responseBody = Newtonsoft.Json.Linq.JObject.Parse(response);

            //If there is an error, it should tell you in the responseBody.messages.resultCode
            if (responseBody.messages != null) {
                creditCardTransaction.ResultCode = responseBody.messages.resultCode;
                creditCardTransaction.ResultMessageCode = responseBody.messages.message[0].code;
                creditCardTransaction.ResultMessageText = responseBody.messages.message[0].text;
                creditCardTransaction.OverallStatus = creditCardTransaction.ResultCode;
            }
            if (responseBody.messages.resultCode == "Error") {
                //Set the message and code as error message and code since it was an error.
                creditCardTransaction.TransactionErrorCode = creditCardTransaction.ResultMessageCode;
                creditCardTransaction.ErrorMessage = creditCardTransaction.ResultMessageText;
                //The thing is, usually the error text in responseBody.messages.message[0].text is really generic.
                //So check for a more detailed error message at responseBody.transactionResponse.error
                if (responseBody.transactionResponse != null && responseBody.transactionResponse.errors != null) {
                    creditCardTransaction.TransactionErrorCode = responseBody.transactionResponse.errors[0].errorCode;
                    creditCardTransaction.ErrorMessage = responseBody.transactionResponse.errors[0].errorText;
                }
                if (creditCardTransaction.ResultCode != null) {
                    creditCardTransaction.OverallStatus = creditCardTransaction.TransactionErrorCode;
                } else {
                    creditCardTransaction.OverallStatus = creditCardTransaction.TransactionErrorCode;
                }
                creditCardTransaction.Succeeded = false;
                _context.CreditCardTransactions.Add(creditCardTransaction);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetCreditCardTransaction", new {
                    id = creditCardTransaction.Id
                }, creditCardTransaction);
            } else {
                if (responseBody.transactionResponse != null) {
                    if (responseBody.transactionResponse.responseCode == "1") {
                        creditCardTransaction.OverallStatus = "Approved";
                        creditCardTransaction.Succeeded = true;
                    } else if (responseBody.transactionResponse.responseCode == "2") {
                        creditCardTransaction.OverallStatus = "Declined";
                        creditCardTransaction.Succeeded = false;
                    } else if (responseBody.transactionResponse.responseCode == "3") {
                        creditCardTransaction.OverallStatus = "Error";
                        creditCardTransaction.Succeeded = false;
                    } else if (responseBody.transactionResponse.responseCode == "4") {
                        creditCardTransaction.OverallStatus = "Held for Review";
                        creditCardTransaction.Succeeded = true;
                    }

                    if (creditCardTransaction.Succeeded == false) {
                        _context.CreditCardTransactions.Add(creditCardTransaction);
                        await _context.SaveChangesAsync();
                        return CreatedAtAction("GetCreditCardTransaction", new {
                            id = creditCardTransaction.Id
                        }, creditCardTransaction);
                    }
                }
            }

            //Transaction succeeded.  So now do some more stuff base on what transaction type it is
            creditCardTransaction.Succeeded = true;
            await _context.SaveChangesAsync();
            if (type == CreditCardTransactionType.Authorization) {
                creditCardTransaction.NotCaptured = true;
            } else if (type == CreditCardTransactionType.Capture) {
                relatedCreditCardTransaction.AmountCaptured = creditCardTransaction.Amount;
                relatedCreditCardTransaction.NotCaptured = false;
            } else if (type == CreditCardTransactionType.AuthorizationAndCapture) {
                creditCardTransaction.AmountCaptured = creditCardTransaction.Amount;
                creditCardTransaction.NotCaptured = false;
            } else if (type == CreditCardTransactionType.Refund) {
                relatedCreditCardTransaction.AmountRefunded = creditCardTransaction.Amount;
            }

            creditCardTransaction.ResultCode = responseBody.messages.resultCode;
            creditCardTransaction.ResultMessageCode = responseBody.messages.message[0].code;
            creditCardTransaction.ResultMessageText = responseBody.messages.message[0].text;
            creditCardTransaction.TransactionId = responseBody.transactionResponse.transId;
            // if(creditCardTransaction.TransactionId == "0"){
            //     creditCardTransaction.ResultMessageCode = responseBody.messages.message[0].code;
            //     creditCardTransaction.ResultMessageText = responseBody.messages.message[0].text;
            // }
            creditCardTransaction.CreatedAt = DateTime.UtcNow;
            _context.CreditCardTransactions.Add(creditCardTransaction);
            await _context.SaveChangesAsync();

            var QuickBooksSyncResults = new List<QuickBooksSyncResult> { };

            //If received funds from this go ahead and create a cash receipt
            if (type == CreditCardTransactionType.Capture || type == CreditCardTransactionType.AuthorizationAndCapture) {
                if (type == CreditCardTransactionType.AuthorizationAndCapture) {
                    creditCardTransaction.CapturedAt = DateTime.UtcNow;
                }
                if (type == CreditCardTransactionType.Capture) {
                    var oldTransaction = await _context.CreditCardTransactions.FirstOrDefaultAsync(item => item.TransactionId == creditCardTransaction.RelatedTransactionId);
                    oldTransaction.CapturedAt = DateTime.UtcNow;
                    if (oldTransaction != null) {
                        oldTransaction.CapturedAt = DateTime.UtcNow;
                        await _context.SaveChangesAsync();
                    }
                }
                var creditCardPaymentMethodType = await _context.SalesOrderPaymentMethods.FirstOrDefaultAsync(item => item.Value.ToLower().Replace(" ", "") == "creditcard");
                int? creditCardPaymentMethodTypeId = null;
                if (creditCardPaymentMethodType != null)
                    creditCardPaymentMethodTypeId = creditCardPaymentMethodType.Id;

                var cashReceiptCreditCardType = await _context.CashReceiptTypes.FirstOrDefaultAsync(item => item.Value == "Credit Card");
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                var newCashReceipt = new CashReceipt {
                    Amount = creditCardTransaction.Amount,
                    Balance = creditCardTransaction.Amount,
                    BankAccountId = creditCardTransaction.BankAccountId,
                    CurrencyOptionId = creditCardTransaction.CurrencyOptionId ?? 1,
                    // CashReceiptTypeId = cashReceiptCreditCardType.Id,
                    SalesOrderPaymentMethodId = creditCardPaymentMethodTypeId,
                    CompanyId = creditCardTransaction.CompanyId,
                    SalesOrderId = creditCardTransaction.SalesOrderId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User),
                    CreditCardTransactionId = creditCardTransaction.Id,
                    DateReceived = DateTime.UtcNow
                    // DateReceived = new DateTimeOffset(DateTime.UtcNow,timeZoneInfo.GetUtcOffset(DateTime.UtcNow)).Date.AddHours(18)
                };
                _context.CashReceipts.Add(newCashReceipt);
                await _context.SaveChangesAsync();

                if (creditCardTransaction.SalesOrderId != null) {
                    var invoices = await _context.Invoices.Include(item => item.LineItems).Include(item => item.CashReceipts).Where(item => item.SalesOrderId == creditCardTransaction.SalesOrderId).ToListAsync();
                    //also go ahead and create an invoice if it doesn't exist
                    if (invoices.Count == 0) {
                        var newInvoice = await Invoice.CreateInvoiceFromSalesOrderId(_context, (int)creditCardTransaction.SalesOrderId, GidIndustrial.Gideon.WebApi.Models.User.GetId(User));

                        newInvoice.CashReceipts.Add(new InvoiceCashReceipt {
                            CashReceiptId = newCashReceipt.Id,
                            Amount = newCashReceipt.Amount,
                            CreatedAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User)
                        });
                        var total = newInvoice.GetTotal();
                        newInvoice.Balance = total - creditCardTransaction.Amount;
                        newCashReceipt.Balance = 0;
                        _context.Invoices.Add(newInvoice);
                        await _context.SaveChangesAsync();
                    } else {
                        var amountLeftToApply = creditCardTransaction.Amount;

                        //If there is one invoice and the balance on it is less than the amount of the payment, need to update the invoice
                        if (invoices.Count == 1) {
                            var invoice = invoices[0];
                            if (invoice.Balance <= amountLeftToApply) {
                                //reload line items from sales order
                                _context.InvoiceLineItems.RemoveRange(invoice.LineItems);
                                await _context.SaveChangesAsync();
                                var salesOrder = await _context.SalesOrders.AsNoTracking()
                                    .Include(item => item.LineItems)
                                    .Include(item => item.BillingAddress)
                                    .Include(item => item.ShippingAddress)
                                    .FirstOrDefaultAsync(item => item.Id == creditCardTransaction.SalesOrderId);
                                invoice.LineItems = salesOrder.LineItems.Select(item => new InvoiceLineItem {
                                    CreatedAt = DateTime.UtcNow,
                                    Description = item.Description,
                                    DiscountPercent = item.DiscountPercent,
                                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User),
                                    Price = item.Price,
                                    Quantity = item.Quantity,
                                    ManufacturerName = item.ManufacturerName,
                                    SalesOrderLineItemId = item.Id,
                                    ProductId = item.ProductId
                                }).ToList();
                                invoice.SalesOrderId = salesOrder.Id;
                                invoice.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
                                invoice.WireTransferFee = salesOrder.WireTransferFee ?? 0;
                                invoice.ShippingAndHandlingFee = salesOrder.ShippingAndHandlingFee ?? 0;
                                invoice.ExpediteFee = salesOrder.ExpediteFee ?? 0;
                                invoice.SalesTax = salesOrder.SalesTax ?? 0;
                                invoice.CreditCardFee = salesOrder.CreditCardFee ?? 0;
                                invoice.ShippingCarrierId = salesOrder.ShippingCarrierId;
                                invoice.ShippingCarrierShippingMethodId = salesOrder.ShippingCarrierShippingMethodId;
                                await _context.SaveChangesAsync();
                                invoice.Balance = await invoice.GetBalance(_context);

                                await _context.SaveChangesAsync();
                            }
                        }

                        foreach (var invoice in invoices) {
                            if (invoice.Balance > 0) {
                                if (amountLeftToApply <= invoice.Balance) {
                                    var newInvoiceCashReceipt = new InvoiceCashReceipt {
                                        InvoiceId = invoice.Id,
                                        CashReceiptId = newCashReceipt.Id,
                                        Amount = amountLeftToApply
                                    };
                                    _context.InvoiceCashReceipts.Add(newInvoiceCashReceipt);
                                    invoice.Balance -= newInvoiceCashReceipt.Amount;
                                    newCashReceipt.Balance -= amountLeftToApply;
                                    await _context.SaveChangesAsync();
                                    // JSB - HIDING QB INTEGRATION
                                    //if (!String.IsNullOrWhiteSpace(invoice.QuickBooksId)) {
                                    //    QuickBooksSyncResults.Add(await invoice.SyncWithQuickBooks(_quickBooksConnector, _context));
                                    //}
                                    break;
                                } else {
                                    var newInvoiceCashReceipt = new InvoiceCashReceipt {
                                        InvoiceId = invoice.Id,
                                        CashReceiptId = newCashReceipt.Id,
                                        Amount = invoice.Balance ?? 0
                                    };
                                    newCashReceipt.Balance -= invoice.Balance ?? 0;
                                    invoice.Balance = 0;
                                    amountLeftToApply -= (invoice.Balance ?? 0);
                                    _context.InvoiceCashReceipts.Add(newInvoiceCashReceipt);
                                    await _context.SaveChangesAsync();
                                    // JSB - HIDING QB INTEGRATION
                                    //if (!String.IsNullOrWhiteSpace(invoice.QuickBooksId)) {
                                    //    QuickBooksSyncResults.Add(await invoice.SyncWithQuickBooks(_quickBooksConnector, _context));
                                    //}
                                }
                            }
                        }
                    }
                    await newCashReceipt.CalculateBalanceAndSave(_context);
                    var relatedSalesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == creditCardTransaction.SalesOrderId);
                    if(relatedSalesOrder != null){
                        await relatedSalesOrder.UpdateBalance(_context);
                    }
                }
            }
            if (type == CreditCardTransactionType.Refund) {
                //If did a refund go ahead and create a cash disbursement
                var paymentMethodCreditCardType = await _context.PurchaseOrderPaymentMethods.FirstOrDefaultAsync(item => item.Value == "Credit Card");
                TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
                var newCashDisbursement = new CashDisbursement {
                    Amount = creditCardTransaction.Amount,
                    Balance = creditCardTransaction.Amount,
                    BankAccountId = creditCardTransaction.BankAccountId,
                    CurrencyOptionId = creditCardTransaction.CurrencyOptionId ?? 1,
                    // PurchaseOrderPaymentMethodId = paymentMethodCreditCardType.Id,
                    CompanyId = creditCardTransaction.CompanyId,
                    SalesOrderId = creditCardTransaction.SalesOrderId,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User),
                    CreditCardTransactionId = creditCardTransaction.Id,
                    DateDisbursed = DateTime.UtcNow
                    // DateReceived = new DateTimeOffset(DateTime.UtcNow,timeZoneInfo.GetUtcOffset(DateTime.UtcNow)).Date.AddHours(18)
                };
                _context.CashDisbursements.Add(newCashDisbursement);
                await _context.SaveChangesAsync();
                if(creditCardTransaction.SalesOrderId != null){
                    var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == creditCardTransaction.SalesOrderId);
                    if(salesOrder != null) {
                        await salesOrder.UpdateBalance(_context);
                    }
                }
            }

            // JSB - HIDING QB INTEGRATION
            //if (QuickBooksSyncResults.FirstOrDefault(item => item.Succeeded == false) != null) {
            //    return StatusCode(StatusCodes.Status500InternalServerError, QuickBooksSyncResults.FirstOrDefault(item => item.Succeeded == false));
            //}

            return CreatedAtAction("GetCreditCardTransaction", new {
                id = creditCardTransaction.Id
            }, creditCardTransaction);
        }

        // // DELETE: CreditCardTransactions/5
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteCreditCardTransaction([FromRoute] int id)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     var creditCardCharge = await _context.CreditCardTransactions.SingleOrDefaultAsync(m => m.Id == id);
        //     if (creditCardCharge == null)
        //     {
        //         return NotFound();
        //     }

        //     _context.CreditCardTransactions.Remove(creditCardCharge);
        //     await _context.SaveChangesAsync();

        //     return Ok(creditCardCharge);
        // }

        private bool CreditCardTransactionExists(int id) {
            return _context.CreditCardTransactions.Any(e => e.Id == id);
        }
    }
}