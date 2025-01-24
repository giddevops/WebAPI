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
    [Route("OutgoingShipmentShippingTermOptions")]
    public class OutgoingShipmentShippingTermOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public OutgoingShipmentShippingTermOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: OutgoingShipmentShippingTermOptions
        [HttpGet]
        public IEnumerable<OutgoingShipmentShippingTermOption> GetOutgoingShipmentShippingTermOptions()
        {
            return _context.OutgoingShipmentShippingTermOptions;
        }

        // GET: OutgoingShipmentShippingTermOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutgoingShipmentShippingTermOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEndOfLifeOption = await _context.OutgoingShipmentShippingTermOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (productEndOfLifeOption == null)
            {
                return NotFound();
            }

            return Ok(productEndOfLifeOption);
        }

        // PUT: OutgoingShipmentShippingTermOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutOutgoingShipmentShippingTermOption([FromRoute] int id, [FromBody] OutgoingShipmentShippingTermOption productEndOfLifeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productEndOfLifeOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(productEndOfLifeOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OutgoingShipmentShippingTermOptionExists(id))
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

        // POST: OutgoingShipmentShippingTermOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostOutgoingShipmentShippingTermOption([FromBody] OutgoingShipmentShippingTermOption productEndOfLifeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OutgoingShipmentShippingTermOptions.Add(productEndOfLifeOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOutgoingShipmentShippingTermOption", new { id = productEndOfLifeOption.Id }, productEndOfLifeOption);
        }

        // DELETE: OutgoingShipmentShippingTermOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteOutgoingShipmentShippingTermOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEndOfLifeOption = await _context.OutgoingShipmentShippingTermOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (productEndOfLifeOption == null)
            {
                return NotFound();
            }

            _context.OutgoingShipmentShippingTermOptions.Remove(productEndOfLifeOption);
            await _context.SaveChangesAsync();

            return Ok(productEndOfLifeOption);
        }

        private bool OutgoingShipmentShippingTermOptionExists(int id)
        {
            return _context.OutgoingShipmentShippingTermOptions.Any(e => e.Id == id);
        }
    }
}