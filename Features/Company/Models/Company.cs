using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace GidIndustrial.Gideon.WebApi.Models {
    public class Company {
        public static HttpClient HttpClient { get; set; }

        public int Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string Name { get; set; }

        public string CustomerTypeId { get; set; }
        public int? AnnualRevenue { get; set; }
        public int? NumberOfEmployees { get; set; }
        public Boolean? AutoInserted { get; set; }

        public string DunsNumber { get; set; }
        public bool? ApprovedForTerms { get; set; }

        public bool? eBayVendor { get; set; }

        // public CompanyAddress BillingAddress { get; set; }
        // public CompanyAddress ShippingAddress { get; set; }

        public List<CompanyContact> CompanyContacts { get; set; }
        public List<CompanyCompany> CompanyCompanies { get; set; }
        public List<CompanyCompany> CompanyRelatedCompanies { get; set; }

        public List<CompanyAlias> Aliases { get; set; }

        // public List<Company> ChildCompanies { get; set; }
        public List<SalesOrder> SalesOrders { get; set; }

        public List<CompanyPhoneNumber> PhoneNumbers { get; set; }
        public List<CompanyEmailAddress> EmailAddresses { get; set; }
        public List<CompanyAddress> Addresses { get; set; } //Billing is always address 0, shipping is always address 1
        public List<CompanyNote> Notes { get; set; }
        public List<CompanyAttachment> Attachments { get; set; }

        // [ForeignKey("ParentCompanyId")]
        public int? ParentCompanyId { get; set; }
        public Company ParentCompany { get; set; }

        public string QuickBooksCustomerId { get; set; }
        public string QuickBooksCustomerSyncToken { get; set; }

        public string QuickBooksVendorId { get; set; }
        public string QuickBooksVendorSyncToken { get; set; }

        public List<Company> ChildCompanies { get; set; }
        public List<CompanyEventLogEntry> EventLogEntries { get; set; }
        public List<CompanyPortal> Portals { get; set; }

        public List<CashDisbursement> CashDisbursements { get; set; }
        public List<CashReceipt> CashReceipts { get; set; }
        public List<Credit> Credits { get; set; }
        // public List<CreditCardCharge> CreditCardCharges { get; set; }
        public List<Invoice> Invoices { get; set; }
        public List<Bill> Bills { get; set; }

        public string AuthorizeNetProfileId { get; set; }
        [NotMapped]
        public Libraries.AuthorizeNet.GetProfileResponse.AuthorizeNetProfile AuthorizeNetProfile { get; set; }

        //Credit cards are stored in authorize.net servers and have to be fetched separately
        [NotMapped]
        public List<CreditCard> CreditCards { get; set; }

        public string GetDefaultEmailAddress() {
            if (this.EmailAddresses.Count == 0)
                return null;
            return this.EmailAddresses.First().EmailAddress.Address;
        }

        public async Task<QuickBooksSyncResult> EnsureCustomerInQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context, bool update = false) {
            if (!String.IsNullOrWhiteSpace(this.QuickBooksCustomerId) && !update) {
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "Company (customer) already has a quickbooks Id. Assuming it's already in qb."
                };
            }
            if (this.PhoneNumbers == null || this.EmailAddresses == null) {
                var fullCompany = await _context.Companies.AsNoTracking()
                    .Include(item => item.PhoneNumbers)
                        .ThenInclude(item => item.PhoneNumber)
                    .Include(item => item.EmailAddresses)
                        .ThenInclude(item => item.EmailAddress)
                    .FirstAsync(item => item.Id == this.Id);

                this.EmailAddresses = fullCompany.EmailAddresses;
                this.PhoneNumbers = fullCompany.PhoneNumbers;
            }

            //first check by name for duplicates with the DisplayName field
            Newtonsoft.Json.Linq.JObject existingCustomerInfoResponseData = await quickBooksConnector.QueryResource("customer", $"SELECT * FROM Customer WHERE DisplayName='{this.Name}'");
            var existingCustomerInfo = existingCustomerInfoResponseData.ToObject<QuickBooks.Models.CustomerQueryResponseContainer>();
            if (existingCustomerInfo.QueryResponse.Customer != null && existingCustomerInfo.QueryResponse.Customer.Count > 0) {
                this.QuickBooksCustomerId = existingCustomerInfo.QueryResponse.Customer[0].Id;
                this.QuickBooksCustomerSyncToken = existingCustomerInfo.QueryResponse.Customer[0].SyncToken;
                _context.Entry(this).State = EntityState.Modified;
                Console.WriteLine("The company has existing quickbooks info");
                await _context.SaveChangesAsync();
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "A company with the same name was found as a customer in QuickBooks. Matching the two"
                };
            } else {
                Console.WriteLine("The company does not have new info");
            }


            try {
                // var parent = await _context.Products.Include(item => item..FirstOrDefaultAsync(item => item.Id == this.Id);
                dynamic responseData = await quickBooksConnector.PostResource("customer", new QuickBooks.Models.Customer {
                    Id = this.QuickBooksCustomerId,
                    SyncToken = this.QuickBooksCustomerSyncToken,
                    DisplayName = this.Name,
                    CompanyName = this.Name,
                    PrimaryPhone = new QuickBooks.Models.PrimaryPhone {
                        FreeFormNumber = this.PhoneNumbers.Count != 0 ? this.PhoneNumbers[0].PhoneNumber.Number : null
                    },
                    PrimaryEmailAddr = new QuickBooks.Models.PrimaryEmailAddr {
                        Address = this.EmailAddresses.Count != 0 ? this.EmailAddresses[0].EmailAddress.Address : null,
                    },
                    //BillAddr = this.BillingAddress == null ? null : new QuickBooks.Models.Address(this.BillingAddress.Address),
                    //ShipAddr = this.ShippingAddress == null ? null : new QuickBooks.Models.Address(this.ShippingAddress.Address),
                });
                var newCustomer = responseData.ToObject<QuickBooks.Models.CustomerResponse>().Customer;
                _context.Entry(this).State = EntityState.Modified;
                this.QuickBooksCustomerId = newCustomer.Id;
                this.QuickBooksCustomerSyncToken = newCustomer.SyncToken;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Error adding Company (customer) to QB. Company Id is " + this.Id + " Message is " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }
            return new QuickBooksSyncResult {
                Succeeded = true,
                Message = "Company Added Successfully"
            };
        }


        public async Task<QuickBooksSyncResult> EnsureVendorInQuickBooks(QuickBooksConnector quickBooksConnector, AppDBContext _context, bool update = false) {
            if (!String.IsNullOrWhiteSpace(this.QuickBooksVendorId) && !update) {
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "Company (vendor) already has a quickbooks Id so assuming it's already in qb"
                };
            }
            if (this.PhoneNumbers == null || this.EmailAddresses == null) {
                var fullCompany = await _context.Companies.AsNoTracking()
                    .Include(item => item.PhoneNumbers)
                        .ThenInclude(item => item.PhoneNumber)
                    .Include(item => item.EmailAddresses)
                        .ThenInclude(item => item.EmailAddress)
                    //.Include(item => item.ShippingAddress)
                    //    .ThenInclude(item => item.Address)
                    //.Include(item => item.BillingAddress)
                    //    .ThenInclude(item => item.Address)
                    .FirstAsync(item => item.Id == this.Id);

                this.EmailAddresses = fullCompany.EmailAddresses;
                this.PhoneNumbers = fullCompany.PhoneNumbers;
                //this.ShippingAddress = fullCompany.ShippingAddress;
                //this.BillingAddress = fullCompany.BillingAddress;
            }

            //first check by name for duplicates
            Newtonsoft.Json.Linq.JObject existingVendorInfoResponseData = await quickBooksConnector.QueryResource("vendor", $"SELECT * FROM Vendor WHERE DisplayName='{this.Name}'");
            var existingVendorInfo = existingVendorInfoResponseData.ToObject<QuickBooks.Models.VendorQueryResponseContainer>();
            if (existingVendorInfo.QueryResponse.Vendor != null && existingVendorInfo.QueryResponse.Vendor.Count > 0) {
                this.QuickBooksVendorId = existingVendorInfo.QueryResponse.Vendor[0].Id;
                this.QuickBooksVendorSyncToken = existingVendorInfo.QueryResponse.Vendor[0].SyncToken;
                _context.Entry(this).State = EntityState.Modified;
                Console.WriteLine("The vendor has existing quickbooks info");
                await _context.SaveChangesAsync();
                return new QuickBooksSyncResult {
                    Succeeded = true,
                    Message = "A vendor with the same name is already in quickbooks and the two were matched"
                };
            } else {
                Console.WriteLine("The company does not have new info");
            }

            try {
                // var parent = await _context.Products.Include(item => item..FirstOrDefaultAsync(item => item.Id == this.Id);
                dynamic responseData = await quickBooksConnector.PostResource("vendor", new QuickBooks.Models.Vendor {
                    Id = this.QuickBooksVendorId,
                    SyncToken = this.QuickBooksVendorSyncToken,
                    DisplayName = this.Name + " (V)",
                    CompanyName = this.Name,
                    PrimaryPhone = new QuickBooks.Models.PrimaryPhone {
                        FreeFormNumber = this.PhoneNumbers.Count != 0 ? this.PhoneNumbers[0].PhoneNumber.Number : null
                    },
                    PrimaryEmailAddr = new QuickBooks.Models.PrimaryEmailAddr {
                        Address = this.EmailAddresses.Count != 0 ? this.EmailAddresses[0].EmailAddress.Address : null,
                    },
                    //BillAddr = this.BillingAddress == null ? null : new QuickBooks.Models.Address(this.BillingAddress.Address),
                });
                var newVendor = responseData.ToObject<QuickBooks.Models.VendorResponse>().Vendor;
                _context.Entry(this).State = EntityState.Modified;
                this.QuickBooksVendorId = newVendor.Id;
                this.QuickBooksVendorSyncToken = newVendor.SyncToken;
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return new QuickBooksSyncResult {
                    Succeeded = false,
                    Message = "Error adding company (vendor) with quickbooks. CompanyId = " + this.Id + ".  Message was " + ex.Message,
                    AdditionalData = ex.StackTrace
                };
            }
            return new QuickBooksSyncResult{
                Succeeded = true,
                Message = "Company (vendor) added to QB succcessfully"
            };
        }


        public async Task<Libraries.AuthorizeNet.GetProfileResponse.AuthorizeNetProfile> GetAuthorizeNetProfile(AppDBContext _context) {
            //first check if this company has a profile
            if (this.AuthorizeNetProfileId == null) {
                return null;
            }
            var requestData = new {
                getCustomerProfileRequest = new {
                    merchantAuthentication = new {
                        name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                        transactionKey = AuthorizeNetApiRequestor.transactionKey
                    },
                    customerProfileId = this.AuthorizeNetProfileId,
                    includeIssuerInfo = "true"
                }
            };
            var response = await AuthorizeNetApiRequestor.DoApiRequest(requestData);
            Console.WriteLine(response);

            var responseData = JsonConvert.DeserializeObject<Libraries.AuthorizeNet.GetProfileResponse.AuthorizeNetProfile>(response);
            this.AuthorizeNetProfile = responseData;
            return responseData;
        }
        public async Task<GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.GetPaymentProfilesResponse.GetCustomerPaymentProfileListResponse> GetAuthorizeNetPaymentProfiles(AppDBContext _context) {
            if (this.AuthorizeNetProfileId == null) {
                return null;
            }
            //first check if this company has a profile
            if (this.AuthorizeNetProfileId == null) {
                return null;
            }
            var requestData = new Libraries.AuthorizeNet.GetPaymentProfilesRequest.GetPaymentProfilesRequest {
                getCustomerPaymentProfileListRequest = new Libraries.AuthorizeNet.GetPaymentProfilesRequest.GetCustomerPaymentProfileListRequest {
                    merchantAuthentication = new Libraries.AuthorizeNet.GetPaymentProfilesRequest.MerchantAuthentication {
                        name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                        transactionKey = AuthorizeNetApiRequestor.transactionKey
                    },

                    // sorting = new Libraries.AuthorizeNet.GetPaymentProfilesRequest.Sorting {
                    //     orderBy = "id",
                    //     orderDescending = "false"
                    // },
                    // paging = new Libraries.AuthorizeNet.GetPaymentProfilesRequest.Paging {
                    //     limit = "10",
                    //     offset = "1"
                    // }
                }
            };
            var response = await AuthorizeNetApiRequestor.DoApiRequest(requestData);
            Console.WriteLine(response);
            var responseData = JsonConvert.DeserializeObject<GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.GetPaymentProfilesResponse.GetCustomerPaymentProfileListResponse>(response);
            return responseData;
        }
        public async Task<Libraries.AuthorizeNet.CreateCustomerResponse.CreateCustomerResponse> CreateAuthorizeNetProfile(AppDBContext _context, Libraries.AuthorizeNet.CreateProfileRequest.OpaqueData opaqueData = null, Libraries.AuthorizeNet.CreateProfileRequest.CreditCard creditCard = null) {
            string email = null;

            //you can submit an email when creating an authorize net profile. So this code gets the email address if it exists
            if (this.EmailAddresses == null) {
                var company = await _context.Companies.Include(item => item.EmailAddresses).ThenInclude(item => item.EmailAddress).FirstOrDefaultAsync(item => item.Id == this.Id);
                this.EmailAddresses = company.EmailAddresses;
            }
            if (this.EmailAddresses.Count > 0) {
                email = this.EmailAddresses[0].EmailAddress.Address;
            }
            var requestData = new {
                createCustomerProfileRequest = new Libraries.AuthorizeNet.CreateProfileRequest.CreateCustomerProfileRequest {
                    merchantAuthentication = new Libraries.AuthorizeNet.CreateProfileRequest.MerchantAuthentication {
                        name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                        transactionKey = AuthorizeNetApiRequestor.transactionKey
                    },
                    profile = new Libraries.AuthorizeNet.CreateProfileRequest.Profile {
                        merchantCustomerId = this.Id.ToString(),
                        // description = "Profile description here",
                        email = email,
                        paymentProfiles = new Libraries.AuthorizeNet.CreateProfileRequest.PaymentProfiles {
                            customerType = "business",

                        }
                    },
                    validationMode = AuthorizeNetApiRequestor.ValidationMode
                }
            };
            if (opaqueData != null) {
                requestData.createCustomerProfileRequest.profile.paymentProfiles.payment = new Libraries.AuthorizeNet.CreateProfileRequest.OpaqueDataPayment {
                    opaqueData = opaqueData
                };
            } else if (creditCard != null) {
                requestData.createCustomerProfileRequest.profile.paymentProfiles.payment = new Libraries.AuthorizeNet.CreateProfileRequest.CreditCardPayment {
                    creditCard = creditCard
                };
            } else {
                throw new Exception("no payment opaqueData or creditCard");
            }

            var response = JsonConvert.DeserializeObject<Libraries.AuthorizeNet.CreateCustomerResponse.CreateCustomerResponse>(await AuthorizeNetApiRequestor.DoApiRequest(requestData));
            if (!String.IsNullOrWhiteSpace(response.customerProfileId)) {
                this.AuthorizeNetProfileId = response.customerProfileId;
                await _context.SaveChangesAsync();
            } else {
                throw new Exception("Error creating profile with authorize.net " + response.messages.message[0].text);
            }
            return response;
        }

        public async Task<Libraries.AuthorizeNet.CreatePaymentProfileResponse.CreatePaymentProfileResponse> AddPaymentMethod(AppDBContext _context, Libraries.AuthorizeNet.CreatePaymentProfileRequest.OpaqueData opaqueData = null, Libraries.AuthorizeNet.CreatePaymentProfileRequest.CreditCard creditCard = null) {
            if (this.AuthorizeNetProfile == null)
                await this.GetAuthorizeNetProfile(_context);
            var requestData = new Libraries.AuthorizeNet.CreatePaymentProfileRequest.CreatePaymentProfileRequest {
                createCustomerPaymentProfileRequest = new Libraries.AuthorizeNet.CreatePaymentProfileRequest.CreateCustomerPaymentProfileRequest {
                    merchantAuthentication = new Libraries.AuthorizeNet.CreatePaymentProfileRequest.MerchantAuthentication {
                        name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                        transactionKey = AuthorizeNetApiRequestor.transactionKey
                    },
                    customerProfileId = this.AuthorizeNetProfileId,
                    paymentProfile = new Libraries.AuthorizeNet.CreatePaymentProfileRequest.PaymentProfile {
                        defaultPaymentProfile = true
                    },
                    validationMode = AuthorizeNetApiRequestor.ValidationMode,
                    
                }
            };
            if (opaqueData != null) {
                requestData.createCustomerPaymentProfileRequest.paymentProfile.payment = new Libraries.AuthorizeNet.CreatePaymentProfileRequest.OpaqueDataPayment {
                    opaqueData = opaqueData
                };
            } else if (creditCard != null) {
                requestData.createCustomerPaymentProfileRequest.paymentProfile.payment = new Libraries.AuthorizeNet.CreatePaymentProfileRequest.CreditCardPayment {
                    creditCard = creditCard
                };
            } else {
                throw new Exception("no payment opaqueData or creditCard");
            }
            var response = JsonConvert.DeserializeObject<Libraries.AuthorizeNet.CreatePaymentProfileResponse.CreatePaymentProfileResponse>(await AuthorizeNetApiRequestor.DoApiRequest(requestData));
          
            if (response.messages.resultCode != "Ok") {
                if (!String.IsNullOrWhiteSpace(response.customerPaymentProfileId))
                {
                    return response;
                }
                throw new Exception("There was an error adding payment profile " + response.messages.message[0].text);
            }
            return response;
        }
    }


    /// <summary>
    /// set up createdAt, fluent api config
    /// </summary>
    class CompanyDBConfiguration : IEntityTypeConfiguration<Company> {
        public void Configure(EntityTypeBuilder<Company> modelBuilder) {
            modelBuilder
                .Property(b => b.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.HasOne(c => c.ParentCompany)
                .WithMany(c => c.ChildCompanies)
                .HasForeignKey(c => c.ParentCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.HasIndex(item => item.Name);
            modelBuilder.HasIndex(item => item.CreatedAt);
        }
    }
}
