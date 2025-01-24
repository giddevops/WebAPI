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
    [Route("RmaLineItems")]
    public class RmaLineItemsController : Controller
    {
        private readonly AppDBContext _context;

        public RmaLineItemsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RmaLineItems
        [HttpGet]
        public IEnumerable<RmaLineItem> GetRmaLineItems()
        {
            return _context.RmaLineItems;
        }

        // GET: RmaLineItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRmaLineItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaLineItem = await _context.RmaLineItems.SingleOrDefaultAsync(m => m.Id == id);

            if (rmaLineItem == null)
            {
                return NotFound();
            }

            return Ok(rmaLineItem);
        }

        // PUT: RmaLineItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRmaLineItem([FromRoute] int id, [FromBody] RmaLineItem rmaLineItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rmaLineItem.Id)
            {
                return BadRequest();
            }

            rmaLineItem.UpdatedAt = DateTime.UtcNow;
            _context.Entry(rmaLineItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RmaLineItemExists(id))
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

        // POST: RmaLineItems
        [HttpPost]
        public async Task<IActionResult> PostRmaLineItem([FromBody] RmaLineItem rmaLineItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            rmaLineItem.CreatedAt = DateTime.UtcNow;
            _context.RmaLineItems.Add(rmaLineItem);
            await _context.SaveChangesAsync();

            rmaLineItem = await _context.RmaLineItems
                .Include(item => item.InventoryItem)
                    .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.Id == rmaLineItem.Id);

            return CreatedAtAction("GetRmaLineItem", new { id = rmaLineItem.Id }, rmaLineItem);
        }

        // DELETE: RmaLineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRmaLineItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaLineItem = await _context.RmaLineItems.SingleOrDefaultAsync(m => m.Id == id);
            if (rmaLineItem == null)
            {
                return NotFound();
            }

            _context.RmaLineItems.Remove(rmaLineItem);
            await _context.SaveChangesAsync();

            return Ok(rmaLineItem);
        }

        private bool RmaLineItemExists(int id)
        {
            return _context.RmaLineItems.Any(e => e.Id == id);
        }
    }
}