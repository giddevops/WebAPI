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
    [Route("QuoteStatusOptions")]
    public class QuoteStatusOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public QuoteStatusOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: QuoteStatusOptions
        [HttpGet]
        public IEnumerable<QuoteStatusOption> GetQuoteStatusOptions()
        {
            return _context.QuoteStatusOptions;
        }

        // GET: QuoteStatusOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuoteStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quoteStatusOption = await _context.QuoteStatusOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (quoteStatusOption == null)
            {
                return NotFound();
            }

            return Ok(quoteStatusOption);
        }

        // PUT: QuoteStatusOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutQuoteStatusOption([FromRoute] int id, [FromBody] QuoteStatusOption quoteStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quoteStatusOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(quoteStatusOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteStatusOptionExists(id))
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

        // POST: QuoteStatusOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostQuoteStatusOption([FromBody] QuoteStatusOption quoteStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.QuoteStatusOptions.Add(quoteStatusOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuoteStatusOption", new { id = quoteStatusOption.Id }, quoteStatusOption);
        }

        // DELETE: QuoteStatusOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteQuoteStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quoteStatusOption = await _context.QuoteStatusOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (quoteStatusOption == null)
            {
                return NotFound();
            }

            _context.QuoteStatusOptions.Remove(quoteStatusOption);
            await _context.SaveChangesAsync();

            return Ok(quoteStatusOption);
        }

        private bool QuoteStatusOptionExists(int id)
        {
            return _context.QuoteStatusOptions.Any(e => e.Id == id);
        }
    }
}