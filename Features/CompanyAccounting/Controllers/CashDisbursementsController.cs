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
    [Route("CashDisbursements")]
    public class CashDisbursementsController : Controller {
        private readonly AppDBContext _context;
        private readonly QuickBooksConnector _quickBooksConnector;

        public CashDisbursementsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: CashDisbursements
        [HttpGet]
        public ListResult GetCashDisbursements(
            [FromQuery] int? companyId,
            [FromQuery] bool hasBalance,
            [FromQuery] int? currencyOptionId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from cashDisbursement in _context.CashDisbursements select cashDisbursement;

            if (companyId != null)
                query = query.Where(item => item.CompanyId == companyId);
            if (hasBalance == true)
                query = query.Where(item => item.Balance > 0);
            if (currencyOptionId != null)
                query = query.Where(item => item.CurrencyOptionId == currencyOptionId);
            if (companyId != null)
                query = query.Where(item => item.CompanyId == companyId);

            query = query
                .Include(item => item.Company)
                .OrderByDescending(item => item.DateDisbursed);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: CashDisbursements/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCashDisbursement([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var cashDisbursement = await _context.CashDisbursements.SingleOrDefaultAsync(m => m.Id == id);

            if (cashDisbursement == null) {
                return NotFound();
            }

            return Ok(cashDisbursement);
        }

        [HttpGet("{id}/GetBillCashDisbursements")]
        public async Task<ListResult> GetBillCashDisbursements(
            [FromRoute] int id,
            [FromQuery] int? ignoreIfRelatedToBillId,
            [FromQuery] int? companyId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from billCashDisbursement in _context.BillCashDisbursements select billCashDisbursement;

            query = query
                .Where(item => item.CashDisbursementId == id)
                .OrderByDescending(item => item.CreatedAt);

            if (ignoreIfRelatedToBillId != null) {
                query = query.Where(item => item.BillId != ignoreIfRelatedToBillId);
            }

            return new ListResult {
                Items = await query.Skip(skip).Take(perPage).ToListAsync(),
                Count = query.Count()
            };
        }

        // PUT: CashDisbursements/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCashDisbursement([FromRoute] int id, [FromBody] CashDisbursement cashDisbursement) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != cashDisbursement.Id) {
                return BadRequest();
            }

            var originalCashDisbursement = await _context.CashDisbursements.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (originalCashDisbursement == null) {
                return NotFound("A cash disbursement with that ID was not found");
            }
            cashDisbursement.Balance = originalCashDisbursement.Balance;
            cashDisbursement.QuickBooksBillPaymentId = originalCashDisbursement.QuickBooksBillPaymentId;
            cashDisbursement.QuickBooksBillPaymentSyncToken = originalCashDisbursement.QuickBooksBillPaymentSyncToken;
            cashDisbursement.QuickBooksRefundReceiptId = originalCashDisbursement.QuickBooksRefundReceiptId;
            cashDisbursement.QuickBooksRefundReceiptSyncToken = originalCashDisbursement.QuickBooksRefundReceiptSyncToken;
            cashDisbursement.UpdatedAt = DateTime.UtcNow;

            _context.Entry(cashDisbursement).State = EntityState.Modified;

            var currentCashDisbursement = await _context.CashDisbursements.Include(item => item.Bills).AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);

            using (var transaction = _context.Database.BeginTransaction()) {
                await _context.SaveChangesAsync();

                if (cashDisbursement.SalesOrderId != null) {
                    var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == cashDisbursement.SalesOrderId);
                    if (salesOrder != null) {
                        await salesOrder.UpdateBalance(_context);
                    }
                }
                if (originalCashDisbursement.SalesOrderId != null && originalCashDisbursement.SalesOrderId != cashDisbursement.SalesOrderId) {
                    var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == originalCashDisbursement.SalesOrderId);
                    if (salesOrder != null) {
                        await salesOrder.UpdateBalance(_context);
                    }
                }
                transaction.Commit();
            }

            // JSB - HIDING QB INTEGRATION
            //if (!String.IsNullOrWhiteSpace(cashDisbursement.QuickBooksBillPaymentId) || !String.IsNullOrWhiteSpace(cashDisbursement.QuickBooksRefundReceiptId)) {
            //    var result = await cashDisbursement.SyncWithQuickBooks(_quickBooksConnector, _context);
            //    if (!result.Succeeded)
            //        return StatusCode(StatusCodes.Status500InternalServerError, result);
            //}

            return NoContent();
        }

        // POST: CashDisbursements
        [HttpPost]
        public async Task<IActionResult> PostCashDisbursement([FromBody] CashDisbursement cashDisbursement) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            cashDisbursement.Balance = cashDisbursement.Amount;
            cashDisbursement.CreatedAt = DateTime.UtcNow;
            cashDisbursement.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            _context.CashDisbursements.Add(cashDisbursement);
            await _context.SaveChangesAsync();
            if (cashDisbursement.SalesOrderId != null) {
                var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == cashDisbursement.SalesOrderId);
                if (salesOrder != null) {
                    await salesOrder.UpdateBalance(_context);
                }
            }

            if (cashDisbursement.CashDisbursementReasonOptionId == CashDisbursementReasonOption.Refund) {
                if (cashDisbursement.RmaId == null) {
                    return BadRequest("You need to have an RMA selected");
                }
                // JSB - HIDING QB INTEGRATION
                //var result = await cashDisbursement.SyncWithQuickBooks(_quickBooksConnector, _context);
                //if (!result.Succeeded)
                //    return StatusCode(StatusCodes.Status500InternalServerError, result);
            }

            return CreatedAtAction("GetCashDisbursement", new { id = cashDisbursement.Id }, cashDisbursement);
        }

        // DELETE: CashDisbursements/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCashDisbursement([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var cashDisbursement = await _context.CashDisbursements.SingleOrDefaultAsync(m => m.Id == id);
            if (cashDisbursement == null) {
                return NotFound();
            }

            using (var transaction = _context.Database.BeginTransaction()) {
                _context.CashDisbursements.Remove(cashDisbursement);

                try {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }
                if (cashDisbursement.SalesOrderId != null) {
                    var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == cashDisbursement.SalesOrderId);
                    if (salesOrder != null) {
                        await salesOrder.UpdateBalance(_context);
                    }
                }
                transaction.Commit();
            }

            // JSB - HIDING QB INTEGRATION
            //var result = await cashDisbursement.DeleteFromQuickBooks(_quickBooksConnector, _context);
            //if (!result.Succeeded)
            //    return StatusCode(StatusCodes.Status500InternalServerError, result);

            return Ok(cashDisbursement);
        }

        private bool CashDisbursementExists(int id) {
            return _context.CashDisbursements.Any(e => e.Id == id);
        }
    }
}