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
    [Route("CurrencyOptions")]
    public class CurrencyOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public CurrencyOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CurrencyOptions
        [HttpGet]
        public IEnumerable<CurrencyOption> GetCurrencyOption()
        {
            return _context.CurrencyOptions;
        }

        // GET: CurrencyOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCurrencyOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingLineItemWarrantyOption = await _context.CurrencyOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (outgoingLineItemWarrantyOption == null)
            {
                return NotFound();
            }

            return Ok(outgoingLineItemWarrantyOption);
        }

        // PUT: CurrencyOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutCurrencyOption([FromRoute] int id, [FromBody] CurrencyOption outgoingLineItemWarrantyOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != outgoingLineItemWarrantyOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(outgoingLineItemWarrantyOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CurrencyOptionExists(id))
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

        // POST: CurrencyOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostCurrencyOption([FromBody] CurrencyOption outgoingLineItemWarrantyOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            outgoingLineItemWarrantyOption.CreatedAt = DateTime.UtcNow;
            _context.CurrencyOptions.Add(outgoingLineItemWarrantyOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCurrencyOption", new { id = outgoingLineItemWarrantyOption.Id }, outgoingLineItemWarrantyOption);
        }

        // DELETE: CurrencyOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteCurrencyOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var outgoingLineItemWarrantyOption = await _context.CurrencyOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (outgoingLineItemWarrantyOption == null)
            {
                return NotFound();
            }

            _context.CurrencyOptions.Remove(outgoingLineItemWarrantyOption);
            await _context.SaveChangesAsync();

            return Ok(outgoingLineItemWarrantyOption);
        }

        private bool CurrencyOptionExists(int id)
        {
            return _context.CurrencyOptions.Any(e => e.Id == id);
        }
    }
}