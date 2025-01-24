using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Inventory.Controllers
{
    [Produces("application/json")]
    [Route("InventoryItemStatusOptions")]
    public class InventoryItemStatusOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public InventoryItemStatusOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: InventoryItemStatusOptions
        [HttpGet]
        public IEnumerable<InventoryItemStatusOption> GetInventoryItemStatusOption()
        {
            return _context.InventoryItemStatusOptions;
        }

        // GET: InventoryItemStatusOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryItemStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryItemStatusOption = await _context.InventoryItemStatusOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (inventoryItemStatusOption == null)
            {
                return NotFound();
            }

            return Ok(inventoryItemStatusOption);
        }

        // PUT: InventoryItemStatusOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryItemStatusOption([FromRoute] int id, [FromBody] InventoryItemStatusOption inventoryItemStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inventoryItemStatusOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(inventoryItemStatusOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryItemStatusOptionExists(id))
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

        // POST: InventoryItemStatusOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostInventoryItemStatusOption([FromBody] InventoryItemStatusOption inventoryItemStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.InventoryItemStatusOptions.Add(inventoryItemStatusOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventoryItemStatusOption", new { id = inventoryItemStatusOption.Id }, inventoryItemStatusOption);
        }

        // DELETE: InventoryItemStatusOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItemStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryItemStatusOption = await _context.InventoryItemStatusOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (inventoryItemStatusOption == null)
            {
                return NotFound();
            }

            _context.InventoryItemStatusOptions.Remove(inventoryItemStatusOption);
            await _context.SaveChangesAsync();

            return Ok(inventoryItemStatusOption);
        }

        private bool InventoryItemStatusOptionExists(int id)
        {
            return _context.InventoryItemStatusOptions.Any(e => e.Id == id);
        }
    }
}