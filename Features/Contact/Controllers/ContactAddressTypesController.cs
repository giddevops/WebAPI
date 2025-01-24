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
    [Route("ContactAddressTypes")]
    public class ContactAddressTypesController : Controller
    {
        private readonly AppDBContext _context;

        public ContactAddressTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ContactAddressTypes
        [HttpGet]
        public IEnumerable<ContactAddressType> GetContactAddressType()
        {
            return _context.ContactAddressTypes;
        }

        // GET: ContactAddressTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactAddressType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactAddressType = await _context.ContactAddressTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (contactAddressType == null)
            {
                return NotFound();
            }

            return Ok(contactAddressType);
        }

        // PUT: ContactAddressTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutContactAddressType([FromRoute] int id, [FromBody] ContactAddressType contactAddressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactAddressType.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactAddressType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactAddressTypeExists(id))
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

        // POST: ContactAddressTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostContactAddressType([FromBody] ContactAddressType contactAddressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContactAddressTypes.Add(contactAddressType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactAddressType", new { id = contactAddressType.Id }, contactAddressType);
        }

        // DELETE: ContactAddressTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteContactAddressType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactAddressType = await _context.ContactAddressTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (contactAddressType == null)
            {
                return NotFound();
            }

            _context.ContactAddressTypes.Remove(contactAddressType);
            await _context.SaveChangesAsync();

            return Ok(contactAddressType);
        }

        private bool ContactAddressTypeExists(int id)
        {
            return _context.ContactAddressTypes.Any(e => e.Id == id);
        }
    }
}