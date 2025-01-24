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
    [Route("SalesOrderOutgoingShipments")]
    public class SalesOrderOutgoingShipmentsController : Controller {
        private readonly AppDBContext _context;

        public SalesOrderOutgoingShipmentsController(AppDBContext context) {
            _context = context;
        }

        // GET: SalesOrderOutgoingShipments?salesOrderId=&outgoingShipmentId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="salesOrderId"></param>
        /// <param name="outgoingShipmentId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetSalesOrderOutgoingShipmentById([FromQuery] int? salesOrderId, [FromQuery] int? outgoingShipmentId) {

            var salesOrderOutgoingShipment = await _context.SalesOrderOutgoingShipments.SingleOrDefaultAsync(m => m.OutgoingShipmentId == outgoingShipmentId && m.SalesOrderId == salesOrderId);

            if (salesOrderOutgoingShipment == null) {
                return NotFound();
            }

            return Ok(salesOrderOutgoingShipment);
        }

        // GET: SalesOrderOutgoingShipments
        /// <summary>
        /// Fetches a list of salesOrder outgoingShipments
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSalesOrderOutgoingShipments([FromQuery] int? salesOrderId, [FromQuery] int? outgoingShipmentId) {
            if (salesOrderId == null && outgoingShipmentId == null) {
                return BadRequest(new {
                    Error = "must have either salesOrderId or outgoingShipmentId querystring param"
                });
            }
            var query = from salesOrder in _context.SalesOrderOutgoingShipments select salesOrder;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (salesOrderId != null) {
                query = query.Where(item => item.SalesOrderId == salesOrderId)
                    .Include(m => m.OutgoingShipment)
                        .ThenInclude(item => item.Boxes)
                            .ThenInclude(item => item.InventoryItems)
                                .ThenInclude(item => item.InventoryItem)
                                    .ThenInclude(item => item.Product)
                    .Include(m => m.OutgoingShipment)
                        .ThenInclude(item => item.ShippingAddress);
            }
            if (outgoingShipmentId != null) {
                query = query.Where(item => item.OutgoingShipmentId == outgoingShipmentId).Include(m => m.SalesOrder);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: SalesOrderOutgoingShipments?outgoingShipmentId=&salesOrderId=
        [HttpPut]
        public async Task<IActionResult> PutSalesOrderOutgoingShipment([FromQuery] int? outgoingShipmentId, [FromQuery] int? salesOrderId, [FromBody] SalesOrderOutgoingShipment salesOrderOutgoingShipment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(salesOrderOutgoingShipment).State = EntityState.Modified;
            var salesOrder = await _context.SalesOrders.FirstOrDefaultAsync(item => item.Id == salesOrderOutgoingShipment.SalesOrderId);

            using (var transaction = _context.Database.BeginTransaction()) {
                await _context.SaveChangesAsync();
                await salesOrder.UpdateShippedProfit(_context);
                transaction.Commit();
            }

            return NoContent();
        }

        // POST: SalesOrderOutgoingShipments
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderOutgoingShipment([FromBody] SalesOrderOutgoingShipment salesOrderOutgoingShipment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.SalesOrderOutgoingShipments.Add(salesOrderOutgoingShipment);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (SalesOrderOutgoingShipmentExists(salesOrderOutgoingShipment.SalesOrderId, salesOrderOutgoingShipment.OutgoingShipmentId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            return CreatedAtAction("GetSalesOrderOutgoingShipment", new { id = salesOrderOutgoingShipment.OutgoingShipmentId }, salesOrderOutgoingShipment);
        }

        // DELETE: SalesOrderOutgoingShipments?salesOrderId=&outgoingShipmentid=
        [HttpDelete]
        public async Task<IActionResult> DeleteSalesOrderOutgoingShipment([FromQuery] int salesOrderId, [FromQuery] int outgoingShipmentId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrderOutgoingShipment = await _context.SalesOrderOutgoingShipments.SingleOrDefaultAsync(m => m.OutgoingShipmentId == outgoingShipmentId && m.SalesOrderId == salesOrderId);
            if (salesOrderOutgoingShipment == null) {
                return NotFound();
            }
            var salesOrder = await _context.SalesOrders.FirstOrDefaultAsync(item => item.Id == salesOrderOutgoingShipment.SalesOrderId);

            using (var transaction = _context.Database.BeginTransaction()) {
                _context.SalesOrderOutgoingShipments.Remove(salesOrderOutgoingShipment);
                await _context.SaveChangesAsync();
                await salesOrder.UpdateShippedProfit(_context);
                transaction.Commit();
            }

            return Ok(salesOrderOutgoingShipment);
        }

        private bool SalesOrderOutgoingShipmentExists(int? salesOrderId, int? outgoingShipmentId) {
            return _context.SalesOrderOutgoingShipments.Any(e => e.OutgoingShipmentId == outgoingShipmentId && e.SalesOrderId == salesOrderId);
        }

    }
}