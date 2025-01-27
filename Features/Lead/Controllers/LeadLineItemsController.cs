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
    [Route("LeadLineItems")]
    public class LeadLineItemsController : Controller
    {
        private readonly AppDBContext _context;

        public LeadLineItemsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: LeadLineItems
        [HttpGet]
        public IEnumerable<LeadLineItem> GetLeadLineItems()
        {
            return _context.LeadLineItems.Include(item => item.Sources);
        }

        // GET: LeadLineItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadLineItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leadLineItem = await _context.LeadLineItems.SingleOrDefaultAsync(m => m.Id == id);

            if (leadLineItem == null)
            {
                return NotFound();
            }

            return Ok(leadLineItem);
        }

        // PUT: LeadLineItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeadLineItem([FromRoute] int id, [FromBody] LeadLineItem leadLineItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != leadLineItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(leadLineItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeadLineItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(leadLineItem); ;
        }

        // POST: LeadLineItems
        [HttpPost]
        public async Task<IActionResult> PostLeadLineItem([FromBody] LeadLineItem leadLineItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            leadLineItem.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);

            _context.LeadLineItems.Add(leadLineItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeadLineItem", new { id = leadLineItem.Id }, leadLineItem);
        }

        // DELETE: LeadLineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeadLineItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leadLineItem = await _context.LeadLineItems.SingleOrDefaultAsync(m => m.Id == id);
            if (leadLineItem == null)
            {
                return NotFound();
            }

            _context.LeadLineItems.Remove(leadLineItem);
            try{
                await _context.SaveChangesAsync();
            }catch(Exception ex){
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(leadLineItem);
        }

        private bool LeadLineItemExists(int id)
        {
            return _context.LeadLineItems.Any(e => e.Id == id);
        }
    }
}