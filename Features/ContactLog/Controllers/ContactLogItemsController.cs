using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.WindowsAzure.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ContactLogItems")]
    public class ContactLogItemsController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration configuration;

        public ContactLogItemsController(AppDBContext context, IConfiguration config) {
            _context = context;
            configuration = config;
        }

        [HttpGet()]
        public async Task<IActionResult> GetContactLogItems([FromQuery] int? leadId, [FromQuery] int? quoteId){
            var query = from contactLogItem in _context.ContactLogItems select contactLogItem;
            
            if(leadId != null)
                query = query.Where(item => item.LeadContactLogItems.Any(i => i.LeadId == leadId));
            else if(quoteId != null)
                query = query.Where(item => item.QuoteContactLogItems.Any(i => i.QuoteId == quoteId));
            else
                return BadRequest("You need to specify leadId or QuoteId");

            return Ok(await query.ToListAsync());
        }

        // GET: ContactLogItem/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetContactLogItem([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var contactLogItem = await _context.ContactLogItems.SingleOrDefaultAsync(m => m.Id == id);

            if (contactLogItem == null) {
                return NotFound();
            }

            return Ok(contactLogItem);
        }

        // PUT: ContactLogItem/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutContactLogItem([FromRoute] int id, [FromBody] ContactLogItem contactLogItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != contactLogItem.Id) {
                return BadRequest();
            }

            var oldItem = await _context.ContactLogItems
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);
            if (oldItem == null) {
                return NotFound("That item was not found");
            }
            if (oldItem.CreatedById != GidIndustrial.Gideon.WebApi.Models.User.GetId(User)) {
                return BadRequest("You are not authorized to edit this because you didn't make it");
            }
            contactLogItem.UpdatedAt = DateTime.UtcNow;
            _context.Entry(contactLogItem).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ContactLogItemExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ContactLogItem
        [HttpPost]
        public async Task<IActionResult> PostContactLogItem([FromBody] ContactLogItem contactLogItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            contactLogItem.CreatedAt = DateTime.UtcNow;
            contactLogItem.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            _context.ContactLogItems.Add(contactLogItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetContactLogItem", new { id = contactLogItem.Id }, contactLogItem);
        }

        // DELETE: ContactLogItem/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteContactLogItem([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var oldItem = await _context.ContactLogItems.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (oldItem == null) {
                return NotFound("That item was not found");
            }
            if (oldItem.CreatedById != GidIndustrial.Gideon.WebApi.Models.User.GetId(User)) {
                return BadRequest("You are not authorized to delete this because you didn't make it");
            }

            var contactLogItem = await _context.ContactLogItems
                .SingleOrDefaultAsync(m => m.Id == id);
            if (contactLogItem == null) {
                return NotFound();
            }

            _context.ContactLogItems.Remove(contactLogItem);
            await _context.SaveChangesAsync();

            return Ok(contactLogItem);
        }

        private bool ContactLogItemExists(int id) {
            return _context.ContactLogItems.Any(e => e.Id == id);
        }
    }
}