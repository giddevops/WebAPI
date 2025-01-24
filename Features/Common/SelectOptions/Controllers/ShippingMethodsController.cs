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
    [Route("ShippingMethods")]
    public class ShippingMethodsController : Controller
    {
        private readonly AppDBContext _context;

        public ShippingMethodsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ShippingMethods
        [HttpGet]
        public IEnumerable<ShippingMethod> GetShippingMethod()
        {
            return _context.ShippingMethods.OrderBy(item => item.Value);
        }

        // GET: ShippingMethods/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShippingMethod([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shippingMethod = await _context.ShippingMethods.SingleOrDefaultAsync(m => m.Id == id);

            if (shippingMethod == null)
            {
                return NotFound();
            }

            return Ok(shippingMethod);
        }

        // PUT: ShippingMethods/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutShippingMethod([FromRoute] int id, [FromBody] ShippingMethod shippingMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shippingMethod.Id)
            {
                return BadRequest();
            }

            _context.Entry(shippingMethod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingMethodExists(id))
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

        // POST: ShippingMethods
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostShippingMethod([FromBody] ShippingMethod shippingMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ShippingMethods.Add(shippingMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShippingMethod", new { id = shippingMethod.Id }, shippingMethod);
        }

        // DELETE: ShippingMethods/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteShippingMethod([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shippingMethod = await _context.ShippingMethods.SingleOrDefaultAsync(m => m.Id == id);
            if (shippingMethod == null)
            {
                return NotFound();
            }

            _context.ShippingMethods.Remove(shippingMethod);
            await _context.SaveChangesAsync();

            return Ok(shippingMethod);
        }

        private bool ShippingMethodExists(int id)
        {
            return _context.ShippingMethods.Any(e => e.Id == id);
        }
    }
}