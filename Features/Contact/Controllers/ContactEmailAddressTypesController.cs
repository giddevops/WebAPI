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
    [Route("ContactEmailAddressTypes")]
    public class ContactEmailAddressTypesController : Controller
    {
        private readonly AppDBContext _context;

        public ContactEmailAddressTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ContactEmailAddressTypes
        [HttpGet]
        public IEnumerable<ContactEmailAddressType> GetContactEmailAddressType()
        {
            return _context.ContactEmailAddressTypes;
        }

        // GET: ContactEmailAddressTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactEmailAddressType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactEmailAddressType = await _context.ContactEmailAddressTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (contactEmailAddressType == null)
            {
                return NotFound();
            }

            return Ok(contactEmailAddressType);
        }

        // PUT: ContactEmailAddressTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutContactEmailAddressType([FromRoute] int id, [FromBody] ContactEmailAddressType contactEmailAddressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactEmailAddressType.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactEmailAddressType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactEmailAddressTypeExists(id))
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

        // POST: ContactEmailAddressTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostContactEmailAddressType([FromBody] ContactEmailAddressType contactEmailAddressType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContactEmailAddressTypes.Add(contactEmailAddressType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactEmailAddressType", new { id = contactEmailAddressType.Id }, contactEmailAddressType);
        }

        // DELETE: ContactEmailAddressTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteContactEmailAddressType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactEmailAddressType = await _context.ContactEmailAddressTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (contactEmailAddressType == null)
            {
                return NotFound();
            }

            _context.ContactEmailAddressTypes.Remove(contactEmailAddressType);
            await _context.SaveChangesAsync();

            return Ok(contactEmailAddressType);
        }

        private bool ContactEmailAddressTypeExists(int id)
        {
            return _context.ContactEmailAddressTypes.Any(e => e.Id == id);
        }
    }
}