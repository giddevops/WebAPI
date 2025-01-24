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
    [Route("CompanyAddressTypes")]
    public class CompanyAddressTypesController : Controller
    {
        private readonly AppDBContext _context;

        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="context"></param>
        public CompanyAddressTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyAddressTypes
        [HttpGet]
        public IEnumerable<CompanyAddressType> GetCompanyAddressType()
        {
            return _context.CompanyAddressTypes;
        }

        // GET: CompanyAddressTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyAddressType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyAddressType = await _context.CompanyAddressTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (companyAddressType == null)
            {
                return NotFound();
            }

            return Ok(companyAddressType);
        }

        // PUT: CompanyAddressTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCompanyAddressType([FromRoute] int id, [FromBody] CompanyAddressType companyAddressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyAddressType.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyAddressType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyAddressTypeExists(id))
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

        // POST: CompanyAddressTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCompanyAddressType([FromBody] CompanyAddressType companyAddressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CompanyAddressTypes.Add(companyAddressType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyAddressType", new { id = companyAddressType.Id }, companyAddressType);
        }

        // DELETE: CompanyAddressTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCompanyAddressType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyAddressType = await _context.CompanyAddressTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (companyAddressType == null)
            {
                return NotFound();
            }

            _context.CompanyAddressTypes.Remove(companyAddressType);
            await _context.SaveChangesAsync();

            return Ok(companyAddressType);
        }

        private bool CompanyAddressTypeExists(int id)
        {
            return _context.CompanyAddressTypes.Any(e => e.Id == id);
        }
    }
}