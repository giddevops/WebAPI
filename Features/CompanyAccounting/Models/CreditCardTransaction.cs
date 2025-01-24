using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;

namespace GidIndustrial.Gideon.WebApi.Models {

    public enum CreditCardTransactionType {
        Authorization = 1,
        Capture = 2,
        AuthorizationAndCapture = 3,
        Refund = 4
    }

    public class CreditCardTransaction {
        public int? Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int? CreatedById { get; set; }

        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }

        public int? SalesOrderId { get; set; }
        public SalesOrder SalesOrder { get; set; }

        public int? CurrencyOptionId { get; set; }

        public decimal Amount { get; set; }
        public decimal? AmountCaptured { get; set; }

        public bool? NotCaptured { get; set; }

        public decimal? AmountRefunded { get; set; }
        public bool? FullyRefunded { get; set; }

        public DateTime? VoidedAt { get; set; }
        public DateTime? CapturedAt { get; set; }

        public string ResultCode { get; set; }
        public string ResultMessageCode { get; set; }
        public string ResultMessageText { get; set; }

        public string TransactionErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public string OverallStatus { get; set; }

        public bool Succeeded { get; set; }

        public string Last4 { get; set; }
        public string CardBrand { get; set; }

        public CreditCardTransactionType CreditCardTransactionType { get; set; } 

        public int? BankAccountId { get; set; }

        public string PaymentProfileId { get; set; }
        public int? CompanyId { get; set; }
        public string TransactionId { get; set; }
        public string RelatedTransactionId { get; set; }

        public async Task<dynamic> FetchTransactionDetails(IConfiguration _configuration) {
            var requestData = new {
                getTransactionDetailsRequest = new {
                    merchantAuthentication = new {
                        name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                        transactionKey = AuthorizeNetApiRequestor.transactionKey
                    },
                    transId = this.TransactionId
                }
            };
            var response = await GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.AuthorizeNetApiRequestor.DoApiRequest(requestData);
            dynamic responseBody = Newtonsoft.Json.Linq.JObject.Parse(response);
            return responseBody;
        }
    }

    class CreditCardTransactionDBConfiguration : IEntityTypeConfiguration<CreditCardTransaction> {
        public void Configure(EntityTypeBuilder<CreditCardTransaction> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
            // modelBuilder.HasOne(item => item.CreditCard)
            //     .WithMany(item => item.Authorizations)
            //     .HasForeignKey(item => item.CreditCardId);

            modelBuilder.HasIndex(item => item.CreditCardTransactionType);
            modelBuilder.HasIndex(item => item.TransactionId);

            modelBuilder.HasOne(item => item.SalesOrder)
                .WithMany(item => item.CreditCardTransactions)
                .HasForeignKey(item => item.SalesOrderId);

            modelBuilder
                .HasOne(item => item.Invoice)
                .WithMany(item => item.CreditCardTransactions)
                .HasForeignKey(item => item.InvoiceId);
        }
    }
}