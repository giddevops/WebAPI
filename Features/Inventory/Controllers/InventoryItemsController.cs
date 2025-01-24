using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.Extensions.Configuration;
using System.Text;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using DinkToPdf;
using WebApi.Services;
using Microsoft.AspNetCore.Hosting;
using DinkToPdf.Contracts;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using GidIndustrial.Gideon.WebApi.Libraries;
using Barcoder;


namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("InventoryItems")]
    public class InventoryItemsController : Controller {
        private readonly AppDBContext _context;
        private IConfiguration _configuration;
        public IHostingEnvironment Environment;
        private IConverter PdfConverter;
        private readonly ViewRender viewRenderer;


        public InventoryItemsController(AppDBContext context, IConfiguration config, ViewRender renderer, IHostingEnvironment env, IConverter converter) {
            _context = context;
            _configuration = config;
            viewRenderer = renderer;
            Environment = env;
            PdfConverter = converter;
        }


        [HttpGet("{id}/GetDefaultSalesOrderIdOrRmaId")]
        public async Task<IActionResult> GetDefaultSalesOrderIdOrRmaId([FromRoute] int? id) {
            var inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(item => item.Id == id);
            if (inventoryItem == null) {
                return NotFound("Inventory item was not found with that Id");
            }
            return Ok(inventoryItem.GetDefaultSalesOrderIdOrRmaId(_context));
        }

        // GET: InventoryItems
        [HttpGet]
        public ListResult GetInventoryItems(
            [FromQuery] int? productId,
            [FromQuery] string partNumber,
            [FromQuery] string partNumberStartsWith,
            [FromQuery] bool hideRelated,
            [FromQuery] int?[] excludeIds,
            [FromQuery] bool? committed,
            [FromQuery] bool? available,
            [FromQuery] int? gidSubLocationOptionId,
            [FromQuery] int? gidLocationOptionId,
            [FromQuery] string gidPartNumber,
            [FromQuery] bool? hideIfHasParents,
            [FromQuery] bool? countOnly,
            [FromQuery] string serialNumber,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from inventoryItem in _context.InventoryItems
                        select inventoryItem;

            if (productId != null) {
                query = query.Where(ii => ii.ProductId == productId);
            }
            if (partNumber != null) {
                query = query.Where(ii => ii.Product.PartNumber == partNumber);
            }
            if (partNumberStartsWith != null) {
                query = query.Where(ii => ii.Product.PartNumber.StartsWith(partNumberStartsWith));
            }
            if (excludeIds.Length > 0) {
                query = query.Where(ii => !excludeIds.Contains(ii.Id));
            }
            if (committed != null) {
                if (committed == true)
                    query = query.Where(ii => ii.InventoryItemStatusOptionId == InventoryItemStatusOption.Committed);
                else
                    query = query.Where(ii => ii.InventoryItemStatusOptionId != InventoryItemStatusOption.Committed);
            }
            if (available != null) {
                if (available == true) {
                    query = query.Where(item => item.InventoryItemStatusOptionId == InventoryItemStatusOption.Available);
                }
            }
            if (gidSubLocationOptionId != null) {
                query = query.Where(ii => ii.GidSubLocationOptionId == gidSubLocationOptionId);
            }
            if (gidLocationOptionId != null) {
                query = query.Where(item => item.GidLocationOptionId == gidLocationOptionId);
            }
            if (!String.IsNullOrWhiteSpace(gidPartNumber)) {
                query = query.Where(item => item.Product.GidPartNumber.StartsWith(gidPartNumber));
            }
            if (hideIfHasParents == true) {
                query = query.Where(ii => ii.ParentRelatedInventoryItem == null);
            }
            if (!String.IsNullOrEmpty(serialNumber)) {
                query = query.Where(item => item.SerialNumber.StartsWith(serialNumber));
            }


            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;

                case "Product.PartNumber":
                    query = sortAscending ? query.OrderBy(item => item.Product.PartNumber) : query.OrderByDescending(item => item.Product.PartNumber);
                    break;
                case "SerialNumber":
                    query = sortAscending ? query.OrderBy(item => item.SerialNumber) : query.OrderByDescending(item => item.SerialNumber);
                    break;
                case "Location":
                    query = sortAscending ? query.OrderBy(item => item.GidSubLocationOption.Name) : query.OrderByDescending(item => item.GidSubLocationOption.Name);
                    break;
                case "Condition":
                    query = sortAscending ? query.OrderBy(item => item.ProductConditionOptionId) : query.OrderByDescending(item => item.ProductConditionOptionId);
                    break;
                case "Status":
                    query = sortAscending ? query.OrderBy(item => item.InventoryItemStatusOptionId) : query.OrderByDescending(item => item.InventoryItemStatusOptionId);
                    break;

                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            if (!String.IsNullOrWhiteSpace(searchString)) {
                searchString = searchString.Trim();
                query = query.Where(
                    item => item.SerialNumber.StartsWith(searchString)
                );
            }

            query = query
                .Include(ii => ii.Product);

            if (countOnly == true) {
                return new ListResult {
                    Items = null,
                    Count = query.Count()
                };
            }

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }


        // GET: Companies/{id}/GetName
        [HttpGet("{id}/GetSerialNumber")]
        public string GetName([FromRoute] int id)
        {
            return _context.InventoryItems
                .Where(item => item.Id == id)
                .Select(item => new InventoryItem
                {
                    Id = item.Id,
                    SerialNumber = item.SerialNumber
                }).FirstOrDefault().SerialNumber;
        }


        // GET: Companies/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<InventoryItem> Search([FromQuery] string query)
        {
            if (Int32.TryParse(query, out int id))
                return _context.InventoryItems
                    .Where(item => item.SerialNumber.StartsWith((query ?? "").Trim()) || item.Id == id)
                    .Take(30);
            else
                return _context.InventoryItems
                    .Where(item => item.SerialNumber.StartsWith((query ?? "").Trim()))
                    .Take(30);
        }


        // GET: InventoryItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var inventoryItem = await _context.InventoryItems
                .Include(m => m.EventLogEntries)
                    .ThenInclude(m => m.EventLogEntry)
                .Include(m => m.Attachments)
                    .ThenInclude(l => l.Attachment)
                .Include(item => item.PurchaseOrderLineItem)
                    .ThenInclude(item => item.PurchaseOrder)
                // .Include(item => item.IncomingShipmentInventoryItems)
                //     .ThenInclude(item => item.IncomingShipment)
                //         .ThenInclude(item => item.PurchaseOrders)
                //             .ThenInclude(item => item.PurchaseOrder)
                //                 .ThenInclude(item => item.LineItems)
                .Include(ii => ii.Product)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (inventoryItem == null) {
                return NotFound();
            }

            return Ok(inventoryItem);
        }

        // GET: InventoryItems/5/Children
        [HttpGet("{id}/Children")]
        public ListResult GetChildren(
            [FromRoute] int Id,
            [FromQuery] int? productId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from inventoryItem in _context.InventoryItemRelatedInventoryItems select inventoryItem;
            query = query
                .Include(ii => ii.ChildInventoryItem)
                    .ThenInclude(c => c.Product);

            query = query.Where(ii => ii.ParentInventoryItemId == Id);

            if (productId != null) {
                query = query.Where(item => item.ChildInventoryItem.ProductId == productId);
            }

            query = query
                .OrderByDescending(q => q.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        [HttpPost("{id}/RelatePieceParts")]
        public async Task<IActionResult> RelatePieceParts([FromRoute] int id, [FromBody] List<RelateInventoryItemData> relationships) {
            var newRelationships = new List<InventoryItemRelatedInventoryItem> { };
            var parentInventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(item => item.Id == id);
            using (var transaction = _context.Database.BeginTransaction()) {
                foreach (var relationship in relationships) {
                    var items = await _context.InventoryItems
                        .Where(item => item.ProductId == relationship.ProductId)
                        .Where(item => item.ParentRelatedInventoryItem == null)
                        .OrderBy(item => item.CreatedAt)
                        .Take(relationship.Quantity)
                        .ToListAsync();
                    var product = await _context.Products.FirstOrDefaultAsync(item => item.Id == relationship.ProductId);
                    if (product == null)
                        return BadRequest("A product with id " + relationship.ProductId.ToString() + " was not found");
                    if (product.Serialized == true)
                        return BadRequest("Cannot add serialized products.  Product Id is " + relationship.ProductId.ToString());

                    for (var i = 0; i < relationship.Quantity; ++i) {
                        var item = items.FirstOrDefault();
                        if (item == null) {
                            item = new InventoryItem {
                                CreatedAt = DateTime.UtcNow,
                                ProductId = relationship.ProductId
                            };
                            _context.InventoryItems.Add(item);
                        }else{
                            items.RemoveAt(0);
                        }
                        item.InventoryItemLocationOptionId = parentInventoryItem.InventoryItemLocationOptionId;
                        item.InventoryItemStatusOptionId = (await _context.InventoryItemStatusOptions.FirstAsync(opt => opt.Value == "Committed")).Id;
                        var newRelationship = new InventoryItemRelatedInventoryItem {
                            ParentInventoryItemId = id,
                            ChildInventoryItemId = item.Id
                        };
                        newRelationships.Add(newRelationship);
                        _context.InventoryItemRelatedInventoryItems.Add(newRelationship);
                    }
                    await _context.SaveChangesAsync();
                }
                transaction.Commit();
            }
            return Ok(newRelationships);
        }


        [HttpGet("{id}/GetChildPartNumbers")]
        public async Task<IActionResult> GetChildPartNumbers([FromRoute] int id) {
            var query = from inventoryItem in _context.InventoryItems
                        join inventoryItemRelatedInventoryItem in _context.InventoryItemRelatedInventoryItems
                            on inventoryItem.Id equals inventoryItemRelatedInventoryItem.ParentInventoryItemId
                        join product in _context.Products on inventoryItemRelatedInventoryItem.ChildInventoryItem.ProductId equals product.Id
                        where inventoryItem.Id == id
                        select new {
                            Id = product.Id,
                            Value = product.PartNumber
                        };
            var results = await query
                .Distinct()
                .ToListAsync();
            return Ok(results);
        }


        /// <summary>
        /// This method generates a pdf of the quote
        /// </summary>
        /// <param name="id">The id of the quote</param>
        /// <returns></returns>
        [HttpGet("{id}/GenerateProductTravellerPdf")]
        public async Task GenerateProductTravellerPdf([FromRoute] int id) {

            var inventoryItem = await _context.InventoryItems
                .Include(item => item.Product)
                    .ThenInclude(item => item.Manufacturer)
                .Include(item => item.Manufacturer)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (inventoryItem == null) {
                Response.StatusCode = 400;
                await Response.Body.WriteAsync(Encoding.UTF8.GetBytes("Inventory item not found"));
                return;
            }
            var inventoryItemLabel = inventoryItem != null ? await ScannerLabel.GetScannerLabel(inventoryItem, _context) : null;
            if (inventoryItemLabel != null)
                ViewBag.InventoryItemBarcode = await inventoryItemLabel.GetQRCodeDataUrl();

            var purchaseOrder = await _context.PurchaseOrders
                .Where(item => item.IncomingShipments.Any(shipment => shipment.IncomingShipment.InventoryItems.Any(ii => ii.InventoryItemId == id)))
                .Include(item => item.Supplier)
                .FirstOrDefaultAsync();
            ViewBag.PurchaseOrder = purchaseOrder;
            var purchaseOrderLabel = purchaseOrder != null ? await ScannerLabel.GetScannerLabel(purchaseOrder, _context) : null;
            if (purchaseOrderLabel != null)
                ViewBag.PurchaseOrderBarcode = await purchaseOrderLabel.GetQRCodeDataUrl();

            var salesOrder = await _context.SalesOrders
                .Where(item =>
                    item.OutgoingShipments.Any(shipment => shipment.OutgoingShipment.Boxes.Any(box => box.InventoryItems.Any(ii => ii.InventoryItemId == id)))
                    || item.InventoryItems.Any(ii => ii.InventoryItemId == id)
                    || item.LineItems.Any(li => li.InventoryItems.Any(ii => ii.InventoryItemId == id))
                )
                .Include(item => item.Company)
                .FirstOrDefaultAsync();
            ViewBag.SalesOrder = salesOrder;
            var salesOrderLabel = salesOrder != null ? await ScannerLabel.GetScannerLabel(salesOrder, _context) : null;
            if (salesOrderLabel != null)
                ViewBag.SalesOrderBarcode = await salesOrderLabel.GetQRCodeDataUrl();

            ViewBag.LogoUrl = Environment.ContentRootPath + @"\Features\Quote\Views\gid-industrial-logo.png";
            ViewBag.NumpadUrl = Environment.ContentRootPath + @"\Resources\Images\barcode-number-input-round-border.png";

            string productTravellerHtml = viewRenderer.Render("~/Features/Inventory/Views/ProductTravellerPdf.cshtml", inventoryItem, ViewData);

            var doc = new HtmlToPdfDocument() {
                GlobalSettings = {
                    ColorMode = ColorMode.Color,
                    Orientation = Orientation.Portrait,
                    PaperSize = PaperKind.A4,
                },
                Objects = {
                    new ObjectSettings() {
                        PagesCount = true,
                        HtmlContent = productTravellerHtml,
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


        // // GET: InventoryItems/5/RelateToSalesOrderLineItem
        // [HttpGet("{id}/RelateToSalesOrderLineItem")]
        // public async Task<IActionResult> RelateToSalesOrderLineItem([FromRoute] int? id, [FromQuery] int? salesOrderLineItemId)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     if (id == null)
        //     {
        //         return BadRequest(new
        //         {
        //             Error = "id is missing from route"
        //         });
        //     }
        //     else if (salesOrderLineItemId == null)
        //     {
        //         return BadRequest(new
        //         {
        //             SerializableError = "salesOrderLineItemId is missing from queryString"
        //         });
        //     }

        //     InventoryItem inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(ii => ii.Id == id);
        //     if(inventoryItem == null){
        //         return NotFound();
        //     }
        //     if(inventoryItem.Committed == true){
        //         return BadRequest(new {
        //             Error = "This inventory item is already committed"
        //         });
        //     }

        //     var newSalesOrderLineItemInventoryItem = new SalesOrderLineItemInventoryItem{
        //         SalesOrderLineItemId = salesOrderLineItemId,
        //         InventoryItemId = id
        //     };
        //     _context.SalesOrderLineItemInventoryItems.Add(newSalesOrderLineItemInventoryItem);
        //     inventoryItem.Committed = true;
        //     await _context.SaveChangesAsync();
        //     return Ok(newSalesOrderLineItemInventoryItem);
        // }

        // // GET: InventoryItems/5/UnrelateToSalesOrderLineItem
        // [HttpGet("{id}/UnrelateToSalesOrderLineItem")]
        // public async Task<IActionResult> UnrelateToSalesOrderLineItem([FromRoute] int? id, [FromQuery] int? salesOrderLineItemId)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     if (id == null)
        //     {
        //         return BadRequest(new
        //         {
        //             Error = "id is missing from route"
        //         });
        //     }
        //     else if (salesOrderLineItemId == null)
        //     {
        //         return BadRequest(new
        //         {
        //             SerializableError = "salesOrderLineItemId is missing from queryString"
        //         });
        //     }

        //     InventoryItem inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(ii => ii.Id == id);
        //     if(inventoryItem == null){
        //         return NotFound();
        //     }
        //     if(inventoryItem.Committed == true){
        //         return BadRequest(new {
        //             Error = "This inventory item is already committed"
        //         });
        //     }

        //     var newSalesOrderLineItemInventoryItem = new SalesOrderLineItemInventoryItem{
        //         SalesOrderLineItemId = salesOrderLineItemId,
        //         InventoryItemId = id
        //     };
        //     _context.SalesOrderLineItemInventoryItems.Add(newSalesOrderLineItemInventoryItem);
        //     inventoryItem.Committed = true;
        //     await _context.SaveChangesAsync();
        //     return Ok(newSalesOrderLineItemInventoryItem);
        // }

        // // GET: InventoryItems/5/RelateToSalesOrderLineItem
        // [HttpGet("{id}/RelateToSalesOrderLineItem")]
        // public async Task<IActionResult> UnelateToSalesOrderLineItem([FromRoute] int? id, [FromRoute] int? salesOrderLineItemId)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     if (id == null)
        //     {
        //         return BadRequest(new
        //         {
        //             Error = "id is missing from route"
        //         });
        //     }
        //     else if (salesOrderLineItemId == null)
        //     {
        //         return BadRequest(new
        //         {
        //             SerializableError = "salesOrderLineItemId is missing from queryString"
        //         });
        //     }

        //     InventoryItem inventoryItem = await _context.InventoryItems.FirstOrDefaultAsync(ii => ii.Id == id);
        //     if(inventoryItem == null){
        //         return NotFound();
        //     }
        //     if(inventoryItem.Committed == true){
        //         return BadRequest(new {
        //             Error = "This inventory item is already committed"
        //         });
        //     }

        //     var newSalesOrderLineItemInventoryItem = new SalesOrderLineItemInventoryItem{
        //         SalesOrderLineItemId = salesOrderLineItemId,
        //         InventoryItemId = id
        //     };
        //     inventoryItem.Committed = true;
        //     await _context.SaveChangesAsync();
        //     return Ok(newSalesOrderLineItemInventoryItem);
        // }

        // // GET: InventoryItems
        // [HttpGet("{id}/Parents")]
        // public ListResult GetParents(
        //     [FromRoute] int Id = 0,
        //     [FromQuery] int skip = 0,
        //     [FromQuery] int perPage = 10
        // )
        // {
        //     var query = from inventoryItem in _context.InventoryItemRelatedInventoryItems select inventoryItem;
        //     query = query
        //         .Include(ii => ii.ParentInventoryItem)
        //             .ThenInclude(p => p.Product);
        //     query = query.Where(ii => ii.ChildInventoryItemId == Id);

        //     query = query
        //         .OrderByDescending(q => q.CreatedAt);

        //     return new ListResult
        //     {
        //         Items = query.Skip(skip).Take(perPage),
        //         Count = query.Count()
        //     };
        // }

        // PUT: InventoryItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryItem([FromRoute] int? id, [FromBody] InventoryItem inventoryItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != inventoryItem.Id) {
                return BadRequest();
            }

            using (var transaction = _context.Database.BeginTransaction()) {
                _context.Entry(inventoryItem).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                var salesOrders = await _context.SalesOrders.AsNoTracking().Where(item => item.LineItems.Any(lineItem => lineItem.InventoryItems.Any(item3 => item3.InventoryItemId == id))).ToListAsync();
                foreach (var salesOrder in salesOrders) {
                    await salesOrder.UpdateShippedProfit(_context);
                }
                transaction.Commit();
            }

            return NoContent();
        }

        // POST: InventoryItems
        [HttpPost]
        public async Task<IActionResult> PostInventoryItem([FromBody] InventoryItem inventoryItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (inventoryItem.ProductId == null) {
                return BadRequest("ProductId is required");
            }


            Product product = await _context.Products.FirstOrDefaultAsync(p => p.Id == inventoryItem.ProductId);
            if (product == null) {
                return BadRequest("Product not found with that Id");
            }
            if (product.Serialized && string.IsNullOrEmpty(inventoryItem.SerialNumber)) {
                return BadRequest("This inventory item has no serial number, but the product is serialized. Please add a serial number");
            }

            inventoryItem.CreatedAt = DateTime.UtcNow;
            inventoryItem.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);

            _context.InventoryItems.Add(inventoryItem);
            //add event log entry
            _context.InventoryItemEventLogEntries.Add(new InventoryItemEventLogEntry {
                InventoryItem = inventoryItem,
                EventLogEntry = new EventLogEntry {
                    Event = "Created",
                    CreatedAt = DateTime.UtcNow,
                    OccurredAt = DateTime.UtcNow,
                    CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
                    UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
                }
            });

            await _context.SaveChangesAsync();

            if (!product.Serialized) {
                inventoryItem.GenerateSerialNumber();
                await _context.SaveChangesAsync();
            }


            return CreatedAtAction("GetInventoryItem", new { id = inventoryItem.Id }, inventoryItem);
        }

        // DELETE: InventoryItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var inventoryItem = await _context.InventoryItems
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (inventoryItem == null) {
                return NotFound();
            }

            // //delete any inventory item relationships that this is a part of
            // var childRelations = _context.InventoryItemRelatedInventoryItems.Where(r => r.ParentInventoryItemId == id);
            // var parentRelations = _context.InventoryItemRelatedInventoryItems.Where(r => r.ChildInventoryItemId == id);
            // _context.InventoryItemRelatedInventoryItems.RemoveRange(childRelations);
            // _context.InventoryItemRelatedInventoryItems.RemoveRange(parentRelations);

            foreach (var itemAttachment in inventoryItem.Attachments.ToList()) {
                await itemAttachment.Attachment.Delete(_context, _configuration);
            }
            _context.InventoryItems.Remove(inventoryItem);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }
            return Ok(inventoryItem);
        }

        private bool InventoryItemExists(int? id) {
            return _context.InventoryItems.Any(e => e.Id == id);
        }
    }

    public class RelateInventoryItemData {
        public int ProductId;
        public int Quantity;
    }
}