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
    [Route("InventoryItemLocationOptions")]
    public class InventoryItemLocationOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public InventoryItemLocationOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: InventoryItemLocationOptions
        [HttpGet]
        public IEnumerable<InventoryItemLocationOption> GetInventoryItemLocationOptions([FromQuery] bool? gidLocationsOnly)
        {
            var query = from option in _context.InventoryItemLocationOptions select option;
            if (gidLocationsOnly == true)
            {
                query = query.Where(item => item.Id != (int)InventoryItemLocationOptions.Receiving && item.Id != (int)InventoryItemLocationOptions.Shipped);
            }
            return query;
        }

        // GET: InventoryItemLocationOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryItemLocationOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryItemLocationOption = await _context.InventoryItemLocationOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (inventoryItemLocationOption == null)
            {
                return NotFound();
            }

            return Ok(inventoryItemLocationOption);
        }

        // PUT: InventoryItemLocationOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryItemLocationOption([FromRoute] int id, [FromBody] InventoryItemLocationOption inventoryItemLocationOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inventoryItemLocationOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(inventoryItemLocationOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryItemLocationOptionExists(id))
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

        // POST: InventoryItemLocationOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostInventoryItemLocationOption([FromBody] InventoryItemLocationOption inventoryItemLocationOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.InventoryItemLocationOptions.Add(inventoryItemLocationOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventoryItemLocationOption", new { id = inventoryItemLocationOption.Id }, inventoryItemLocationOption);
        }

        // DELETE: InventoryItemLocationOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItemLocationOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryItemLocationOption = await _context.InventoryItemLocationOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (inventoryItemLocationOption == null)
            {
                return NotFound();
            }

            _context.InventoryItemLocationOptions.Remove(inventoryItemLocationOption);
            await _context.SaveChangesAsync();

            return Ok(inventoryItemLocationOption);
        }

        private bool InventoryItemLocationOptionExists(int id)
        {
            return _context.InventoryItemLocationOptions.Any(e => e.Id == id);
        }
    }
}