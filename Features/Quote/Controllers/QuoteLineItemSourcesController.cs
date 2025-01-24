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
    [Route("QuoteLineItemSources")]
    public class QuoteLineItemSourcesController : Controller
    {
        private readonly AppDBContext _context;

        public QuoteLineItemSourcesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: QuoteLineItemSources?quoteLineItemId=&sourceId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="quoteLineItemId"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetQuoteLineItemSourceById([FromQuery] int? quoteLineItemId, [FromQuery] int? sourceId)
        {

            var quoteLineItemSource = await _context.QuoteLineItemSources
                .SingleOrDefaultAsync(m => m.SourceId == sourceId && m.QuoteLineItemId == quoteLineItemId);

            if (quoteLineItemSource == null)
            {
                return NotFound();
            }

            return Ok(quoteLineItemSource);
        }


        // GET: QuoteLineItemSources
        /// <summary>
        /// Fetches a list of quoteLineItem sources
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetQuoteLineItemSources([FromQuery] int? quoteLineItemId, [FromQuery] int? sourceId) {
            if (quoteLineItemId == null && sourceId == null) {
                return BadRequest(new {
                    Error = "must have either quoteLineItemId or sourceId or purchaseOrderId querystring param"
                });
            }
            var query = from quoteLineItem in _context.QuoteLineItemSources select quoteLineItem;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (quoteLineItemId != null) {
                query = query
                    .Where(item => item.QuoteLineItemId == quoteLineItemId)
                    .Include(m => m.Source)
                        .ThenInclude(s => s.Supplier)
                    .Include(m => m.Source)
                        .ThenInclude(s => s.Currency);
                    
            }
            if (sourceId != null)
                query = query.Where(item => item.SourceId == sourceId).Include(m => m.QuoteLineItem);

            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: QuoteLineItemSources?sourceId=&quoteLineItemId=
        [HttpPut]
        public async Task<IActionResult> PutQuoteLineItemSource([FromQuery] int? sourceId, [FromQuery] int? quoteLineItemId, [FromBody] QuoteLineItemSource quoteLineItemSource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(quoteLineItemSource).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: QuoteLineItemSources
        [HttpPost]
        public async Task<IActionResult> PostQuoteLineItemSource([FromBody] QuoteLineItemSource quoteLineItemSource)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(quoteLineItemSource.Source != null){
                if(quoteLineItemSource.Source.CurrencyOptionId == null){
                    quoteLineItemSource.Source.CurrencyOptionId = (await _context.CurrencyOptions.FirstOrDefaultAsync(item => item.Value == "USD")).Id;
                }
            }

            _context.QuoteLineItemSources.Add(quoteLineItemSource);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (QuoteLineItemSourceExists(quoteLineItemSource.QuoteLineItemId, quoteLineItemSource.SourceId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetQuoteLineItemSource", new { id = quoteLineItemSource.SourceId }, quoteLineItemSource);
        }

        // DELETE: QuoteLineItemSources?quoteLineItemId=&sourceid=
        [HttpDelete]
        public async Task<IActionResult> DeleteQuoteLineItemSource([FromQuery] int quoteLineItemId, [FromQuery] int sourceId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quoteLineItemSource = await _context.QuoteLineItemSources.SingleOrDefaultAsync(m => m.SourceId == sourceId && m.QuoteLineItemId == quoteLineItemId);
            if (quoteLineItemSource == null)
            {
                return NotFound();
            }

            _context.QuoteLineItemSources.Remove(quoteLineItemSource);
            await _context.SaveChangesAsync();

            return Ok(quoteLineItemSource);
        }

        private bool QuoteLineItemSourceExists(int? quoteLineItemId, int? sourceId)
        {
            return _context.QuoteLineItemSources.Any(e => e.SourceId == sourceId && e.QuoteLineItemId == quoteLineItemId);
        }
    }
}