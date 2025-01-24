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
    [Route("SalesOrderPaymentMethods")]
    public class SalesOrderPaymentMethodsController : Controller {
        private readonly AppDBContext _context;
        private readonly QuickBooksConnector _quickBooksConnector;

        public SalesOrderPaymentMethodsController(AppDBContext context, QuickBooksConnector quickBooksConnector) {
            _context = context;
            _quickBooksConnector = quickBooksConnector;
        }

        // GET: SalesOrderPaymentMethods
        [HttpGet]
        public IEnumerable<SalesOrderPaymentMethod> GetSalesOrderPaymentMethods() {
            return _context.SalesOrderPaymentMethods;
        }

        
        // GET: SalesOrderPaymentMethods
        [HttpGet("SelectOptions")]
        public IEnumerable<SalesOrderPaymentMethod> GetSalesOrderPaymentMethodSelectOptions() {
            return _context.SalesOrderPaymentMethods.Where(item => item.Active);
        }

        // GET: SalesOrderPaymentMethods/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderPaymentMethod([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var SalesOrderPaymentMethod = await _context.SalesOrderPaymentMethods.SingleOrDefaultAsync(m => m.Id == id);

            if (SalesOrderPaymentMethod == null) {
                return NotFound();
            }

            return Ok(SalesOrderPaymentMethod);
        }

        // PUT: SalesOrderPaymentMethods/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderPaymentMethod([FromRoute] int id, [FromBody] SalesOrderPaymentMethod SalesOrderPaymentMethod) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != SalesOrderPaymentMethod.Id) {
                return BadRequest();
            }
            var oldItem = await _context.SalesOrderPaymentMethods.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if(oldItem == null){
                return BadRequest("Item not found with that Id");
            }
            SalesOrderPaymentMethod.QuickBooksId = oldItem.QuickBooksId;
            SalesOrderPaymentMethod.QuickBooksSyncToken = oldItem.QuickBooksSyncToken;

            _context.Entry(SalesOrderPaymentMethod).State = EntityState.Modified;

            using (var transaction = _context.Database.BeginTransaction()) {

                await _context.SaveChangesAsync();
                // JSB - HIDING QB INTEGRATION
                //await SalesOrderPaymentMethod.SyncWithQuickBooks(_quickBooksConnector, _context);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }

            return NoContent();
        }

        // POST: SalesOrderPaymentMethods
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderPaymentMethod([FromBody] SalesOrderPaymentMethod SalesOrderPaymentMethod) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            using (var transaction = _context.Database.BeginTransaction()) {
                _context.SalesOrderPaymentMethods.Add(SalesOrderPaymentMethod);
                await _context.SaveChangesAsync();
                // JSB - HIDING QB INTEGRATION
                //await SalesOrderPaymentMethod.SyncWithQuickBooks(_quickBooksConnector, _context);
                await _context.SaveChangesAsync();
                transaction.Commit();
            }
            return CreatedAtAction("GetSalesOrderPaymentMethod", new { id = SalesOrderPaymentMethod.Id }, SalesOrderPaymentMethod);
        }

        // // DELETE: SalesOrderPaymentMethods/5
        // [RequirePermission("EditDropdownOptions")]
        // [HttpDelete("{id}")]
        // public async Task<IActionResult> DeleteSalesOrderPaymentMethod([FromRoute] int id) {
        //     if (!ModelState.IsValid) {
        //         return BadRequest(ModelState);
        //     }

        //     var SalesOrderPaymentMethod = await _context.SalesOrderPaymentMethods.SingleOrDefaultAsync(m => m.Id == id);
        //     if (SalesOrderPaymentMethod == null) {
        //         return NotFound();
        //     }

        //     _context.SalesOrderPaymentMethods.Remove(SalesOrderPaymentMethod);
        //     await _context.SaveChangesAsync();

        //     return Ok(SalesOrderPaymentMethod);
        // }

        private bool SalesOrderPaymentMethodExists(int id) {
            return _context.SalesOrderPaymentMethods.Any(e => e.Id == id);
        }
    }
}