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
    [Route("CashDisbursementReasonOptions")]
    public class CashDisbursementReasonOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public CashDisbursementReasonOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CashDisbursementReasonOptions
        [HttpGet]
        public IEnumerable<CashDisbursementReasonOption> GetCashDisbursementReasonOption()
        {
            return _context.CashDisbursementReasonOptions;
        }

        // GET: CashDisbursementReasonOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCashDisbursementReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cashDisbursementReasonOption = await _context.CashDisbursementReasonOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (cashDisbursementReasonOption == null)
            {
                return NotFound();
            }

            return Ok(cashDisbursementReasonOption);
        }

        // PUT: CashDisbursementReasonOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCashDisbursementReasonOption([FromRoute] int id, [FromBody] CashDisbursementReasonOption cashDisbursementReasonOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cashDisbursementReasonOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(cashDisbursementReasonOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CashDisbursementReasonOptionExists(id))
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

        // POST: CashDisbursementReasonOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCashDisbursementReasonOption([FromBody] CashDisbursementReasonOption cashDisbursementReasonOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CashDisbursementReasonOptions.Add(cashDisbursementReasonOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCashDisbursementReasonOption", new { id = cashDisbursementReasonOption.Id }, cashDisbursementReasonOption);
        }

        // DELETE: CashDisbursementReasonOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCashDisbursementReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cashDisbursementReasonOption = await _context.CashDisbursementReasonOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (cashDisbursementReasonOption == null)
            {
                return NotFound();
            }

            _context.CashDisbursementReasonOptions.Remove(cashDisbursementReasonOption);
            await _context.SaveChangesAsync();

            return Ok(cashDisbursementReasonOption);
        }

        private bool CashDisbursementReasonOptionExists(int id)
        {
            return _context.CashDisbursementReasonOptions.Any(e => e.Id == id);
        }
    }
}