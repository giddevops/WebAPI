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
    [Route("ShippingCarrierShippingMethods")]
    public class ShippingCarrierShippingMethodsController : Controller
    {
        private readonly AppDBContext _context;

        public ShippingCarrierShippingMethodsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ShippingCarrierShippingMethods
        [HttpGet]
        public IEnumerable<ShippingCarrierShippingMethod> GetShippingCarrierShippingMethods([FromQuery] int? shippingCarrierId)
        {
            var query = from shippingCarrierShippingMethod in _context.ShippingCarrierShippingMethods select shippingCarrierShippingMethod;

            if (shippingCarrierId != null)
            {
                query = query.Where(scsm => scsm.ShippingCarrierId == shippingCarrierId);
            }

            return query;
        }

        // GET: ShippingCarriers
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetShippingCarriers([FromQuery] int? shippingCarrierId)
        {
            var query = from shippingCarrierShippingMethod in _context.ShippingCarrierShippingMethods select shippingCarrierShippingMethod;

            if (shippingCarrierId != null)
            {
                query = query.Where(scsm => scsm.ShippingCarrierId == shippingCarrierId);
            }
            return query.Select(sc => new
            {
                Id = sc.Id,
                Value = sc.Name
            });
        }

        // GET: ShippingCarrierShippingMethods/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetShippingCarrierShippingMethod([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shippingCarrierShippingMethod = await _context.ShippingCarrierShippingMethods.SingleOrDefaultAsync(m => m.Id == id);

            if (shippingCarrierShippingMethod == null)
            {
                return NotFound();
            }

            return Ok(shippingCarrierShippingMethod);
        }

        // PUT: ShippingCarrierShippingMethods/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutShippingCarrierShippingMethod([FromRoute] int? id, [FromBody] ShippingCarrierShippingMethod shippingCarrierShippingMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != shippingCarrierShippingMethod.Id)
            {
                return BadRequest();
            }

            _context.Entry(shippingCarrierShippingMethod).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShippingCarrierShippingMethodExists(id))
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

        // POST: ShippingCarrierShippingMethods
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostShippingCarrierShippingMethod([FromBody] ShippingCarrierShippingMethod shippingCarrierShippingMethod)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ShippingCarrierShippingMethods.Add(shippingCarrierShippingMethod);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShippingCarrierShippingMethod", new { id = shippingCarrierShippingMethod.Id }, shippingCarrierShippingMethod);
        }

        // DELETE: ShippingCarrierShippingMethods/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteShippingCarrierShippingMethod([FromRoute] int? id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var shippingCarrierShippingMethod = await _context.ShippingCarrierShippingMethods.SingleOrDefaultAsync(m => m.Id == id);
            if (shippingCarrierShippingMethod == null)
            {
                return NotFound();
            }

            _context.ShippingCarrierShippingMethods.Remove(shippingCarrierShippingMethod);
            await _context.SaveChangesAsync();

            return Ok(shippingCarrierShippingMethod);
        }

        private bool ShippingCarrierShippingMethodExists(int? id)
        {
            return _context.ShippingCarrierShippingMethods.Any(e => e.Id == id);
        }
    }
}