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
    [Route("ProductCompositeItemOptions")]
    public class ProductCompositeItemOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public ProductCompositeItemOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ProductCompositeItemOptions
        [HttpGet]
        public IEnumerable<ProductCompositeItemOption> GetProductCompositeItemOptions()
        {
            return _context.ProductCompositeItemOptions;
        }

        // GET: ProductCompositeItemOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductCompositeItemOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCompositeItemOption = await _context.ProductCompositeItemOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (productCompositeItemOption == null)
            {
                return NotFound();
            }

            return Ok(productCompositeItemOption);
        }

        // PUT: ProductCompositeItemOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutProductCompositeItemOption([FromRoute] int id, [FromBody] ProductCompositeItemOption productCompositeItemOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productCompositeItemOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(productCompositeItemOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductCompositeItemOptionExists(id))
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

        // POST: ProductCompositeItemOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostProductCompositeItemOption([FromBody] ProductCompositeItemOption productCompositeItemOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductCompositeItemOptions.Add(productCompositeItemOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductCompositeItemOption", new { id = productCompositeItemOption.Id }, productCompositeItemOption);
        }

        // DELETE: ProductCompositeItemOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteProductCompositeItemOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCompositeItemOption = await _context.ProductCompositeItemOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (productCompositeItemOption == null)
            {
                return NotFound();
            }

            _context.ProductCompositeItemOptions.Remove(productCompositeItemOption);
            await _context.SaveChangesAsync();

            return Ok(productCompositeItemOption);
        }

        private bool ProductCompositeItemOptionExists(int id)
        {
            return _context.ProductCompositeItemOptions.Any(e => e.Id == id);
        }
    }
}