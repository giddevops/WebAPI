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
    [RequirePermission("ManageBilling")]
    [Route("BankAccounts")]
    public class BankAccountsController : Controller
    {
        private readonly AppDBContext _context;

        public BankAccountsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: BankAccounts
        [HttpGet]
        public ListResult GetBankAccounts(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10
        ){
            var query = from bankAccount in _context.BankAccounts select bankAccount;

            query = query
                .OrderByDescending(item => item.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: BankAccounts/SelectOptions
        [HttpGet("SelectOptions")]
        public async Task<IActionResult> GetBankAccountSelectOptions()
        {
            var bankAccounts = await _context.BankAccounts.OrderBy(item => item.Name).Select(item => new {
                Id = item.Id,
                Value = item.Name
            }).ToListAsync();

            return Ok(bankAccounts);
        }

        [HttpGet("DefaultBankAccountForCCTransaction")]
        public async Task<IActionResult> GetDefaultBankAccountForCCTransaction(){
            return (Ok(await _context.BankAccounts.FirstOrDefaultAsync(item => item.IsDefaultCCAccount == true)));
        }

        // GET: BankAccounts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBankAccount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bankAccount = await _context.BankAccounts.SingleOrDefaultAsync(m => m.Id == id);

            if (bankAccount == null)
            {
                return NotFound();
            }

            return Ok(bankAccount);
        }

        // PUT: BankAccounts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBankAccount([FromRoute] int id, [FromBody] BankAccount bankAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != bankAccount.Id)
            {
                return BadRequest();
            }

            _context.Entry(bankAccount).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BankAccountExists(id))
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

        // POST: BankAccounts
        [HttpPost]
        public async Task<IActionResult> PostBankAccount([FromBody] BankAccount bankAccount)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(String.IsNullOrWhiteSpace(bankAccount.QuickBooksId)){
                // return BadRequest("Bank Account ca")
            }
            bankAccount.CreatedAt = DateTime.UtcNow;
            _context.BankAccounts.Add(bankAccount);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBankAccount", new { id = bankAccount.Id }, bankAccount);
        }

        // DELETE: BankAccounts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBankAccount([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var bankAccount = await _context.BankAccounts.SingleOrDefaultAsync(m => m.Id == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();

            return Ok(bankAccount);
        }

        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.Id == id);
        }
    }
}