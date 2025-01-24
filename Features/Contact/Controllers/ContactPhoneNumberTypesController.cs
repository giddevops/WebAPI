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
    [Route("ContactPhoneNumberTypes")]
    public class ContactPhoneNumberTypesController : Controller
    {
        private readonly AppDBContext _context;

        public ContactPhoneNumberTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ContactPhoneNumberTypes
        [HttpGet]
        public IEnumerable<ContactPhoneNumberType> GetContactPhoneNumberType()
        {
            return _context.ContactPhoneNumberTypes;
        }

        // GET: ContactPhoneNumberTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactPhoneNumberType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactPhoneNumberType = await _context.ContactPhoneNumberTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (contactPhoneNumberType == null)
            {
                return NotFound();
            }

            return Ok(contactPhoneNumberType);
        }

        // PUT: ContactPhoneNumberTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutContactPhoneNumberType([FromRoute] int id, [FromBody] ContactPhoneNumberType contactPhoneNumberType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactPhoneNumberType.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactPhoneNumberType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactPhoneNumberTypeExists(id))
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

        // POST: ContactPhoneNumberTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostContactPhoneNumberType([FromBody] ContactPhoneNumberType contactPhoneNumberType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContactPhoneNumberTypes.Add(contactPhoneNumberType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactPhoneNumberType", new { id = contactPhoneNumberType.Id }, contactPhoneNumberType);
        }

        // DELETE: ContactPhoneNumberTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteContactPhoneNumberType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactPhoneNumberType = await _context.ContactPhoneNumberTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (contactPhoneNumberType == null)
            {
                return NotFound();
            }

            _context.ContactPhoneNumberTypes.Remove(contactPhoneNumberType);
            await _context.SaveChangesAsync();

            return Ok(contactPhoneNumberType);
        }

        private bool ContactPhoneNumberTypeExists(int id)
        {
            return _context.ContactPhoneNumberTypes.Any(e => e.Id == id);
        }
    }
}