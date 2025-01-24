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
    [Route("SalesOrderPurchaseOrders")]
    public class SalesOrderPurchaseOrdersController : Controller {
        private readonly AppDBContext _context;

        public SalesOrderPurchaseOrdersController(AppDBContext context) {
            _context = context;
        }

        // GET: SalesOrderPurchaseOrders?salesOrderId=&purchaseOrderId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="salesOrderId"></param>
        /// <param name="purchaseOrderId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetSalesOrderPurchaseOrderById([FromQuery] int? salesOrderId, [FromQuery] int? purchaseOrderId) {

            var salesOrderPurchaseOrder = await _context.SalesOrderPurchaseOrders.SingleOrDefaultAsync(m => m.PurchaseOrderId == purchaseOrderId && m.SalesOrderId == salesOrderId);

            if (salesOrderPurchaseOrder == null) {
                return NotFound();
            }

            return Ok(salesOrderPurchaseOrder);
        }

        // GET: SalesOrderPurchaseOrders
        /// <summary>
        /// Fetches a list of salesOrder purchaseOrders
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSalesOrderPurchaseOrders([FromQuery] int? salesOrderId, [FromQuery] int? purchaseOrderId) {
            if (salesOrderId == null && purchaseOrderId == null) {
                return BadRequest(new {
                    Error = "must have either salesOrderId or purchaseOrderId querystring param"
                });
            }
            var query = from salesOrder in _context.SalesOrderPurchaseOrders select salesOrder;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (salesOrderId != null) {
                query = query.Where(item => item.SalesOrderId == salesOrderId)
                    .Include(m => m.PurchaseOrder)
                        .ThenInclude(item => item.LineItems)
                            .ThenInclude(item => item.Product);
            }
            if (purchaseOrderId != null) {
                query = query.Where(item => item.PurchaseOrderId == purchaseOrderId).Include(m => m.SalesOrder);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: SalesOrderPurchaseOrders?purchaseOrderId=&salesOrderId=
        [HttpPut]
        public async Task<IActionResult> PutSalesOrderPurchaseOrder([FromQuery] int? purchaseOrderId, [FromQuery] int? salesOrderId, [FromBody] SalesOrderPurchaseOrder salesOrderPurchaseOrder) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(salesOrderPurchaseOrder).State = EntityState.Modified;

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

        // POST: SalesOrderPurchaseOrders
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderPurchaseOrder([FromBody] SalesOrderPurchaseOrder salesOrderPurchaseOrder) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.SalesOrderPurchaseOrders.Add(salesOrderPurchaseOrder);
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (SalesOrderPurchaseOrderExists(salesOrderPurchaseOrder.SalesOrderId, salesOrderPurchaseOrder.PurchaseOrderId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            //now go through line items and add/relate sources

            salesOrderPurchaseOrder = await _context.SalesOrderPurchaseOrders
                .Include(item => item.PurchaseOrder)
                    .ThenInclude(item => item.LineItems)
                        .ThenInclude(item => item.Product)
                .FirstOrDefaultAsync(item => item.SalesOrderId == salesOrderPurchaseOrder.SalesOrderId && item.PurchaseOrderId == salesOrderPurchaseOrder.PurchaseOrderId);

            var purchaseOrder = salesOrderPurchaseOrder.PurchaseOrder;
            var salesOrder = await _context.SalesOrders
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Sources)
                        .ThenInclude(item => item.Source)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Sources)
                        .ThenInclude(item => item.PurchaseOrder)
                .FirstOrDefaultAsync(item => item.Id == salesOrderPurchaseOrder.SalesOrderId);

            //First check if this purchase order is already related on the source line item level. If so, do nothing
            if (salesOrder.LineItems.Any(salesOrderLineItem => salesOrderLineItem.Sources.Any(lineItemSource => lineItemSource.PurchaseOrderId == purchaseOrder.Id))) {

            } else { //purchase order is not already related

                //go through all the line items of the sales order to see where existing sources could be related
                foreach (var salesOrderLineItem in salesOrder.LineItems) {
                    var matchingPurchaseOrderLineItem = purchaseOrder.LineItems.FirstOrDefault(item => item.ProductId == salesOrderLineItem.ProductId);
                    if (matchingPurchaseOrderLineItem == null)
                        continue;
                    var quantityRemaining = matchingPurchaseOrderLineItem.Quantity - matchingPurchaseOrderLineItem.QuantityUsedForRelateSalesOrder;

                    // check if there are any sources from this supplier already that do not have a purchase order
                    var sourcesWithSameSupplierAsPO = salesOrderLineItem.Sources.Where(item => item.Source.SupplierId == purchaseOrder.SupplierId && item.PurchaseOrderId == null).ToList();

                    foreach (var source in sourcesWithSameSupplierAsPO) {
                        //if there are enough items in the purchase order to satisfy the Source quantity, use them
                        if (source.Quantity <= quantityRemaining) {
                            matchingPurchaseOrderLineItem.QuantityUsedForRelateSalesOrder += source.Quantity ?? 0;
                            source.PurchaseOrderId = purchaseOrder.Id;
                            _context.Entry(source).State = EntityState.Modified;
                        } else if (quantityRemaining > 0) { //if there are not enough items in the PO to satisfy the source, reduce the quantity
                            matchingPurchaseOrderLineItem.QuantityUsedForRelateSalesOrder += source.Quantity ?? 0;
                            source.PurchaseOrderId = purchaseOrder.Id;
                            source.Quantity = quantityRemaining;
                            source.Source.Quantity = quantityRemaining;
                            _context.Entry(source).State = EntityState.Modified;
                        } else {
                            //if there are no items left in the PO that haven't been assigned to other sources, don't relate anything
                        }
                    }
                }

                //now go through all line items and add sources if there are still things remaining
                foreach (var salesOrderLineItem in salesOrder.LineItems) {
                    var matchingPurchaseOrderLineItem = purchaseOrder.LineItems.FirstOrDefault(item => item.ProductId == salesOrderLineItem.ProductId);
                    if (matchingPurchaseOrderLineItem == null)
                        continue;
                    var quantityRemaining = matchingPurchaseOrderLineItem.Quantity - matchingPurchaseOrderLineItem.QuantityUsedForRelateSalesOrder;
                    if (quantityRemaining == 0)
                        continue;

                    var quantityNeeded = salesOrderLineItem.Quantity - salesOrderLineItem.Sources.Sum(item => item.Quantity);
                    if (quantityNeeded <= 0)
                        continue;

                    var quantityToAdd = quantityNeeded <= quantityRemaining ? quantityNeeded : quantityRemaining;

                    var costFractionOfItemsToAdd = (matchingPurchaseOrderLineItem.Cost * quantityToAdd) / purchaseOrder.GetLineItemsTotal();
                    var estimatedShippingCost = purchaseOrder.ShippingAndHandlingFee * costFractionOfItemsToAdd;

                    salesOrderLineItem.Sources.Add(new SalesOrderLineItemSource {
                        PurchaseOrderId = matchingPurchaseOrderLineItem.PurchaseOrderId,
                        Quantity = quantityToAdd,
                        Source = new Source {
                            ProductId = salesOrderLineItem.ProductId,
                            Quantity = quantityToAdd,
                            SupplierId = purchaseOrder.SupplierId,
                            VerifiedAt = DateTime.UtcNow,
                            CreatedAt = DateTime.UtcNow,
                            ShippingCost = estimatedShippingCost,
                            Cost = matchingPurchaseOrderLineItem.Cost,
                            CurrencyOptionId = purchaseOrder.CurrencyOptionId
                        }
                    });
                    matchingPurchaseOrderLineItem.QuantityUsedForRelateSalesOrder += quantityToAdd ?? 0;
                }
                await _context.SaveChangesAsync();
            }

            return CreatedAtAction("GetSalesOrderPurchaseOrder", new { id = salesOrderPurchaseOrder.PurchaseOrderId }, salesOrderPurchaseOrder);
        }

        // DELETE: SalesOrderPurchaseOrders?salesOrderId=&purchaseOrderid=
        [HttpDelete]
        public async Task<IActionResult> DeleteSalesOrderPurchaseOrder([FromQuery] int salesOrderId, [FromQuery] int purchaseOrderId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrderPurchaseOrder = await _context.SalesOrderPurchaseOrders.SingleOrDefaultAsync(m => m.PurchaseOrderId == purchaseOrderId && m.SalesOrderId == salesOrderId);
            if (salesOrderPurchaseOrder == null) {
                return NotFound();
            }

            _context.SalesOrderPurchaseOrders.Remove(salesOrderPurchaseOrder);
            await _context.SaveChangesAsync();

            return Ok(salesOrderPurchaseOrder);
        }

        private bool SalesOrderPurchaseOrderExists(int? salesOrderId, int? purchaseOrderId) {
            return _context.SalesOrderPurchaseOrders.Any(e => e.PurchaseOrderId == purchaseOrderId && e.SalesOrderId == salesOrderId);
        }
    }
}