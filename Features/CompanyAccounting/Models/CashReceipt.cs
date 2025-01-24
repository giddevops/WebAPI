using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class CashReceipt {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public string ReferenceNumber { get; set; }

        public decimal Amount { get; set; }
        public decimal TransactionFeeAmount { get; set; }

        public decimal Balance { get; set; }

        public int? CurrencyOptionId { get; set; }
        public int? CashReceiptTypeId { get; set; }

        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        public int? CreditCardTransactionId { get; set; }

        public int? TransactionFeePaymentBankAccountId { get; set; }
        public BankAccount TransactionFeePaymentBankAccount { get; set; }

        public string QuickBooksId { get; set; }
        public string QuickBooksSyncToken { get; set; }

        public string QuickBooksPurchaseId { get; set; }
        public string QuickBooksPurchaseSyncToken { get; set; }

        public DateTime? DateReceived { get; set; }

        public SalesOrderPaymentMethod PaymentMethod { get; set; }
        public int? SalesOrderPaymentMethodId { get; set; }


        public List<InvoiceCashReceipt> Invoices { get; set; }

        public int? SalesOrderId { get; set; }

        public SalesOrder SalesOrder { get; set;  }

        public decimal GetBalance() {
            return this.Amount - this.Invoices.Sum(item => item.Amount);
        }

        public async Task<decimal> CalculateBalanceAndSave(AppDBContext context) {
            var state = context.Entry(this).State;
            context.Entry(this).State = EntityState.Detached;
            var cashReceipt = await context.CashReceipts
                .Include(item => item.Invoices)
                .ThenInclude(item => item.Invoice)
                .FirstOrDefaultAsync(item => item.Id == this.Id);
            cashReceipt.Balance = cashReceipt.GetBalance();
            await context.SaveChangesAsync();
            context.Entry(cashReceipt).State = EntityState.Detached;
            context.Entry(this).State = state;
            this.Balance = cashReceipt.Balance;
            return cashReceipt.Balance;
        }

        public async Task<QuickBooksSyncResult> EnsureCompanyInQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            var result = await this.Company.EnsureCustomerInQuickBooks(quickBooksConnector, _context);
            return result;
        }

        public async Task<QuickBooksSyncResult> DeleteFromQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (String.IsNullOrWhiteSpace(this.QuickBooksId) && String.IsNullOrWhiteSpace(this.QuickBooksPurchaseId))
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "There was no quickbooks id so this is probably not in quickbooks"
                };

            //first delete the expense
            if (!String.IsNullOrWhiteSpace(this.QuickBooksPurchaseId)) {
                try {
                    QuickBooks.Models.PurchaseResponse getResponseData = (await quickBooksConnector.GetResource("purchase", this.QuickBooksPurchaseId))
                        .ToObject<QuickBooks.Models.PurchaseResponse>();
                    this.QuickBooksPurchaseSyncToken = getResponseData.Purchase.SyncToken;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error getting sync token for bank fee expense for cash recipt id {this.Id}. Message was {ex.Message}"
                    };
                }

                dynamic expense;
                try {
                    var responseData = await quickBooksConnector.DeleteResource("payment", new {
                        Id = this.QuickBooksPurchaseId,
                        SyncToken = this.QuickBooksPurchaseSyncToken
                    });
                    expense = responseData.ToObject<QuickBooks.Models.PaymentDeleteResponse>().Payment;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error deleting from quickbooks bank fee expense for cash receipt Id = {this.Id}.  The message was {ex.Message}"
                    };
                }
                if (expense.Status == "Deleted") {
                    this.QuickBooksPurchaseId = null;
                    this.QuickBooksPurchaseSyncToken = null;
                    await _context.SaveChangesAsync();
                }
            }

            if (!String.IsNullOrWhiteSpace(this.QuickBooksId)) {
                try {
                    QuickBooks.Models.PaymentResponse getResponseData = (await quickBooksConnector.GetResource("payment", this.QuickBooksId))
                        .ToObject<QuickBooks.Models.PaymentResponse>();
                    this.QuickBooksSyncToken = getResponseData.Payment.SyncToken;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error getting the latest sync token for cash receipt {this.Id}. Message was {ex.Message}"
                    };
                }

                dynamic newPayment;
                try {
                    var responseData = await quickBooksConnector.DeleteResource("payment", new {
                        Id = this.QuickBooksId,
                        SyncToken = this.QuickBooksSyncToken
                    });
                    newPayment = responseData.ToObject<QuickBooks.Models.PaymentDeleteResponse>().Payment;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error deleting from quickbooks cash receipt Id = {this.Id}.  The message was {ex.Message}"
                    };
                }

                if (newPayment.Status == "Deleted") {
                    this.QuickBooksId = null;
                    this.QuickBooksSyncToken = null;
                    await _context.SaveChangesAsync();
                    return new QuickBooksSyncResult {
                        Succeeded = true,
                        Message = $"Deleted successfully the cash receipt from quickbooks"
                    };
                }
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = $"The quickbooks object status wasn't deleted.  Need to investigate. Status was: ${newPayment.Status}"
                };
            }
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = ""
            };
        }

        public async Task<QuickBooksSyncResult> SyncWithQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context, bool syncInvoice = false) {
            if (this.CurrencyOptionId != 1)
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = $"The cashreceipt was not synced because the currency option was not set to USD"
                };

            var cashReceipt = await _context.CashReceipts.AsNoTracking()
                .Include(item => item.PaymentMethod)
                .Include(item => item.Invoices)
                    .ThenInclude(item => item.Invoice)
                        .ThenInclude(item => item.LineItems)
                            .ThenInclude(item => item.Product)
                .Include(item => item.Invoices)
                    .ThenInclude(item => item.Invoice)
                        .ThenInclude(item => item.Company)
                .Include(item => item.TransactionFeePaymentBankAccount)
                .Include(item => item.Company)
                .Include(item => item.BankAccount)
                .FirstAsync(item => item.Id == this.Id);

            if (cashReceipt.BankAccount.Name.Contains("AIB")) {
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = $"The cashreceipt was not synced because the bank account was for europe"
                };
            }

            var status = await cashReceipt.EnsureCompanyInQuickBooks(quickBooksConnector, _context);
            if (!status.Succeeded)
                return status;

            //First synchronize the cc processing fee if there is one
            if (!String.IsNullOrWhiteSpace(cashReceipt.QuickBooksPurchaseId)) {
                //get sync token and make sure they match
                try {
                    QuickBooks.Models.PurchaseResponse getResponseData = (await quickBooksConnector.GetResource("purchase", cashReceipt.QuickBooksPurchaseId))
                        .ToObject<QuickBooks.Models.PurchaseResponse>();
                    cashReceipt.QuickBooksPurchaseSyncToken = getResponseData.Purchase.SyncToken;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error getting sync token for cash recipt id {this.Id}. Message was {ex.Message}"
                    };
                }
            }

            //only synchronize a fee if there is either a) a fee, or b) a fee was previously recorded
            if (cashReceipt.TransactionFeeAmount != 0 || !String.IsNullOrWhiteSpace(cashReceipt.QuickBooksPurchaseId)) {
                dynamic newPurchase;
                try {
                    dynamic data = await quickBooksConnector.PostResource("purchase", cashReceipt.GetQuickBooksPurchase(quickBooksConnector, _context));
                    newPurchase = data.ToObject<QuickBooks.Models.PurchaseResponse>().Purchase;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error syncing the fee expense for cash receipt {this.Id}. Message was {ex.Message}"
                    };
                }
                try {
                    _context.Entry(this).State = EntityState.Modified;
                    this.QuickBooksPurchaseId = newPurchase.Id;
                    this.QuickBooksPurchaseSyncToken = newPurchase.SyncToken;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"The expense fee was synced, but there was an error saving it to the database " + ex.Message,
                        AdditionalData = ex.StackTrace
                    };
                }
            }


            //Now synchronize the cash receipt
            if (!String.IsNullOrWhiteSpace(cashReceipt.QuickBooksId)) {
                //first have to get the object to make sure have latest sync token
                try {
                    QuickBooks.Models.PaymentResponse getResponseData = (await quickBooksConnector.GetResource("payment", cashReceipt.QuickBooksId))
                        .ToObject<QuickBooks.Models.PaymentResponse>();
                    cashReceipt.QuickBooksSyncToken = getResponseData.Payment.SyncToken;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error getting the latest sync token for cash receipt {this.Id}. Message was {ex.Message}"
                    };
                }
            }
            dynamic responseData;
            try {
                responseData = await quickBooksConnector.PostResource("payment", await cashReceipt.GetQuickBooksObject(quickBooksConnector, _context));
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = $"Error syncing cash receipt {this.Id}. Message was {ex.Message}"
                };
            }

            if (syncInvoice && cashReceipt.Invoices.Count > 0) {
                foreach (var invoiceCashReceipt in cashReceipt.Invoices) {
                    var invoice = await _context.Invoices.AsNoTracking().FirstOrDefaultAsync(item => item.Id == invoiceCashReceipt.InvoiceId);
                    await invoice.SyncWithQuickBooks(quickBooksConnector, _context);
                }
            }

            try {
                var newPayment = responseData.ToObject<QuickBooks.Models.PaymentResponse>().Payment;
                _context.Entry(this).State = EntityState.Modified;
                this.QuickBooksId = newPayment.Id;
                this.QuickBooksSyncToken = newPayment.SyncToken;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Sync was successful, but error saving the information to the Gideon database" + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = "Success"
            };
        }

        public async Task<QuickBooks.Models.QuickBooksPayment> GetQuickBooksObject(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            var lines = new List<QuickBooks.Models.PaymentLine>();
            if (this.Invoices.Any() && !String.IsNullOrWhiteSpace(this.Invoices.First().Invoice.QuickBooksId)) {
                foreach (var invoiceCashReceipt in this.Invoices) {
                    // await invoiceCashReceipt.Invoice.SyncWithQuickBooks(quickBooksConnector, _context);
                    lines.Add(new QuickBooks.Models.PaymentLine {
                        Amount = invoiceCashReceipt.Amount,
                        LinkedTxn = new List<QuickBooks.Models.QuickBooksLinkedTxn>{
                            new QuickBooks.Models.QuickBooksLinkedTxn{
                                TxnId = invoiceCashReceipt.Invoice.QuickBooksId,
                                TxnType = "Invoice"
                            }
                        }
                    });
                }
            }
            var paymentObj = new QuickBooks.Models.QuickBooksPayment {
                Id = this.QuickBooksId,
                SyncToken = this.QuickBooksSyncToken,
                CustomerRef = new QuickBooks.Models.CustomerRef {
                    value = this.Company.QuickBooksCustomerId
                },
                DepositToAccountRef = new QuickBooks.Models.DepositToAccountRef {
                    value = BankAccount.QuickBooksId
                },

                TxnDate = this.DateReceived.HasValue ? this.DateReceived.Value.ToString("yyyy-MM-dd") : null,
                TotalAmt = this.Amount,
                UnappliedAmt = this.Balance,
                Line = lines,
                PaymentRefNum = this.ReferenceNumber
            };
            if (this.PaymentMethod != null) {
                paymentObj.PaymentMethodRef = new QuickBooks.Models.PaymentMethodRef {
                    value = this.PaymentMethod.QuickBooksId
                };
            }
            return paymentObj;
        }
        public QuickBooks.Models.Purchase GetQuickBooksPurchase(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            var purchase = new QuickBooks.Models.Purchase {
                Id = this.QuickBooksPurchaseId,
                SyncToken = this.QuickBooksSyncToken,
                PaymentType = "Cash",
                AccountRef = new QuickBooks.Models.AccountRef {
                    value = this.TransactionFeePaymentBankAccount.QuickBooksId        //paypal account id "207"
                },
                Line = new List<QuickBooks.Models.PurchaseLine> {
                    new QuickBooks.Models.PurchaseLine{
                        DetailType = "AccountBasedExpenseLineDetail",
                        Amount = this.TransactionFeeAmount,
                        AccountBasedExpenseLineDetail = new QuickBooks.Models.PurchaseAccountBasedExpenseLineDetail{
                            TaxCodeRef = new QuickBooks.Models.TaxCodeRef {
                                value = "NON"
                            },
                            AccountRef = new QuickBooks.Models.AccountRef{
                                value = QuickBooksConnector.DefaultBankFeeExpenseAccountId
                            }
                        }
                    }
                }
            };
            return purchase;
        }
    }


    class CashReceiptDBConfiguration : IEntityTypeConfiguration<CashReceipt> {
        public void Configure(EntityTypeBuilder<CashReceipt> modelBuilder) {
            modelBuilder.HasOne(item => item.Company)
                .WithMany(item => item.CashReceipts)
                .HasForeignKey(item => item.CompanyId);

            modelBuilder.HasOne(item => item.BankAccount).WithMany().HasForeignKey(item => item.BankAccountId);
            modelBuilder.HasOne(item => item.PaymentMethod).WithMany().HasForeignKey(item => item.SalesOrderPaymentMethodId);
            modelBuilder.HasOne(item => item.TransactionFeePaymentBankAccount).WithMany().HasForeignKey(item => item.TransactionFeePaymentBankAccountId);

            modelBuilder.HasIndex(item => item.CurrencyOptionId);
        }
    }
}