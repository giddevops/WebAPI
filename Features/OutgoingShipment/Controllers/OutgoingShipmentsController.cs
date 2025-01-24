using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using DinkToPdf;
using WebApi.Services;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Hosting;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("OutgoingShipments")]
    public class OutgoingShipmentsController : Controller {
        private readonly AppDBContext _context;
        private readonly ViewRender viewRenderer;
        private IConverter PdfConverter;
        public IHostingEnvironment Environment;

        public OutgoingShipmentsController(AppDBContext context, ViewRender renderer, IConverter converter, IHostingEnvironment env) {
            _context = context;
            viewRenderer = renderer;
            PdfConverter = converter;
            Environment = env;
        }

        // GET: OutgoingShipments
        [HttpGet]
        public ListResult GetOutgoingShipments(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from outgoingShipment in _context.OutgoingShipments select outgoingShipment;
            query = query
                .OrderByDescending(q => q.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: OutgoingShipments/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutgoingShipment([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var outgoingShipment = await _context.OutgoingShipments
                .Include(item => item.ShippingAddress)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (outgoingShipment == null) {
                return NotFound();
            }

            return Ok(outgoingShipment);
        }

        // PUT: OutgoingShipments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOutgoingShipment([FromRoute] int? id, [FromBody] OutgoingShipment outgoingShipment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != outgoingShipment.Id) {
                return BadRequest();
            }

            //if all outgoing shipments are shipped, will mark it as shipped
            var shouldMarkSalesOrderStatusShipped = false;

            //check if this was just marked as shipped.  If so, log it in the event log entries table
            OutgoingShipment originalOutgoingShipment = await _context.OutgoingShipments
                .Include(item => item.SalesOrderOutgoingShipment)
                .Include(item => item.RmaOutgoingShipment)
                    .ThenInclude(item => item.Rma)
                .AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);

            int? salesOrderId = null;
            var isSalesOrder = false;
            var isRma = false;
            if (originalOutgoingShipment.SalesOrderOutgoingShipment != null)
            {
                salesOrderId = originalOutgoingShipment.SalesOrderOutgoingShipment.SalesOrderId;
                isSalesOrder = true;
            }
            if (originalOutgoingShipment.RmaOutgoingShipment != null)
            {
                salesOrderId = originalOutgoingShipment.RmaOutgoingShipment.Rma.SalesOrderId;
                isRma = true;
            }

            if (originalOutgoingShipment.ShippedAt == null && outgoingShipment.ShippedAt != null) {
                if (isSalesOrder)
                {
                    _context.SalesOrderEventLogEntries.Add(new SalesOrderEventLogEntry
                    {
                        EventLogEntry = new EventLogEntry
                        {
                            OccurredAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                            Event = "Shipment #" + outgoingShipment.Id + " shipped"
                        },
                        SalesOrderId = salesOrderId
                    });
                }else if (isRma)
                {
                    _context.RmaEventLogEntries.Add(new RmaEventLogEntry
                    {
                        EventLogEntry = new EventLogEntry
                        {
                            OccurredAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                            Event = "Shipment #" + outgoingShipment.Id + " shipped"
                        },
                        RmaId = originalOutgoingShipment.RmaOutgoingShipment.RmaId.Value
                    });
                }

                var inventoryItems = await _context.InventoryItems.Where(item => item.OutgoingShipmentBoxes.Any(outgoingShipmentBoxInventoryItem => outgoingShipmentBoxInventoryItem.OutgoingShipmentBox.OutgoingShipmentId == id)).ToListAsync();
                inventoryItems.ForEach(inventoryItem => {
                    inventoryItem.InventoryItemStatusOptionId = InventoryItemStatusOption.Shipped;
                    _context.Entry(inventoryItem).State = EntityState.Modified;
                });

                //check if all outgoing shipments are now marked shipped. If so, change status to shipped.
                if (isRma)
                {

                }
                else if(isSalesOrder)
                {
                    var outgoingShipments = await _context.OutgoingShipments
                        .Where(item => item.SalesOrderOutgoingShipment.SalesOrderId == salesOrderId)
                        .Where(item => item.ShippedAt == null)
                        .Where(item => item.Id != originalOutgoingShipment.Id).ToListAsync();
                    if (outgoingShipments.Count == 0)
                    {
                        shouldMarkSalesOrderStatusShipped = true;
                    }
                }


                //Now check if it was marked as "not shipped"
            } else if (originalOutgoingShipment.ShippedAt != null && outgoingShipment.ShippedAt == null) {
                if (isSalesOrder)
                {
                    _context.SalesOrderEventLogEntries.Add(new SalesOrderEventLogEntry
                    {
                        EventLogEntry = new EventLogEntry
                        {
                            OccurredAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                            Event = "Shipment #" + outgoingShipment.Id + " un-shipped"
                        },
                        SalesOrderId = salesOrderId
                    });
                }else if (isRma)
                {
                    _context.RmaEventLogEntries.Add(new RmaEventLogEntry
                    {
                        EventLogEntry = new EventLogEntry
                        {
                            OccurredAt = DateTime.UtcNow,
                            CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                            Event = "Shipment #" + outgoingShipment.Id + " un-shipped"
                        },
                        RmaId = originalOutgoingShipment.RmaOutgoingShipment.RmaId.Value
                    });
                }

                var inventoryItems = await _context.InventoryItems.Where(item => item.OutgoingShipmentBoxes.Any(outgoingShipmentBoxInventoryItem => outgoingShipmentBoxInventoryItem.OutgoingShipmentBox.OutgoingShipmentId == id)).ToListAsync();
                inventoryItems.ForEach(inventoryItem => {
                    inventoryItem.InventoryItemStatusOptionId = InventoryItemStatusOption.Committed;
                    _context.Entry(inventoryItem).State = EntityState.Modified;
                });
            }

            _context.Entry(outgoingShipment).State = EntityState.Modified;
            if (outgoingShipment.ShippingAddress != null) {
                if (outgoingShipment.ShippingAddress.Id != null) {
                    _context.Entry(outgoingShipment.ShippingAddress).State = EntityState.Modified;
                } else {
                    _context.Entry(outgoingShipment.ShippingAddress).State = EntityState.Added;
                }
            }


            try {
                if (isSalesOrder)
                {
                    var salesOrder = await _context.SalesOrders.AsNoTracking()
                        .Where(item => item.Id == salesOrderId)
                        .FirstOrDefaultAsync();
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        await _context.SaveChangesAsync();
                        if (salesOrder != null)
                        {
                            await salesOrder.UpdateShippedProfit(_context);
                        }
                        transaction.Commit();
                    }
                    // if (salesOrder != null) {
                    //     var shippedStatus = await _context.SalesOrderStatusOptions
                    //         .FirstOrDefaultAsync(item => item.Value.ToLower() == "shipped" || item.Value.ToLower() == "shipment complete");
                    //     if (shippedStatus != null && shouldMarkSalesOrderStatusShipped) {
                    //         _context.Entry(salesOrder).State = EntityState.Detached;
                    //         salesOrder = await _context.SalesOrders.FirstOrDefaultAsync(item => item.Id == salesOrder.Id);
                    //         salesOrder.SalesOrderStatusOptionId = shippedStatus.Id;
                    //         await _context.SaveChangesAsync();
                    //     }
                    // }
                }
                else
                {
                    await _context.SaveChangesAsync();
                }
            }
            catch (DbUpdateConcurrencyException) {
                if (!OutgoingShipmentExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }



            return NoContent();
        }

        // POST: OutgoingShipments
        [HttpPost]
        public async Task<IActionResult> PostOutgoingShipment([FromBody] OutgoingShipment outgoingShipment) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.OutgoingShipments.Add(outgoingShipment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOutgoingShipment", new { id = outgoingShipment.Id }, outgoingShipment);
        }

        // DELETE: OutgoingShipments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOutgoingShipment([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var outgoingShipment = await _context.OutgoingShipments.SingleOrDefaultAsync(m => m.Id == id);
            if (outgoingShipment == null) {
                return NotFound();
            }

            if (outgoingShipment.ShippedAt != null) {
                return BadRequest(new {
                    Error = "You can't delete a shipment that's arleady been shipped"
                });
            }

            _context.OutgoingShipments.Remove(outgoingShipment);
            await _context.SaveChangesAsync();

            return Ok(outgoingShipment);
        }

        private bool OutgoingShipmentExists(int? id) {
            return _context.OutgoingShipments.Any(e => e.Id == id);
        }

        [HttpGet("{id}/GenerateCommercialInvoicePdf")]
        public async Task GenerateCommercialInvoicePdf([FromRoute] int id) {

            var outgoingShipment = await _context.OutgoingShipments
                .Include(item => item.Boxes)
                    .ThenInclude(item => item.InventoryItems)
                        .ThenInclude(item => item.InventoryItem)
                            .ThenInclude(item => item.SalesOrderLineItems)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.BillingAddress)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.Company)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.PaymentMethod)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.ShippingAddress)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.GidLocationOption)
                            .ThenInclude(item => item.MainAddress)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.Contact)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.SalesPerson)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.LineItems)
                            .ThenInclude(item => item.Product)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.LineItems)
                            .ThenInclude(item => item.CountryOfOrigin)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.CurrencyOption)
                .Include(item => item.ShippingAddress)
                .Include(item => item.ShippingCarrier)
                .Include(item => item.OutgoingShipmentShippingTermOption)
                .Include(item => item.ShippingCarrierShippingMethod)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (outgoingShipment == null) {
                Response.StatusCode = 400;
                return;
            }
            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            outgoingShipment.Preparer = currentUser;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = await outgoingShipment.SalesOrderOutgoingShipment.SalesOrder.CheckIfIsGidEurope(_context);
            string commercialInvoiceHtml = viewRenderer.Render("~/Features/OutgoingShipment/Views/CommercialInvoicePdf.cshtml", outgoingShipment, ViewData);
            var doc = new HtmlToPdfDocument() {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = commercialInvoiceHtml,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        // HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };
            byte[] data = PdfConverter.Convert(doc);
            // Response.Headers.ContentType = "application/pdf";
            Response.ContentType = "application/pdf";
            await Response.Body.WriteAsync(data, 0, data.Length);
            return;
        }


        [HttpGet("{id}/GeneratePackingSlipPdf")]
        public async Task GeneratePackingSlipPdf([FromRoute] int id) {

            var outgoingShipment = await _context.OutgoingShipments
                .Include(item => item.Boxes)
                    .ThenInclude(item => item.InventoryItems)
                        .ThenInclude(item => item.InventoryItem)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.BillingAddress)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.Company)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.PaymentMethod)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.ShippingAddress)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.GidLocationOption)
                            .ThenInclude(item => item.MainAddress)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.Contact)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.SalesPerson)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.LineItems)
                            .ThenInclude(item => item.Product)
                .Include(item => item.SalesOrderOutgoingShipment)
                    .ThenInclude(item => item.SalesOrder)
                        .ThenInclude(item => item.CurrencyOption)
                .Include(item => item.RmaOutgoingShipment)
                    .ThenInclude(item => item.Rma)
                .Include(item => item.ShippingAddress)
                .Include(item => item.ShippingCarrier)
                .Include(item => item.OutgoingShipmentShippingTermOption)
                .Include(item => item.ShippingCarrierShippingMethod)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (outgoingShipment == null) {
                Response.StatusCode = 400;
                return;
            }
            User currentUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            outgoingShipment.Preparer = currentUser;

            ViewData["LogoUrl"] = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewData["IsGidEurope"] = false;
            SalesOrder salesOrder = null;

            Address shippingAddress = null;

            if (outgoingShipment.SalesOrderOutgoingShipment != null)
            {
                salesOrder = outgoingShipment.SalesOrderOutgoingShipment.SalesOrder;
                ViewBag.shippingAddress = salesOrder.ShippingAddress;
                ViewBag.salesOrder = salesOrder;
            } else if(outgoingShipment.RmaOutgoingShipment != null)
            {
                var rma = outgoingShipment.RmaOutgoingShipment.Rma;
                salesOrder = await _context.SalesOrders
                    .Include(item => item.BillingAddress)
                    .Include(item => item.Company)
                    .Include(item => item.PaymentMethod)
                    .Include(item => item.ShippingAddress)
                    .Include(item => item.GidLocationOption)
                        .ThenInclude(item => item.MainAddress)
                    .Include(item => item.Contact)
                    .Include(item => item.SalesPerson)
                    .Include(item => item.LineItems)
                        .ThenInclude(item => item.Product)
                    .Include(item => item.CurrencyOption)
                    .FirstOrDefaultAsync(item => item.Id == outgoingShipment.RmaOutgoingShipment.Rma.SalesOrderId);

                ViewBag.shippingAddress = salesOrder.ShippingAddress;
                ViewBag.salesOrder = salesOrder;
                ViewBag.rma = rma;
            }
            ViewData["IsGidEurope"] = await ViewBag.salesOrder.CheckIfIsGidEurope(_context);
            string commercialInvoiceHtml = viewRenderer.Render("~/Features/OutgoingShipment/Views/PackingSlipPdf.cshtml", outgoingShipment, ViewData);
            var doc = new HtmlToPdfDocument() {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = commercialInvoiceHtml,
                        WebSettings = { DefaultEncoding = "utf-8" },
                        // HeaderSettings = { FontSize = 9, Right = "Page [page] of [toPage]", Line = true, Spacing = 2.812 }
                    }
                }
            };
            byte[] data = PdfConverter.Convert(doc);
            // Response.Headers.ContentType = "application/pdf";
            Response.ContentType = "application/pdf";
            await Response.Body.WriteAsync(data, 0, data.Length);
            return;
        }
    }
}