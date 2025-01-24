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
    [Route("CompanyPhoneNumberTypes")]
    public class CompanyPhoneNumberTypesController : Controller
    {
        private readonly AppDBContext _context;

        public CompanyPhoneNumberTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyPhoneNumberTypes
        [HttpGet]
        public IEnumerable<CompanyPhoneNumberType> GetCompanyPhoneNumberType()
        {
            return _context.CompanyPhoneNumberTypes;
        }

        // GET: CompanyPhoneNumberTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyPhoneNumberType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyPhoneNumberType = await _context.CompanyPhoneNumberTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (companyPhoneNumberType == null)
            {
                return NotFound();
            }

            return Ok(companyPhoneNumberType);
        }

        // PUT: CompanyPhoneNumberTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCompanyPhoneNumberType([FromRoute] int id, [FromBody] CompanyPhoneNumberType companyPhoneNumberType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyPhoneNumberType.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyPhoneNumberType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyPhoneNumberTypeExists(id))
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

        // POST: CompanyPhoneNumberTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCompanyPhoneNumberType([FromBody] CompanyPhoneNumberType companyPhoneNumberType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CompanyPhoneNumberTypes.Add(companyPhoneNumberType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyPhoneNumberType", new { id = companyPhoneNumberType.Id }, companyPhoneNumberType);
        }

        // DELETE: CompanyPhoneNumberTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCompanyPhoneNumberType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyPhoneNumberType = await _context.CompanyPhoneNumberTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (companyPhoneNumberType == null)
            {
                return NotFound();
            }

            _context.CompanyPhoneNumberTypes.Remove(companyPhoneNumberType);
            await _context.SaveChangesAsync();

            return Ok(companyPhoneNumberType);
        }

        private bool CompanyPhoneNumberTypeExists(int id)
        {
            return _context.CompanyPhoneNumberTypes.Any(e => e.Id == id);
        }
    }
}