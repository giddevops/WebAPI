using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class PaymentTerm {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        //The name of the payment term
        public string Name { get; set; }

        //For QB - STANDARD - if DueDays is not null vs DATE_DRIVEN
        public string Type { get; set; }

        //Number of days its due in
        public int DueDays { get; set; }

        public int? DiscountDays { get; set; }
        public decimal DiscountPercent { get; set; }

        public int? DueNextMonthDays { get; set; }
        public int? DiscountDayOfMonth { get; set; }
        public int? DayOfMonthDue { get; set; }

        public string QuickBooksSyncToken { get; set; }
        public string QuickBooksId { get; set; }

        public bool Active { get; set; }
        public bool? Locked { get; set; }

        public static async Task GetAllFromQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            Newtonsoft.Json.Linq.JObject existingTermResponseData = (await quickBooksConnector.QueryResource("term", "SELECT * FROM Term"));
            var existingTerms = existingTermResponseData.ToObject<QuickBooks.Models.TermQueryResponseContainer>().QueryResponse.Term;
            foreach (var qbTerm in existingTerms) {
                var localTerm = await _context.PaymentTerms.FirstOrDefaultAsync(item => item.QuickBooksId == qbTerm.Id);
                if (localTerm == null) {
                    localTerm = await _context.PaymentTerms.FirstOrDefaultAsync(item => item.Name.ToLower() == qbTerm.Name.ToLower());
                    if (localTerm != null && String.IsNullOrEmpty(localTerm.QuickBooksId)) {
                        localTerm.QuickBooksId = qbTerm.Id;
                        localTerm.UpdateFromQuickbooksObject(qbTerm);
                        continue;
                    }
                    await _context.PaymentTerms.AddAsync(PaymentTerm.GetPaymentTermFromQuickBooksObject(qbTerm));
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<QuickBooksSyncResult> SyncWithQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (!String.IsNullOrWhiteSpace(this.QuickBooksId)) {
                //first have to get the object to make sure have latest sync token
                try {
                    QuickBooks.Models.TermResponse getResponseData = (await quickBooksConnector.GetResource("term", this.QuickBooksId))
                        .ToObject<QuickBooks.Models.TermResponse>();
                    this.QuickBooksSyncToken = getResponseData.Term.SyncToken;
                }
                catch (Exception ex) {
                    return new QuickBooksSyncResult {
                        Succeeded = false,
                        Message = "Unable to get term from qb: " + ex.Message,
                    AdditionalData = ex.StackTrace
                    };
                }
            }
            dynamic newTerm;
            try {
                dynamic responseData = await quickBooksConnector.PostResource("term", this.GetQuickBooksObject());
                newTerm = responseData.ToObject<QuickBooks.Models.TermResponse>().Term;
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult{
                    Succeeded = false,
                    Message = "Error syncing with quickbooks: " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }
            this.QuickBooksId = newTerm.Id;
            this.QuickBooksSyncToken = newTerm.SyncToken;
            return new QuickBooksSyncResult{
                Succeeded = true,
                Message = "Success"
            };
        }

        public QuickBooks.Models.Term GetQuickBooksObject() {
            return new QuickBooks.Models.Term {
                Id = this.QuickBooksId,
                SyncToken = this.QuickBooksSyncToken,
                Name = this.Name,
                DiscountPercent = this.DiscountPercent,
                DiscountDays = this.DiscountDays,
                Type = this.Type,
                Active = this.Active,
                DueDays = this.DueDays
            };
        }
        public static PaymentTerm GetPaymentTermFromQuickBooksObject(QuickBooks.Models.Term qbTerm) {
            return new PaymentTerm {
                QuickBooksId = qbTerm.Id,
                QuickBooksSyncToken = qbTerm.SyncToken,
                Name = qbTerm.Name,
                DiscountPercent = qbTerm.DiscountPercent,
                DiscountDays = qbTerm.DiscountDays,
                Type = qbTerm.Type,
                Active = qbTerm.Active,
                DueDays = qbTerm.DueDays
            };
        }
        public void UpdateFromQuickbooksObject(QuickBooks.Models.Term qbTerm) {
            this.QuickBooksSyncToken = qbTerm.SyncToken;
            this.Name = qbTerm.Name;
            this.DiscountPercent = qbTerm.DiscountPercent;
            this.DiscountDays = qbTerm.DiscountDays;
            this.Type = qbTerm.Type;
            this.Active = qbTerm.Active;
            this.DueDays = qbTerm.DueDays;
        }
    }
}