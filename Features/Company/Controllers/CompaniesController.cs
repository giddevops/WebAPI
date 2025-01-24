using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.Extensions.Configuration;
using GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Companies")]
    public class CompaniesController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration _configuration;
        public QuickBooksConnector _quickBooksConnector;

        public CompaniesController(AppDBContext context, IConfiguration configuration, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _configuration = configuration;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: Companies
        [HttpGet]
        public ListResult GetCompanies(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string name = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from company in _context.Companies select company;
            query = query.OrderByDescending(c => c.CreatedAt);

            if (name != null) {
                name = name.Trim();
                if (name.Length > 0) {
                    query = query.Where(item => item.Name.StartsWith(name));
                }
            }
            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "Name":
                    query = sortAscending ? query.OrderBy(item => item.Name) : query.OrderByDescending(item => item.Name);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(searchString)) {
                searchString = searchString.Trim();
                query = query.Where(item =>
                    // item.Name.StartsWith(searchString) ||
                    // item.PhoneNumbers.Any(cPhone => cPhone.PhoneNumber.Number.StartsWith(searchString)) ||
                    // item.EmailAddresses.Any(cEmail => cEmail.EmailAddress.Address.StartsWith(searchString))

                    EF.Functions.Like(item.Name, searchString + '%') ||
                    item.PhoneNumbers.Any(cPhone => EF.Functions.Like(cPhone.PhoneNumber.Number, searchString + '%')) ||
                    item.EmailAddresses.Any(cEmail => EF.Functions.Like(cEmail.EmailAddress.Address, searchString + '%'))
                );
            }

            var count = -1;
            if (String.IsNullOrWhiteSpace(searchString))
                count = query.Count();

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = count
            };
        }

        /// <summary>
        /// Find companies with matching name OR alias
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("GetByName")]
        public async Task<IActionResult> GetCompanyByName([FromQuery] string name) {
            name = name.Trim();
            return Ok(await _context.Companies.FirstOrDefaultAsync(item =>
                String.Equals(item.Name, name, StringComparison.OrdinalIgnoreCase) ||
                item.Aliases.Any(companyAlias => String.Equals(companyAlias.Alias, name, StringComparison.OrdinalIgnoreCase))
            ));
        }

        [HttpPost("{id}/GetOrCreatePortalFromListing")]
        public async Task<IActionResult> GetOrCreatePortalFromListing([FromRoute] int Id, [FromBody] Listing listing) {
            var company = await _context.Companies
                .Include(item => item.Portals)
                    .ThenInclude(item => item.Portal)
                .FirstOrDefaultAsync(item => item.Id == Id);
            if (company == null) {
                return NotFound("Company was not found with that id");
            }

            var companyPortal = company.Portals.Find(item => item.Portal.Name == listing.Portal);
            if (companyPortal != null) {
                return Ok(companyPortal);
            }

            var portal = await _context.Portals.FirstOrDefaultAsync(item => String.Equals(item.Name, listing.Portal, StringComparison.OrdinalIgnoreCase));
            if (portal == null) {
                portal = new Portal {
                    Name = listing.Portal,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Portals.Add(portal);
                await _context.SaveChangesAsync();
            }

            companyPortal = new CompanyPortal {
                PortalId = portal.Id,
                CompanyId = company.Id,
                // Rating = listing.
                Username = listing.Seller != null ? listing.Seller.Name : listing.Portal
            };
            _context.CompanyPortals.Add(companyPortal);
            await _context.SaveChangesAsync();
            return Ok(companyPortal);
        }

        [HttpPost("CreateCompanyFromListing")]
        public async Task<IActionResult> CreateCompanyFromListing([FromBody] Listing listing) {

            var portal = await _context.Portals.FirstOrDefaultAsync(item => String.Equals(item.Name, listing.Portal, StringComparison.OrdinalIgnoreCase));
            if (portal == null) {
                portal = new Portal {
                    Name = listing.Portal,
                    CreatedAt = DateTime.UtcNow
                };
                _context.Portals.Add(portal);
                await _context.SaveChangesAsync();
            }
            var companyName = "";
            if (listing.Seller != null && listing.Seller.Name != null) {
                companyName = listing.Seller.Name;
            } else {
                companyName = listing.Portal;
            }
            if (String.IsNullOrWhiteSpace(companyName)) {
                return BadRequest(new {
                    Error = "Company name and portal are blank"
                });
            }
            var newCompany = new Company {
                Name = companyName,
                Portals = new List<CompanyPortal> {
                    new CompanyPortal{
                        CreatedAt=  DateTime.UtcNow,
                        PortalId = portal.Id,
                        PositiveFeedbackPercent = listing.Seller.PositiveFeedbackPercent,
                        Username = listing.Seller != null ? listing.Seller.Name : listing.Portal
                    }
                }
            };
            _context.Companies.Add(newCompany);
            await _context.SaveChangesAsync();
            return Ok(newCompany);
        }

        [HttpGet("{id}/CashDisbursements")]
        public IActionResult GetCashDisbursements(
            [FromRoute] int? id,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = _context.CashDisbursements.Where(item => item.CompanyId == id).OrderByDescending(item => item.DateDisbursed);

            return Ok(new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            });
        }

        // [HttpGet("{id}/CashReceipts")]
        // public async Task<IActionResult> GetCashReceipts(
        //     [FromRoute] int? id,
        //     [FromQuery] int skip = 0,
        //     [FromQuery] int perPage = 10
        // ) {
        //     var query = _context.CashReceipts.Where(item => item.CompanyId == id);

        //     return Ok(new ListResult {
        //         Items = query.Skip(skip).Take(perPage),
        //         Count = query.Count()
        //     });
        // }

        [HttpGet("{id}/Bills")]
        public async Task<IActionResult> GetBills(
            [FromRoute] int? id,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = _context.Bills.Where(item => item.CompanyId == id);

            return Ok(new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            });
        }

        // GET: Companies/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<Company> Search([FromQuery] string query) {
            return _context.Companies
                .Where(company =>
                    company.Name.StartsWith(query)
                    || company.PhoneNumbers.Any(companyPhone => companyPhone.PhoneNumber.Number.StartsWith(query))
                    || company.EmailAddresses.Any(companyEmailAddress => companyEmailAddress.EmailAddress.Address.StartsWith(query))
                    || company.Portals.Any(companyPortal => companyPortal.Username.StartsWith(query))
                )
                .Include(c => c.Addresses)
                    .ThenInclude(a => a.Address)
                .Include(c => c.PhoneNumbers)
                    .ThenInclude(a => a.PhoneNumber)
                .Take(30);
        }

        [RequirePermission("ManageBilling")]
        [HttpPost("{id}/BillingProfile")]
        public async Task<IActionResult> CreateBillingProfile([FromRoute] int id) {
            var company = await _context.Companies.Include(item => item.EmailAddresses).ThenInclude(item => item.EmailAddress).FirstOrDefaultAsync(item => item.Id == id);
            if (company == null) {
                return NotFound(new {
                    Error = "A company was not found with that id"
                });
            }
            if (company.AuthorizeNetProfileId != null) {
                return BadRequest(new {
                    Error = "Company already has a profile"
                });
            }
            var authorizeNetProfile = await company.GetAuthorizeNetProfile(_context);
            return Ok(authorizeNetProfile);
        }

        [RequirePermission("ManageBilling")]
        [HttpGet("{id}/BillingProfile")]
        public async Task<IActionResult> BillingProfile([FromRoute] int id) {
            var company = await _context.Companies.FirstOrDefaultAsync(item => item.Id == id);
            if (company == null) {
                return NotFound(new {
                    Error = "A company was not found with that id"
                });
            }
            if (company.AuthorizeNetProfileId == null) {
                return BadRequest(new {
                    Error = "Company does not have an authorize.net profile"
                });
            }
            return Ok(await company.GetAuthorizeNetProfile(_context));
        }

        [RequirePermission("ManageBilling")]
        [HttpGet("{id}/CreditCards")]
        public async Task<IActionResult> GetCreditCards([FromRoute] int id) {
            var company = await _context.Companies.FirstOrDefaultAsync(item => item.Id == id);
            if (company == null) {
                return NotFound(new {
                    Error = "A company was not found with that id"
                });
            }
            return Ok(await company.GetAuthorizeNetPaymentProfiles(_context));
        }

        [HttpPost("{id}/CreditCards")]
        public async Task<IActionResult> SaveCreditCard([FromRoute] int id, 
            [FromBody] GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.CreateProfileRequest.OpaqueData opaqueData,
            [FromQuery] string cardCode) {
            var company = await _context.Companies.FirstOrDefaultAsync(item => item.Id == id);
            if (company == null) {
                return NotFound(new {
                    Error = "A company was not found with that id"
                });
            }
            try {
                dynamic result;

                string paymentProfileId = null;

                if (String.IsNullOrWhiteSpace(company.AuthorizeNetProfileId)) {
                    result = await company.CreateAuthorizeNetProfile(_context, opaqueData);

                    paymentProfileId = (result as GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.CreateCustomerResponse.CreateCustomerResponse).customerPaymentProfileIdList.First();

                    
                } else {
                    result = await company.AddPaymentMethod(_context, new GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.CreatePaymentProfileRequest.OpaqueData {
                        dataDescriptor = opaqueData.dataDescriptor,
                        dataValue = opaqueData.dataValue
                    });

                    paymentProfileId = (result as GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.CreatePaymentProfileResponse.CreatePaymentProfileResponse).customerPaymentProfileId;
                }

                // Add payment profile card code if payment profile exists
                if (paymentProfileId != null)
                {
                    // Save to db
                    _context.PaymentProfileCodes.Add(new PaymentProfileCode() { PaymentProfileId = paymentProfileId, CardCode = Encryption.EncryptData(cardCode) });
                    await _context.SaveChangesAsync();
                }

                return Ok(result);
            }
            catch (Exception e) {
                Console.WriteLine(e.StackTrace);
                return BadRequest(e.Message);
            }
        }

        // GET: Companies/{id}/GetName
        [HttpGet("{id}/GetName")]
        public string GetName([FromRoute] int id) {
            return _context.Companies
                .Where(company => company.Id == id)
                .Select(company => new Company {
                    Id = company.Id,
                    Name = company.Name
                }).FirstOrDefault().Name;
        }

        [HttpGet("GetGIDIndustrial")]
        public async Task<IActionResult> GetGIDIndustrial() {
            var company = await _context.Companies
                .Where(co => co.Name == "GID Industrial")
                .Include(co => co.Addresses)
                    .ThenInclude(adr => adr.Address)
                .FirstOrDefaultAsync();
            if (company == null) {
                return NotFound("NOT FOUND");
            }
            return Ok(company);
        }

        /// <summary>
        /// Get Contacts for company
        /// Companies/{id}/Contacts...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Contacts")]
        public async Task<Company> GetContacts([FromRoute] int id) {
            //Here I "Include" companycontacts multiple times.  This is necesary because you can only do "ThenInclude" on one property of the companyContacts at a time.  So I have to include it three times so I can ThenInclude the address phone and email address
            return await _context.Companies
                .Where(company => company.Id == id)
                .Include(c => c.CompanyContacts)
                    .ThenInclude(c => c.Contact)
                        .ThenInclude(m => m.PhoneNumbers)
                            .ThenInclude(m => m.PhoneNumber)
                .Include(c => c.CompanyContacts)
                    .ThenInclude(c => c.Contact)
                        .ThenInclude(m => m.Addresses)
                            .ThenInclude(m => m.Address)
                .Include(c => c.CompanyContacts)
                    .ThenInclude(c => c.Contact)
                        .ThenInclude(m => m.EmailAddresses)
                            .ThenInclude(m => m.EmailAddress)
                .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Get Companies for company
        /// Companies/{id}/Companies...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/Companies")]
        public async Task<Company> GetCompanies([FromRoute] int id)
        {
            //Here I "Include" companycontacts multiple times.  This is necesary because you can only do "ThenInclude" on one property of the companyContacts at a time.  So I have to include it three times so I can ThenInclude the address phone and email address
            return await _context.Companies
                .Where(company => company.Id == id)
                .Include(c => c.CompanyCompanies)
                    .ThenInclude(c => c.RelatedCompany)
                        .ThenInclude(m => m.PhoneNumbers)
                            .ThenInclude(m => m.PhoneNumber)
                .Include(c => c.CompanyCompanies)
                    .ThenInclude(c => c.RelatedCompany)
                        .ThenInclude(m => m.Addresses)
                            .ThenInclude(m => m.Address)
                .Include(c => c.CompanyCompanies)
                    .ThenInclude(c => c.RelatedCompany)
                        .ThenInclude(m => m.EmailAddresses)
                            .ThenInclude(m => m.EmailAddress)
                .SingleOrDefaultAsync();
        }

        // GET: Companies/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompany([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var company = await _context.Companies
                .Include(m => m.PhoneNumbers)
                    .ThenInclude(m => m.PhoneNumber)
                .Include(m => m.Addresses)
                    .ThenInclude(m => m.Address)
                .Include(m => m.EmailAddresses)
                    .ThenInclude(m => m.EmailAddress)
                .Include(m => m.Notes)
                    .ThenInclude(m => m.Note)
                .Include(c => c.Attachments)
                    .ThenInclude(a => a.Attachment)
                .Include(c => c.ChildCompanies)
                .Include(c => c.CompanyContacts)
                    .ThenInclude(c => c.Contact)
                .Include(c => c.EventLogEntries)
                    .ThenInclude(c => c.EventLogEntry)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (company == null) {
                return NotFound();
            }

            return Ok(company);
        }


        // Companies/5/Portals
        [HttpGet("{id}/Portals")]
        public async Task<IActionResult> GetPortals([FromRoute] int id) {
            var company = await _context.Companies
                .Include(item => item.Portals)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (company == null) {
                return NotFound();
            }
            return Ok(company.Portals);
        }

        // Companies/5/Aliases
        [HttpGet("{id}/Aliases")]
        public async Task<IActionResult> GetAliases([FromRoute] int id) {
            var company = await _context.Companies
                .Include(item => item.Aliases)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (company == null) {
                return NotFound();
            }
            return Ok(company.Aliases);
        }


        // PUT: Companies/5
        /// <summary>
        /// Update a company
        /// </summary>
        /// <param name="id">The id of the company to update</param>
        /// <param name="company">The new company object</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompany([FromRoute] int id, [FromBody] Company company) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != company.Id) {
                return BadRequest();
            }

            var dbCompany = await _context.Companies.AsNoTracking()
                .Include(c => c.PhoneNumbers).ThenInclude(c => c.PhoneNumber)
                .Include(c => c.Addresses).ThenInclude(c => c.Address)
                .Include(c => c.EmailAddresses).ThenInclude(c => c.EmailAddress)
                .SingleOrDefaultAsync(c => c.Id == id);

            //add/modify/remove associated phone numbers
            if (dbCompany.PhoneNumbers != null) {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldPhoneNumbers = dbCompany.PhoneNumbers.Select(p => p.PhoneNumberId).ToList();
                List<int?> newPhoneNumbers = company.PhoneNumbers.Where(p => p.PhoneNumberId != null).Select(p => p.PhoneNumberId).ToList();

                //Mark new items as added
                List<CompanyPhoneNumber> added = company.PhoneNumbers.Where(p => p.PhoneNumberId == null).ToList();

                //mark all items without an id as modified
                List<CompanyPhoneNumber> modified = company.PhoneNumbers.Where(p => oldPhoneNumbers.Any(oldPhoneNumberId => oldPhoneNumberId == p.PhoneNumberId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                List<CompanyPhoneNumber> deleted = dbCompany.PhoneNumbers.Where(p => !newPhoneNumbers.Any(newPhoneNumberId => newPhoneNumberId == p.PhoneNumberId)).ToList();


                added.ForEach(item => {
                    _context.CompanyPhoneNumbers.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item => {
                    _context.CompanyPhoneNumbers.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item => {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.PhoneNumbers.Remove(item.PhoneNumber);
                });

            }

            //add/modify/delete associated addresses
            if (dbCompany.Addresses != null) {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldAddresses = dbCompany.Addresses.Select(p => p.AddressId).ToList();
                List<int?> newAddresses = company.Addresses.Where(p => p.AddressId != null).Select(p => p.AddressId).ToList();

                //Mark new items as added
                List<CompanyAddress> added = company.Addresses.Where(p => p.AddressId == null).ToList();

                //mark all items without an id as modified
                List<CompanyAddress> modified = company.Addresses.Where(p => oldAddresses.Any(oldAddressId => oldAddressId == p.AddressId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                List<CompanyAddress> deleted = dbCompany.Addresses.Where(p => !newAddresses.Any(newAddressId => newAddressId == p.AddressId)).ToList();


                added.ForEach(item => {
                    _context.CompanyAddresses.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item => {
                    _context.CompanyAddresses.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item => {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.Addresses.Remove(item.Address);
                });

            }

            //add/modify/delete associated addresses
            if (dbCompany.EmailAddresses != null) {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldEmailAddresses = dbCompany.EmailAddresses.Select(p => p.EmailAddressId).ToList();
                List<int?> newEmailAddresses = company.EmailAddresses.Where(p => p.EmailAddressId != null).Select(p => p.EmailAddressId).ToList();

                //Mark new items as added
                List<CompanyEmailAddress> added = company.EmailAddresses.Where(p => p.EmailAddressId == null).ToList();

                //mark all items without an id as modified
                List<CompanyEmailAddress> modified = company.EmailAddresses.Where(p => oldEmailAddresses.Any(oldEmailAddressId => oldEmailAddressId == p.EmailAddressId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                List<CompanyEmailAddress> deleted = dbCompany.EmailAddresses.Where(p => !newEmailAddresses.Any(newEmailAddressId => newEmailAddressId == p.EmailAddressId)).ToList();


                added.ForEach(item => {
                    _context.CompanyEmailAddresses.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item => {
                    _context.CompanyEmailAddresses.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item => {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.EmailAddresses.Remove(item.EmailAddress);
                });

            }

            company.QuickBooksCustomerId = dbCompany.QuickBooksCustomerId;
            company.QuickBooksCustomerSyncToken = dbCompany.QuickBooksCustomerSyncToken;

            _context.Entry(company).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!CompanyExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            // code to update in quickbooks. Unfortunately quickbooks thinks I'm trying to delete the object, so thsi doesn't work
            // if (!String.IsNullOrWhiteSpace(company.QuickBooksId)) {
            //     company = await _context.Companies.AsNoTracking()
            //         .Include(item => item.PhoneNumbers)
            //             .ThenInclude(item => item.PhoneNumber)
            //         .Include(item => item.EmailAddresses)
            //             .ThenInclude(item => item.EmailAddress)
            //         .Include(item => item.ShippingAddress)
            //             .ThenInclude(item => item.Address)
            //         .Include(item => item.BillingAddress)
            //             .ThenInclude(item => item.Address)
            //         .FirstAsync(item => item.Id == company.Id);
            //     await company.EnsureInQuickBooks(_quickBooksConnector, _context, true);
            // }

            return NoContent();
        }

        // POST: Companies
        /// <summary>
        /// Create a company
        /// </summary>
        /// <param name="company"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostCompany([FromBody] Company company) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            //add to event log entry the fact that it was created and who created it
            _context.CompanyEventLogEntries.Add(new CompanyEventLogEntry {
                Company = company,
                EventLogEntry = new EventLogEntry {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });
            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompany", new { id = company.Id }, company);
        }

        // DELETE: Companies/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompany([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var company = await _context.Companies
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (company == null) {
                return NotFound();
            }

            foreach (var itemAttachment in company.Attachments.ToList()) {
                await itemAttachment.Attachment.Delete(_context, _configuration);
            }
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            return Ok(company);
        }

        // [HttpPut("{id}/PaymentProfile")]
        // [RequirePermission("ManageBilling")]
        // public async Task<IActionResult> PutPaymentProfile([FromRoute] int id, [FromBody] dynamic paymentProfile) {
        //     if (paymentProfile == null) {
        //         return BadRequest(new {
        //             Error = "Please include a payment profile"
        //         });
        //     }
        //     var company = await _context.Companies.FirstOrDefaultAsync(item => item.Id == id);
        //     if (company == null) {
        //         return BadRequest(new {
        //             Error = "Company does not exist with that id"
        //         });
        //     }
        //     if (String.IsNullOrWhiteSpace(company.AuthorizeNetProfileId)) {
        //         return BadRequest(new {
        //             Error = "Company does not have an authorize.net profile Id"
        //         });
        //     }

        //     var requestData = new {
        //         updateCustomerPaymentProfileRequest = new {
        //             merchantAuthentication = new {
        //                 name = AuthorizeNetApiRequestor.merchantAuthenticationName,
        //                 transactionKey = AuthorizeNetApiRequestor.transactionKey
        //             },
        //             customerProfileId = company.AuthorizeNetProfileId,
        //             paymentProfile = paymentProfile
        //         }
        //     };

        //     var response = await GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.AuthorizeNetApiRequestor.DoApiRequest(requestData);
        //     dynamic responseBody = Newtonsoft.Json.Linq.JObject.Parse(response);

        //     var results = GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.AuthorizeNetApiRequestor.GetResult(responseBody);
        //     if(results.ResultCode != "Ok"){

        //         return BadRequest(new {
        //             Error = "Was not able to update",
        //             Details = results,
        //             MoreDetails = responseBody.validationDirectResponse
        //         });
        //     }

        //     return Ok();
        // }

        private bool CompanyExists(int id) {
            return _context.Companies.Any(e => e.Id == id);
        }


        [HttpDelete("{id}/PaymentProfiles/{paymentProfileId}")]
        [RequirePermission("ManageBilling")]
        public async Task<IActionResult> DeletePaymentProfile([FromRoute] int id, [FromRoute] int? paymentProfileId) {
            if (paymentProfileId == null) {
                return BadRequest(new {
                    Error = "Please include a payment profile"
                });
            }
            var company = await _context.Companies.FirstOrDefaultAsync(item => item.Id == id);
            if (company == null) {
                return BadRequest(new {
                    Error = "Company does not exist with that id"
                });
            }
            if (String.IsNullOrWhiteSpace(company.AuthorizeNetProfileId)) {
                return BadRequest(new {
                    Error = "Company does not have an authorize.net profile Id"
                });
            }

            var requestData = new {
                deleteCustomerPaymentProfileRequest = new {
                    merchantAuthentication = new {
                        name = AuthorizeNetApiRequestor.merchantAuthenticationName,
                        transactionKey = AuthorizeNetApiRequestor.transactionKey
                    },
                    customerProfileId = company.AuthorizeNetProfileId,
                    customerPaymentProfileId = paymentProfileId
                }
            };

            var response = await GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.AuthorizeNetApiRequestor.DoApiRequest(requestData);
            dynamic responseBody = Newtonsoft.Json.Linq.JObject.Parse(response);

            var results = GidIndustrial.Gideon.WebApi.Libraries.AuthorizeNet.AuthorizeNetApiRequestor.GetResult(responseBody);
            if (results.ResultCode != "Ok") {

                return BadRequest(new {
                    Error = "Was not able to update",
                    Details = results,
                    MoreDetails = responseBody.validationDirectResponse
                });
            }

            return Ok();
        }
    }
}