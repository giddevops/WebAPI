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
    [Route("Bills")]
    public class BillsController : Controller {
        private readonly AppDBContext _context;
        private readonly QuickBooksConnector _quickBooksConnector;

        public BillsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: Bills
        [HttpGet]
        public ListResult GetBills(
            [FromQuery] int? companyId,
            [FromQuery] bool? unpaidOnly,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true,
            [FromQuery] int? id = null,
            [FromQuery] DateTime? createdAtStartDate = null,
            [FromQuery] DateTime? createdAtEndDate = null
        ) {
            var query = from bill in _context.Bills select bill;

            if (companyId != null)
                query = query.Where(item => item.CompanyId == companyId);

            if (unpaidOnly == true)
                query = query.Where(item => item.Balance > 0);

            if (id != null)
                query = query.Where(item => item.Id == id);

            if (createdAtStartDate != null)
                query = query.Where(l => l.CreatedAt >= createdAtStartDate);

            if (createdAtEndDate != null)
                query = query.Where(l => l.CreatedAt <= createdAtEndDate);


            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "Balance":
                    query = sortAscending ? query.OrderBy(item => item.Balance) : query.OrderByDescending(item => item.Balance);
                    break;
                case "CreatedById":
                    query = sortAscending ? query.OrderBy(item => item.CreatedById) : query.OrderByDescending(item => item.CreatedById);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            query = query
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }


        // GET: Bills
        [HttpGet("Search")]
        public async Task<IActionResult> SearchBills(
            [FromQuery] int? query,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var dbQuery = from rma in _context.Bills select rma;

            dbQuery = dbQuery.Where(rma => rma.Id == query);


            dbQuery = dbQuery
                .OrderByDescending(q => q.CreatedAt);

            var items = await dbQuery.Select(item => new {
                Id = item.Id,
                Name = item.Id
            }).ToListAsync();

            return Ok(items);
        }

        // GET: Bills/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBill([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var bill = await _context.Bills
                .Include(item => item.CashDisbursements)
                    .ThenInclude(item => item.CashDisbursement)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (bill == null) {
                return NotFound();
            }

            return Ok(bill);
        }

        // GET: Bills/5/LineItems
        [HttpGet("{id}/LineItems")]
        public async Task<IActionResult> GetBillLineItems([FromRoute] int id, [FromQuery] bool? includeInventoryItems) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var query = from so in _context.Bills select so;

            query = query
                .Include(m => m.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.PurchaseOrderLineItem)
                //.ThenInclude(item => item.InventoryItems)
                //    .ThenInclude(item => item.InventoryItem)
                .Include(m => m.Company);

            // var lineItems = purchaseOrder.LineItems;
            // var shippedStatus = await _context.InventoryItemStatusOptions.FirstOrDefaultAsync(item => item.Value == "Shipped");
            // if(shippedStatus == null){
            //     return BadRequest(new {
            //         Error = "Unable to find the 'Shipped' inventory item status id."
            //     });
            // }
            // lineItems.ForEach(lineItem => {
            //     lineItem.Quantity = lineItem.InventoryItems.Count(lineItemInventoryItem => lineItemInventoryItem.InventoryItem.InventoryItemStatusOptionId == shippedStatus.Id);
            // });


            Bill purchaseOrder = await query
                .FirstOrDefaultAsync(l => l.Id == id);

            var shippedStatus = await _context.InventoryItemStatusOptions.FirstOrDefaultAsync(item => item.Value == "Shipped");
            if (shippedStatus == null) {
                return BadRequest(new {
                    Error = "Unable to find the 'Shipped' inventory item status id."
                });
            }
            // purchaseOrder.LineItems.ForEach(lineItem => {
            //     lineItem.QuantityShipped = lineItem.BillLineItem.InventoryItems.Count(lineItemInventoryItem => lineItemInventoryItem.InventoryItem.InventoryItemStatusOptionId == shippedStatus.Id);
            // });

            return Ok(purchaseOrder.LineItems);
        }

        [HttpGet("{id}/BillCashDisbursements")]
        public ListResult BillCashDisbursements([FromRoute] int id, [FromQuery] int perPage = 100, [FromQuery] int skip = 0) {
            var query = _context.BillCashDisbursements
                .Include(item => item.CashDisbursement)
                .Where(item => item.BillId == id);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // PUT: Bills/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBill([FromRoute] int id, [FromBody] Bill bill) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != bill.Id) {
                return BadRequest();
            }

            var existingBill = await _context.Bills.AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == id);

            bill.QuickBooksId = existingBill.QuickBooksId;
            bill.QuickBooksSyncToken = existingBill.QuickBooksSyncToken;

            _context.Entry(bill).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!BillExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            var newBill = await _context.Bills.AsNoTracking()
                .Include(item => item.LineItems)
                .Include(item => item.CashDisbursements)
                    .ThenInclude(item => item.CashDisbursement)
                .FirstOrDefaultAsync(item => item.Id == id);
            bill.Balance = newBill.GetBalance();
            await _context.SaveChangesAsync();


            var result = await bill.SyncWithQuickBooks(_quickBooksConnector, _context);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, result);

            return Ok(bill);
        }

        // POST: Bills
        [HttpPost]
        public async Task<IActionResult> PostBill([FromBody] Bill bill) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            bill.CreatedAt = DateTime.UtcNow;
            if (bill.LineItems != null)
                bill.Balance = bill.GetBalance();

            bill.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            _context.Bills.Add(bill);
            await _context.SaveChangesAsync();

            var result = await bill.SyncWithQuickBooks(_quickBooksConnector, _context);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, result);

            return CreatedAtAction("GetBill", new { id = bill.Id }, bill);
        }

        // DELETE: Bills/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBill([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var bill = await _context.Bills
                .Include(item => item.CashDisbursements)
                // .Include(item => item.Attachments)
                //     .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (bill == null) {
                return NotFound();
            }

            if (bill.CashDisbursements.Count > 0) {
                return BadRequest("You can't delete this bill until you remove all related Cash Disbursements");
            }


            // foreach(var itemAttachment in bill.Attachments.ToList()){
            //     await itemAttachment.Attachment.Delete(_context, _configuration);
            // }
            _context.Bills.Remove(bill);
            await _context.SaveChangesAsync();


            var result = await bill.DeleteFromQuickBooks(_quickBooksConnector, _context);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, result);
                
            return Ok(bill);
        }

        private bool BillExists(int id) {
            return _context.Bills.Any(e => e.Id == id);
        }
    }
}