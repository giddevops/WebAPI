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
    [Route("OutgoingShipmentBoxDimensionOptions")]
    public class OutgoingShipmentBoxDimensionOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public OutgoingShipmentBoxDimensionOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: OutgoingShipmentBoxDimensionOptions
        [HttpGet]
        public IEnumerable<OutgoingShipmentBoxDimensionOption> GetOutgoingShipmentBoxDimensionOptions()
        {
            return _context.OutgoingShipmentBoxDimensionOptions;
        }

        // GET: OutgoingShipmentBoxDimensionOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOutgoingShipmentBoxDimensionOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEndOfLifeOption = await _context.OutgoingShipmentBoxDimensionOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (productEndOfLifeOption == null)
            {
                return NotFound();
            }

            return Ok(productEndOfLifeOption);
        }

        // PUT: OutgoingShipmentBoxDimensionOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutOutgoingShipmentBoxDimensionOption([FromRoute] int id, [FromBody] OutgoingShipmentBoxDimensionOption productEndOfLifeOption)
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
                if (!OutgoingShipmentBoxDimensionOptionExists(id))
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

        // POST: OutgoingShipmentBoxDimensionOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostOutgoingShipmentBoxDimensionOption([FromBody] OutgoingShipmentBoxDimensionOption productEndOfLifeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OutgoingShipmentBoxDimensionOptions.Add(productEndOfLifeOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOutgoingShipmentBoxDimensionOption", new { id = productEndOfLifeOption.Id }, productEndOfLifeOption);
        }

        // DELETE: OutgoingShipmentBoxDimensionOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteOutgoingShipmentBoxDimensionOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEndOfLifeOption = await _context.OutgoingShipmentBoxDimensionOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (productEndOfLifeOption == null)
            {
                return NotFound();
            }

            _context.OutgoingShipmentBoxDimensionOptions.Remove(productEndOfLifeOption);
            await _context.SaveChangesAsync();

            return Ok(productEndOfLifeOption);
        }

        private bool OutgoingShipmentBoxDimensionOptionExists(int id)
        {
            return _context.OutgoingShipmentBoxDimensionOptions.Any(e => e.Id == id);
        }
    }
}