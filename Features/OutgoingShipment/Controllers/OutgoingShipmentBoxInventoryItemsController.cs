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
    [Route("OutgoingShipmentBoxInventoryItems")]
    public class OutgoingShipmentBoxInventoryItemsController : Controller
    {
        private readonly AppDBContext _context;

        public OutgoingShipmentBoxInventoryItemsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: OutgoingShipmentBoxInventoryItems?outgoingShipmentBoxId=&inventoryItemId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="outgoingShipmentBoxId"></param>
        /// <param name="inventoryItemId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetOutgoingShipmentBoxInventoryItemById([FromQuery] int? outgoingShipmentBoxId, [FromQuery] int? inventoryItemId)
        {

            var outgoingShipmentBoxInventoryItem = await _context.OutgoingShipmentBoxInventoryItems.SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.OutgoingShipmentBoxId == outgoingShipmentBoxId);

            if (outgoingShipmentBoxInventoryItem == null)
            {
                return NotFound();
            }

            return Ok(outgoingShipmentBoxInventoryItem);
        }

        // GET: OutgoingShipmentBoxInventoryItems
        /// <summary>
        /// Fetches a list of outgoingShipmentBox inventoryItems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOutgoingShipmentBoxInventoryItems([FromQuery] int? outgoingShipmentBoxId, [FromQuery] int? inventoryItemId)
        {
            if (outgoingShipmentBoxId == null && inventoryItemId == null)
            {
                return BadRequest(new
                {
                    Error = "must have either outgoingShipmentBoxId or inventoryItemId querystring param"
                });
            }
            var query = from outgoingShipmentBox in _context.OutgoingShipmentBoxInventoryItems select outgoingShipmentBox;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (outgoingShipmentBoxId != null)
            {
                query = query.Where(item => item.OutgoingShipmentBoxId == outgoingShipmentBoxId).Include(m => m.InventoryItem);
            }
            if (inventoryItemId != null)
            {
                query = query.Where(item => item.InventoryItemId == inventoryItemId).Include(m => m.OutgoingShipmentBox);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: OutgoingShipmentBoxInventoryItems?inventoryItemId=&outgoingShipmentBoxId=
        [HttpPut]
        public async Task<IActionResult> PutOutgoingShipmentBoxInventoryItem([FromQuery] int? inventoryItemId, [FromQuery] int? outgoingShipmentBoxId, [FromBody] OutgoingShipmentBoxInventoryItem outgoingShipmentBoxInventoryItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.Entry(outgoingShipmentBoxInventoryItem).State = EntityState.Modified;

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

        // POST: OutgoingShipmentBoxInventoryItems
        [HttpPost]
        public async Task<IActionResult> PostOutgoingShipmentBoxInventoryItem([FromBody] OutgoingShipmentBoxInventoryItem outgoingShipmentBoxInventoryItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OutgoingShipmentBoxInventoryItems.Add(outgoingShipmentBoxInventoryItem);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (OutgoingShipmentBoxInventoryItemExists(outgoingShipmentBoxInventoryItem.OutgoingShipmentBoxId, outgoingShipmentBoxInventoryItem.InventoryItemId))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            outgoingShipmentBoxInventoryItem = await _context.OutgoingShipmentBoxInventoryItems
                .Include(item => item.InventoryItem)
                    .ThenInclude(item => item.Product)
                .Include(item => item.OutgoingShipmentBox)
                .FirstOrDefaultAsync(item => item.OutgoingShipmentBoxId == outgoingShipmentBoxInventoryItem.OutgoingShipmentBoxId && item.InventoryItemId == outgoingShipmentBoxInventoryItem.InventoryItemId);

            return CreatedAtAction("GetOutgoingShipmentBoxInventoryItem", new { id = outgoingShipmentBoxInventoryItem.InventoryItemId }, outgoingShipmentBoxInventoryItem);
        }

        // DELETE: OutgoingShipmentBoxInventoryItems?outgoingShipmentBoxId=&inventoryItemid=
        [HttpDelete]
        public async Task<IActionResult> DeleteOutgoingShipmentBoxInventoryItem([FromQuery] int outgoingShipmentBoxId, [FromQuery] int inventoryItemId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingShipmentBoxInventoryItem = await _context.OutgoingShipmentBoxInventoryItems.SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.OutgoingShipmentBoxId == outgoingShipmentBoxId);
            if (outgoingShipmentBoxInventoryItem == null)
            {
                return BadRequest(new
                {
                    outgoingShipmentBoxId = outgoingShipmentBoxId,
                    inventoryItemId = inventoryItemId
                });
            }

            _context.OutgoingShipmentBoxInventoryItems.Remove(outgoingShipmentBoxInventoryItem);
            await _context.SaveChangesAsync();

            return Ok(outgoingShipmentBoxInventoryItem);
        }

        private bool OutgoingShipmentBoxInventoryItemExists(int? outgoingShipmentBoxId, int? inventoryItemId)
        {
            return _context.OutgoingShipmentBoxInventoryItems.Any(e => e.InventoryItemId == inventoryItemId && e.OutgoingShipmentBoxId == outgoingShipmentBoxId);
        }
    }
}