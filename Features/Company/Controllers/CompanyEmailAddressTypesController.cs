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
    [Route("CompanyEmailAddressTypes")]
    public class CompanyEmailAddressTypesController : Controller
    {
        private readonly AppDBContext _context;

        public CompanyEmailAddressTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyEmailAddressTypes
        [HttpGet]
        public IEnumerable<CompanyEmailAddressType> GetCompanyEmailAddressType()
        {
            return _context.CompanyEmailAddressTypes;
        }

        // GET: CompanyEmailAddressTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyEmailAddressType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyEmailAddressType = await _context.CompanyEmailAddressTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (companyEmailAddressType == null)
            {
                return NotFound();
            }

            return Ok(companyEmailAddressType);
        }

        // PUT: CompanyEmailAddressTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCompanyEmailAddressType([FromRoute] int id, [FromBody] CompanyEmailAddressType companyEmailAddressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyEmailAddressType.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyEmailAddressType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyEmailAddressTypeExists(id))
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

        // POST: CompanyEmailAddressTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCompanyEmailAddressType([FromBody] CompanyEmailAddressType companyEmailAddressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CompanyEmailAddressTypes.Add(companyEmailAddressType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyEmailAddressType", new { id = companyEmailAddressType.Id }, companyEmailAddressType);
        }

        // DELETE: CompanyEmailAddressTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCompanyEmailAddressType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyEmailAddressType = await _context.CompanyEmailAddressTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (companyEmailAddressType == null)
            {
                return NotFound();
            }

            _context.CompanyEmailAddressTypes.Remove(companyEmailAddressType);
            await _context.SaveChangesAsync();

            return Ok(companyEmailAddressType);
        }

        private bool CompanyEmailAddressTypeExists(int id)
        {
            return _context.CompanyEmailAddressTypes.Any(e => e.Id == id);
        }
    }
}