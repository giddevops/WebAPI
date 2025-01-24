using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using WebApi.Features.Controllers;

namespace GidIndustrial.Gideon.WebApi.Models {

    public class QuickBooksSyncResult {
        public bool Succeeded;
        public string Message;
        public dynamic AdditionalData;
    }

    public class QuickBooksApiInfo {
        public string RefreshToken;
        public DateTime RefreshTokenExpiresAt;
        public DateTime LastUpdated;
        public string RealmId;
    }

    public class QuickBooksConnector {
        private QuickBooksApiInfo _quickBooksApiInfo;
        public IServiceProvider _services;

        public static string RedirectUrl = "http://localhost:58263/QuickBooks/OauthRedirect";

        public static string url = "https://sandbox-quickbooks.api.intuit.com/v3/company/";
        public static string AccessToken;
        public static DateTime? AccessTokenExpires;
        public static HttpClient client = new HttpClient();

        //these are for invoices
        public static string WireTransferProductId = "84";
        // public static string SalesTaxProductId = "41";
        // public static string ShippingAndHandlingProductId = "24";
        public static string ExpediteFeeProductId = "85";
        public static string RefundProductId = "86";

        // public static string InvoiceAccountId = "128";

        //These are for bills
        public static string WireTransferExpenseAccountId = "196";
        public static string SalesTaxExpenseAccountId = "198";
        public static string ShippingAndHandlingExpenseAccountId = "199";
        public static string ExpediteFeeExpenseAccountId = "197";

        public static string DefaultProductIncomingAccountId = "201"; //4000
        public static string DefaultProductAssetAccountId = null; //
        public static string DefaultProductExpenseAccountId = "192"; //5000 //must be cost of goods sold account

        public static string DefaultBankFeeExpenseAccountId = "161";

        public static string ClientId = "Q0exRgwiP75EwfxmQuIga9OWlTf6lv6YWF8ZyBNzlRbO6iYMNp";
        public static string ClientSecret = "LKYEPkWY7UcG0ZI3Nsij49q2BgeX8y4aCCWvBKtV";
        public static bool isLiveMode = false;

        // public const string CashDisbursementsAccountId = "138";
        // public const string WireTransferIncomeAccountId = "143";
        // public const string SalesTaxIncomeAccountId = "142";
        // public const string ShippingAndHandlingIncomeAccountId = "135";
        // public const string ExpediteFeeIncomeAccountId = "145";

        public static void EnableLiveMode() {
            Console.WriteLine("ENABLING LIVE QUICKBOOKS MODE");

            url = "https://quickbooks.api.intuit.com/v3/company/";
            isLiveMode = true;

            WireTransferProductId = "1062";
            // SalesTaxProductId = "518";
            // ShippingAndHandlingProductId = "1064";
            ExpediteFeeProductId = "1065";

            RefundProductId = "1066";

            WireTransferExpenseAccountId = "149"; // this is for bills
            SalesTaxExpenseAccountId = "81";
            ShippingAndHandlingExpenseAccountId = "27";
            ExpediteFeeExpenseAccountId = "150";

            DefaultProductIncomingAccountId = "1"; //4000   "138";
            // DefaultProductAssetAccountId = null; //"134";
            DefaultProductExpenseAccountId = "26"; //5000

            DefaultBankFeeExpenseAccountId = "5";

            // services.Get
            ClientId = "Q0lOKD7lJBshce04WU6NPQaxMCJylvo6U7z8GHbdTE1e6s1dP7";
            ClientSecret = "VZiCfAO5ngCsYAB17QqaQn8JMzJuN9zQMkj2UCOX";

            QuickBooksConnector.RedirectUrl = "https://gideon-api.gidindustrial.com/QuickBooks/OauthRedirect";
        }

        public QuickBooksConnector(IServiceProvider services, IHostingEnvironment environment) {
            _services = services;
            using (var _context = _services.CreateScope().ServiceProvider.GetRequiredService<AppDBContext>()) {
                this.FetchQuickBooksApiInfo(_context);
            }

            if (environment.IsDevelopment()) {

            } else {
                QuickBooksConnector.EnableLiveMode();
            }
        }

        public void SetQuickBooksApiInfo(QuickBooksApiInfo info) {
            this._quickBooksApiInfo = info;
        }

