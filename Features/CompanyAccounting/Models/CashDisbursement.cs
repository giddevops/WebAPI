using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class CashDisbursement {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? CashDisbursementReasonOptionId { get; set; }
        public int? CurrencyOptionId { get; set; }

        public int? PurchaseOrderPaymentMethodId { get; set; }

        public decimal Amount { get; set; }
        public decimal Balance { get; set; }

        public int? PaymentAccountId { get; set; }
        public PaymentAccount PaymentAccount { get; set; }

        public string ReferenceNumber { get; set; }
        public string Note { get; set; }

        public string QuickBooksBillPaymentId { get; set; }
        public string QuickBooksBillPaymentSyncToken { get; set; }
        public string QuickBooksRefundReceiptId { get; set; }
        public string QuickBooksRefundReceiptSyncToken { get; set; }

        // public Transact

        public DateTime? DateDisbursed { get; set; }

        public int? SalesOrderId { get; set; }

        public int? RmaId { get; set; }
        public Rma Rma { get; set; }

        public int? CashDisbursementTypeId { get; set; }

        // public int?
        public List<BillCashDisbursement> Bills { get; set; }
        public int? BankAccountId { get; set; }
        public BankAccount BankAccount { get; set; }
        // public int? CashDisbursementTypeId { get; set; }
        public int? CreditCardTransactionId { get; set; }

        public decimal GetBalance() {
            return this.Amount - this.Bills.Sum(item => item.Amount);
        }

        public async Task<QuickBooksSyncResult> EnsureVendorInQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            return await this.Company.EnsureVendorInQuickBooks(quickBooksConnector, _context);
        }
        public async Task<QuickBooksSyncResult> EnsureCustomerInQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            return await this.Company.EnsureCustomerInQuickBooks(quickBooksConnector, _context);
        }

        public async Task<QuickBooksSyncResult> SyncWithQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (this.CurrencyOptionId != 1)
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "No currency option id"
                };
            var cashDisbursement = await _context.CashDisbursements.AsNoTracking()
                .Include(item => item.Bills)
                    .ThenInclude(item => item.Bill)
                        .ThenInclude(item => item.LineItems)
                            .ThenInclude(item => item.Product)
                .Include(item => item.Bills)
                    .ThenInclude(item => item.Bill)
                        .ThenInclude(item => item.Company)
                .Include(item => item.Rma)
                    .ThenInclude(item => item.LineItems)
                        .ThenInclude(item => item.SalesOrderLineItem)
                            .ThenInclude(item => item.Product)
                .Include(item => item.Rma)
                    .ThenInclude(item => item.LineItems)
                        .ThenInclude(item => item.InventoryItem)
                .Include(item => item.Company)
                .Include(item => item.BankAccount)
                .FirstAsync(item => item.Id == this.Id);


            //if it's for a bill, create a BillPayment in quickbooks.  If it's for a refund, create a RefundReceipt
            //The BillPayment will need to be applied to an actual bill, but for a RefundReceipt, that's it.
            if (this.CashDisbursementReasonOptionId == CashDisbursementReasonOption.Bill) {
                if (this.Bills.Count == 0 && !String.IsNullOrWhiteSpace(this.QuickBooksBillPaymentId)) { //they have unrelated all bills, need to remove from quickbooks
                    return await this.DeleteFromQuickBooks(quickBooksConnector, _context);
                } else if (this.Bills.Count == 0) {
                    return new QuickBooksSyncResult {
                        Succeeded = true,
                        Message = "No bills so not importing"
                    };
                }
                var result = await cashDisbursement.EnsureVendorInQuickBooks(quickBooksConnector, _context);
                if (!result.Succeeded)
                    return result;

                if (!String.IsNullOrWhiteSpace(cashDisbursement.QuickBooksBillPaymentId)) {
                    //first have to get the object to make sure have latest sync token
                    try {
                        QuickBooks.Models.BillPaymentResponse getResponseData = (await quickBooksConnector.GetResource("billpayment", cashDisbursement.QuickBooksBillPaymentId))
                            .ToObject<QuickBooks.Models.BillPaymentResponse>();
                        cashDisbursement.QuickBooksBillPaymentSyncToken = getResponseData.BillPayment.SyncToken;
                    }
                    catch (Exception ex) {
                        return new QuickBooksSyncResult {
                            Succeeded = false,
                            Message = $"Error getting latest sync token for disbursement(billpayment) id {this.Id}. Message: {ex.Message}"
                        };
                    }
                }

                dynamic newBillPayment;
                try {
                    dynamic responseData = await quickBooksConnector.PostResource("billpayment", await cashDisbursement.GetBillPaymentQuickBooksObject(quickBooksConnector, _context));
                    newBillPayment = responseData.ToObject<QuickBooks.Models.BillPaymentResponse>().BillPayment;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error syncing disbursement(billpayment) id {this.Id}. Message: {ex.Message}"
                    };
                }
                try {
                    this.QuickBooksBillPaymentId = newBillPayment.Id;
                    this.QuickBooksBillPaymentSyncToken = newBillPayment.SyncToken;
                    _context.Entry(this).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Sync success, but failed to save to Gideon database for disbursement(billpayment) id {this.Id}. Message: {ex.Message}"
                    };
                }
            } else if (this.CashDisbursementReasonOptionId == CashDisbursementReasonOption.Refund) {
                if (this.RmaId == null && !String.IsNullOrWhiteSpace(this.QuickBooksRefundReceiptId)) {
                    var result = await this.DeleteFromQuickBooks(quickBooksConnector, _context);
                    if (!result.Succeeded)
                        return result;
                } else if (this.RmaId == null) {
                    return new QuickBooksSyncResult {
                        Succeeded = true,
                        Message = "No rma so not doing anything"
                    };
                }
                await cashDisbursement.EnsureCustomerInQuickBooks(quickBooksConnector, _context);
                if (!String.IsNullOrWhiteSpace(cashDisbursement.QuickBooksRefundReceiptId)) {
                    try {
                        //first have to get the object to make sure have latest sync token
                        QuickBooks.Models.RefundReceiptResponse getResponseData = (await quickBooksConnector.GetResource("refundreceipt", cashDisbursement.QuickBooksRefundReceiptId))
                            .ToObject<QuickBooks.Models.RefundReceiptResponse>();
                        cashDisbursement.QuickBooksRefundReceiptSyncToken = getResponseData.RefundReceipt.SyncToken;
                    }
                    catch (Exception ex) {
                        return new QuickBooksSyncResult {
                            Succeeded = false,
                            Message = $"Error getting latest sync token for disbursement(refundreceipt) id {this.Id}. Message: {ex.Message}"
                        };
                    }
                }
                dynamic newRefundReceipt;
                try {
                    dynamic responseData = await quickBooksConnector.PostResource("refundreceipt", await cashDisbursement.GetRefundReceiptQuickBooksObject(quickBooksConnector, _context));
                    newRefundReceipt = responseData.ToObject<QuickBooks.Models.RefundReceiptResponse>().RefundReceipt;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error syncing disbursement(refundreceipt) id {this.Id}. Message: {ex.Message}"
                    };
                }

                try {
                    this.QuickBooksRefundReceiptId = newRefundReceipt.Id;
                    this.QuickBooksRefundReceiptSyncToken = newRefundReceipt.SyncToken;
                    _context.Entry(this).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Sync success, but failed to save to Gideon database for disbursement(refundreceipt) id {this.Id}. Message: {ex.Message}"
                    };
                }
            }
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = $"Success"
            };
        }

        public async Task<QuickBooksSyncResult> DeleteFromQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (String.IsNullOrWhiteSpace(this.QuickBooksBillPaymentId) && String.IsNullOrWhiteSpace(this.QuickBooksRefundReceiptId)) {
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "not in qb yet so nothing to do (no id)"
                };
            }
            if (!String.IsNullOrWhiteSpace(this.QuickBooksBillPaymentId)) {
                QuickBooks.Models.BillPaymentDeleted newBillPayment;
                try {
                    dynamic responseData = await quickBooksConnector.DeleteResource("billpayment", new {
                        Id = this.QuickBooksBillPaymentId,
                        SyncToken = this.QuickBooksBillPaymentSyncToken
                    });
                    newBillPayment = responseData.ToObject<QuickBooks.Models.BillPaymentDeleteResponse>().BillPayment;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error deleteing cashDisbursement(billpayment) from qb id {this.Id}. Message: {ex.Message}"
                    };
                }
                if (newBillPayment.Status == "Deleted") {
                    try {
                        this.QuickBooksBillPaymentId = null;
                        this.QuickBooksBillPaymentSyncToken = null;
                        _context.Entry(this).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex) {
                        return new QuickBooksSyncResult {
                            Succeeded = false,
                            Message = $"deleted successfully from qb but error saving in Gideon cashDisbursement(refundreceipt) from qb id {this.Id}. Message: {ex.Message}"
                        };
                    }
                }
            } else if (!String.IsNullOrWhiteSpace(this.QuickBooksRefundReceiptId)) {
                QuickBooks.Models.RefundReceiptDeleted newRefundReceipt;
                try {
                    dynamic responseData = await quickBooksConnector.DeleteResource("refundreceipt", new {
                        Id = this.QuickBooksRefundReceiptId,
                        SyncToken = this.QuickBooksRefundReceiptSyncToken
                    });
                    newRefundReceipt = responseData.ToObject<QuickBooks.Models.RefundReceiptDeleteResponse>().RefundReceipt;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = $"Error deleteing cashDisbursement(refundreceipt) from qb id {this.Id}. Message: {ex.Message}"
                    };
                }
                if (newRefundReceipt.Status == "Deleted") {
                    try {
                        this.QuickBooksRefundReceiptId = null;
                        this.QuickBooksRefundReceiptSyncToken = null;
                        _context.Entry(this).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                    catch (Exception ex) {
                        return new QuickBooksSyncResult {
                            Succeeded = false,
                            Message = $"deleted successfully from qb but error saving in Gideon cashDisbursement(refundreceipt) from qb id {this.Id}. Message: {ex.Message}"
                        };
                    }
                }
            }
            return new QuickBooksSyncResult {
                Succeeded = false,
                Message = "nothing to do"
            };
        }

        public async Task<QuickBooks.Models.BillPayment> GetBillPaymentQuickBooksObject(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            var Lines = new List<QuickBooks.Models.BillPaymentLine> { };
            foreach (var billCashDisbursement in this.Bills) {
                await billCashDisbursement.Bill.SyncWithQuickBooks(quickBooksConnector, _context);
                Lines.Add(new QuickBooks.Models.BillPaymentLine {
                    Amount = billCashDisbursement.Amount,
                    LinkedTxn = new List<QuickBooks.Models.QuickBooksLinkedTxn>{
                        new QuickBooks.Models.QuickBooksLinkedTxn{
                            TxnId = billCashDisbursement.Bill.QuickBooksId,
                            TxnType = "Bill"
                        }
                    }
                });
            }
            return new QuickBooks.Models.BillPayment {
                Id = this.QuickBooksBillPaymentId,
                SyncToken = this.QuickBooksBillPaymentSyncToken,
                TxnDate = this.DateDisbursed.HasValue ? this.DateDisbursed.Value.ToString("yyyy-MM-dd") : null,
                TotalAmt = this.Amount,
                VendorRef = new QuickBooks.Models.VendorRef {
                    value = this.Company.QuickBooksVendorId
                },
                PayType = "Check",
                Line = Lines,
                CheckPayment = new QuickBooks.Models.CheckPayment {
                    BankAccountRef = new QuickBooks.Models.BankAccountRef {
                        value = this.BankAccount.QuickBooksId
                    }
                }
            };
        }

        public async Task<QuickBooks.Models.RefundReceipt> GetRefundReceiptQuickBooksObject(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            var Lines = new List<QuickBooks.Models.RefundReceiptLine> { };

            /** Not going to do individual line items */
            // //Rma line items are individual inventory items.  
            // //It is not desirable to list each inventory item separately, so they need to be condensed by salees order line item
            // var lineItems = this.Rma.GetGroupedLineItems();
            // foreach (var lineItem in lineItems) {
            //     await lineItem.Product.EnsureInQuickBooks(quickBooksConnector, _context);
            //     Lines.Add(new QuickBooks.Models.RefundReceiptLine {
            //         Amount = lineItem.Price*lineItem.Quantity,
            //         DetailType = "SalesItemLineDetail",
            //         SalesItemLineDetail = new QuickBooks.Models.SalesItemLineDetail{
            //             UnitPrice = lineItem.Price,
            //             Qty = lineItem.Quantity,
            //             ItemRef = new QuickBooks.Models.ItemRef{
            //                 value = lineItem.Product.QuickBooksId
            //             }
            //         }
            //     });
            // }

            Lines.Add(new QuickBooks.Models.RefundReceiptLine {
                Amount = this.Amount,
                DetailType = "SalesItemLineDetail",
                SalesItemLineDetail = new QuickBooks.Models.SalesItemLineDetail {
                    UnitPrice = this.Amount,
                    Qty = 1,
                    ItemRef = new QuickBooks.Models.ItemRef {
                        value = QuickBooksConnector.RefundProductId
                    }
                }
            });

            return new QuickBooks.Models.RefundReceipt {
                Id = this.QuickBooksRefundReceiptId,
                SyncToken = this.QuickBooksRefundReceiptSyncToken,
                TxnDate = this.CreatedAt.HasValue ? this.CreatedAt.Value.ToString("yyyy-MM-dd") : null,
                TotalAmt = this.Rma.CreditAmount ?? 0,
                CustomerRef = new QuickBooks.Models.CustomerRef {
                    value = this.Company.QuickBooksCustomerId
                },
                Line = Lines,
                DepositToAccountRef = new QuickBooks.Models.DepositToAccountRef {
                    value = this.BankAccount.QuickBooksId
                }
            };
        }
    }

    class CashDisbursementDBConfiguration : IEntityTypeConfiguration<CashDisbursement> {
        public void Configure(EntityTypeBuilder<CashDisbursement> modelBuilder) {
            modelBuilder.HasOne(item => item.Company)
                .WithMany(item => item.CashDisbursements)
                .HasForeignKey(item => item.CompanyId);

            modelBuilder.HasOne(item => item.PaymentAccount)
                .WithMany(item => item.CashDisbursements)
                .HasForeignKey(item => item.PaymentAccountId);

            modelBuilder.HasOne(item => item.BankAccount)
                .WithMany()
                .HasForeignKey(item => item.BankAccountId);

            modelBuilder.HasOne(item => item.Rma)
                .WithMany()
                .HasForeignKey(item => item.RmaId);

            modelBuilder.HasIndex(item => item.CurrencyOptionId);
        }
    }
}