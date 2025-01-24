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
    [Route("InvoiceCredits")]
    [RequirePermission("ManageBilling")]
    public class InvoiceCreditsController : Controller {
        private readonly AppDBContext _context;

        public InvoiceCreditsController(AppDBContext context) {
            _context = context;
        }

        // GET: InvoiceCredits?invoiceId=&creditId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="invoiceId"></param>
        /// <param name="creditId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetInvoiceCreditById([FromQuery] int? invoiceId, [FromQuery] int? creditId) {

            var invoiceCredit = await _context.InvoiceCredits.SingleOrDefaultAsync(m => m.CreditId == creditId && m.InvoiceId == invoiceId);

            if (invoiceCredit == null) {
                return NotFound();
            }

            return Ok(invoiceCredit);
        }

        // GET: InvoiceCredits
        /// <summary>
        /// Fetches a list of invoice credits
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetInvoiceCredits([FromQuery] int? invoiceId, [FromQuery] int? creditId) {
            if (invoiceId == null && creditId == null) {
                return BadRequest(new {
                    Error = "Must have either invoiceId or creditId querystring param"
                });
            }
            var query = from invoice in _context.InvoiceCredits select invoice;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (invoiceId != null) {
                query = query.Where(item => item.InvoiceId == invoiceId).Include(m => m.Credit);
            }
            if (creditId != null) {
                query = query.Where(item => item.CreditId == creditId).Include(m => m.Invoice);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // // PUT: InvoiceCredits?creditId=&invoiceId=
        // [HttpPut]
        // public async Task<IActionResult> PutInvoiceCredit([FromQuery] int? creditId, [FromQuery] int? invoiceId, [FromBody] InvoiceCredit invoiceCredit)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     _context.Entry(invoiceCredit).State = EntityState.Modified;

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

        // POST: InvoiceCredits
        [HttpPost]
        public async Task<IActionResult> PostInvoiceCredit([FromBody] InvoiceCredit invoiceCredit) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (invoiceCredit.Amount == null || invoiceCredit.Amount == 0) {
                return BadRequest("You need so specify an amount");
            }

            var credit = await _context.Credits
                .Include(item => item.Invoices).AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == invoiceCredit.CreditId);
            if (credit == null) {
                return BadRequest("A credit doesn't exist with that Id");
            }
            var creditBalance = credit.GetBalance();
            if (invoiceCredit.Amount > creditBalance) {
                return BadRequest("The amount you specified is greater than the credit balance");
            }

            //validate that this can happen first
            var invoice = await _context.Invoices
                .Include(item => item.LineItems)
                .Include(item => item.CashReceipts)
                    .ThenInclude(item => item.CashReceipt)
                .Include(item => item.Credits)
                    .ThenInclude(item => item.Credit)
                .AsNoTracking().FirstOrDefaultAsync(item => item.Id == invoiceCredit.InvoiceId);

            if (invoice == null) {
                return BadRequest("Invoice does not exist with that id");
            }
            var invoiceBalance = await invoice.GetBalance(_context);
            if (invoiceCredit.Amount > invoiceBalance) {
                return BadRequest("The amount you are trying to apply is greater than the remaining balance on the invoice.  Invoice balance: " + invoiceBalance.ToString());
            }

            invoice.Balance = invoiceBalance - invoiceCredit.Amount;
            _context.Entry(invoice).State = EntityState.Modified;

            credit.Balance = creditBalance - invoiceCredit.Amount;
            _context.Entry(credit).State = EntityState.Modified;

            invoiceCredit.CreatedAt = DateTime.UtcNow;
            invoiceCredit.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            _context.InvoiceCredits.Add(invoiceCredit);
            
            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (InvoiceCreditExists(invoiceCredit.InvoiceId, invoiceCredit.CreditId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            return CreatedAtAction("GetInvoiceCredit", new { id = invoiceCredit.CreditId }, invoiceCredit);
        }

        // DELETE: InvoiceCredits?invoiceId=&creditid=
        [HttpDelete]
        public async Task<IActionResult> DeleteInvoiceCredit([FromQuery] int invoiceId, [FromQuery] int creditId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var invoiceCredit = await _context.InvoiceCredits.SingleOrDefaultAsync(m => m.CreditId == creditId && m.InvoiceId == invoiceId);
            if (invoiceCredit == null) {
                return NotFound();
            }

            _context.InvoiceCredits.Remove(invoiceCredit);

            var invoice = await _context.Invoices.FirstOrDefaultAsync(item => item.Id == invoiceCredit.InvoiceId);
            invoice.Balance += invoiceCredit.Amount;
            var credit = await _context.Credits.FirstOrDefaultAsync(item => item.Id == invoiceCredit.CreditId);
            credit.Balance += invoiceCredit.Amount;
            await _context.SaveChangesAsync();

            return Ok(invoiceCredit);
        }

        private bool InvoiceCreditExists(int? invoiceId, int? creditId) {
            return _context.InvoiceCredits.Any(e => e.CreditId == creditId && e.InvoiceId == invoiceId);
        }
    }
}