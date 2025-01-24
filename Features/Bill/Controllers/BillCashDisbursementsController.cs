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
    [Route("BillCashDisbursements")]
    [RequirePermission("ManageBilling")]
    public class BillCashDisbursementsController : Controller {
        private readonly AppDBContext _context;
        private readonly QuickBooksConnector _quickBooksConnector;

        public BillCashDisbursementsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: BillCashDisbursements?billId=&cashDisbursementId=
        /// <summary>
        /// This function has a special route because it is a compound primary key
        /// </summary>
        /// <param name="billId"></param>
        /// <param name="cashDisbursementId"></param>
        /// <returns></returns>
        [HttpGet("ById")]
        public async Task<IActionResult> GetBillCashDisbursementById([FromQuery] int? billId, [FromQuery] int? cashDisbursementId) {

            var billCashDisbursement = await _context.BillCashDisbursements.SingleOrDefaultAsync(m => m.CashDisbursementId == cashDisbursementId && m.BillId == billId);

            if (billCashDisbursement == null) {
                return NotFound();
            }

            return Ok(billCashDisbursement);
        }

        // GET: BillCashDisbursements
        /// <summary>
        /// Fetches a list of bill cashDisbursements
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBillCashDisbursements([FromQuery] int? billId, [FromQuery] int? cashDisbursementId) {
            if (billId == null && cashDisbursementId == null) {
                return BadRequest(new {
                    Error = "Must have either billId or cashDisbursementId querystring param"
                });
            }
            var query = from bill in _context.BillCashDisbursements select bill;
            //if they have the sales order line item and are looking for related inventory items, find related items, and include the full inventory item object as well.
            if (billId != null) {
                query = query.Where(item => item.BillId == billId).Include(m => m.CashDisbursement);
            }
            if (cashDisbursementId != null) {
                query = query.Where(item => item.CashDisbursementId == cashDisbursementId).Include(m => m.Bill);
            }
            var items = await query.ToListAsync();
            return Ok(items);
        }

        // // PUT: BillCashDisbursements?cashDisbursementId=&billId=
        // [HttpPut]
        // public async Task<IActionResult> PutBillCashDisbursement([FromQuery] int? cashDisbursementId, [FromQuery] int? billId, [FromBody] BillCashDisbursement billCashDisbursement)
        // {
        //     if (!ModelState.IsValid)
        //     {
        //         return BadRequest(ModelState);
        //     }
        //     _context.Entry(billCashDisbursement).State = EntityState.Modified;

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

        // POST: BillCashDisbursements
        [HttpPost]
        public async Task<IActionResult> PostBillCashDisbursement([FromBody] BillCashDisbursement billCashDisbursement) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (billCashDisbursement.Amount == 0) {
                return BadRequest("You need so specify an amount");
            }

            var cashDisbursement = await _context.CashDisbursements
                .Include(item => item.Bills).AsNoTracking()
                .FirstOrDefaultAsync(item => item.Id == billCashDisbursement.CashDisbursementId);
            if (cashDisbursement == null) {
                return BadRequest("A Cash Disbursement doesn't exist with that Id");
            }
            var cashDisbursementBalance = cashDisbursement.GetBalance();
            if (billCashDisbursement.Amount > cashDisbursementBalance) {
                Console.WriteLine("Cash Disbursement Balance is " + cashDisbursementBalance);
                return BadRequest("The amount you specified is greater than the Cash Disbursement balance");
            }

            //validate that this can happen first
            var bill = await _context.Bills.AsNoTracking()
                .Include(item => item.LineItems)
                .Include(item => item.CashDisbursements)
                    .ThenInclude(item => item.CashDisbursement)
                .AsNoTracking().FirstOrDefaultAsync(item => item.Id == billCashDisbursement.BillId);

            if (bill == null) {
                return BadRequest("Bill does not exist with that id");
            }
            var billBalance = bill.GetBalance();
            if (billCashDisbursement.Amount > billBalance) {
                Console.WriteLine("Bill balance is " + billBalance);
                return BadRequest("The amount you are trying to apply is greater than the remaining balance of the bill. Remaining balance: " + billBalance.ToString());
            }

            bill.Balance = billBalance - billCashDisbursement.Amount;
            _context.Entry(bill).State = EntityState.Modified;

            cashDisbursement.Balance = cashDisbursementBalance - billCashDisbursement.Amount;
            _context.Entry(cashDisbursement).State = EntityState.Modified;

            billCashDisbursement.CreatedAt = DateTime.UtcNow;
            billCashDisbursement.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            _context.BillCashDisbursements.Add(billCashDisbursement);

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException) {
                if (BillCashDisbursementExists(billCashDisbursement.BillId, billCashDisbursement.CashDisbursementId)) {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                } else {
                    throw;
                }
            }

            _context.Entry(bill).State = EntityState.Detached;

            // JSB - HIDING QB INTEGRATION
            //await cashDisbursement.SyncWithQuickBooks(_quickBooksConnector, _context);

            return CreatedAtAction("GetBillCashDisbursement", new { id = billCashDisbursement.CashDisbursementId }, billCashDisbursement);
        }

        // DELETE: BillCashDisbursements?billId=&cashDisbursementid=
        [HttpDelete]
        public async Task<IActionResult> DeleteBillCashDisbursement([FromQuery] int billId, [FromQuery] int cashDisbursementId) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var billCashDisbursement = await _context.BillCashDisbursements.SingleOrDefaultAsync(m => m.CashDisbursementId == cashDisbursementId && m.BillId == billId);
            if (billCashDisbursement == null) {
                return NotFound();
            }

            _context.BillCashDisbursements.Remove(billCashDisbursement);

            var bill = await _context.Bills.FirstOrDefaultAsync(item => item.Id == billCashDisbursement.BillId);
            bill.Balance += billCashDisbursement.Amount;
            var cashDisbursement = await _context.CashDisbursements.FirstOrDefaultAsync(item => item.Id == billCashDisbursement.CashDisbursementId);
            cashDisbursement.Balance += billCashDisbursement.Amount;
            // _context.Entry(bill).State = EntityState.Modified;
            await _context.SaveChangesAsync();


            // JSB - HIDING QB INTEGRATION
            //await cashDisbursement.SyncWithQuickBooks(_quickBooksConnector, _context);

            return Ok(billCashDisbursement);
        }

        private bool BillCashDisbursementExists(int? billId, int? cashDisbursementId) {
            return _context.BillCashDisbursements.Any(e => e.CashDisbursementId == cashDisbursementId && e.BillId == billId);
        }
    }
}