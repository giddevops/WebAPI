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
    [Route("ProductAliasTypes")]
    public class ProductAliasTypesController : Controller
    {
        private readonly AppDBContext _context;

        public ProductAliasTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ProductAliasTypes
        [HttpGet]
        public IEnumerable<ProductAliasType> GetProductAliasTypes()
        {
            return _context.ProductAliasTypes;
        }

        // GET: ProductAliasTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAliasType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productAliasType = await _context.ProductAliasTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (productAliasType == null)
            {
                return NotFound();
            }

            return Ok(productAliasType);
        }

        // PUT: ProductAliasTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutProductAliasType([FromRoute] int id, [FromBody] ProductAliasType productAliasType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productAliasType.Id)
            {
                return BadRequest();
            }

            _context.Entry(productAliasType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductAliasTypeExists(id))
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

        // POST: ProductAliasTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostProductAliasType([FromBody] ProductAliasType productAliasType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ProductAliasTypes.Add(productAliasType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductAliasType", new { id = productAliasType.Id }, productAliasType);
        }

        // DELETE: ProductAliasTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteProductAliasType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productAliasType = await _context.ProductAliasTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (productAliasType == null)
            {
                return NotFound();
            }

            _context.ProductAliasTypes.Remove(productAliasType);
            await _context.SaveChangesAsync();

            return Ok(productAliasType);
        }

        private bool ProductAliasTypeExists(int id)
        {
            return _context.ProductAliasTypes.Any(e => e.Id == id);
        }
    }
}