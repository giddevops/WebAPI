using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Sources")]
    public class SourcesController : Controller {
        private readonly AppDBContext _context;

        public SourcesController(AppDBContext context) {
            _context = context;
        }

        // GET: Sources
        [HttpGet]
        public async Task<IActionResult> GetSources(
            [FromQuery] int? productId,
            [FromQuery] int? hideRelatedToQuoteLineItemId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from source in _context.Sources select source;

            query = query
                .OrderByDescending(q => q.CreatedAt);

            query = query.Include(s => s.Supplier).Include(s => s.Product).Include(item => item.Currency);

            if (hideRelatedToQuoteLineItemId != null) {
                query = query.Where(item => !item.QuoteLineItems.Any(quoteLineItem => quoteLineItem.QuoteLineItemId == hideRelatedToQuoteLineItemId));
            }

            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "Product.PartNumber":
                    query = sortAscending ? query.OrderBy(item => item.Product.PartNumber) : query.OrderByDescending(item => item.Product.PartNumber);
                    break;
                case "Supplier.Name":
                    query = sortAscending ? query.OrderBy(item => item.Supplier.Name) : query.OrderByDescending(item => item.Supplier.Name);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            if (productId != null) {
                query = query.Where(item => item.ProductId == productId);
                var groupQuery = query.Where(s => s.SupplierId != null)
                    .OrderByDescending(item => item.CreatedAt)
                    .GroupBy(item => item.SupplierId)
                    .Select(grp => grp.OrderByDescending(item => item.CreatedAt).First())
                    .Include(item => item.Supplier);
                //load both sets into ram first and then paginate
                //doing Concat before ToList() would invariably fail to include the Supplier object
                var withoutSupplierQuery = query.Where(item => item.SupplierId == null);
                var itemsWithSupplier = await groupQuery.ToListAsync();
                var itemsWithoutSupplier = await withoutSupplierQuery.Where(item => item.SupplierId == null).ToListAsync();
                var allItems = itemsWithSupplier.Concat(itemsWithoutSupplier);

                return Ok(new ListResult {
                    Items = allItems.Skip(skip).Take(perPage),
                    Count = allItems.Count()
                });
            }


            return Ok(new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            });
        }

        // GET: Sources/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSource([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var source = await _context.Sources.Include(item => item.Contact).SingleOrDefaultAsync(m => m.Id == id);

            IQueryable<Lead> leadsQuery = from leadLineItemSource in _context.LeadLineItemSources
                                          join leadLineItem in _context.LeadLineItems on leadLineItemSource.LeadLineItemId equals leadLineItem.Id
                                          join lead in _context.Leads on leadLineItem.LeadId equals lead.Id
                                          where leadLineItemSource.SourceId == id
                                          select lead;

            source.Leads = await leadsQuery.ToListAsync();


            if (source == null) {
                return NotFound();
            }

            return Ok(source);
        }

        // PUT: Sources/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSource([FromRoute] int id, [FromBody] Source source) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != source.Id) {
                return BadRequest();
            }

            _context.Entry(source).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!SourceExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Sources
        [HttpPost]
        public async Task<IActionResult> PostSource([FromBody] Source source) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (source.CurrencyOptionId == null) {
                source.CurrencyOptionId = (await _context.CurrencyOptions.FirstOrDefaultAsync(item => item.Value == "USD")).Id;
            }

            _context.Sources.Add(source);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSource", new { id = source.Id }, source);
        }

        // DELETE: Sources/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSource([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var source = await _context.Sources.SingleOrDefaultAsync(m => m.Id == id);
            if (source == null) {
                return NotFound();
            }

            _context.Sources.Remove(source);
            await _context.SaveChangesAsync();

            return Ok(source);
        }

        private bool SourceExists(int id) {
            return _context.Sources.Any(e => e.Id == id);
        }
    }
}