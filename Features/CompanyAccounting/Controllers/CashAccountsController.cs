using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Controllers
{
    [Produces("application/json")]
    [Route("CashAccounts")]
    public class CashAccountsController : Controller
    {
        private readonly AppDBContext _context;

        public CashAccountsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CashAccounts
        [HttpGet]
        public ListResult GetCashAccounts(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ){
            var query = from cashAccount in _context.CashAccounts select cashAccount;

            query = query
                .OrderByDescending(item => item.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: CashAccounts/SelectOptions
        [HttpGet("SelectOptions")]
        public async Task<IActionResult> GetCashAccountSelectOptions()
        {
            var cashAccounts = await _context.CashAccounts.OrderBy(item => item.Name).Select(item => new {
                Id = item.Id,
                Value = item.Name
            }).ToListAsync();

            return Ok(cashAccounts);
        }

        // GET: CashAccounts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCashAccount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cashAccount = await _context.CashAccounts.SingleOrDefaultAsync(m => m.Id == id);

            if (cashAccount == null)
            {
                return NotFound();
            }

            return Ok(cashAccount);
        }

        // PUT: CashAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCashAccount([FromRoute] int id, [FromBody] CashAccount cashAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cashAccount.Id)
            {
                return BadRequest();
            }

            _context.Entry(cashAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CashAccountExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: CashAccounts
        [HttpPost]
        public async Task<IActionResult> PostCashAccount([FromBody] CashAccount cashAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            cashAccount.CreatedAt = DateTime.UtcNow;
            _context.CashAccounts.Add(cashAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCashAccount", new { id = cashAccount.Id }, cashAccount);
        }

        // DELETE: CashAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCashAccount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cashAccount = await _context.CashAccounts.SingleOrDefaultAsync(m => m.Id == id);
            if (cashAccount == null)
            {
                return NotFound();
            }

            _context.CashAccounts.Remove(cashAccount);
            await _context.SaveChangesAsync();

            return Ok(cashAccount);
        }

        private bool CashAccountExists(int id)
        {
            return _context.CashAccounts.Any(e => e.Id == id);
        }
    }
}