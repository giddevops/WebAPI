using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.Extensions.Configuration;
using Dapper;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Contacts")]
    public class ContactsController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration _configuration;

        public ContactsController(AppDBContext context, IConfiguration config) {
            _context = context;
            _configuration = config;
        }

        // GET: Contacts
        [HttpGet]
        public ListResult GetContacts(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string name = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from contact in _context.Contacts select contact;

            if (name != null) {
                name = name.Trim();
                if (name.Length > 0) {
                    var nameSplit = name.Split(" ");
                    if (nameSplit.Length == 1) {
                        query = query.Where(item => item.FirstName.StartsWith(name) || item.LastName.StartsWith(name));
                    } else if (nameSplit.Length == 2) {
                        query = query.Where(item => item.FirstName.StartsWith(nameSplit[0]) && item.LastName.StartsWith(nameSplit[1]));
                    }
                }
            }

            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "LastName":
                    query = sortAscending ? query.OrderBy(item => item.LastName) : query.OrderByDescending(item => item.LastName);
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
                //query = query.Where(item =>
                //    EF.Functions.Like(item.FirstName, searchString + '%') ||
                //    EF.Functions.Like(item.LastName, searchString + '%') ||
                //    item.PhoneNumbers.Any(cPhone => EF.Functions.Like(cPhone.PhoneNumber.Number, searchString + '%')) ||
                //    item.EmailAddresses.Any(cEmail => EF.Functions.Like(cEmail.EmailAddress.Address, searchString + '%'))

                //// item.FirstName.StartsWith(searchString) || 
                //// item.LastName.StartsWith(searchString) ||
                //// item.PhoneNumbers.Any(cPhone => cPhone.PhoneNumber.Number.StartsWith(searchString)) ||
                //// item.EmailAddresses.Any(cEmail => cEmail.EmailAddress.Address.StartsWith(searchString))
                //);
                var searchStringLike = searchString + "%";
                int searchStringNumber;
                if (Int32.TryParse(searchString, out searchStringNumber) == false)
                {
                    searchStringNumber = 0;
                }
                var querySting = @"SELECT DISTINCT Contact.*
FROM Contact
LEFT JOIN ContactPhoneNumber on ContactPhoneNumber.ContactId = Contact.Id
LEFT JOIN PhoneNumber ON ContactPhoneNumber.PhoneNumberId = PhoneNumber.Id AND PhoneNumber.Number LIKE @searchStringLike
LEFT JOIN ContactEmailAddress on ContactEmailAddress.ContactId = Contact.Id
LEFT JOIN EmailAddress ON ContactEmailAddress.EmailAddressId = EmailAddress.Id AND EmailAddress.Address LIKE @searchStringLike
WHERE Contact.FirstName LIKE @searchStringLike
OR Contact.LastName LIKE @searchStringLike
OR PhoneNumber.Number LIKE @searchStringLike
OR EmailAddress.Address LIKE @searchStringLike
OR Contact.Id LIKE @searchStringNumber
                ORDER BY Contact.Id DESC
                OFFSET @skip ROWS FETCH NEXT @perPage ROWS ONLY;
                ";
                var connection = _context.Database.GetDbConnection();
                var result = connection.Query<dynamic>(querySting, new
                {
                    searchStringLike = searchStringLike,
                    searchStringNumber = searchStringNumber,
                    skip = skip,
                    perPage = perPage
                });
                return new ListResult
                {
                    Items = result,
                    Count = -1
                };
            }

            var count = -1;
            if (String.IsNullOrWhiteSpace(searchString))
                count = query.Count();

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = count
            };
        }


        // GET: Contacts/RelateToCompany/{id}?CompanyId=...
        [HttpGet("RelateToCompany/{contactId}")]
        public IActionResult RelateToCompany([FromRoute] int contactId, [FromQuery] int companyId) {
            CompanyContact companyContact = _context.CompanyContacts.AsNoTracking().FirstOrDefault(cc => cc.CompanyId == companyId && cc.ContactId == contactId);
            if (companyContact == null) {
                _context.CompanyContacts.Add(new CompanyContact {
                    CompanyId = companyId,
                    ContactId = contactId
                });
                _context.SaveChanges();
            }
            return NoContent();
        }

        // GET: Contacts/Search?query=...
        [HttpGet("Search")]
        public async Task<IActionResult> Search([FromQuery] string query) {
            IQueryable<Contact> search = _context.Contacts;
            query = query.Trim();
            string[] querySplit = query.Split(' ');
            if (querySplit.Length == 2) {
                search = search.Where(item => item.FirstName.StartsWith(querySplit[0]) && item.LastName.StartsWith(querySplit[1]));
            } else {
                search = search.Where(item => item.FirstName.StartsWith(query) || item.LastName.StartsWith(query));
            }

            search = search
                .Include(c => c.Addresses)
                    .ThenInclude(a => a.Address)
                .Include(item => item.EmailAddresses)
                    .ThenInclude(item => item.EmailAddress)
                .Include(c => c.PhoneNumbers)
                    .ThenInclude(a => a.PhoneNumber);

            var contacts = await search.ToListAsync();

            return Ok(contacts.Select(contact => new {
                Id = contact.Id,
                Name = contact.FirstName + " " + contact.LastName,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Addresses = contact.Addresses,
                PhoneNumbers = contact.PhoneNumbers,
                EmailAddresses = contact.EmailAddresses
            }));
        }

        // GET: Contacts/GetName/{id}=...
        [HttpGet("GetName/{id}")]
        public string GetName([FromRoute] int id) {
            Contact contact = _context.Contacts
                .Where(c => c.Id == id)
                .Select(c => new Contact {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName
                }).FirstOrDefault();
            return contact.FirstName + " " + contact.LastName;
        }


        // GET: Contacts/GetName/{id}=...
        [HttpGet("ContactsForCompany/{companyId}")]
        public IEnumerable<dynamic> GetContactsForCompany([FromRoute] int companyId) {
            return _context.Contacts
                .Where(c => c.CompanyContacts.Any(cc => cc.CompanyId == companyId))
                .Select(c => new {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Value = c.FirstName + " " + c.LastName
                });
        }

        // GET: Contacts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContact([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var contact = await _context.Contacts
                .Include(c => c.Notes)
                    .ThenInclude(c => c.Note)
                .Include(c => c.Addresses)
                    .ThenInclude(c => c.Address)
                .Include(c => c.PhoneNumbers)
                    .ThenInclude(c => c.PhoneNumber)
                .Include(c => c.EmailAddresses)
                    .ThenInclude(c => c.EmailAddress)
                .Include(c => c.CompanyContacts)
                .Include(c => c.Attachments)
                    .ThenInclude(c => c.Attachment)
                .Include(c => c.EventLogEntries)
                    .ThenInclude(c => c.EventLogEntry)
                // .ThenInclude(c => c.Company)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (contact == null) {
                return NotFound();
            }

            return Ok(contact);
        }

        // PUT: Contacts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContact([FromRoute] int id, [FromBody] Contact contact) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != contact.Id) {
                return BadRequest();
            }

            var dbContact = await _context.Contacts.AsNoTracking()
                .Include(c => c.PhoneNumbers).ThenInclude(c => c.PhoneNumber)
                .Include(c => c.Addresses).ThenInclude(c => c.Address)
                .Include(c => c.EmailAddresses).ThenInclude(c => c.EmailAddress)
                .Include(c => c.CompanyContacts).ThenInclude(c => c.Company)
                .SingleOrDefaultAsync(c => c.Id == id);

            //add/modify/remove associated phone numbers
            if (dbContact.PhoneNumbers != null) {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldPhoneNumbers = dbContact.PhoneNumbers.Select(p => p.PhoneNumberId).ToList();
                List<int?> newPhoneNumbers = contact.PhoneNumbers.Where(p => p.PhoneNumberId != null).Select(p => p.PhoneNumberId).ToList();

                //Mark new items as added
                List<ContactPhoneNumber> added = contact.PhoneNumbers.Where(p => p.PhoneNumberId == null).ToList();

                //mark all items without an id as modified
                List<ContactPhoneNumber> modified = contact.PhoneNumbers.Where(p => oldPhoneNumbers.Any(oldPhoneNumberId => oldPhoneNumberId == p.PhoneNumberId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                List<ContactPhoneNumber> deleted = dbContact.PhoneNumbers.Where(p => !newPhoneNumbers.Any(newPhoneNumberId => newPhoneNumberId == p.PhoneNumberId)).ToList();


                added.ForEach(item => {
                    _context.ContactPhoneNumbers.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item => {
                    _context.ContactPhoneNumbers.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item => {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.PhoneNumbers.Remove(item.PhoneNumber);
                });

            }

            //add/modify/delete associated addresses
            if (dbContact.Addresses != null) {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldAddresses = dbContact.Addresses.Select(p => p.AddressId).ToList();
                List<int?> newAddresses = contact.Addresses.Where(p => p.AddressId != null).Select(p => p.AddressId).ToList();

                //Mark new items as added
                List<ContactAddress> added = contact.Addresses.Where(p => p.AddressId == null).ToList();

                //mark all items without an id as modified
                List<ContactAddress> modified = contact.Addresses.Where(p => oldAddresses.Any(oldAddressId => oldAddressId == p.AddressId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                List<ContactAddress> deleted = dbContact.Addresses.Where(p => !newAddresses.Any(newAddressId => newAddressId == p.AddressId)).ToList();


                added.ForEach(item => {
                    _context.ContactAddresses.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item => {
                    _context.ContactAddresses.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item => {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.Addresses.Remove(item.Address);
                });

            }

            //add/modify/delete associated addresses
            if (dbContact.EmailAddresses != null) {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldEmailAddresses = dbContact.EmailAddresses.Select(p => p.EmailAddressId).ToList();
                List<int?> newEmailAddresses = contact.EmailAddresses.Where(p => p.EmailAddressId != null).Select(p => p.EmailAddressId).ToList();

                //Mark new items as added
                List<ContactEmailAddress> added = contact.EmailAddresses.Where(p => p.EmailAddressId == null).ToList();

                //mark all items without an id as modified
                List<ContactEmailAddress> modified = contact.EmailAddresses.Where(p => oldEmailAddresses.Any(oldEmailAddressId => oldEmailAddressId == p.EmailAddressId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                List<ContactEmailAddress> deleted = dbContact.EmailAddresses.Where(p => !newEmailAddresses.Any(newEmailAddressId => newEmailAddressId == p.EmailAddressId)).ToList();


                added.ForEach(item => {
                    _context.ContactEmailAddresses.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item => {
                    _context.ContactEmailAddresses.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item => {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.EmailAddresses.Remove(item.EmailAddress);
                });

            }

            //add/modify/delete associated companycontacts
            if (dbContact.CompanyContacts != null) {

                //get a list of the old phone numbers that were already in the List
                List<int?> oldCompanies = dbContact.CompanyContacts.Select(p => p.CompanyId).ToList();
                List<int?> newCompanies = contact.CompanyContacts.Where(p => p.CompanyId != null).Select(p => p.CompanyId).ToList();

                //Mark new items as added
                List<CompanyContact> added = contact.CompanyContacts.Where(p => p.CompanyId == null || !oldCompanies.Any(oldCompanyId => oldCompanyId == p.CompanyId)).ToList();

                //mark all items without an id as modified
                List<CompanyContact> modified = contact.CompanyContacts.Where(p => oldCompanies.Any(oldCompanyId => oldCompanyId == p.CompanyId)).ToList();

                //get deleted items. This is items that are in the old list but not in the new
                List<CompanyContact> deleted = dbContact.CompanyContacts.Where(p => !newCompanies.Any(newCompanyId => newCompanyId == p.CompanyId)).ToList();

                added.ForEach(item => {
                    _context.CompanyContacts.Add(item);
                    _context.Entry(item).State = EntityState.Added;
                });
                modified.ForEach(item => {
                    _context.CompanyContacts.Update(item);
                    _context.Entry(item).State = EntityState.Modified;
                });
                deleted.ForEach(item => {
                    _context.Entry(item).State = EntityState.Deleted;
                    _context.CompanyContacts.Remove(item);
                });

            }

            _context.Entry(contact).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ContactExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// POST: Contacts
        /// Create a contact
        /// </summary>
        /// <param name="contact"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostContact([FromBody] Contact contact) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            //add to event log entry the fact that it was created and who created it
            _context.ContactEventLogEntries.Add(new ContactEventLogEntry {
                Contact = contact,
                EventLogEntry = new EventLogEntry {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContact", new { id = contact.Id }, contact);
        }

        // DELETE: Contacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContact([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var contact = await _context.Contacts
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contact == null) {
                return NotFound();
            }

            foreach (var itemAttachment in contact.Attachments.ToList()) {
                await itemAttachment.Attachment.Delete(_context, _configuration);
            }
            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(contact);
        }

        private bool ContactExists(int id) {
            return _context.Contacts.Any(e => e.Id == id);
        }
    }
}