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
    [Route("QuoteLineItems")]
    public class QuoteLineItemsController : Controller
    {
        private readonly AppDBContext _context;

        public QuoteLineItemsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: QuoteLineItems
        [HttpGet]
        public IEnumerable<QuoteLineItem> GetQuoteLineItems()
        {
            var lineItems = _context.QuoteLineItems.OrderBy(i => i.Order);

            return lineItems;
        }

        // GET: QuoteLineItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetQuoteLineItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var quoteLineItem = await _context.QuoteLineItems.SingleOrDefaultAsync(m => m.Id == id);

            if (quoteLineItem == null)
            {
                return NotFound();
            }

            return Ok(quoteLineItem);
        }

        // PUT: QuoteLineItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutQuoteLineItem([FromRoute] int id, [FromBody] QuoteLineItem quoteLineItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != quoteLineItem.Id)
            {
                return BadRequest();
            }

            _context.Entry(quoteLineItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!QuoteLineItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var quote = await _context.Quotes
                .Include(item => item.LineItems)
                .FirstOrDefaultAsync(item => item.Id == quoteLineItem.QuoteId);
            await quote.UpdateTotal(_context);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // POST: QuoteLineItems
        [HttpPost]
        public async Task<IActionResult> PostQuoteLineItem([FromBody] QuoteLineItem quoteLineItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.QuoteLineItems.Add(quoteLineItem);
            await _context.SaveChangesAsync();
            
            var quote = await _context.Quotes
                .Include(item => item.LineItems)
                .FirstOrDefaultAsync(item => item.Id == quoteLineItem.QuoteId);
            await quote.UpdateTotal(_context);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetQuoteLineItem", new { id = quoteLineItem.Id }, quoteLineItem);
        }

        // DELETE: QuoteLineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuoteLineItem([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("Good Work");
                return BadRequest(ModelState);
            }

            var quoteLineItem = await _context.QuoteLineItems.SingleOrDefaultAsync(m => m.Id == id);
            if (quoteLineItem == null)
            {
                return NotFound();
            }

            using(var transaction = _context.Database.BeginTransaction()){
                _context.QuoteLineItems.Remove(quoteLineItem);
                try{
                    await _context.SaveChangesAsync();
                }catch(Exception ex){
                    return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
                
                var quote = await _context.Quotes
                    .Include(item => item.LineItems)
                    .FirstOrDefaultAsync(item => item.Id == quoteLineItem.QuoteId);
                await quote.UpdateTotal(_context);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            return Ok(quoteLineItem);
        }

        private bool QuoteLineItemExists(int id)
        {
            return _context.QuoteLineItems.Any(e => e.Id == id);
        }
    }
}