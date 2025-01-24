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
    [Route("SalesOrderLineItemSources")]
    public class SalesOrderLineItemSourcesController : Controller {
        private readonly AppDBContext _context;

        public SalesOrderLineItemSourcesController(AppDBContext context) {
            _context = context;
        }

        // GET: SalesOrderLineItemSources?salesOrderLineItemId=&sourceId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="salesOrderLineItemId"></param>
        /// <param name="sourceId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetSalesOrderLineItemSourceById([FromQuery] int? salesOrderLineItemId, [FromQuery] int? sourceId) {

            var salesOrderLineItemSource = await _context.SalesOrderLineItemSources
                .SingleOrDefaultAsync(m => m.SourceId == sourceId && m.SalesOrderLineItemId == salesOrderLineItemId);

            if (salesOrderLineItemSource == null) {
                return NotFound();
            }

            return Ok(salesOrderLineItemSource);
        }

        // GET: SalesOrderLineItemSources
        /// <summary>
        /// Fetches a list of salesOrderLineItem sources
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSalesOrderLineItemSources([FromQuery] int? salesOrderLineItemId, [FromQuery] int? sourceId, [FromQuery] int? purchaseOrderId) {
            if (salesOrderLineItemId == null && sourceId == null && purchaseOrderId == null) {
                return BadRequest(new {
                    Error = "must have either salesOrderLineItemId or sourceId or purchaseOrderId querystring param"
                });
            }
            var query = from salesOrderLineItem in _context.SalesOrderLineItemSources select salesOrderLineItem;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (salesOrderLineItemId != null) {
                query = query
                    .Where(item => item.SalesOrderLineItemId == salesOrderLineItemId)
                    .Include(item => item.PurchaseOrder)
                        .ThenInclude(item => item.Supplier)
                    .Include(item => item.PurchaseOrder)
                        .ThenInclude(item => item.LineItems)
                    .Include(m => m.Source)
                        .ThenInclude(s => s.Supplier)
                    .Include(m => m.Source)
                        .ThenInclude(s => s.Currency);
            }
            if (sourceId != null)
                query = query.Where(item => item.SourceId == sourceId).Include(m => m.SalesOrderLineItem);
            if (purchaseOrderId != null)
                query = query.Where(item => item.PurchaseOrderId == purchaseOrderId)
                    .Include(m => m.Source)
                        .ThenInclude(s => s.Supplier);

            var items = await query.ToListAsync();
            return Ok(items);
        }

        // PUT: SalesOrderLineItemSources?sourceId=&salesOrderLineItemId=
        [HttpPut]
        public async Task<IActionResult> PutSalesOrderLineItemSource([FromQuery] int? sourceId, [FromQuery] int? salesOrderLineItemId, [FromBody] SalesOrderLineItemSource salesOrderLineItemSource) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Entry(salesOrderLineItemSource).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                throw;
            }

            return NoContent();
        }

        // POST: SalesOrderLineItemSources
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderLineItemSource([FromBody] SalesOrderLineItemSource salesOrderLineItemSource) {

            _context.SalesOrderLineItemSources.Add(salesOrderLineItemSource);


            //Get the ids of all items
            int? sourceId = null;
            if (salesOrderLineItemSource.Source != null && salesOrderLineItemSource.Source.Id != null && salesOrderLineItemSource.Source.Id > 0) {
                sourceId = salesOrderLineItemSource.Source.Id;
            } else if (salesOrderLineItemSource.SourceId != null && salesOrderLineItemSource.SourceId > 0) {
                sourceId = salesOrderLineItemSource.SourceId;
            }
            int? purchaseOrderId = null;
            if (salesOrderLineItemSource.PurchaseOrder != null && salesOrderLineItemSource.PurchaseOrder.Id != null && salesOrderLineItemSource.PurchaseOrder.Id > 0) {
                purchaseOrderId = salesOrderLineItemSource.PurchaseOrder.Id;
            } else if (salesOrderLineItemSource.PurchaseOrderId != null && salesOrderLineItemSource.PurchaseOrderId > 0) {
                purchaseOrderId = salesOrderLineItemSource.PurchaseOrderId;
            }
            int? salesOrderLineItemId = null;
            if (salesOrderLineItemSource.SalesOrderLineItem != null && salesOrderLineItemSource.SalesOrderLineItem.Id != null && salesOrderLineItemSource.SalesOrderLineItem.Id > 0) {
                salesOrderLineItemId = salesOrderLineItemSource.SalesOrderLineItem.Id;
            } else if (salesOrderLineItemSource.SalesOrderLineItemId != null && salesOrderLineItemSource.SalesOrderLineItemId > 0) {
                salesOrderLineItemId = salesOrderLineItemSource.SalesOrderLineItemId;
            }

            
            if(salesOrderLineItemSource.Source != null){
                if(salesOrderLineItemSource.Source.CurrencyOptionId == null){
                    salesOrderLineItemSource.Source.CurrencyOptionId = (await _context.CurrencyOptions.FirstOrDefaultAsync(item => item.Value == "USD")).Id;
                }
            }

            //if they are making a source from a listing, no need for other complicated counting stuff
            if(salesOrderLineItemSource.Source != null && sourceId == null && purchaseOrderId == null){
                await _context.SaveChangesAsync();
                return Ok(salesOrderLineItemSource);
            }


            //Need to decrement the amount of items available for the source since a purchase order has gone out
            //however, it is possible that the source is being created in this request automatically
            //this would occur if a purchase order is being directly related to a sales order line item and there was no matching sales order
            if (sourceId != null) {
                var source = await _context.Sources.FirstOrDefaultAsync(item => item.Id == salesOrderLineItemSource.SourceId);
                if (source == null) {
                    return BadRequest(new {
                        Error = "Source not found with that id"
                    });
                }
                source.Quantity -= salesOrderLineItemSource.Quantity;
                _context.Entry(source).State = EntityState.Modified;
            }

            //if there was no source submitted, need to find a matching source, or create a new one
            if (sourceId == null) {
                if (purchaseOrderId == null) {
                    return BadRequest(new {
                        Error = "PurchaseOrderId is required if no source is submitted"
                    });
                } else 
                if (salesOrderLineItemId == null) {
                    return BadRequest(new {
                        Error = "salesOrderLineItemId is required"
                    });
                } else {
                    //get related purchase order and sales order becuase they contain info needed to creat the right source
                    var purchaseOrder = await _context.PurchaseOrders
                        .Include(item => item.Supplier)
                        .FirstOrDefaultAsync(item => item.Id == purchaseOrderId);
                    if (purchaseOrder == null) {
                        return BadRequest(new {
                            Error = "PurchaseOrder not found with that id"
                        });
                    }
                    var salesOrderLineItem = await _context.SalesOrderLineItems.FirstOrDefaultAsync(item => item.Id == salesOrderLineItemId);
                    if (purchaseOrder == null) {
                        return BadRequest(new {
                            Error = "PurchaseOrder not found with that id"
                        });
                    }

                    var oldSource = await _context.Sources
                        .OrderByDescending(item => item.CreatedAt)
                        .FirstOrDefaultAsync(item => item.SupplierId == purchaseOrder.SupplierId);
                    //If source not found, create it
                    if (oldSource == null) {
                        var source = new Source {
                            CreatedAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                            SupplierId = purchaseOrder.SupplierId,
                            ProductId = salesOrderLineItem.ProductId,
                            Quantity = 0,
                        };
                        salesOrderLineItemSource.Source = source;
                        _context.Sources.Add(source);
                    } else { // if source was found, decrement quantity by quantity in purchase order, but don't let quantity go negative
                        oldSource.Quantity -= salesOrderLineItemSource.Quantity;
                        if (oldSource.Quantity < 0)
                            oldSource.Quantity = 0;
                        _context.Entry(oldSource).State = EntityState.Modified;
                        salesOrderLineItemSource.Source = oldSource;
                    }
                }
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (SalesOrderLineItemSourceExists(salesOrderLineItemSource.SalesOrderLineItemId, salesOrderLineItemSource.SourceId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            //make sure related objects are included
            salesOrderLineItemSource = await _context.SalesOrderLineItemSources
                .Include(item => item.PurchaseOrder)
                .Include(item => item.SalesOrderLineItem)
                .Include(item => item.Source)
                    .ThenInclude(item => item.Supplier)
                .FirstOrDefaultAsync(item =>
                    item.SalesOrderLineItemId == salesOrderLineItemSource.SalesOrderLineItemId && item.SourceId == salesOrderLineItemSource.SourceId);

            return CreatedAtAction("GetSalesOrderLineItemSource", new { id = salesOrderLineItemSource.SourceId }, salesOrderLineItemSource);
        }

        // DELETE: SalesOrderLineItemSources?salesOrderLineItemId=&sourceid=
        [HttpDelete]
        public async Task<IActionResult> DeleteSalesOrderLineItemSource([FromQuery] int salesOrderLineItemId, [FromQuery] int sourceId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var salesOrderLineItemSource = await _context.SalesOrderLineItemSources.SingleOrDefaultAsync(m => m.SourceId == sourceId && m.SalesOrderLineItemId == salesOrderLineItemId);
            if (salesOrderLineItemSource == null) {
                return NotFound();
            }

            _context.SalesOrderLineItemSources.Remove(salesOrderLineItemSource);
            var source = await _context.Sources.FirstOrDefaultAsync(item => item.Id == salesOrderLineItemSource.SourceId);
            if (source == null) {
                return BadRequest(new {
                    Error = "Source not found with that id"
                });
            }
            source.Quantity += salesOrderLineItemSource.Quantity;
            _context.Entry(source).State = EntityState.Modified;

            await _context.SaveChangesAsync();

            return Ok(salesOrderLineItemSource);
        }

        private bool SalesOrderLineItemSourceExists(int? salesOrderLineItemId, int? sourceId) {
            return _context.SalesOrderLineItemSources.Any(e => e.SourceId == sourceId && e.SalesOrderLineItemId == salesOrderLineItemId);
        }
    }
}