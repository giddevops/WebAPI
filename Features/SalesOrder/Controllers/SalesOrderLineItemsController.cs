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
    [Route("SalesOrderLineItems")]
    public class SalesOrderLineItemsController : Controller {
        private readonly AppDBContext _context;

        public SalesOrderLineItemsController(AppDBContext context) {
            _context = context;
        }

        // GET: SalesOrderLineItems
        [HttpGet]
        public IEnumerable<SalesOrderLineItem> GetSalesOrderLineItem() {
            return _context.SalesOrderLineItems;
        }

        // GET: SalesOrderLineItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderLineItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrderLineItem = await _context.SalesOrderLineItems.SingleOrDefaultAsync(m => m.Id == id);

            if (salesOrderLineItem == null) {
                return NotFound();
            }

            return Ok(salesOrderLineItem);
        }

        // PUT: SalesOrderLineItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderLineItem([FromRoute] int? id, [FromBody] SalesOrderLineItem salesOrderLineItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != salesOrderLineItem.Id) {
                return BadRequest();
            }

            using (var transaction = _context.Database.BeginTransaction()) {
                var salesOrderId = salesOrderLineItem.SalesOrderId;
                var salesOrder = await _context.SalesOrders.AsNoTracking().FirstOrDefaultAsync(item => item.Id == salesOrderId);

                _context.Entry(salesOrderLineItem).State = EntityState.Modified;

                await _context.SaveChangesAsync();

                await salesOrderLineItem.UpdateSalesOrderTotal(_context);
                await salesOrder.UpdateShippedProfit(_context);
                transaction.Commit();
            }

            return NoContent();
        }

        // POST: SalesOrderLineItems
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderLineItem([FromBody] SalesOrderLineItem salesOrderLineItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.SalesOrderLineItems.Add(salesOrderLineItem);
            await _context.SaveChangesAsync();
            await salesOrderLineItem.UpdateSalesOrderTotal(_context);

            return CreatedAtAction("GetSalesOrderLineItem", new { id = salesOrderLineItem.Id }, salesOrderLineItem);
        }

        // DELETE: SalesOrderLineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderLineItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrderLineItem = await _context.SalesOrderLineItems.SingleOrDefaultAsync(m => m.Id == id);
            if (salesOrderLineItem == null) {
                return NotFound();
            }
            using (var transaction = _context.Database.BeginTransaction()) {
                var salesOrderId = salesOrderLineItem.SalesOrderId;

                _context.SalesOrderLineItems.Remove(salesOrderLineItem);
                try {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
                await salesOrderLineItem.UpdateSalesOrderTotal(_context);

                var salesOrder = await _context.SalesOrders.AsNoTracking()
                    .Include(item => item.LineItems)
                    .FirstOrDefaultAsync(item => item.Id == salesOrderId);
                await salesOrder.UpdateShippedProfit(_context);
                await salesOrder.UpdateBalance(_context);
                transaction.Commit();
            }

            return Ok(salesOrderLineItem);
        }

        private bool SalesOrderLineItemExists(int? id) {
            return _context.SalesOrderLineItems.Any(e => e.Id == id);
        }
    }
}