        public void FetchQuickBooksApiInfo(AppDBContext _context) {
            var configInfo = _context.Configs.FirstOrDefault(item => item.Id == 1);
            if (configInfo == null || String.IsNullOrWhiteSpace(configInfo.Data))
                return;

            var dataString = Encryption.DecryptString(configInfo.Data);
            var quickBooksApiInfo = JsonConvert.DeserializeObject<QuickBooksApiInfo>(dataString);
            // throw new Exception("The quickbooks api info is " + JsonConvert.SerializeObject(quickBooksApiInfo));
            this._quickBooksApiInfo = quickBooksApiInfo;
        }

        public async Task<Newtonsoft.Json.Linq.JObject> QueryResource(string resourceName, string query) {
            return JsonConvert.DeserializeObject(await this.DoQuickBooksApiRequest(resourceName, "GET", null, query));
        }

        public async Task<Newtonsoft.Json.Linq.JObject> GetResource(string resourceName, dynamic requestData) {
            return JsonConvert.DeserializeObject(await this.DoQuickBooksApiRequest(resourceName, "GET", requestData));
        }

        public async Task<Newtonsoft.Json.Linq.JObject> PostResource(string resourceName, dynamic requestData) {
            return JsonConvert.DeserializeObject(await this.DoQuickBooksApiRequest(resourceName, "CREATE", requestData));
        }

        public async Task<Newtonsoft.Json.Linq.JObject> DeleteResource(string resourceName, dynamic requestData) {
            return JsonConvert.DeserializeObject(await this.DoQuickBooksApiRequest(resourceName, "DELETE", requestData));
        }
        public async Task GetQuickBooksInvoices(AppDBContext context) {
            var result = await this.QueryResource("invoice", "select count(*) from invoice");
            var resultParsed = result.ToObject<QuickBooks.Models.CountResponse>();
            int count = resultParsed.QueryResponse.totalCount;
            Console.WriteLine("Count is " + count);

            var maxResults = 100;
            for (var i = 1; i <= count; i += maxResults) {
                result = await this.QueryResource("invoice", $"select * from invoice order by id asc startposition {i} maxresults {maxResults}");
                var existingProductInfo = result.ToObject<QuickBooks.Models.InvoiceQueryResponseContainer>();

                foreach (var invoice in existingProductInfo.QueryResponse.Invoice) {
                    var matchingInvoice = await context.QuickBooksInvoices.FirstOrDefaultAsync(item => item.Id == invoice.Id);
                    if (matchingInvoice != null)
                        continue;
                    Console.WriteLine("Adding invoice " + invoice.Id);
                    context.QuickBooksInvoices.Add(invoice);
                }
                await context.SaveChangesAsync();
            }
        }

