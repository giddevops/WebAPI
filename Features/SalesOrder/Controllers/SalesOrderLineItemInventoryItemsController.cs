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
    [Route("SalesOrderLineItemInventoryItems")]
    public class SalesOrderLineItemInventoryItemsController : Controller {
        private readonly AppDBContext _context;

        public SalesOrderLineItemInventoryItemsController(AppDBContext context) {
            _context = context;
        }

        // GET: SalesOrderLineItemInventoryItems?salesOrderLineItemId=&inventoryItemId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="salesOrderLineItemId"></param>
        /// <param name="inventoryItemId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetSalesOrderLineItemInventoryItemById([FromQuery] int? salesOrderLineItemId, [FromQuery] int? inventoryItemId) {

            var salesOrderLineItemInventoryItem = await _context.SalesOrderLineItemInventoryItems
                .Include(m => m.InventoryItem)
                .SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.SalesOrderLineItemId == salesOrderLineItemId);

            if (salesOrderLineItemInventoryItem == null) {
                return NotFound();
            }

            return Ok(salesOrderLineItemInventoryItem);
        }

        // GET: SalesOrderLineItemInventoryItems
        /// <summary>
        /// Fetches a list of salesOrderLineItem inventoryItems
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSalesOrderLineItemInventoryItems([FromQuery] int? salesOrderLineItemId, [FromQuery] int? inventoryItemId) {
            if (salesOrderLineItemId == null && inventoryItemId == null) {
                return BadRequest(new {
                    Error = "must have either salesOrderLinItemId or inventoryItemId querystring param"
                });
            }
            var query = from salesOrderLineItem in _context.SalesOrderLineItemInventoryItems select salesOrderLineItem;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (salesOrderLineItemId != null) {
                query = query.Where(item => item.SalesOrderLineItemId == salesOrderLineItemId).Include(m => m.InventoryItem).ThenInclude(m => m.Product);
            }
            if (inventoryItemId != null) {
                query = query.Where(item => item.InventoryItemId == inventoryItemId).Include(m => m.SalesOrderLineItem);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: SalesOrderLineItemInventoryItems?inventoryItemId=&salesOrderLineItemId=
        [HttpPut]
        public async Task<IActionResult> PutSalesOrderLineItemInventoryItem([FromQuery] int? inventoryItemId, [FromQuery] int? salesOrderLineItemId, [FromBody] SalesOrderLineItemInventoryItem salesOrderLineItemInventoryItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(salesOrderLineItemInventoryItem).State = EntityState.Modified;

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

        //These must be created through this URL `InventoryItems/${inventoryItem.Id}/RelateToSalesOrderLineItem`
        //this ensures that the committed flag is properly set on the inventory item
        // POST: SalesOrderLineItemInventoryItems
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderLineItemInventoryItem([FromBody] SalesOrderLineItemInventoryItem salesOrderLineItemInventoryItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            int? salesOrderLineItemId, inventoryItemId;

            //prevent adding if inventory item is already comitted
            try {
                inventoryItemId = salesOrderLineItemInventoryItem.InventoryItemId ?? salesOrderLineItemInventoryItem.InventoryItem.Id;
            }
            catch {
                return BadRequest(new {
                    Error = "No InventoryItem or InventoryItemId was included in the POST object"
                });
            }
            InventoryItem inventoryItem = await _context.InventoryItems
                .FirstOrDefaultAsync(ii => ii.Id == salesOrderLineItemInventoryItem.InventoryItemId);
            if (inventoryItem.InventoryItemStatusOptionId == InventoryItemStatusOption.Committed) {
                return BadRequest(new {
                    Error = "This inventory item has already been comitted"
                });
            }
            try {
                salesOrderLineItemId = salesOrderLineItemInventoryItem.SalesOrderLineItemId ?? salesOrderLineItemInventoryItem.SalesOrderLineItem.Id;
            }
            catch {
                return BadRequest(new {
                    Error = "No SalesOrderLineItem or SalesOrderLineItemId was included in the POST object"
                });
            }

            _context.SalesOrderLineItemInventoryItems.Add(salesOrderLineItemInventoryItem);
            //Mark the inventory item as committed so it can't be added to any other.
            // inventoryItem.Committed = true;
            inventoryItem.InventoryItemStatusOptionId = InventoryItemStatusOption.Committed;
            _context.Entry(inventoryItem).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (SalesOrderLineItemInventoryItemExists(salesOrderLineItemInventoryItem.SalesOrderLineItemId, salesOrderLineItemInventoryItem.InventoryItemId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }
            salesOrderLineItemInventoryItem = await _context.SalesOrderLineItemInventoryItems
                .Include(item => item.SalesOrderLineItem)
                .Include(item => item.InventoryItem)
                .FirstOrDefaultAsync(item => item.SalesOrderLineItemId == salesOrderLineItemId && item.InventoryItemId == inventoryItemId);
            return CreatedAtAction("GetSalesOrderLineItemInventoryItem", new { id = salesOrderLineItemInventoryItem.InventoryItemId }, salesOrderLineItemInventoryItem);
        }

        // DELETE: SalesOrderLineItemInventoryItems?salesOrderLineItemId=&inventoryItemid=
        [HttpDelete]
        public async Task<IActionResult> DeleteSalesOrderLineItemInventoryItem([FromQuery] int salesOrderLineItemId, [FromQuery] int inventoryItemId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrderLineItemInventoryItem = await _context.SalesOrderLineItemInventoryItems
                .Include(item => item.SalesOrderLineItem)
                .SingleOrDefaultAsync(m => m.InventoryItemId == inventoryItemId && m.SalesOrderLineItemId == salesOrderLineItemId);
            if (salesOrderLineItemInventoryItem == null) {
                return NotFound();
            }

            //first check if the inventory item is a part of a box that has already been shipped.  If so stop them from shipping
            InventoryItem inventoryItem = await _context.InventoryItems
                .Include(item => item.OutgoingShipmentBoxes)
                    .ThenInclude(item => item.OutgoingShipmentBox)
                        .ThenInclude(item => item.OutgoingShipment)
                            .ThenInclude(item => item.SalesOrderOutgoingShipment)
                .FirstAsync(ii => ii.Id == salesOrderLineItemInventoryItem.InventoryItemId);

            //Check if the item has already been shipped with THIS sales order.  It could have been shipped with another sales order and then returned through an RMA, and this code allows for that.
            //First, check if the inventory item is part of any shipment boxes
            if (inventoryItem != null && inventoryItem.OutgoingShipmentBoxes.Count != 0) {
                //if so, go through all the shipment boxes
                foreach (var outgoingShipmentBox in inventoryItem.OutgoingShipmentBoxes) {
                    if (
                        //check if the box was for the current sales order
                        outgoingShipmentBox.OutgoingShipmentBox.OutgoingShipment.SalesOrderOutgoingShipment.SalesOrderId == salesOrderLineItemInventoryItem.SalesOrderLineItem.SalesOrderId &&
                        //then check if the shipment was shipped already
                        outgoingShipmentBox.OutgoingShipmentBox.OutgoingShipment.ShippedAt != null
                    ) {
                        return BadRequest(new {
                            Error = "You can't remove this since it's already part of a box that has been shipped"
                        });
                    }
                }
            }

            //Mark the inventory item as NOT committed so it is available for other sales orders now
            // inventoryItem.Committed = false;
            inventoryItem.InventoryItemStatusOptionId = InventoryItemStatusOption.Available;
            _context.Entry(inventoryItem).State = EntityState.Modified;

            //remove the inventory item from any shipment boxes
            List<OutgoingShipmentBoxInventoryItem> outgoingShipmentBoxInventoryItems = await _context.OutgoingShipmentBoxInventoryItems
                .Where(item =>
                    item.InventoryItemId == inventoryItemId &&
                    item.OutgoingShipmentBox.OutgoingShipment.SalesOrderOutgoingShipment.SalesOrderId == salesOrderLineItemInventoryItem.SalesOrderLineItem.SalesOrderId
                ).ToListAsync();
            outgoingShipmentBoxInventoryItems.ForEach(item => _context.OutgoingShipmentBoxInventoryItems.Remove(item));

            //remove the relationship between sales order line item and inventory item.
            _context.SalesOrderLineItemInventoryItems.Remove(salesOrderLineItemInventoryItem);

            await _context.SaveChangesAsync();

            return Ok();
        }

        private bool SalesOrderLineItemInventoryItemExists(int? salesOrderLineItemId, int? inventoryItemId) {
            return _context.SalesOrderLineItemInventoryItems.Any(e => e.InventoryItemId == inventoryItemId && e.SalesOrderLineItemId == salesOrderLineItemId);
        }
    }
}