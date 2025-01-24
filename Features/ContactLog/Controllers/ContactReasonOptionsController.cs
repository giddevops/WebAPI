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
    [Route("ContactReasonOptions")]
    public class ContactReasonOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public ContactReasonOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ContactReasonOptions
        [HttpGet]
        public IEnumerable<ContactReasonOption> GetContactReasonOption()
        {
            return _context.ContactReasonOptions;
        }

        // GET: ContactReasonOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactReasonOption = await _context.ContactReasonOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (contactReasonOption == null)
            {
                return NotFound();
            }

            return Ok(contactReasonOption);
        }

        // PUT: ContactReasonOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutContactReasonOption([FromRoute] int id, [FromBody] ContactReasonOption contactReasonOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != contactReasonOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(contactReasonOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactReasonOptionExists(id))
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

        // POST: ContactReasonOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostContactReasonOption([FromBody] ContactReasonOption contactReasonOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ContactReasonOptions.Add(contactReasonOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactReasonOption", new { id = contactReasonOption.Id }, contactReasonOption);
        }

        // DELETE: ContactReasonOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteContactReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contactReasonOption = await _context.ContactReasonOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (contactReasonOption == null)
            {
                return NotFound();
            }

            _context.ContactReasonOptions.Remove(contactReasonOption);
            await _context.SaveChangesAsync();

            return Ok(contactReasonOption);
        }

        private bool ContactReasonOptionExists(int id)
        {
            return _context.ContactReasonOptions.Any(e => e.Id == id);
        }
    }
}