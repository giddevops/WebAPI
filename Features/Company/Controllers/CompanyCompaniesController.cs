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
    [Route("CompanyCompanies")]
    public class CompanyCompaniesController : Controller
    {
        private readonly AppDBContext _context;

        public CompanyCompaniesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyCompanies?companyId=&relatedCompanyId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="relatedCompanyId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetCompanyCompanyById([FromQuery] int? companyId, [FromQuery] int? relatedCompanyId)
        {

            var companyCompany = await _context.CompanyCompanies.SingleOrDefaultAsync(m => m.RelatedCompanyId == relatedCompanyId && m.CompanyId == companyId);

            if (companyCompany == null)
            {
                return NotFound();
            }

            return Ok(companyCompany);
        }

        // GET: CompanyCompanies
        /// <summary>
        /// Fetches a list of company contacts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCompanyCompanies([FromQuery] int? companyId, [FromQuery] int? relatedCompanyId)
        {
            if (companyId == null && relatedCompanyId == null)
            {
                return BadRequest(new
                {
                    Error = "Must have either companyId or relatedCompanyId querystring param"
                });
            }
            var query = from company in _context.CompanyCompanies select company;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (companyId != null)
            {
                query = query.Where(item => item.CompanyId == companyId).Include(m => m.RelatedCompany);
            }
            if (relatedCompanyId != null)
            {
                query = query.Where(item => item.RelatedCompanyId == relatedCompanyId).Include(m => m.Company);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: CompanyCompanies?relatedCompanyId=&companyId=
        [HttpPut]
        public async Task<IActionResult> PutCompanyCompany([FromQuery] int? relatedCompanyId, [FromQuery] int? companyId, [FromBody] CompanyCompany companyCompany)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(companyCompany).State = EntityState.Modified;

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

        // POST: CompanyCompanies
        [HttpPost]
        public async Task<IActionResult> PostCompanyCompany([FromBody] CompanyCompany companyCompany)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //If the lead "Position" is a string, need to add it to the list of companyCompanyRelationshipTypes and then set that relationshipt Id as the relationship type for the company and contact
            if (!string.IsNullOrWhiteSpace(companyCompany.RelationshipTypeString))
            {
                var existingRelationship = await _context.CompanyCompanyRelationshipTypes.FirstOrDefaultAsync(cc => cc.Value == companyCompany.RelationshipTypeString);
                if (existingRelationship != null)
                {
                    companyCompany.CompanyCompanyRelationshipTypeId = existingRelationship.Id;
                }
                else
                {
                    var newCompanyCompanyRelationshipType = new CompanyCompanyRelationshipType
                    {
                        Value = companyCompany.RelationshipTypeString
                    };
                    _context.CompanyCompanyRelationshipTypes.Add(newCompanyCompanyRelationshipType);
                    await _context.SaveChangesAsync();
                    companyCompany.CompanyCompanyRelationshipTypeId = newCompanyCompanyRelationshipType.Id;
                }
            }

            _context.CompanyCompanies.Add(companyCompany);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (CompanyCompanyExists(companyCompany.CompanyId, companyCompany.RelatedCompanyId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetCompanyCompany", new { id = companyCompany.RelatedCompanyId }, companyCompany);
        }

        // DELETE: CompanyCompanies?companyId=&contactid=
        [HttpDelete]
        public async Task<IActionResult> DeleteCompanyCompany([FromQuery] int companyId, [FromQuery] int relatedCompanyId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyCompany = await _context.CompanyCompanies.SingleOrDefaultAsync(m => m.RelatedCompanyId == relatedCompanyId && m.CompanyId == companyId);
            if (companyCompany == null)
            {
                return NotFound();
            }

            _context.CompanyCompanies.Remove(companyCompany);
            await _context.SaveChangesAsync();

            return Ok(companyCompany);
        }

        private bool CompanyCompanyExists(int? companyId, int? relatedCompanyId)
        {
            return _context.CompanyCompanies.Any(e => e.RelatedCompanyId == relatedCompanyId && e.CompanyId == companyId);
        }
    }
}