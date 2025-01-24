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
    [Route("CashReceipts")]
    public class CashReceiptsController : Controller {
        private readonly AppDBContext _context;
        private QuickBooksConnector _quickBooksConnector;

        public CashReceiptsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: CashReceipts
        [HttpGet]
        public ListResult GetCashReceipts(
            [FromQuery] int? companyId,
            [FromQuery] bool hasBalance,
            [FromQuery] int? currencyOptionId,
            [FromQuery] int? salesOrderId,
            [FromQuery] int? ignoreRelatedToInvoiceId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from cashReceipt in _context.CashReceipts select cashReceipt;

            if (companyId != null)
                query = query.Where(item => item.CompanyId == companyId);
            if (hasBalance == true)
                query = query.Where(item => item.Balance > 0);
            if (currencyOptionId != null)
                query = query.Where(item => item.CurrencyOptionId == currencyOptionId);
            if (salesOrderId != null)
                query = query.Where(item => item.Invoices.Any(invoice => invoice.Invoice.SalesOrderId == salesOrderId) || item.SalesOrderId == salesOrderId);
            if (ignoreRelatedToInvoiceId != null)
                query = query.Where(item => !item.Invoices.Any(invoiceCashReceipt => invoiceCashReceipt.InvoiceId == ignoreRelatedToInvoiceId));

            query = query
                // .Include(item => item.Invoices)
                //     .ThenInclude(item => item.Invoice)
                .OrderByDescending(item => item.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: CashReceipts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCashReceipt([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var cashReceipt = await _context.CashReceipts
                .Include(item => item.Invoices)
                .ThenInclude(item => item.Invoice).SingleOrDefaultAsync(m => m.Id == id);

            if (cashReceipt == null) {
                return NotFound();
            }

            return Ok(cashReceipt);
        }

        // GET: CashReceipts/5
        [HttpGet("{id}/CheckIfShouldBeSyncedToQuickBooks")]
        public async Task<IActionResult> CheckIfShouldBeSyncedToQuickBooks([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var cashReceipt = await _context.CashReceipts
                .Include(item => item.Invoices)
                .ThenInclude(item => item.Invoice).SingleOrDefaultAsync(m => m.Id == id);

            if (cashReceipt == null) {
                return NotFound();
            }

            if (cashReceipt.Invoices.Select(item => item.Invoice.SentAt).Where(item => item.HasValue).Count() == 0)
                return Ok(true);

            return Ok(false);
        }

        [HttpGet("{id}/GetInvoiceCashReceipts")]
        public async Task<ListResult> GetInvoiceCashReceipts(
            [FromRoute] int id,
            [FromQuery] int? salesOrderId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from invoiceCashReceipt in _context.InvoiceCashReceipts select invoiceCashReceipt;
            if (salesOrderId != null) {
                query = query.Where(item => item.Invoice.SalesOrderId == salesOrderId);
            }
            query = query
                .Where(item => item.CashReceiptId == id)
                .OrderByDescending(item => item.CreatedAt);

            return new ListResult {
                Items = await query.Skip(skip).Take(perPage).ToListAsync(),
                Count = query.Count()
            };
        }

        // 
        [HttpGet("{id}/ResyncWithQuickBooks")]
        public async Task<IActionResult> ResyncWithQuickBooks([FromRoute] int id) {
            var cashReceipt = await _context.CashReceipts
                .Include(item => item.Invoices)
                    .ThenInclude(item => item.Invoice)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (cashReceipt == null) {
                return NotFound("An cash receipt with that Id doesn't exist");
            }
            if (cashReceipt.Invoices.Count == 0) {
                return BadRequest("This cash receipt cannot be synced until it has at least one invoice that has been synced");
            }
            if (cashReceipt.Invoices.Select(item => item.Invoice.SentAt).Where(item => item.HasValue).Count() == 0) {
                return BadRequest("This cash receipt is not related to any invoices that have been sent (or marked as sent). Cash receipts and don't get copied to quickbooks until the invoice is sent or marked as sent");
            }

            // JSB - HIDING QB INTEGRATION
            //var result = await cashReceipt.SyncWithQuickBooks(_quickBooksConnector, _context, true);
            //if (result.Succeeded)
            //    return Ok(cashReceipt);
            //return StatusCode(StatusCodes.Status500InternalServerError, result);
            return StatusCode(StatusCodes.Status200OK);
        }

        // PUT: CashReceipts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCashReceipt([FromRoute] int id, [FromBody] CashReceipt cashReceipt) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != cashReceipt.Id) {
                return BadRequest();
            }

            var originalCashReceipt = await _context.CashReceipts
                .AsNoTracking()
                .Include(item => item.Invoices)
                .FirstOrDefaultAsync(item => item.Id == id);


            bool canChangeQuickBooksId = false;
            if (GidIndustrial.Gideon.WebApi.Models.User.HasPermission(_context, User, "ManageBilling")) {
                canChangeQuickBooksId = true;
            }
            if (String.IsNullOrWhiteSpace(cashReceipt.QuickBooksId) || !canChangeQuickBooksId) {
                cashReceipt.QuickBooksId = originalCashReceipt.QuickBooksId;
            }
            cashReceipt.QuickBooksSyncToken = originalCashReceipt.QuickBooksSyncToken;

            if (originalCashReceipt == null) {
                return NotFound("A cash receipt with that ID was not found");
            }
            cashReceipt.Invoices = originalCashReceipt.Invoices;
            cashReceipt.Balance = cashReceipt.GetBalance();
            if (cashReceipt.Balance < 0) {
                return BadRequest("Changing the amount to that number would cause the balance on this cash receipt to become negative");
            }

            _context.Entry(cashReceipt).State = EntityState.Modified;


            // Update any associated sales orders
            using (var transaction = _context.Database.BeginTransaction()) {
                await _context.SaveChangesAsync();
                
                if (cashReceipt.SalesOrderId != null) {
                    var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == cashReceipt.SalesOrderId);
                    if (salesOrder != null) {
                        await salesOrder.UpdateBalance(_context);
                    }
                }
                if (originalCashReceipt.SalesOrderId != null && originalCashReceipt.SalesOrderId != cashReceipt.SalesOrderId) {
                    var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == originalCashReceipt.SalesOrderId);
                    if (salesOrder != null) {
                        await salesOrder.UpdateBalance(_context);
                    }
                }
                transaction.Commit();
            }

            // JSB - HIDING QB INTEGRATION
            //var result = await cashReceipt.SyncWithQuickBooks(_quickBooksConnector, _context, true);
            //if (!result.Succeeded)
            //    return StatusCode(StatusCodes.Status500InternalServerError, result);

            return NoContent();
        }

        // POST: CashReceipts
        [HttpPost]
        public async Task<IActionResult> PostCashReceipt([FromBody] CashReceipt cashReceipt) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            cashReceipt.Balance = cashReceipt.Amount;
            cashReceipt.CreatedAt = DateTime.UtcNow;
            cashReceipt.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            if (cashReceipt.Invoices != null) {
                foreach (var invoiceCashReceipt in cashReceipt.Invoices) {
                    invoiceCashReceipt.CreatedAt = DateTime.UtcNow;
                    invoiceCashReceipt.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
                }
            }

            _context.CashReceipts.Add(cashReceipt);

            bool shouldSyncQB = false;
            if (cashReceipt.Invoices != null && cashReceipt.Invoices.Count > 0) {
                var invoice = await _context.Invoices
                    .Include(item => item.LineItems)
                    .Include(item => item.CashReceipts)
                        .ThenInclude(item => item.CashReceipt)
                    .Include(item => item.Credits)
                        .ThenInclude(item => item.Credit)
                    .AsNoTracking().FirstOrDefaultAsync(item => item.Id == cashReceipt.Invoices[0].InvoiceId);

                invoice.Balance = await invoice.GetBalance(_context) - cashReceipt.Invoices[0].Amount;
                _context.Entry(invoice).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                _context.Entry(invoice).State = EntityState.Detached;
                if (invoice.QuickBooksId != null)
                    shouldSyncQB = true;
            } else {
                await _context.SaveChangesAsync();
            }
            _context.Entry(cashReceipt).State = EntityState.Detached;

            //update the balance on the cash receipt
            using (var transaction = _context.Database.BeginTransaction()) {
                await cashReceipt.CalculateBalanceAndSave(_context);

                if (cashReceipt.SalesOrderId != null) {
                    var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == cashReceipt.SalesOrderId);
                    if (salesOrder != null) {
                        await salesOrder.UpdateBalance(_context);
                    }
                }
                transaction.Commit();
            }

            // JSB - HIDING QB INTEGRATION
            //if (shouldSyncQB) {
            //    var result = await cashReceipt.SyncWithQuickBooks(_quickBooksConnector, _context, true);
            //    if (!result.Succeeded)
            //        return StatusCode(StatusCodes.Status500InternalServerError, result);
            //}

            return CreatedAtAction("GetCashReceipt", new { id = cashReceipt.Id }, cashReceipt);
        }

        // DELETE: CashReceipts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCashReceipt([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var cashReceipt = await _context.CashReceipts.SingleOrDefaultAsync(m => m.Id == id);
            if (cashReceipt == null) {
                return NotFound();
            }



            using (var transaction = _context.Database.BeginTransaction()) {
                _context.CashReceipts.Remove(cashReceipt);
                
                try {
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex) {
                    return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
                }

                if (cashReceipt.SalesOrderId != null) {
                    var salesOrder = await _context.SalesOrders.Include(item => item.LineItems).FirstOrDefaultAsync(item => item.Id == cashReceipt.SalesOrderId);
                    if (salesOrder != null) {
                        await salesOrder.UpdateBalance(_context);
                    }
                }
                transaction.Commit();
            }

            // JSB - HIDING QB INTEGRATION
            //var result = await cashReceipt.DeleteFromQuickBooks(_quickBooksConnector, _context);
            //if (!result.Succeeded)
            //    return StatusCode(StatusCodes.Status500InternalServerError, result);

            return Ok(cashReceipt);
        }

        private bool CashReceiptExists(int id) {
            return _context.CashReceipts.Any(e => e.Id == id);
        }
    }
}