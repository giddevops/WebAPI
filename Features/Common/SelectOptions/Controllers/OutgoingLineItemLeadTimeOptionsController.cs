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
    [Route("OutgoingLineItemLeadTimeOptions")]
    public class OutgoingLineItemLeadTimeOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public OutgoingLineItemLeadTimeOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: OutgoingLineItemLeadTimeOptions
        [HttpGet]
        public IEnumerable<OutgoingLineItemLeadTimeOption> GetOutgoingLineItemLeadTimeOption()
        {
            return _context.OutgoingLineItemLeadTimeOptions.OrderBy(item => item.Value);
        }

        // GET: OutgoingLineItemLeadTimeOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutgoingLineItemLeadTimeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingLineItemLeadTimeOption = await _context.OutgoingLineItemLeadTimeOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (outgoingLineItemLeadTimeOption == null)
            {
                return NotFound();
            }

            return Ok(outgoingLineItemLeadTimeOption);
        }

        // PUT: OutgoingLineItemLeadTimeOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutOutgoingLineItemLeadTimeOption([FromRoute] int id, [FromBody] OutgoingLineItemLeadTimeOption outgoingLineItemLeadTimeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != outgoingLineItemLeadTimeOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(outgoingLineItemLeadTimeOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OutgoingLineItemLeadTimeOptionExists(id))
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

        // POST: OutgoingLineItemLeadTimeOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostOutgoingLineItemLeadTimeOption([FromBody] OutgoingLineItemLeadTimeOption outgoingLineItemLeadTimeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            outgoingLineItemLeadTimeOption.CreatedAt = DateTime.UtcNow;
            _context.OutgoingLineItemLeadTimeOptions.Add(outgoingLineItemLeadTimeOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOutgoingLineItemLeadTimeOption", new { id = outgoingLineItemLeadTimeOption.Id }, outgoingLineItemLeadTimeOption);
        }

        // DELETE: OutgoingLineItemLeadTimeOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteOutgoingLineItemLeadTimeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingLineItemLeadTimeOption = await _context.OutgoingLineItemLeadTimeOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (outgoingLineItemLeadTimeOption == null)
            {
                return NotFound();
            }

            _context.OutgoingLineItemLeadTimeOptions.Remove(outgoingLineItemLeadTimeOption);
            await _context.SaveChangesAsync();

            return Ok(outgoingLineItemLeadTimeOption);
        }

        private bool OutgoingLineItemLeadTimeOptionExists(int id)
        {
            return _context.OutgoingLineItemLeadTimeOptions.Any(e => e.Id == id);
        }
    }
}