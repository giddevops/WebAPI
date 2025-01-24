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
    [Route("ProductConditionOptions")]
    public class ProductConditionOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public ProductConditionOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ProductConditionOptions
        [HttpGet]
        public IEnumerable<ProductConditionOption> GetProductConditionOptions()
        {
            return _context.ProductConditionOptions;
        }

        // GET: ProductConditionOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductConditionOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productConditionOption = await _context.ProductConditionOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (productConditionOption == null)
            {
                return NotFound();
            }

            return Ok(productConditionOption);
        }

        // PUT: ProductConditionOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutProductConditionOption([FromRoute] int id, [FromBody] ProductConditionOption productConditionOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productConditionOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(productConditionOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductConditionOptionExists(id))
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

        // POST: ProductConditionOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostProductConditionOption([FromBody] ProductConditionOption productConditionOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductConditionOptions.Add(productConditionOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductConditionOption", new { id = productConditionOption.Id }, productConditionOption);
        }

        // DELETE: ProductConditionOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteProductConditionOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productConditionOption = await _context.ProductConditionOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (productConditionOption == null)
            {
                return NotFound();
            }

            _context.ProductConditionOptions.Remove(productConditionOption);
            await _context.SaveChangesAsync();

            return Ok(productConditionOption);
        }

        private bool ProductConditionOptionExists(int id)
        {
            return _context.ProductConditionOptions.Any(e => e.Id == id);
        }
    }
}