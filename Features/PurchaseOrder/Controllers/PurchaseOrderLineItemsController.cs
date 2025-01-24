using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using QuickBooks.Models;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("PurchaseOrderLineItems")]
    public class PurchaseOrderLineItemsController : Controller {
        private readonly AppDBContext _context;

        public PurchaseOrderLineItemsController(AppDBContext context) {
            _context = context;
        }

        // GET: PurchaseOrderLineItems
        [HttpGet]
        public IEnumerable<PurchaseOrderLineItem> GetPurchaseOrderLineItems() {
            return _context.PurchaseOrderLineItems;
        }

        // GET: PurchaseOrderLineItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderLineItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var purchaseOrderLineItem = await _context.PurchaseOrderLineItems.SingleOrDefaultAsync(m => m.Id == id);

            if (purchaseOrderLineItem == null) {
                return NotFound();
            }

            return Ok(purchaseOrderLineItem);
        }

        // PUT: PurchaseOrderLineItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderLineItem([FromRoute] int? id, [FromBody] PurchaseOrderLineItem purchaseOrderLineItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != purchaseOrderLineItem.Id) {
                return BadRequest();
            }

            var previousLineItem = await _context.PurchaseOrderLineItems.AsNoTracking().FirstOrDefaultAsync(item => item.Id == purchaseOrderLineItem.Id);

            _context.Entry(purchaseOrderLineItem).State = EntityState.Modified;

            using (var transaction = _context.Database.BeginTransaction())
            {
                var purchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(item => item.Id == purchaseOrderLineItem.PurchaseOrderId);
                if (previousLineItem.ProductId != purchaseOrderLineItem.ProductId)
                {
                    return BadRequest("You can't change the product for a line item once it has been sent right now. You need to create a new one and delete the old one.");
                }
                try
                {
                    await purchaseOrderLineItem.SyncShipments(previousLineItem.Quantity, purchaseOrder, GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User), _context);
                }catch(Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                await _context.SaveChangesAsync();
                //Update purchase order total
                await purchaseOrder.UpdateTotal(_context);

                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            return NoContent();
        }

        // POST: PurchaseOrderLineItems
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrderLineItem([FromBody] PurchaseOrderLineItem purchaseOrderLineItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.PurchaseOrderLineItems.Add(purchaseOrderLineItem);
            using (var transaction = _context.Database.BeginTransaction()) {
                await _context.SaveChangesAsync();
                //Update purchase order total
                var purchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(item => item.Id == purchaseOrderLineItem.PurchaseOrderId);
                await purchaseOrder.UpdateTotal(_context);
                try
                {
                    await purchaseOrderLineItem.SyncShipments(0, purchaseOrder, GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User), _context);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }
                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            return CreatedAtAction("GetPurchaseOrderLineItem", new { id = purchaseOrderLineItem.Id }, purchaseOrderLineItem);
        }

        // DELETE: PurchaseOrderLineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrderLineItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var purchaseOrderLineItem = await _context.PurchaseOrderLineItems.SingleOrDefaultAsync(m => m.Id == id);
            if (purchaseOrderLineItem == null) {
                return NotFound();
            }
            var previousQuantity = purchaseOrderLineItem.Quantity;
            var purchaseOrderId = purchaseOrderLineItem.PurchaseOrderId;
            _context.Entry(purchaseOrderLineItem).State = EntityState.Deleted;

            using (var transaction = _context.Database.BeginTransaction()) {
                var purchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(item => item.Id == purchaseOrderId);
                var sentStatus = await _context.PurchaseOrderStatusOptions.FirstOrDefaultAsync(item => item.Value == "Sent");
                // if purchase order has already been sent, remove generated inventory items
                try
                {
                    purchaseOrderLineItem.Quantity = 0;
                    await purchaseOrderLineItem.SyncShipments(previousQuantity, purchaseOrder, GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User), _context);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex.Message);
                }

                await _context.SaveChangesAsync();
                //Update purchase order total
                purchaseOrder = await _context.PurchaseOrders.FirstOrDefaultAsync(item => item.Id == purchaseOrderId);
                await purchaseOrder.UpdateTotal(_context);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            return Ok(purchaseOrderLineItem);
        }

        private bool PurchaseOrderLineItemExists(int? id) {
            return _context.PurchaseOrderLineItems.Any(e => e.Id == id);
        }
    }
}