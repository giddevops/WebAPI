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
    [Route("PurchaseOrderReasonOptions")]
    public class PurchaseOrderReasonOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseOrderReasonOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrderReasonOptions
        [HttpGet]
        public IEnumerable<PurchaseOrderReasonOption> GetPurchaseOrderReasonOptions()
        {
            return _context.PurchaseOrderReasonOptions.OrderBy(item => item.Value);
        }

        // GET: PurchaseOrderReasonOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrderReasonOption = await _context.PurchaseOrderReasonOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (purchaseOrderReasonOption == null)
            {
                return NotFound();
            }

            return Ok(purchaseOrderReasonOption);
        }

        // PUT: PurchaseOrderReasonOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderReasonOption([FromRoute] int id, [FromBody] PurchaseOrderReasonOption purchaseOrderReasonOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchaseOrderReasonOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrderReasonOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderReasonOptionExists(id))
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

        // POST: PurchaseOrderReasonOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrderReasonOption([FromBody] PurchaseOrderReasonOption purchaseOrderReasonOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PurchaseOrderReasonOptions.Add(purchaseOrderReasonOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrderReasonOption", new { id = purchaseOrderReasonOption.Id }, purchaseOrderReasonOption);
        }

        // DELETE: PurchaseOrderReasonOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrderReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrderReasonOption = await _context.PurchaseOrderReasonOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (purchaseOrderReasonOption == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderReasonOptions.Remove(purchaseOrderReasonOption);
            await _context.SaveChangesAsync();

            return Ok(purchaseOrderReasonOption);
        }

        private bool PurchaseOrderReasonOptionExists(int id)
        {
            return _context.PurchaseOrderReasonOptions.Any(e => e.Id == id);
        }
    }
}