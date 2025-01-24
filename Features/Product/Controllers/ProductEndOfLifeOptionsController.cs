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
    [Route("ProductEndOfLifeOptions")]
    public class ProductEndOfLifeOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public ProductEndOfLifeOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ProductEndOfLifeOptions
        [HttpGet]
        public IEnumerable<ProductEndOfLifeOption> GetProductEndOfLifeOptions()
        {
            return _context.ProductEndOfLifeOptions;
        }

        // GET: ProductEndOfLifeOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductEndOfLifeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEndOfLifeOption = await _context.ProductEndOfLifeOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (productEndOfLifeOption == null)
            {
                return NotFound();
            }

            return Ok(productEndOfLifeOption);
        }

        // PUT: ProductEndOfLifeOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutProductEndOfLifeOption([FromRoute] int id, [FromBody] ProductEndOfLifeOption productEndOfLifeOption)
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
                if (!ProductEndOfLifeOptionExists(id))
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

        // POST: ProductEndOfLifeOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostProductEndOfLifeOption([FromBody] ProductEndOfLifeOption productEndOfLifeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductEndOfLifeOptions.Add(productEndOfLifeOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductEndOfLifeOption", new { id = productEndOfLifeOption.Id }, productEndOfLifeOption);
        }

        // DELETE: ProductEndOfLifeOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteProductEndOfLifeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productEndOfLifeOption = await _context.ProductEndOfLifeOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (productEndOfLifeOption == null)
            {
                return NotFound();
            }

            _context.ProductEndOfLifeOptions.Remove(productEndOfLifeOption);
            await _context.SaveChangesAsync();

            return Ok(productEndOfLifeOption);
        }

        private bool ProductEndOfLifeOptionExists(int id)
        {
            return _context.ProductEndOfLifeOptions.Any(e => e.Id == id);
        }
    }
}