        public async Task GetQuickBooksCashReceipts(AppDBContext context) {
            var result = await this.QueryResource("cashReceipt", "select count(*) from payment");
            var resultParsed = result.ToObject<QuickBooks.Models.CountResponse>();
            int count = resultParsed.QueryResponse.totalCount;
            Console.WriteLine("Cash Receipt Count is " + count);

            var maxResults = 100;
            for (var i = 1; i <= count; i += maxResults) {
                result = await this.QueryResource("payment", $"select * from payment order by id asc startposition {i} maxresults {maxResults}");
                var existingProductInfo = result.ToObject<QuickBooks.Models.PaymentQueryResponseContainer>();

                foreach (var payment in existingProductInfo.QueryResponse.Payment) {
                    var matchingPayment = await context.QuickBooksPayments.FirstOrDefaultAsync(item => item.Id == payment.Id);
                    if (matchingPayment != null)
                        continue;
                    Console.WriteLine("Adding payment " + payment.Id);
                    context.QuickBooksPayments.Add(payment);
                }
                await context.SaveChangesAsync();
            }
        }
        public async Task ReSyncAllInvoices(AppDBContext context) {
            var invoices = await context.Invoices.Where(item => item.QuickBooksId != null).ToListAsync();
            Console.WriteLine("Num Invoices to be synced = ", invoices.Count);
            foreach (var invoice in invoices) {
                var result = await invoice.SyncWithQuickBooks(this, context);
                if (result.Succeeded != true) {
                    context.InvoiceSyncLogItems.Add(new InvoiceSyncLogItem {
                        CreatedAt = DateTime.UtcNow,
                        InvoiceId = invoice.Id,
                        QuickBooksErrorMessage = result.Message
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
        public async Task SyncQB(AppDBContext context) {
            QuickBooksConnector.EnableLiveMode();
            bool dryRun = false;

            // //first change cash receipt type to payment method
            // var cashReceipts = await context.CashReceipts.ToListAsync();
            // foreach (var cashReceipt in cashReceipts) {
            //     if (cashReceipt.SalesOrderPaymentMethodId == null && cashReceipt.CashReceiptTypeId != null) {
            //         switch (cashReceipt.CashReceiptTypeId) {
            //             case 1: //		Credit Card	
            //                 cashReceipt.SalesOrderPaymentMethodId = 1;
            //                 break;
            //             case 2: //		Check	
            //                 cashReceipt.SalesOrderPaymentMethodId = 4;
            //                 break;
            //             case 3: //		Wire	
            //                 cashReceipt.SalesOrderPaymentMethodId = 2;
            //                 break;
            //             case 4: //		PayPal	
            //                 cashReceipt.SalesOrderPaymentMethodId = 3;
            //                 break;
            //             case 5: //		ACH	
            //                 cashReceipt.SalesOrderPaymentMethodId = 12;
            //                 break;
            //             default:
            //                 break;
            //         }
            //     }

            //     if (!dryRun)
            //         await context.SaveChangesAsync();
            //     context.Entry(cashReceipt).State = EntityState.Detached;
            // }


            await this.GetNewToken();
            Console.WriteLine("QQQQQQQQQBBBBBB>>>>>>>>>>>>>>>>>>>>>");
            Console.WriteLine(QuickBooksConnector.AccessToken);
            Console.WriteLine("WASSSSS");

            // var query = 

            // await this.GetQuickBooksInvoices(context);
            // await this.GetQuickBooksCashReceipts(context);
            await this.ReSyncAllInvoices(context);
            return;


            var InvoicesWithoutQuickBooksId = new List<Invoice> { };
            var CashReceiptsWithoutQuickBooksId = new List<CashReceipt> { };
            var CashReceiptsCouldntFindMatchingInQuickBooks = new List<CashReceipt> { };
            var CashReceiptsWithAmbigousMatches = new List<CashReceipt> { };
            var CashReceiptsFoundAMatch = new List<CashReceipt> { };




            var invoicesToMatch = await context.Invoices
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Where(item => item.GidLocationOptionId == 1)
                .Where(item => item.CurrencyOptionId == 1)
                .Where(item => item.SentAt != null)
                // .ThenInclude(item => item.)
                .Where(item => item.Id > 10000).ToListAsync();

            foreach (var invoice in invoicesToMatch) {
                if (String.IsNullOrWhiteSpace(invoice.QuickBooksId)) {
                    InvoicesWithoutQuickBooksId.Add(invoice);
                    Console.WriteLine("!!!!No quickbooks id for invoice " + invoice.QuickBooksId);
                }

                if (!dryRun)
                    await invoice.SyncWithQuickBooks(this, context, true);

                foreach (var invoiceCashReceipt in invoice.CashReceipts) {
                    var cashReceipt = invoiceCashReceipt.CashReceipt;

                    if (String.IsNullOrWhiteSpace(cashReceipt.QuickBooksId)) {
                        CashReceiptsWithoutQuickBooksId.Add(cashReceipt);
                        Console.WriteLine("!!!!No quickbooks id found for cash receipt Id " + cashReceipt.Id.ToString());
                        //search for it first
                        Newtonsoft.Json.Linq.JObject existingCashReceiptInfoResponseData = await this.QueryResource("payment", $"SELECT * FROM Payment WHERE TotalAmt='{cashReceipt.Amount}'");
                        var existingCashReceiptInfo = existingCashReceiptInfoResponseData.ToObject<QuickBooks.Models.PaymentQueryResponseContainer>();
                        if (existingCashReceiptInfo.QueryResponse.Payment != null && existingCashReceiptInfo.QueryResponse.Payment.Count > 0) {
                            if (existingCashReceiptInfo.QueryResponse.Payment.Count > 1) {
                                CashReceiptsWithAmbigousMatches.Add(cashReceipt);
                            } else {
                                CashReceiptsFoundAMatch.Add(cashReceipt);
                            }
                        } else {
                            Console.WriteLine("NOT FOUND");
                            CashReceiptsCouldntFindMatchingInQuickBooks.Add(cashReceipt);
                        }
                    }
                }
            }
            Console.WriteLine("Invoices to add : \n");
        }

        public async Task<dynamic> DoQuickBooksApiRequest(string resourceName, string action, dynamic requestData = null, string query = null, int? entityId = null) {

            using (var _context = _services.CreateScope().ServiceProvider.GetRequiredService<AppDBContext>()) {
                // this.FetchQuickBooksApiInfo(_context);
                var logItem = new QuickBooksLogItem();

                //make sure not expired
                if (QuickBooksConnector.AccessTokenExpires == null || QuickBooksConnector.AccessTokenExpires < DateTime.UtcNow) {
                    await this.GetNewToken();
                }

                var url = QuickBooksConnector.url;// "https://sandbox-quickbooks.api.intuit.com/v3/company/";
                                                  // var url = "https://quickbooks.api.intuit.com/v3/company/";

                url += this._quickBooksApiInfo.RealmId + "/";
                if (query != null) {
                    url += "query?query=" + WebUtility.UrlEncode(query);
                } else {
                    url += resourceName;
                }
                if (action == "DELETE") {
                    url += "?operation=delete";
                } else if (action == "GET" && requestData != null)
                    url += "/" + requestData;

                if (entityId != null) {
                    url += "/" + entityId;
                }

                if (url.Contains("?"))
                    url += "&minorversion=28";
                else
                    url += "?minorversion=28";

                TelemetryClient telemetryClient = new TelemetryClient();

                if (action == "CREATE") {
                    var body = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>QB REQUEST CONTENT<<<<<<<<<<<<<<<<<<<<<<");
                    Console.WriteLine("POST: " + url);
                    Console.WriteLine(body);
                    logItem.RequestMessage = url + "\n" + body;
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>END QB CONTENT<<<<<<<<<<<<<<<<<<<<<<<<<<");

                    telemetryClient.TrackTrace("QB Request: url:" + url + "\n\nBody:\n " + body, SeverityLevel.Information, new Dictionary<string, string> { { "url", "url" } });

                    var request = new HttpRequestMessage {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Post,
                        Content = content
                    };
                    request.Headers.Add("Authorization", "Bearer " + QuickBooksConnector.AccessToken);
                    request.Headers.Add("Accept", "application/json");

                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    logItem.ResponseMessage = responseBody;
                    logItem.StatusCode = (int)response.StatusCode;
                    if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300) {
                        Console.WriteLine(responseBody);
                        logItem.Successful = false;
                        _context.Add(logItem);
                        await _context.SaveChangesAsync();
                        throw new Exception(responseBody);
                    }
                    Console.WriteLine("[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[ QB RESPONSE ]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]]");
                    Console.WriteLine(responseBody);
                    Console.WriteLine("[[[[[[[[[[[[[[[[[[[[[[[[[[[[[[ END QB RESPONSE ]]]]]]]]]]]]]]]]]]]]]]]]]]]]]");

                    return responseBody;
                } else if (action == "DELETE") {
                    var body = JsonConvert.SerializeObject(requestData);
                    var content = new StringContent(body, System.Text.Encoding.UTF8, "application/json");
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>QB DELETE REQUEST CONTENT<<<<<<<<<<<<<<<<<<<<<<");
                    Console.WriteLine(body);
                    logItem.RequestMessage = "POST: " + url + "\n" + body;
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>END QB CONTENT<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    telemetryClient.TrackTrace("QB Request: url:" + url + "\n\nBody:\n " + body, SeverityLevel.Information, new Dictionary<string, string> { { "url", "url" } });
                    var request = new HttpRequestMessage {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Post,
                        Content = content
                    };
                    request.Headers.Add("Authorization", "Bearer " + QuickBooksConnector.AccessToken);
                    request.Headers.Add("Accept", "application/json");

                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    logItem.ResponseMessage = responseBody;
                    logItem.StatusCode = (int)response.StatusCode;
                    if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300) {
                        logItem.Successful = false;
                        _context.Add(logItem);
                        await _context.SaveChangesAsync();
                        throw new Exception(responseBody);
                    }
                    return responseBody;
                } else if (action == "GET") {
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>QB GET URL<<<<<<<<<<<<<<<<<<<<<<");
                    Console.WriteLine(url);
                    logItem.RequestMessage = "GET: " + url;
                    Console.WriteLine(">>>>>>>>>>>>>>>>>>>>>>>>END QB GET URL<<<<<<<<<<<<<<<<<<<<<<<<<<");
                    telemetryClient.TrackTrace("QB Request: url:" + url, SeverityLevel.Information, new Dictionary<string, string> { { "url", "url" } });
                    var request = new HttpRequestMessage {
                        RequestUri = new Uri(url),
                        Method = HttpMethod.Get
                    };
                    request.Headers.Add("Authorization", "Bearer " + QuickBooksConnector.AccessToken);
                    request.Headers.Add("Accept", "application/json");

                    var response = await client.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    logItem.ResponseMessage = responseBody;
                    logItem.StatusCode = (int)response.StatusCode;
                    if ((int)response.StatusCode < 200 || (int)response.StatusCode >= 300) {
                        Console.WriteLine(responseBody);
                        logItem.Successful = false;
                        _context.Add(logItem);
                        await _context.SaveChangesAsync();
                        throw new Exception(responseBody);
                    }
                    return responseBody;
                }
            }
            return null;
        }

        public async Task<dynamic> GetNewToken() {
            Console.WriteLine(">>>>GETTING NEW TOKEN<<<<<<");
            if (this._quickBooksApiInfo == null || String.IsNullOrWhiteSpace(this._quickBooksApiInfo.RefreshToken)) {
                var dbContext = _services.CreateScope().ServiceProvider.GetRequiredService<AppDBContext>();
                this.FetchQuickBooksApiInfo(dbContext);
                // throw new Exception("Refresh token is null " + Newtonsoft.Json.JsonConvert.SerializeObject(this._quickBooksApiInfo));
            }
            if (String.IsNullOrWhiteSpace(this._quickBooksApiInfo.RefreshToken)) {
                throw new Exception("Refresh token is null! You need to Re-Authorize quickbooks (admin settings => reauthorize quickbooks) " + Newtonsoft.Json.JsonConvert.SerializeObject(this._quickBooksApiInfo));
            }

            var clientId = QuickBooksConnector.ClientId;
            var clientSecret = QuickBooksConnector.ClientSecret;

            var url = "https://oauth.platform.intuit.com/oauth2/v1/tokens/bearer";
            var bodyParams = new Dictionary<string, string>{
                    { "grant_type", "refresh_token" },
                    { "refresh_token", this._quickBooksApiInfo.RefreshToken }
                };

            var authorizationHeader = System.Convert.ToBase64String(System.Text.ASCIIEncoding.ASCII.GetBytes(clientId + ":" + clientSecret));
            var content = new FormUrlEncodedContent(bodyParams);
            var request = new HttpRequestMessage {
                RequestUri = new Uri(url),
                Method = HttpMethod.Post,
                Content = content
            };
            request.Headers.Add("Authorization", "Basic " + authorizationHeader);
            // client.DefaultRequestHeaders.Add("Authorization", "Basic " + authorizationHeader);


            var response = await client.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();
            QuickBooksTokenResponse responseBodyParsed = JsonConvert.DeserializeObject<QuickBooksTokenResponse>(responseBody);

            this._quickBooksApiInfo.RefreshToken = responseBodyParsed.refresh_token;
            this._quickBooksApiInfo.RefreshTokenExpiresAt = DateTime.UtcNow.AddSeconds(responseBodyParsed.X_refresh_token_expires_in).AddSeconds(-15);
            this._quickBooksApiInfo.LastUpdated = DateTime.UtcNow;
            QuickBooksConnector.AccessToken = responseBodyParsed.access_token;
            QuickBooksConnector.AccessTokenExpires = DateTime.UtcNow.AddSeconds(responseBodyParsed.expires_in - 15);

            if (string.IsNullOrWhiteSpace(this._quickBooksApiInfo.RefreshToken)) {
                throw new Exception("Attempted to get a new token, but the refresh token was null " + responseBody);
            }

            var quickBooksApiInfoString = JsonConvert.SerializeObject(this._quickBooksApiInfo);

            var _context = _services.CreateScope().ServiceProvider.GetRequiredService<AppDBContext>();

            // var _context = _services.GetRequiredService<AppDBContext>();
            var quickBooksConfig = await _context.Configs.FirstOrDefaultAsync(item => item.Id == 1);
            quickBooksConfig.Data = Encryption.EncryptData(quickBooksApiInfoString);
            await _context.SaveChangesAsync();

            return responseBody;
        }

    }
}