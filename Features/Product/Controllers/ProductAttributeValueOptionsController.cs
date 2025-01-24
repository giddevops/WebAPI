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
    [Route("ProductAttributeValueOptions")]
    public class ProductAttributeValueOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public ProductAttributeValueOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ProductAttributeValueOptions
        [HttpGet]
        public IEnumerable<ProductAttributeValueOption> GetProductAttributeValueOptions([FromQuery] int? productAttributeId)
        {
            return _context.ProductAttributeValueOptions
                .Where(item => item.ProductAttributeId == productAttributeId)
                .OrderBy(item => item.Value);
        }

        // GET: ProductAttributeValueOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAttributeValueOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCompositeItemOption = await _context.ProductAttributeValueOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (productCompositeItemOption == null)
            {
                return NotFound();
            }

            return Ok(productCompositeItemOption);
        }

        // PUT: ProductAttributeValueOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutProductAttributeValueOption([FromRoute] int id, [FromBody] ProductAttributeValueOption productCompositeItemOption)
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
                if (!ProductAttributeValueOptionExists(id))
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

        // POST: ProductAttributeValueOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostProductAttributeValueOption([FromBody] ProductAttributeValueOption productCompositeItemOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductAttributeValueOptions.Add(productCompositeItemOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductAttributeValueOption", new { id = productCompositeItemOption.Id }, productCompositeItemOption);
        }

        // DELETE: ProductAttributeValueOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteProductAttributeValueOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productCompositeItemOption = await _context.ProductAttributeValueOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (productCompositeItemOption == null)
            {
                return NotFound();
            }

            _context.ProductAttributeValueOptions.Remove(productCompositeItemOption);
            await _context.SaveChangesAsync();

            return Ok(productCompositeItemOption);
        }

        private bool ProductAttributeValueOptionExists(int id)
        {
            return _context.ProductAttributeValueOptions.Any(e => e.Id == id);
        }
    }
}