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
    [Route("Credits")]
    public class CreditsController : Controller {
        private readonly AppDBContext _context;

        public CreditsController(AppDBContext context) {
            _context = context;
        }

        // GET: Credits
        [HttpGet]
        public ListResult GetCredits(
            [FromQuery] int? companyId,
            [FromQuery] bool hasBalance,
            [FromQuery] int? currencyOptionId,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from credit in _context.Credits select credit;

            if (companyId != null)
                query = query.Where(item => item.CompanyId == companyId);
            if (hasBalance == true)
                query = query.Where(item => item.Balance > 0);
            if (currencyOptionId != null)
                query = query.Where(item => item.CurrencyOptionId == currencyOptionId);

            query = query
                .OrderByDescending(item => item.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: Credits/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCredit([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var credit = await _context.Credits.SingleOrDefaultAsync(m => m.Id == id);

            if (credit == null) {
                return NotFound();
            }

            return Ok(credit);
        }

        [HttpGet("{id}/GetInvoiceCredits")]
        public async Task<ListResult> GetInvoiceCredits(
            [FromRoute] int id,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ) {
            var query = from invoiceCredit in _context.InvoiceCredits select invoiceCredit;

            query = query
                .Where(item => item.CreditId == id)
                .OrderByDescending(item => item.CreatedAt);

            return new ListResult {
                Items = await query.Skip(skip).Take(perPage).ToListAsync(),
                Count = query.Count()
            };
        }

        // PUT: Credits/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCredit([FromRoute] int id, [FromBody] Credit credit) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != credit.Id) {
                return BadRequest();
            }

            //make sure balance can only be edited by the program
            var originalCredit = await _context.Credits.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            credit.Balance = originalCredit.Balance;

            _context.Entry(credit).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!CreditExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Credits
        [HttpPost]
        public async Task<IActionResult> PostCredit([FromBody] Credit credit) {
            credit.Balance = credit.Amount;
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            credit.CreatedAt = DateTime.UtcNow;
            credit.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            _context.Credits.Add(credit);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCredit", new { id = credit.Id }, credit);
        }

        // DELETE: Credits/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCredit([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var credit = await _context.Credits.SingleOrDefaultAsync(m => m.Id == id);
            if (credit == null) {
                return NotFound();
            }

            _context.Credits.Remove(credit);
            await _context.SaveChangesAsync();

            return Ok(credit);
        }

        private bool CreditExists(int id) {
            return _context.Credits.Any(e => e.Id == id);
        }
    }
}