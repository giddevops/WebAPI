using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Controllers
{
    [Produces("application/json")]
    [Route("CompanyContacts")]
    public class CompanyContactsController : Controller
    {
        private readonly AppDBContext _context;

        public CompanyContactsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyContacts?companyId=&contactId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="contactId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetCompanyContactById([FromQuery] int? companyId, [FromQuery] int? contactId)
        {

            var companyContact = await _context.CompanyContacts.SingleOrDefaultAsync(m => m.ContactId == contactId && m.CompanyId == companyId);

            if (companyContact == null)
            {
                return NotFound();
            }

            return Ok(companyContact);
        }

        // GET: CompanyContacts
        /// <summary>
        /// Fetches a list of company contacts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCompanyContacts([FromQuery] int? companyId, [FromQuery] int? contactId)
        {
            if (companyId == null && contactId == null)
            {
                return BadRequest(new
                {
                    Error = "Must have either companyId or contactId querystring param"
                });
            }
            var query  = from company in _context.CompanyContacts select company;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if(companyId != null){
                query = query.Where(item => item.CompanyId == companyId).Include(m => m.Contact);
            }
            if(contactId != null){
                query = query.Where(item => item.ContactId == contactId).Include(m => m.Company);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: CompanyContacts?contactId=&companyId=
        [HttpPut]
        public async Task<IActionResult> PutCompanyContact([FromQuery] int? contactId, [FromQuery] int? companyId, [FromBody] CompanyContact companyContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(companyContact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: CompanyContacts
        [HttpPost]
        public async Task<IActionResult> PostCompanyContact([FromBody] CompanyContact companyContact)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //If the lead "Position" is a string, need to add it to the list of companyContactRelationshipTypes and then set that relationshipt Id as the relationship type for the company and contact
            if (!string.IsNullOrWhiteSpace(companyContact.RelationshipTypeString))
            {
                var existingRelationship = await _context.CompanyContactRelationshipTypes.FirstOrDefaultAsync(cc => cc.Value == companyContact.RelationshipTypeString);
                if (existingRelationship != null)
                {
                    companyContact.CompanyContactRelationshipTypeId = existingRelationship.Id;
                }
                else
                {
                    var newCompanyContactRelationshipType = new CompanyContactRelationshipType
                    {
                        Value = companyContact.RelationshipTypeString
                    };
                    _context.CompanyContactRelationshipTypes.Add(newCompanyContactRelationshipType);
                    await _context.SaveChangesAsync();
                    companyContact.CompanyContactRelationshipTypeId = newCompanyContactRelationshipType.Id;
                }
            }

            _context.CompanyContacts.Add(companyContact);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompanyContactExists(companyContact.CompanyId, companyContact.ContactId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCompanyContact", new { id = companyContact.ContactId }, companyContact);
        }

        // DELETE: CompanyContacts?companyId=&contactid=
        [HttpDelete]
        public async Task<IActionResult> DeleteCompanyContact([FromQuery] int companyId, [FromQuery] int contactId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyContact = await _context.CompanyContacts.SingleOrDefaultAsync(m => m.ContactId == contactId && m.CompanyId == companyId);
            if (companyContact == null)
            {
                return NotFound();
            }

            _context.CompanyContacts.Remove(companyContact);
            await _context.SaveChangesAsync();

            return Ok(companyContact);
        }

        private bool CompanyContactExists(int? companyId, int? contactId)
        {
            return _context.CompanyContacts.Any(e => e.ContactId == contactId && e.CompanyId == companyId);
        }
    }
}