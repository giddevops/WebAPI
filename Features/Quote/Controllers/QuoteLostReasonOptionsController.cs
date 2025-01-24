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
    [Route("QuoteLostReasonOptions")]
    public class QuoteLostReasonOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public QuoteLostReasonOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: QuoteLostReasonOptions
        [HttpGet]
        public IEnumerable<QuoteLostReasonOption> GetQuoteLostReasonOptions()
        {
            return _context.QuoteLostReasonOptions;
        }

        // GET: QuoteLostReasonOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuoteLostReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quoteLostReason = await _context.QuoteLostReasonOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (quoteLostReason == null)
            {
                return NotFound();
            }

            return Ok(quoteLostReason);
        }

        // PUT: QuoteLostReasonOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutQuoteLostReasonOption([FromRoute] int id, [FromBody] QuoteLostReasonOption quoteLostReason)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quoteLostReason.Id)
            {
                return BadRequest();
            }

            _context.Entry(quoteLostReason).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteLostReasonOptionExists(id))
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

        // POST: QuoteLostReasonOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostQuoteLostReasonOption([FromBody] QuoteLostReasonOption quoteLostReason)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            quoteLostReason.CreatedAt = DateTime.UtcNow;
            _context.QuoteLostReasonOptions.Add(quoteLostReason);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuoteLostReasonOption", new { id = quoteLostReason.Id }, quoteLostReason);
        }

        // DELETE: QuoteLostReasonOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteQuoteLostReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quoteLostReason = await _context.QuoteLostReasonOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (quoteLostReason == null)
            {
                return NotFound();
            }

            _context.QuoteLostReasonOptions.Remove(quoteLostReason);
            await _context.SaveChangesAsync();

            return Ok(quoteLostReason);
        }

        private bool QuoteLostReasonOptionExists(int id)
        {
            return _context.QuoteLostReasonOptions.Any(e => e.Id == id);
        }
    }
}