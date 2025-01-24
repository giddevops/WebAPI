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
    [Route("CompanyCompanyRelationshipTypes")]
    public class CompanyCompanyRelationshipTypesController : Controller
    {
        private readonly AppDBContext _context;

        public CompanyCompanyRelationshipTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyCompanyRelationshipTypes
        [HttpGet]
        public IEnumerable<CompanyCompanyRelationshipType> GetCompanyCompanyRelationshipType()
        {
            return _context.CompanyCompanyRelationshipTypes;
        }

        // GET: CompanyCompanyRelationshipTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyCompanyRelationshipType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyContactRelationshipType = await _context.CompanyCompanyRelationshipTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (companyContactRelationshipType == null)
            {
                return NotFound();
            }

            return Ok(companyContactRelationshipType);
        }

        // PUT: CompanyCompanyRelationshipTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCompanyCompanyRelationshipType([FromRoute] int id, [FromBody] CompanyCompanyRelationshipType companyContactRelationshipType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyContactRelationshipType.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyContactRelationshipType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyCompanyRelationshipTypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: CompanyCompanyRelationshipTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCompanyCompanyRelationshipType([FromBody] CompanyCompanyRelationshipType companyContactRelationshipType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CompanyCompanyRelationshipTypes.Add(companyContactRelationshipType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyCompanyRelationshipType", new { id = companyContactRelationshipType.Id }, companyContactRelationshipType);
        }

        // DELETE: CompanyCompanyRelationshipTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCompanyCompanyRelationshipType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyContactRelationshipType = await _context.CompanyCompanyRelationshipTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (companyContactRelationshipType == null)
            {
                return NotFound();
            }

            _context.CompanyCompanyRelationshipTypes.Remove(companyContactRelationshipType);
            await _context.SaveChangesAsync();

            return Ok(companyContactRelationshipType);
        }

        private bool CompanyCompanyRelationshipTypeExists(int id)
        {
            return _context.CompanyCompanyRelationshipTypes.Any(e => e.Id == id);
        }
    }
}