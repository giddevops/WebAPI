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
    [Route("ContactAddresses")]
    public class ContactAddressesController : Controller
    {
        private readonly AppDBContext _context;

        public ContactAddressesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ContactAddresses
        [HttpGet]
        public IEnumerable<ContactAddress> GetContactAddresses()
        {
            return _context.ContactAddresses;
        }

        // GET: ContactAddresses/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactAddress = await _context.ContactAddresses.SingleOrDefaultAsync(m => m.ContactId == id);

            if (contactAddress == null)
            {
                return NotFound();
            }

            return Ok(contactAddress);
        }

        // PUT: ContactAddresses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactAddress([FromRoute] int id, [FromBody] ContactAddress contactAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactAddress.ContactId)
            {
                return BadRequest();
            }

            _context.Entry(contactAddress).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactAddressExists(id))
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

        // POST: ContactAddresses
        [HttpPost]
        public async Task<IActionResult> PostContactAddress([FromBody] ContactAddress contactAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContactAddresses.Add(contactAddress);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (ContactAddressExists(contactAddress.ContactId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetContactAddress", new { id = contactAddress.ContactId }, contactAddress);
        }

        // DELETE: ContactAddresses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactAddress([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactAddress = await _context.ContactAddresses.SingleOrDefaultAsync(m => m.ContactId == id);
            if (contactAddress == null)
            {
                return NotFound();
            }

            _context.ContactAddresses.Remove(contactAddress);
            await _context.SaveChangesAsync();

            return Ok(contactAddress);
        }

        private bool ContactAddressExists(int id)
        {
            return _context.ContactAddresses.Any(e => e.ContactId == id);
        }
    }
}