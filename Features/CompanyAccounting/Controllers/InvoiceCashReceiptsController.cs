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
    [Route("InvoiceCashReceipts")]
    [RequirePermission("ManageBilling")]
    public class InvoiceCashReceiptsController : Controller {
        private readonly AppDBContext _context;
        private readonly QuickBooksConnector _quickBooksConnector;

        public InvoiceCashReceiptsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: InvoiceCashReceipts?invoiceId=&CashReceiptId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="CashReceiptId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetInvoiceCashReceiptById([FromQuery] int? invoiceId, [FromQuery] int? CashReceiptId) {

            var invoiceCashReceipt = await _context.InvoiceCashReceipts.SingleOrDefaultAsync(m => m.CashReceiptId == CashReceiptId && m.InvoiceId == invoiceId);

            if (invoiceCashReceipt == null) {
                return NotFound();
            }

            return Ok(invoiceCashReceipt);
        }

        // GET: InvoiceCashReceipts
        /// <summary>
        /// Fetches a list of invoice CashReceipts
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInvoiceCashReceipts([FromQuery] int? invoiceId, [FromQuery] int? CashReceiptId) {
            if (invoiceId == null && CashReceiptId == null) {
                return BadRequest(new {
                    Error = "Must have either invoiceId or CashReceiptId querystring param"
                });
            }
            var query = from invoice in _context.InvoiceCashReceipts select invoice;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (invoiceId != null) {
                query = query.Where(item => item.InvoiceId == invoiceId).Include(m => m.CashReceipt);
            }
            if (CashReceiptId != null) {
                query = query.Where(item => item.CashReceiptId == CashReceiptId).Include(m => m.Invoice);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // // PUT: InvoiceCashReceipts?CashReceiptId=&invoiceId=
        // [HttpPut]
        // public async Task<IActionResult> PutInvoiceCashReceipt([FromQuery] int? CashReceiptId, [FromQuery] int? invoiceId, [FromBody] InvoiceCashReceipt invoiceCashReceipt)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     _context.Entry(invoiceCashReceipt).State = EntityState.Modified;

        //     try
        //     {
        //         await _context.SaveChangesAsync();
        //     }
        //     catch (DbUpdateConcurrencyException)
        //     {
        //         {
        //             throw;
        //         }
        //     }

        //     return NoContent();
        // }

        // POST: InvoiceCashReceipts
        [HttpPost]
        public async Task<IActionResult> PostInvoiceCashReceipt([FromBody] InvoiceCashReceipt invoiceCashReceipt) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (invoiceCashReceipt.Amount == 0) {
                return BadRequest("You need so specify an amount");
            }

            var cashReceipt = await _context.CashReceipts
                .Include(item => item.Invoices).AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == invoiceCashReceipt.CashReceiptId);
            if (cashReceipt == null) {
                return BadRequest("A CashReceipt doesn't exist with that Id");
            }
            var CashReceiptBalance = cashReceipt.GetBalance();
            if (invoiceCashReceipt.Amount > CashReceiptBalance) {
                return BadRequest("The amount you specified is greater than the CashReceipt balance");
            }
            if (invoiceCashReceipt.CashReceipt != null) {
                invoiceCashReceipt.CashReceipt.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            }

            //validate that this can happen first
            var invoice = await _context.Invoices
                .Include(item => item.LineItems)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .AsNoTracking().FirstOrDefaultAsync(item => item.Id == invoiceCashReceipt.InvoiceId);

            if (invoice == null) {
                return BadRequest("Invoice does not exist with that id");
            }
            var invoiceBalance = await invoice.GetBalance(_context);
            if (invoiceCashReceipt.Amount > invoiceBalance) {
                return BadRequest("The amount you are trying to apply is greater than the balance of the invoice.  Invoice balance: " + invoiceBalance.ToString());
            }

            invoice.Balance = invoiceBalance - invoiceCashReceipt.Amount;
            _context.Entry(invoice).State = EntityState.Modified;

            cashReceipt.Balance = CashReceiptBalance - invoiceCashReceipt.Amount;
            _context.Entry(cashReceipt).State = EntityState.Modified;

            invoiceCashReceipt.CreatedAt = DateTime.UtcNow;
            invoiceCashReceipt.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            _context.InvoiceCashReceipts.Add(invoiceCashReceipt);

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (InvoiceCashReceiptExists(invoiceCashReceipt.InvoiceId, invoiceCashReceipt.CashReceiptId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            _context.Entry(invoice).State = EntityState.Detached;
            // JSB - HIDING QB INTEGRATION
            //var result = await cashReceipt.SyncWithQuickBooks(_quickBooksConnector, _context);
            //if (!result.Succeeded)
            //    return StatusCode(StatusCodes.Status500InternalServerError, result);

            return CreatedAtAction("GetInvoiceCashReceipt", new { id = invoiceCashReceipt.CashReceiptId }, invoiceCashReceipt);
        }

        // DELETE: InvoiceCashReceipts?invoiceId=&CashReceiptid=
        [HttpDelete]
        public async Task<IActionResult> DeleteInvoiceCashReceipt([FromQuery] int invoiceId, [FromQuery] int CashReceiptId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var invoiceCashReceipt = await _context.InvoiceCashReceipts.SingleOrDefaultAsync(m => m.CashReceiptId == CashReceiptId && m.InvoiceId == invoiceId);
            if (invoiceCashReceipt == null) {
                return NotFound();
            }

            _context.InvoiceCashReceipts.Remove(invoiceCashReceipt);

            var invoice = await _context.Invoices.FirstOrDefaultAsync(item => item.Id == invoiceCashReceipt.InvoiceId);
            invoice.Balance += invoiceCashReceipt.Amount;
            var cashReceipt = await _context.CashReceipts.FirstOrDefaultAsync(item => item.Id == invoiceCashReceipt.CashReceiptId);
            cashReceipt.Balance += invoiceCashReceipt.Amount;
            await _context.SaveChangesAsync();


            _context.Entry(invoice).State = EntityState.Detached;
            // JSB - HIDING QB INTEGRATION
            //var result = await cashReceipt.SyncWithQuickBooks(_quickBooksConnector, _context);
            //if (!result.Succeeded)
            //    return StatusCode(StatusCodes.Status500InternalServerError, result);

            return Ok(invoiceCashReceipt);
        }

        private bool InvoiceCashReceiptExists(int? invoiceId, int? CashReceiptId) {
            return _context.InvoiceCashReceipts.Any(e => e.CashReceiptId == CashReceiptId && e.InvoiceId == invoiceId);
        }
    }
}