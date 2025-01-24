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
    [Route("ShippingCarriers")]
    public class ShippingCarriersController : Controller {
        private readonly AppDBContext _context;

        public ShippingCarriersController(AppDBContext context) {
            _context = context;
        }

        // GET: ShippingCarriers
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetShippingCarriers() {
            var carriers = _context.ShippingCarriers.Select(sc => new {
                Id = sc.Id,
                Value = sc.Name
            });
            return carriers;
            // return _context.ShippingCarriers;
        }

        // GET: ShippingCarriers
        [HttpGet()]
        public IActionResult GetShippingCarriersListResult(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] bool forOnlineQuote = false) {

            if (forOnlineQuote) {
                return Ok(_context.ShippingCarriers
                    .Include(item => item.ShippingMethods).Where(item => item.HideFromCustomer != true));
            }
            var query = from shippingCarrer in _context.ShippingCarriers select shippingCarrer;

            query = query
                .OrderByDescending(q => q.CreatedAt);

            var results = new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
            return Ok(results);
        }

        // GET: ShippingCarriers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShippingCarrier([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var shippingCarrier = await _context.ShippingCarriers
                .Include(sc => sc.ShippingMethods)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (shippingCarrier == null) {
                return NotFound();
            }

            return Ok(shippingCarrier);
        }

        // PUT: ShippingCarriers/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShippingCarrier([FromRoute] int? id, [FromBody] ShippingCarrier shippingCarrier) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != shippingCarrier.Id) {
                return BadRequest();
            }

            _context.Entry(shippingCarrier).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ShippingCarrierExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ShippingCarriers
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostShippingCarrier([FromBody] ShippingCarrier shippingCarrier) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.ShippingCarriers.Add(shippingCarrier);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShippingCarrier", new { id = shippingCarrier.Id }, shippingCarrier);
        }

        // DELETE: ShippingCarriers/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingCarrier([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var shippingCarrier = await _context.ShippingCarriers.SingleOrDefaultAsync(m => m.Id == id);
            if (shippingCarrier == null) {
                return NotFound();
            }

            _context.ShippingCarriers.Remove(shippingCarrier);
            await _context.SaveChangesAsync();

            return Ok(shippingCarrier);
        }

        private bool ShippingCarrierExists(int? id) {
            return _context.ShippingCarriers.Any(e => e.Id == id);
        }
    }
}