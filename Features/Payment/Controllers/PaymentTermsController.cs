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
    [Route("PaymentTerms")]
    public class PaymentTermsController : Controller {
        private readonly AppDBContext _context;
        private readonly QuickBooksConnector _quickBooksConnector;

        public PaymentTermsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: PaymentTerms
        [HttpGet]
        public IEnumerable<PaymentTerm> GetPaymentTerms() {
            return _context.PaymentTerms;
        }

        // GET: PaymentTerms
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetPaymentTermsSelectOptions() {
            return _context.PaymentTerms.Where(item => item.Active).OrderBy(item => item.Name).Select(item => new {
                Id = item.Id,
                Value = item.Name
            });
        }

        // GET: PaymentTerms/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPaymentTerm([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var PaymentTerm = await _context.PaymentTerms.SingleOrDefaultAsync(m => m.Id == id);

            if (PaymentTerm == null) {
                return NotFound();
            }

            return Ok(PaymentTerm);
        }

        // PUT: PaymentTerms/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPaymentTerm([FromRoute] int id, [FromBody] PaymentTerm PaymentTerm) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != PaymentTerm.Id) {
                return BadRequest();
            }

            var oldItem = await _context.PaymentTerms.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (oldItem == null) {
                return BadRequest("Item not found with that Id");
            }
            PaymentTerm.QuickBooksId = oldItem.QuickBooksId;
            PaymentTerm.QuickBooksSyncToken = oldItem.QuickBooksSyncToken;

            _context.Entry(PaymentTerm).State = EntityState.Modified;

            using (var transaction = _context.Database.BeginTransaction()) {
                // await _context.SaveChangesAsync();
                //commented out because it was causing sync problems. qb would complain it couldn't deactivate a term because it was the default for a company,
                // var result = await PaymentTerm.SyncWithQuickBooks(_quickBooksConnector, _context);
                // if (!result.Succeeded)
                //     return StatusCode(StatusCodes.Status500InternalServerError, result);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }


            return NoContent();
        }

        // POST: PaymentTerms
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostPaymentTerm([FromBody] PaymentTerm PaymentTerm) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            using (var transaction = _context.Database.BeginTransaction()) {
                _context.PaymentTerms.Add(PaymentTerm);
                await _context.SaveChangesAsync();
                await PaymentTerm.SyncWithQuickBooks(_quickBooksConnector, _context);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            return CreatedAtAction("GetPaymentTerm", new { id = PaymentTerm.Id }, PaymentTerm);
        }

        // // DELETE: PaymentTerms/5
        // [RequirePermission("EditDropdownOptions")]
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeletePaymentTerm([FromRoute] int id) {
        //     if (!ModelState.IsValid) {
        //         return BadRequest(ModelState);
        //     }

        //     var PaymentTerm = await _context.PaymentTerms.SingleOrDefaultAsync(m => m.Id == id);
        //     if (PaymentTerm == null) {
        //         return NotFound();
        //     }
        //     await PaymentTerm.SyncWithQuickBooks(_quickBooksConnector, _context);


        //     _context.PaymentTerms.Remove(PaymentTerm);
        //     await _context.SaveChangesAsync();

        //     return Ok(PaymentTerm);
        // }

        private bool PaymentTermExists(int id) {
            return _context.PaymentTerms.Any(e => e.Id == id);
        }
    }
}