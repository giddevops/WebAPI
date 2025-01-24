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
    [Route("CashReceiptTypes")]
    public class CashReceiptTypesController : Controller
    {
        private readonly AppDBContext _context;

        public CashReceiptTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CashReceiptTypes
        [HttpGet]
        public IEnumerable<CashReceiptType> GetCashReceiptType()
        {
            return _context.CashReceiptTypes;
        }

        // GET: CashReceiptTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCashReceiptType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cashReceiptType = await _context.CashReceiptTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (cashReceiptType == null)
            {
                return NotFound();
            }

            return Ok(cashReceiptType);
        }

        // PUT: CashReceiptTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCashReceiptType([FromRoute] int id, [FromBody] CashReceiptType cashReceiptType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cashReceiptType.Id)
            {
                return BadRequest();
            }

            _context.Entry(cashReceiptType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CashReceiptTypeExists(id))
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

        // POST: CashReceiptTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCashReceiptType([FromBody] CashReceiptType cashReceiptType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.CashReceiptTypes.Add(cashReceiptType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCashReceiptType", new { id = cashReceiptType.Id }, cashReceiptType);
        }

        // DELETE: CashReceiptTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCashReceiptType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cashReceiptType = await _context.CashReceiptTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (cashReceiptType == null)
            {
                return NotFound();
            }

            _context.CashReceiptTypes.Remove(cashReceiptType);
            await _context.SaveChangesAsync();

            return Ok(cashReceiptType);
        }

        private bool CashReceiptTypeExists(int id)
        {
            return _context.CashReceiptTypes.Any(e => e.Id == id);
        }
    }
}