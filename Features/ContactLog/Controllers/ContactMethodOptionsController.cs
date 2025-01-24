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
    [Route("ContactMethodOptions")]
    public class ContactMethodOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public ContactMethodOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ContactMethodOptions
        [HttpGet]
        public IEnumerable<ContactMethodOption> GetContactMethodOption()
        {
            return _context.ContactMethodOptions;
        }

        // GET: ContactMethodOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactMethodOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactMethodOption = await _context.ContactMethodOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (contactMethodOption == null)
            {
                return NotFound();
            }

            return Ok(contactMethodOption);
        }

        // PUT: ContactMethodOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutContactMethodOption([FromRoute] int id, [FromBody] ContactMethodOption contactMethodOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactMethodOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactMethodOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactMethodOptionExists(id))
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

        // POST: ContactMethodOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostContactMethodOption([FromBody] ContactMethodOption contactMethodOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContactMethodOptions.Add(contactMethodOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactMethodOption", new { id = contactMethodOption.Id }, contactMethodOption);
        }

        // DELETE: ContactMethodOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteContactMethodOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactMethodOption = await _context.ContactMethodOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (contactMethodOption == null)
            {
                return NotFound();
            }

            _context.ContactMethodOptions.Remove(contactMethodOption);
            await _context.SaveChangesAsync();

            return Ok(contactMethodOption);
        }

        private bool ContactMethodOptionExists(int id)
        {
            return _context.ContactMethodOptions.Any(e => e.Id == id);
        }
    }
}