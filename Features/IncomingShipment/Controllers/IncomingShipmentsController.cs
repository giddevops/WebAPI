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
    [Route("IncomingShipments")]
    public class IncomingShipmentsController : Controller {
        private readonly AppDBContext _context;

        public IncomingShipmentsController(AppDBContext context) {
            _context = context;
        }

        // GET: IncomingShipments
        [HttpGet]
        public IEnumerable<IncomingShipment> GetIncomingShipments() {
            return _context.IncomingShipments;
        }

        // GET: IncomingShipments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetIncomingShipment([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var incomingShipment = await _context.IncomingShipments.SingleOrDefaultAsync(m => m.Id == id);

            if (incomingShipment == null) {
                return NotFound();
            }

            return Ok(incomingShipment);
        }

        // PUT: IncomingShipments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutIncomingShipment([FromRoute] int? id, [FromBody] IncomingShipment incomingShipment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != incomingShipment.Id) {
                return BadRequest();
            }

            _context.Entry(incomingShipment).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!IncomingShipmentExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: IncomingShipments
        [HttpPost]
        public async Task<IActionResult> PostIncomingShipment([FromBody] IncomingShipment incomingShipment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            incomingShipment.CreatedAt = DateTime.UtcNow;
            _context.IncomingShipments.Add(incomingShipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetIncomingShipment", new { id = incomingShipment.Id }, incomingShipment);
        }

        // DELETE: IncomingShipments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteIncomingShipment([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var incomingShipment = await _context.IncomingShipments
                .Include(item => item.TrackingEvents)
                    .ThenInclude(item => item.ShipmentTrackingEvent)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (incomingShipment == null) {
                return NotFound();
            }
            //need to delete all tracking info for this shipment
            foreach (var incomingShipmentTrackingEvent in incomingShipment.TrackingEvents) {
                _context.Entry(incomingShipmentTrackingEvent.ShipmentTrackingEvent).State = EntityState.Deleted;
                _context.Entry(incomingShipmentTrackingEvent).State = EntityState.Deleted;
            }
            _context.IncomingShipments.Remove(incomingShipment);
            await _context.SaveChangesAsync();

            return Ok(incomingShipment);
        }

        private bool IncomingShipmentExists(int? id) {
            return _context.IncomingShipments.Any(e => e.Id == id);
        }
    }
}