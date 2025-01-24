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
    [Route("RmaOutgoingShipments")]
    public class RmaOutgoingShipmentsController : Controller
    {
        private readonly AppDBContext _context;

        public RmaOutgoingShipmentsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RmaOutgoingShipments?rmaId=&outgoingShipmentId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="rmaId"></param>
        /// <param name="outgoingShipmentId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetRmaOutgoingShipmentById([FromQuery] int? rmaId, [FromQuery] int? outgoingShipmentId)
        {

            var rmaOutgoingShipment = await _context.RmaOutgoingShipments.SingleOrDefaultAsync(m => m.OutgoingShipmentId == outgoingShipmentId && m.RmaId == rmaId);

            if (rmaOutgoingShipment == null)
            {
                return NotFound();
            }

            return Ok(rmaOutgoingShipment);
        }

        // GET: RmaOutgoingShipments
        /// <summary>
        /// Fetches a list of rma outgoingShipments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRmaOutgoingShipments([FromQuery] int? rmaId, [FromQuery] int? outgoingShipmentId)
        {
            if (rmaId == null && outgoingShipmentId == null)
            {
                return BadRequest(new
                {
                    Error = "must have either rmaId or outgoingShipmentId querystring param"
                });
            }
            var query  = from rma in _context.RmaOutgoingShipments select rma;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if(rmaId != null){
                query = query.Where(item => item.RmaId == rmaId)
                    .Include(m => m.OutgoingShipment)
                        .ThenInclude(item => item.Boxes)
                            .ThenInclude(item => item.InventoryItems)
                                .ThenInclude(item => item.InventoryItem)
                                    .ThenInclude(item => item.Product)
                    .Include(m => m.OutgoingShipment)
                        .ThenInclude(m => m.ShippingAddress);
            }
            if(outgoingShipmentId != null){
                query = query.Where(item => item.OutgoingShipmentId == outgoingShipmentId).Include(m => m.Rma);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: RmaOutgoingShipments?outgoingShipmentId=&rmaId=
        [HttpPut]
        public async Task<IActionResult> PutRmaOutgoingShipment([FromQuery] int? outgoingShipmentId, [FromQuery] int? rmaId, [FromBody] RmaOutgoingShipment rmaOutgoingShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(rmaOutgoingShipment).State = EntityState.Modified;

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

        // POST: RmaOutgoingShipments
        [HttpPost]
        public async Task<IActionResult> PostRmaOutgoingShipment([FromBody] RmaOutgoingShipment rmaOutgoingShipment)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RmaOutgoingShipments.Add(rmaOutgoingShipment);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (RmaOutgoingShipmentExists(rmaOutgoingShipment.RmaId, rmaOutgoingShipment.OutgoingShipmentId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetRmaOutgoingShipment", new { id = rmaOutgoingShipment.OutgoingShipmentId }, rmaOutgoingShipment);
        }

        // DELETE: RmaOutgoingShipments?rmaId=&outgoingShipmentid=
        [HttpDelete]
        public async Task<IActionResult> DeleteRmaOutgoingShipment([FromQuery] int rmaId, [FromQuery] int outgoingShipmentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaOutgoingShipment = await _context.RmaOutgoingShipments.SingleOrDefaultAsync(m => m.OutgoingShipmentId == outgoingShipmentId && m.RmaId == rmaId);
            if (rmaOutgoingShipment == null)
            {
                return NotFound();
            }

            _context.RmaOutgoingShipments.Remove(rmaOutgoingShipment);
            await _context.SaveChangesAsync();

            return Ok(rmaOutgoingShipment);
        }

        private bool RmaOutgoingShipmentExists(int? rmaId, int? outgoingShipmentId)
        {
            return _context.RmaOutgoingShipments.Any(e => e.OutgoingShipmentId == outgoingShipmentId && e.RmaId == rmaId);
        }
    }
}