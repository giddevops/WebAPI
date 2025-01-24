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
    [Route("PurchaseOrderStatusOptions")]
    public class PurchaseOrderStatusOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseOrderStatusOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrderStatusOptions
        [HttpGet]
        public IEnumerable<PurchaseOrderStatusOption> GetPurchaseOrderStatusOption()
        {
            return _context.PurchaseOrderStatusOptions.OrderBy(item => item.Value);
        }

        // GET: PurchaseOrderStatusOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrderStatusOption = await _context.PurchaseOrderStatusOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (purchaseOrderStatusOption == null)
            {
                return NotFound();
            }

            return Ok(purchaseOrderStatusOption);
        }

        // PUT: PurchaseOrderStatusOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderStatusOption([FromRoute] int id, [FromBody] PurchaseOrderStatusOption purchaseOrderStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchaseOrderStatusOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(purchaseOrderStatusOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderStatusOptionExists(id))
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

        // POST: PurchaseOrderStatusOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrderStatusOption([FromBody] PurchaseOrderStatusOption purchaseOrderStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PurchaseOrderStatusOptions.Add(purchaseOrderStatusOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrderStatusOption", new { id = purchaseOrderStatusOption.Id }, purchaseOrderStatusOption);
        }

        // DELETE: PurchaseOrderStatusOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrderStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrderStatusOption = await _context.PurchaseOrderStatusOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (purchaseOrderStatusOption == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderStatusOptions.Remove(purchaseOrderStatusOption);
            await _context.SaveChangesAsync();

            return Ok(purchaseOrderStatusOption);
        }

        private bool PurchaseOrderStatusOptionExists(int id)
        {
            return _context.PurchaseOrderStatusOptions.Any(e => e.Id == id);
        }
    }
}