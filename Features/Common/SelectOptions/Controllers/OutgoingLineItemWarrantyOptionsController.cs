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
    [Route("OutgoingLineItemWarrantyOptions")]
    public class OutgoingLineItemWarrantyOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public OutgoingLineItemWarrantyOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: OutgoingLineItemWarrantyOptions
        [HttpGet]
        public IEnumerable<OutgoingLineItemWarrantyOption> GetOutgoingLineItemWarrantyOption()
        {
            return _context.OutgoingLineItemWarrantyOptions.OrderBy(item => item.Value);
        }

        // GET: OutgoingLineItemWarrantyOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutgoingLineItemWarrantyOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingLineItemWarrantyOption = await _context.OutgoingLineItemWarrantyOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (outgoingLineItemWarrantyOption == null)
            {
                return NotFound();
            }

            return Ok(outgoingLineItemWarrantyOption);
        }

        // PUT: OutgoingLineItemWarrantyOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutOutgoingLineItemWarrantyOption([FromRoute] int id, [FromBody] OutgoingLineItemWarrantyOption outgoingLineItemWarrantyOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != outgoingLineItemWarrantyOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(outgoingLineItemWarrantyOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OutgoingLineItemWarrantyOptionExists(id))
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

        // POST: OutgoingLineItemWarrantyOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostOutgoingLineItemWarrantyOption([FromBody] OutgoingLineItemWarrantyOption outgoingLineItemWarrantyOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            outgoingLineItemWarrantyOption.CreatedAt = DateTime.UtcNow;
            _context.OutgoingLineItemWarrantyOptions.Add(outgoingLineItemWarrantyOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOutgoingLineItemWarrantyOption", new { id = outgoingLineItemWarrantyOption.Id }, outgoingLineItemWarrantyOption);
        }

        // DELETE: OutgoingLineItemWarrantyOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteOutgoingLineItemWarrantyOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingLineItemWarrantyOption = await _context.OutgoingLineItemWarrantyOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (outgoingLineItemWarrantyOption == null)
            {
                return NotFound();
            }

            _context.OutgoingLineItemWarrantyOptions.Remove(outgoingLineItemWarrantyOption);
            await _context.SaveChangesAsync();

            return Ok(outgoingLineItemWarrantyOption);
        }

        private bool OutgoingLineItemWarrantyOptionExists(int id)
        {
            return _context.OutgoingLineItemWarrantyOptions.Any(e => e.Id == id);
        }
    }
}