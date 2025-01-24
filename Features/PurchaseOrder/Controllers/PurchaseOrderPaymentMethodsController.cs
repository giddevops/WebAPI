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
    [Route("PurchaseOrderPaymentMethods")]
    public class PurchaseOrderPaymentMethodsController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseOrderPaymentMethodsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrderPaymentMethods
        [HttpGet]
        public IEnumerable<PurchaseOrderPaymentMethod> GetPurchaseOrderPaymentMethods()
        {
            return _context.PurchaseOrderPaymentMethods.Where(item => item.Hidden == null || item.Hidden == false).OrderBy(item => item.Value);
        }


        // GET: PurchaseOrderPaymentMethods/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderPaymentMethod([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var PurchaseOrderPaymentMethod = await _context.PurchaseOrderPaymentMethods.SingleOrDefaultAsync(m => m.Id == id);

            if (PurchaseOrderPaymentMethod == null)
            {
                return NotFound();
            }

            return Ok(PurchaseOrderPaymentMethod);
        }

        // PUT: PurchaseOrderPaymentMethods/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderPaymentMethod([FromRoute] int id, [FromBody] PurchaseOrderPaymentMethod PurchaseOrderPaymentMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != PurchaseOrderPaymentMethod.Id)
            {
                return BadRequest();
            }

            _context.Entry(PurchaseOrderPaymentMethod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderPaymentMethodExists(id))
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

        // POST: PurchaseOrderPaymentMethods
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrderPaymentMethod([FromBody] PurchaseOrderPaymentMethod PurchaseOrderPaymentMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.PurchaseOrderPaymentMethods.Add(PurchaseOrderPaymentMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrderPaymentMethod", new { id = PurchaseOrderPaymentMethod.Id }, PurchaseOrderPaymentMethod);
        }

        // DELETE: PurchaseOrderPaymentMethods/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrderPaymentMethod([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var PurchaseOrderPaymentMethod = await _context.PurchaseOrderPaymentMethods.SingleOrDefaultAsync(m => m.Id == id);
            if (PurchaseOrderPaymentMethod == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderPaymentMethods.Remove(PurchaseOrderPaymentMethod);
            await _context.SaveChangesAsync();

            return Ok(PurchaseOrderPaymentMethod);
        }

        private bool PurchaseOrderPaymentMethodExists(int id)
        {
            return _context.PurchaseOrderPaymentMethods.Any(e => e.Id == id);
        }
    }
}