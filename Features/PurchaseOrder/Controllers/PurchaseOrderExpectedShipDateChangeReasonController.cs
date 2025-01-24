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
    [Route("PurchaseOrderExpectedShipDateChangeReasons")]
    public class PurchaseOrderExpectedShipDateChangeReasonController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseOrderExpectedShipDateChangeReasonController(AppDBContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrderExpectedShipDateChangeReasons
        [HttpGet]
        public IEnumerable<PurchaseOrderExpectedShipDateChangeReason> GetPurchaseOrderExpectedShipDateChangeReasons()
        {
            return _context.PurchaseOrderExpectedShipDateChangeReasons.OrderBy(item => item.Value);
        }

        // GET: PurchaseOrderExpectedShipDateChangeReasons/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderExpectedShipDateChangeReason([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrderExpectedShipDateChangeReason = await _context.PurchaseOrderExpectedShipDateChangeReasons.SingleOrDefaultAsync(m => m.Id == id);

            if (purchaseOrderExpectedShipDateChangeReason == null)
            {
                return NotFound();
            }

            return Ok(purchaseOrderExpectedShipDateChangeReason);
        }

        // PUT: PurchaseOrderExpectedShipDateChangeReasons/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderExpectedShipDateChangeReason([FromRoute] int id, [FromBody] PurchaseOrderExpectedShipDateChangeReason purchaseOrderExpectedShipDateChangeReason)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchaseOrderExpectedShipDateChangeReason.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrderExpectedShipDateChangeReason).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderExpectedShipDateChangeReasonExists(id))
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

        // POST: PurchaseOrderExpectedShipDateChangeReasons
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrderExpectedShipDateChangeReason([FromBody] PurchaseOrderExpectedShipDateChangeReason purchaseOrderExpectedShipDateChangeReason)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PurchaseOrderExpectedShipDateChangeReasons.Add(purchaseOrderExpectedShipDateChangeReason);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrderExpectedShipDateChangeReason", new { id = purchaseOrderExpectedShipDateChangeReason.Id }, purchaseOrderExpectedShipDateChangeReason);
        }

        // DELETE: PurchaseOrderExpectedShipDateChangeReasons/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrderExpectedShipDateChangeReason([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrderExpectedShipDateChangeReason = await _context.PurchaseOrderExpectedShipDateChangeReasons.SingleOrDefaultAsync(m => m.Id == id);
            if (purchaseOrderExpectedShipDateChangeReason == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderExpectedShipDateChangeReasons.Remove(purchaseOrderExpectedShipDateChangeReason);
            await _context.SaveChangesAsync();

            return Ok(purchaseOrderExpectedShipDateChangeReason);
        }

        private bool PurchaseOrderExpectedShipDateChangeReasonExists(int id)
        {
            return _context.PurchaseOrderExpectedShipDateChangeReasons.Any(e => e.Id == id);
        }
    }
}