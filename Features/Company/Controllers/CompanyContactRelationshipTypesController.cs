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
    [Route("CompanyContactRelationshipTypes")]
    public class CompanyContactRelationshipTypesController : Controller
    {
        private readonly AppDBContext _context;

        public CompanyContactRelationshipTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyContactRelationshipTypes
        [HttpGet]
        public IEnumerable<CompanyContactRelationshipType> GetCompanyContactRelationshipType()
        {
            return _context.CompanyContactRelationshipTypes;
        }

        // GET: CompanyContactRelationshipTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyContactRelationshipType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyContactRelationshipType = await _context.CompanyContactRelationshipTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (companyContactRelationshipType == null)
            {
                return NotFound();
            }

            return Ok(companyContactRelationshipType);
        }

        // PUT: CompanyContactRelationshipTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCompanyContactRelationshipType([FromRoute] int id, [FromBody] CompanyContactRelationshipType companyContactRelationshipType)
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
                if (!CompanyContactRelationshipTypeExists(id))
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

        // POST: CompanyContactRelationshipTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCompanyContactRelationshipType([FromBody] CompanyContactRelationshipType companyContactRelationshipType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CompanyContactRelationshipTypes.Add(companyContactRelationshipType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyContactRelationshipType", new { id = companyContactRelationshipType.Id }, companyContactRelationshipType);
        }

        // DELETE: CompanyContactRelationshipTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCompanyContactRelationshipType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyContactRelationshipType = await _context.CompanyContactRelationshipTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (companyContactRelationshipType == null)
            {
                return NotFound();
            }

            _context.CompanyContactRelationshipTypes.Remove(companyContactRelationshipType);
            await _context.SaveChangesAsync();

            return Ok(companyContactRelationshipType);
        }

        private bool CompanyContactRelationshipTypeExists(int id)
        {
            return _context.CompanyContactRelationshipTypes.Any(e => e.Id == id);
        }
    }
}