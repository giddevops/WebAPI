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
    [Route("InventoryItemConditionOptions")]
    public class InventoryItemConditionOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public InventoryItemConditionOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: InventoryItemConditionOptions
        [HttpGet]
        public IEnumerable<InventoryItemConditionOption> GetInventoryItemConditionOption()
        {
            return _context.InventoryItemConditionOptions;
        }

        // GET: InventoryItemConditionOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetInventoryItemConditionOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryItemConditionOption = await _context.InventoryItemConditionOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (inventoryItemConditionOption == null)
            {
                return NotFound();
            }

            return Ok(inventoryItemConditionOption);
        }

        // PUT: InventoryItemConditionOptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInventoryItemConditionOption([FromRoute] int id, [FromBody] InventoryItemConditionOption inventoryItemConditionOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != inventoryItemConditionOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(inventoryItemConditionOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryItemConditionOptionExists(id))
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

        // POST: InventoryItemConditionOptions
        [HttpPost]
        public async Task<IActionResult> PostInventoryItemConditionOption([FromBody] InventoryItemConditionOption inventoryItemConditionOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.InventoryItemConditionOptions.Add(inventoryItemConditionOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInventoryItemConditionOption", new { id = inventoryItemConditionOption.Id }, inventoryItemConditionOption);
        }

        // DELETE: InventoryItemConditionOptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInventoryItemConditionOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var inventoryItemConditionOption = await _context.InventoryItemConditionOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (inventoryItemConditionOption == null)
            {
                return NotFound();
            }

            _context.InventoryItemConditionOptions.Remove(inventoryItemConditionOption);
            await _context.SaveChangesAsync();

            return Ok(inventoryItemConditionOption);
        }

        private bool InventoryItemConditionOptionExists(int id)
        {
            return _context.InventoryItemConditionOptions.Any(e => e.Id == id);
        }
    }
}