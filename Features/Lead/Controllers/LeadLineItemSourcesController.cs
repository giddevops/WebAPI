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
    [Route("LeadLineItemSources")]
    public class LeadLineItemSourcesController : Controller {
        private readonly AppDBContext _context;

        public LeadLineItemSourcesController(AppDBContext context) {
            _context = context;
        }


        // GET: LeadLineItemSources?leadLineItemId=&sourceId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="leadLineItemId"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetLeadLineItemSourceById([FromQuery] int? leadLineItemId, [FromQuery] int? sourceId) {

            var leadLineItemSource = await _context.LeadLineItemSources.SingleOrDefaultAsync(m => m.SourceId == sourceId && m.LeadLineItemId == leadLineItemId);

            if (leadLineItemSource == null) {
                return NotFound();
            }

            return Ok(leadLineItemSource);
        }

        // GET: LeadLineItemSources
        /// <summary>
        /// Fetches a list of leadLineItem sources
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetLeadLineItemSources([FromQuery] int? leadLineItemId, [FromQuery] int? sourceId) {
            if (leadLineItemId == null && sourceId == null) {
                return BadRequest(new {
                    Error = "must have either leadLineItemId or sourceId or purchaseOrderId querystring param"
                });
            }
            var query = from leadLineItem in _context.LeadLineItemSources select leadLineItem;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (leadLineItemId != null) {
                query = query
                    .Where(item => item.LeadLineItemId == leadLineItemId)
                    .Include(m => m.Source)
                        .ThenInclude(s => s.Supplier);
            }
            if (sourceId != null)
                query = query.Where(item => item.SourceId == sourceId).Include(m => m.LeadLineItem);

            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: LeadLineItemSources?sourceId=&leadLineItemId=
        [HttpPut]
        public async Task<IActionResult> PutLeadLineItemSource([FromQuery] int? sourceId, [FromQuery] int? leadLineItemId, [FromBody] LeadLineItemSource leadLineItemSource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(leadLineItemSource).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: LeadLineItemSources
        [HttpPost]
        public async Task<IActionResult> PostLeadLineItemSource([FromBody] LeadLineItemSource leadLineItemSource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.LeadLineItemSources.Add(leadLineItemSource);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (LeadLineItemSourceExists(leadLineItemSource.LeadLineItemId, leadLineItemSource.SourceId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            return CreatedAtAction("GetLeadLineItemSource", new { id = leadLineItemSource.SourceId }, leadLineItemSource);
        }

        // DELETE: LeadLineItemSources?leadLineItemId=&sourceid=
        [HttpDelete]
        public async Task<IActionResult> DeleteLeadLineItemSource([FromQuery] int leadLineItemId, [FromQuery] int sourceId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var leadLineItemSource = await _context.LeadLineItemSources.SingleOrDefaultAsync(m => m.SourceId == sourceId && m.LeadLineItemId == leadLineItemId);
            if (leadLineItemSource == null) {
                return NotFound();
            }

            _context.LeadLineItemSources.Remove(leadLineItemSource);
            await _context.SaveChangesAsync();

            return Ok(leadLineItemSource);
        }

        private bool LeadLineItemSourceExists(int? leadLineItemId, int? sourceId) {
            return _context.LeadLineItemSources.Any(e => e.SourceId == sourceId && e.LeadLineItemId == leadLineItemId);
        }
    }
}