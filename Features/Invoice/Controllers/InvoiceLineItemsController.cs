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
    [Route("InvoiceLineItems")]
    public class InvoiceLineItemsController : Controller {
        private readonly AppDBContext _context;
        private QuickBooksConnector _quickBooksConnector;

        public InvoiceLineItemsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: InvoiceLineItems
        [HttpGet]
        public IEnumerable<InvoiceLineItem> GetInvoiceLineItems() {
            return _context.InvoiceLineItems;
        }

        // GET: InvoiceLineItems/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoiceLineItem([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var invoiceLineItem = await _context.InvoiceLineItems.SingleOrDefaultAsync(m => m.Id == id);

            if (invoiceLineItem == null) {
                return NotFound();
            }

            return Ok(invoiceLineItem);
        }

        // PUT: InvoiceLineItems/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoiceLineItem([FromRoute] int id, [FromBody] InvoiceLineItem invoiceLineItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != invoiceLineItem.Id) {
                return BadRequest();
            }

            _context.Entry(invoiceLineItem).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!InvoiceLineItemExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            var newInvoice = await _context.Invoices
                .Include(item => item.LineItems)
                    .ThenInclude(item => item.Product)
                .Include(item => item.Company)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .FirstOrDefaultAsync(item => item.Id == invoiceLineItem.InvoiceId);
            newInvoice.Balance = await newInvoice.GetBalance(_context);
            await _context.SaveChangesAsync();
            if(newInvoice.SentAt.HasValue){
                newInvoice.SyncWithQuickBooks(_quickBooksConnector, _context);
            }

            return NoContent();
        }

        // POST: InvoiceLineItems
        [HttpPost]
        public async Task<IActionResult> PostInvoiceLineItem([FromBody] InvoiceLineItem invoiceLineItem) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.InvoiceLineItems.Add(invoiceLineItem);
            await _context.SaveChangesAsync();


            var newInvoice = await _context.Invoices
                .Include(item => item.LineItems)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .FirstOrDefaultAsync(item => item.Id == invoiceLineItem.InvoiceId);
            newInvoice.Balance = await newInvoice.GetBalance(_context);
            await _context.SaveChangesAsync();
            if(newInvoice.SentAt.HasValue){
                newInvoice.SyncWithQuickBooks(_quickBooksConnector, _context);
            }

            return CreatedAtAction("GetInvoiceLineItem", new { id = invoiceLineItem.Id }, invoiceLineItem);
        }

        // DELETE: InvoiceLineItems/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoiceLineItem([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var invoiceLineItem = await _context.InvoiceLineItems.SingleOrDefaultAsync(m => m.Id == id);
            if (invoiceLineItem == null) {
                return NotFound();
            }

            _context.InvoiceLineItems.Remove(invoiceLineItem);
            await _context.SaveChangesAsync();

            var newInvoice = await _context.Invoices
                .Include(item => item.LineItems)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .FirstOrDefaultAsync(item => item.Id == invoiceLineItem.InvoiceId);
            newInvoice.Balance = await newInvoice.GetBalance(_context);
            await _context.SaveChangesAsync();
            if(newInvoice.SentAt.HasValue){
                newInvoice.SyncWithQuickBooks(_quickBooksConnector, _context);
            }

            return Ok(invoiceLineItem);
        }

        private bool InvoiceLineItemExists(int id) {
            return _context.InvoiceLineItems.Any(e => e.Id == id);
        }
    }
}