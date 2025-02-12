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
    [Route("BillLineItems")]
    public class BillLineItemsController : Controller {
        private readonly AppDBContext _context;
        private readonly QuickBooksConnector _quickBooksConnector;

        public BillLineItemsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: BillLineItems
        [HttpGet]
        public IEnumerable<BillLineItem> GetBillLineItems() {
            return _context.BillLineItems;
        }

        // GET: BillLineItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBillLineItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var billLineItem = await _context.BillLineItems.SingleOrDefaultAsync(m => m.Id == id);

            if (billLineItem == null) {
                return NotFound();
            }

            return Ok(billLineItem);
        }

        // PUT: BillLineItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBillLineItem([FromRoute] int? id, [FromBody] BillLineItem billLineItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != billLineItem.Id) {
                return BadRequest();
            }

            //update balance on the bill
            var bill = await _context.Bills
                .AsNoTracking() //have to make it no tracking or it complains about the line item already being tracked.  So once I get the new balance i have to select the bill AGAIN but without line items so that I can update the balance
                .Include(item => item.LineItems)
                .Include(item => item.CashDisbursements)
                    .ThenInclude(item => item.CashDisbursement)
                .FirstOrDefaultAsync(item => item.Id == billLineItem.BillId);

            var index = bill.LineItems.FindIndex(item => item.Id == id);
            bill.LineItems[index] = billLineItem;
            var balance = bill.GetBalance();
            bill = await _context.Bills.FirstOrDefaultAsync(item => item.Id == bill.Id);
            bill.Balance = balance;

            _context.Entry(billLineItem).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!BillLineItemExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            await bill.SyncWithQuickBooks(_quickBooksConnector, _context);

            return Ok(billLineItem);
        }

        // // POST: BillLineItems
        // [HttpPost]
        // public async Task<IActionResult> PostBillLineItem([FromBody] BillLineItem billLineItem)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }

        //     _context.BillLineItems.Add(billLineItem);
        //     await _context.SaveChangesAsync();

        //     return CreatedAtAction("GetBillLineItem", new { id = billLineItem.Id }, billLineItem);
        // }

        // DELETE: BillLineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBillLineItem([FromRoute] int? id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var billLineItem = await _context.BillLineItems.SingleOrDefaultAsync(m => m.Id == id);
            if (billLineItem == null) {
                return NotFound();
            }

            //update balance on the bill
            var bill = await _context.Bills
                .AsNoTracking() //have to make it no tracking or it complains about the line item already being tracked.  So once I get the new balance i have to select the bill AGAIN but without line items so that I can update the balance
                .Include(item => item.LineItems)
                .Include(item => item.CashDisbursements)
                    .ThenInclude(item => item.CashDisbursement)
                .FirstOrDefaultAsync(item => item.Id == billLineItem.BillId);

            var index = bill.LineItems.FindIndex(item => item.Id == id);
            bill.LineItems.RemoveAt(index);
            var balance = bill.GetBalance();
            bill = await _context.Bills.FirstOrDefaultAsync(item => item.Id == bill.Id);
            bill.Balance = balance;

            _context.Entry(billLineItem).State = EntityState.Modified;

            _context.BillLineItems.Remove(billLineItem);
            await _context.SaveChangesAsync();

            return Ok(billLineItem);
        }

        private bool BillLineItemExists(int? id) {
            return _context.BillLineItems.Any(e => e.Id == id);
        }
    }
}