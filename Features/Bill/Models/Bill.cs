using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Bill {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? CompanyId { get; set; }
        public Company Company { get; set; }

        public int? PurchaseOrderId { get; set; }
        public PurchaseOrder PurchaseOrder { get; set; }

        public decimal Balance { get; set; }

        public DateTime? DateDue { get; set; }
        public DateTime? EnteredAt { get; set; }

        public string QuickBooksId { get; set; }
        public string QuickBooksSyncToken { get; set; }

        public int? CurrencyOptionId { get; set; }
        public int? GidLocationOptionId { get; set; }

        public decimal ShippingAndHandlingFee { get; set; }
        public decimal SalesTax { get; set; }
        public decimal WireTransferFee { get; set; }
        public decimal ExpediteFee { get; set; }
        public decimal DiscountPercent { get; set; }

        public List<BillLineItem> LineItems { get; set; }
        public List<BillCashDisbursement> CashDisbursements { get; set; }

        public List<BillAttachment> Attachments { get; set; }
        public List<BillChatMessage> ChatMessages { get; set; }

        public decimal GetTotal() {
            decimal total = LineItems.Sum(item => item.Price * (100 - item.DiscountPercent) / 100 * item.Quantity);

            total += ShippingAndHandlingFee;
            total += SalesTax;
            total += WireTransferFee;
            total += ExpediteFee;
            total += DiscountPercent;

            return total;
        }

        public decimal GetBalance() {
            var amountPaid = CashDisbursements.Sum(item => item.Amount);
            return GetTotal() - amountPaid;
        }

        public async Task<QuickBooksSyncResult> EnsureLineItemsAndCompanyInQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (this.LineItems != null && this.LineItems.Count == 0)
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "There are no line items in this bill so can't sync with QB"
                };
            foreach (var lineItem in this.LineItems) {
                var result = await lineItem.Product.EnsureInQuickBooks(quickBooksConnector, _context);
                if (!result.Succeeded)
                    return result;
            }
            var data = await this.Company.EnsureVendorInQuickBooks(quickBooksConnector, _context);
            if (!data.Succeeded)
                return data;
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = "Line Items and Company synced successfully"
            };
        }

        public async Task<QuickBooksSyncResult> DeleteFromQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (String.IsNullOrWhiteSpace(this.QuickBooksId))
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "No quickbooks synce Id was found for this. It may already have been removed from QB, or it might not have been ever successfully added in the first place"
                };

            //delete from qb
            dynamic newBill;
            try {
                dynamic responseData = await quickBooksConnector.DeleteResource("bill", new {
                    Id = this.QuickBooksId,
                    SyncToken = this.QuickBooksSyncToken
                });
                newBill = responseData.ToObject<QuickBooks.Models.BillDeleteResponse>().Bill;
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Error deleteing quickbooks object. Bill Id = " + this.Id.ToString() + ".  The error was " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }

            //save the fact that it was deleted to gideon
            try {
                if (newBill.Status == "Deleted") {
                    this.QuickBooksId = null;
                    this.QuickBooksSyncToken = null;
                    _context.Entry(this).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Quickbooks object was deleted, but there was an error saving that fact to the Gideon Database " + this.Id.ToString() + ".  The error was " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = "Bill deleted successfully"
            };
        }

        //Quickbooks api uses a POST request for both Create and Update, so I use the same method to create/update
        public async Task<QuickBooksSyncResult> SyncWithQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (this.GidLocationOptionId != 1)
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Item needs a gid location option before it can be synced (need to make sure it's for USA because europe ones aren't synced)"
                };

            var bill = await _context.Bills.AsNoTracking()
                    .Include(item => item.Company)
                    .Include(item => item.LineItems)
                        .ThenInclude(item => item.Product)
                    .FirstAsync(item => item.Id == this.Id);
            if (bill.LineItems != null && bill.LineItems.Count == 0)
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Bill has no line items so it can't by synced with quickbooks"
                };

            var result = await bill.EnsureLineItemsAndCompanyInQuickBooks(quickBooksConnector, _context);
            if (!result.Succeeded)
                return result;

            if (!String.IsNullOrWhiteSpace(bill.QuickBooksId)) {
                //first have to get the object to make sure have latest sync token
                try {
                    QuickBooks.Models.BillResponse getResponseData = (await quickBooksConnector.GetResource("bill", bill.QuickBooksId))
                        .ToObject<QuickBooks.Models.BillResponse>();
                    bill.QuickBooksSyncToken = getResponseData.Bill.SyncToken;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = "Error getting the latest quickbooks sync token for the Bill Id " + this.Id + ".  The error message was " + ex.Message,
                    AdditionalData = ex.StackTrace
                    };
                }
            }

            QuickBooks.Models.BillResponse responseData = null;
            try {
                responseData = (await quickBooksConnector.PostResource("bill", bill.GetQuickBooksObject()))
                   .ToObject<QuickBooks.Models.BillResponse>();
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Error syncing Bill Id " + this.Id + ".  The error message was " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }

            try {
                var newBill = responseData.Bill;
                this.QuickBooksId = newBill.Id;
                this.QuickBooksSyncToken = newBill.SyncToken;
                _context.Entry(this).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "The bill was synced successfully, but there was an error updating that fact with Gideon"
                };
            }
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = "Bill updated in quickbooks successfully"
            };
        }

        public QuickBooks.Models.Bill GetQuickBooksObject() {
            var qbLineItems = new List<QuickBooks.Models.BillLine>();
            var index = 0;
            foreach (var lineItem in this.LineItems) {
                ++index;
                qbLineItems.Add(new QuickBooks.Models.BillLine {
                    Amount = (lineItem.Quantity * lineItem.Price) * (1 - .01m * this.DiscountPercent),
                    Description = lineItem.Description,
                    LineNum = index,
                    DetailType = "ItemBasedExpenseLineDetail",
                    ItemBasedExpenseLineDetail = new QuickBooks.Models.ItemBasedExpenseLineDetail {
                        ItemRef = new QuickBooks.Models.ItemRef {
                            value = lineItem.Product.QuickBooksId
                        },
                        UnitPrice = lineItem.Price,
                        Qty = lineItem.Quantity
                    }
                });
            }

            //now add shipping, expedite fee, wire transfer fee, and sales tax
            // Shipping
            qbLineItems.Add(new QuickBooks.Models.BillLine {
                Amount = this.ShippingAndHandlingFee,
                DetailType = "AccountBasedExpenseLineDetail",
                AccountBasedExpenseLineDetail = new QuickBooks.Models.AccountBasedExpenseLineDetail {
                    AccountRef = new QuickBooks.Models.AccountRef {
                        value = QuickBooksConnector.ShippingAndHandlingExpenseAccountId
                    },
                    TaxInclusiveAmt = this.ShippingAndHandlingFee
                }
            });
            // Wire Transfer
            qbLineItems.Add(new QuickBooks.Models.BillLine {
                Amount = this.WireTransferFee,
                DetailType = "AccountBasedExpenseLineDetail",
                AccountBasedExpenseLineDetail = new QuickBooks.Models.AccountBasedExpenseLineDetail {
                    AccountRef = new QuickBooks.Models.AccountRef {
                        value = QuickBooksConnector.WireTransferExpenseAccountId
                    },
                    TaxInclusiveAmt = this.WireTransferFee
                }
            });
            // Sales Tax
            qbLineItems.Add(new QuickBooks.Models.BillLine {
                Amount = this.SalesTax,
                DetailType = "AccountBasedExpenseLineDetail",
                AccountBasedExpenseLineDetail = new QuickBooks.Models.AccountBasedExpenseLineDetail {
                    AccountRef = new QuickBooks.Models.AccountRef {
                        value = QuickBooksConnector.SalesTaxExpenseAccountId
                    },
                    TaxInclusiveAmt = this.SalesTax
                }
            });
            // Expedite Fee
            qbLineItems.Add(new QuickBooks.Models.BillLine {
                Amount = this.ExpediteFee,
                DetailType = "AccountBasedExpenseLineDetail",
                AccountBasedExpenseLineDetail = new QuickBooks.Models.AccountBasedExpenseLineDetail {
                    AccountRef = new QuickBooks.Models.AccountRef {
                        value = QuickBooksConnector.ExpediteFeeExpenseAccountId
                    },
                    TaxInclusiveAmt = this.ExpediteFee
                }
            });
            return new QuickBooks.Models.Bill {
                Id = this.QuickBooksId,
                SyncToken = this.QuickBooksSyncToken,
                DocNumber = this.Id.ToString(),
                Line = qbLineItems,
                VendorRef = new QuickBooks.Models.VendorRef {
                    value = this.Company.QuickBooksVendorId
                },
                DueDate = DateDue.HasValue ? DateDue.Value.ToString("yyyy-MM-dd") : null,
            };
        }
    }

    class BillDBConfiguration : IEntityTypeConfiguration<Bill> {
        public void Configure(EntityTypeBuilder<Bill> modelBuilder) {
            modelBuilder.HasOne(item => item.PurchaseOrder)
            .WithMany(item => item.Bills)
            .HasForeignKey(item => item.PurchaseOrderId);

            modelBuilder.HasOne(item => item.Company)
                .WithMany(item => item.Bills)
                .HasForeignKey(item => item.CompanyId);

            modelBuilder.HasIndex(item => item.Balance);
            modelBuilder.HasIndex(item => item.CreatedAt);
            modelBuilder.HasIndex(item => item.CreatedById);
            
        }
    }
}


