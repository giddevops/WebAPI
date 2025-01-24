using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class SalesOrderPaymentMethod {
        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Value { get; set; }
        public bool? Locked { get; set; }
        
        public string QuickBooksSyncToken { get; set; }
        public string QuickBooksId { get; set; }

        public bool Active { get; set; }

        /// <summary>
        /// Corresponds to quickbooks Type which can be either CREDIT_CARD or NON_CREDIT_CARD
        /// </summary>
        /// <value></value>
        public string Type { get; set; }

        public static async Task GetAllFromQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            Newtonsoft.Json.Linq.JObject existingPaymentMethodResponseData = (await quickBooksConnector.QueryResource("paymentmethod", "SELECT * FROM PaymentMethod"));
            var existingPaymentMethods = existingPaymentMethodResponseData.ToObject<QuickBooks.Models.PaymentMethodQueryResponseContainer>().QueryResponse.PaymentMethod;
            foreach (var qbPaymentMethod in existingPaymentMethods) {
                var localSalesOrderPaymentMethod = await _context.SalesOrderPaymentMethods.FirstOrDefaultAsync(item => item.QuickBooksId == qbPaymentMethod.Id);
                if (localSalesOrderPaymentMethod == null) {
                    localSalesOrderPaymentMethod = await _context.SalesOrderPaymentMethods.FirstOrDefaultAsync(item => item.Value.ToLower() == qbPaymentMethod.Name.ToLower());
                    if (localSalesOrderPaymentMethod != null && String.IsNullOrEmpty(localSalesOrderPaymentMethod.QuickBooksId)) {
                        localSalesOrderPaymentMethod.QuickBooksId = qbPaymentMethod.Id;
                        localSalesOrderPaymentMethod.UpdateFromQuickbooksObject(qbPaymentMethod);
                        continue;
                    }
                    await _context.SalesOrderPaymentMethods.AddAsync(SalesOrderPaymentMethod.GetSalesOrderPaymentMethodFromQuickBooksObject(qbPaymentMethod));
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task SyncWithQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context) {
            if (!String.IsNullOrWhiteSpace(this.QuickBooksId)) {
                //first have to get the object to make sure have latest sync token
                QuickBooks.Models.PaymentMethodResponse getResponseData = (await quickBooksConnector.GetResource("paymentmethod", this.QuickBooksId))
                    .ToObject<QuickBooks.Models.PaymentMethodResponse>();
                this.QuickBooksSyncToken = getResponseData.PaymentMethod.SyncToken;
            }
            dynamic responseData = await quickBooksConnector.PostResource("paymentmethod", this.GetQuickBooksObject());
            var newPaymentMethod = responseData.ToObject<QuickBooks.Models.PaymentMethodResponse>().PaymentMethod;
            this.QuickBooksId = newPaymentMethod.Id;
            this.QuickBooksSyncToken = newPaymentMethod.SyncToken;
        }

        public QuickBooks.Models.PaymentMethod GetQuickBooksObject() {
            return new QuickBooks.Models.PaymentMethod {
                Id = this.QuickBooksId,
                SyncToken = this.QuickBooksSyncToken,
                Name = this.Value,
                Type = this.Type,
                Active = this.Active
            };
        }
        public static SalesOrderPaymentMethod GetSalesOrderPaymentMethodFromQuickBooksObject(QuickBooks.Models.PaymentMethod qbPaymentMethod) {
            return new SalesOrderPaymentMethod {
                QuickBooksId = qbPaymentMethod.Id,
                QuickBooksSyncToken = qbPaymentMethod.SyncToken,
                Value = qbPaymentMethod.Name,
                Type = qbPaymentMethod.Type,
                Active = qbPaymentMethod.Active
            };
        }
        public void UpdateFromQuickbooksObject(QuickBooks.Models.PaymentMethod qbPaymentMethod) {
            this.QuickBooksSyncToken = qbPaymentMethod.SyncToken;
            this.Value = qbPaymentMethod.Name;
            this.Type = qbPaymentMethod.Type;
            this.Active = qbPaymentMethod.Active;
        }
    }
}
