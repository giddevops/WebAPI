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
    [Route("QuoteFilters")]
    public class QuoteFiltersController : Controller
    {
        private readonly AppDBContext _context;

        public QuoteFiltersController(AppDBContext context)
        {
            _context = context;
        }

        // GET: QuoteFilters
        [HttpGet]
        public IEnumerable<QuoteFilter> GetQuoteFilters()
        {
            return _context.QuoteFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
        }

        // GET: QuoteFilters/SelectOptions
        // Transform the objects to match other select options with Id, Value pairs
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetQuoteFilterSelectOptions()
        {
            return _context.QuoteFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)).Select(lf => new{
                Id = lf.Id,
                Value = lf.Name
            });
        }

        // GET: QuoteFilters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuoteFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quoteFilter = await _context.QuoteFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));

            if (quoteFilter == null)
            {
                return NotFound();
            }

            return Ok(quoteFilter);
        }

        // PUT: QuoteFilters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuoteFilter([FromRoute] int id, [FromBody] QuoteFilter quoteFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quoteFilter.Id)
            {
                return BadRequest();
            }
            
            var dbQuoteFilter = await _context.QuoteFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));
            if(dbQuoteFilter == null){
                return NotFound();
            }


            _context.Entry(quoteFilter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteFilterExists(id))
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

        // POST: QuoteFilters
        [HttpPost]
        public async Task<IActionResult> PostQuoteFilter([FromBody] QuoteFilter quoteFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            quoteFilter.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            quoteFilter.CreatedAt = DateTime.UtcNow;

            _context.QuoteFilters.Add(quoteFilter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuoteFilter", new { id = quoteFilter.Id }, quoteFilter);
        }

        // DELETE: QuoteFilters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuoteFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quoteFilter = await _context.QuoteFilters.SingleOrDefaultAsync(m => m.Id == id && m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            if (quoteFilter == null)
            {
                return NotFound();
            }

            _context.QuoteFilters.Remove(quoteFilter);
            await _context.SaveChangesAsync();

            return Ok(quoteFilter);
        }

        private bool QuoteFilterExists(int id)
        {
            return _context.QuoteFilters.Any(e => e.Id == id);
        }
    }
}