using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("ProductTypeAliases")]
    public class ProductTypeAliasesController : Controller {
        private readonly AppDBContext _context;

        public ProductTypeAliasesController(AppDBContext context) {
            _context = context;
        }

        // GET: ProductTypeAliases
        [HttpGet]
        public IActionResult GetProductTypeAliases([FromQuery] int? productTypeId) {
            if (productTypeId == null) {
                return BadRequest("productTypeId querystring parameter must be specified");
            }
            return Ok(_context.ProductTypeAliases.Where(item => item.ProductTypeId == productTypeId).OrderBy(item => item.Alias));
        }

        // GET: ProductTypeAliases/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductTypeAlias([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productTypeAlias = await _context.ProductTypeAliases.SingleOrDefaultAsync(m => m.Id == id);

            if (productTypeAlias == null) {
                return NotFound();
            }

            return Ok(productTypeAlias);
        }

        
        // POST: ProductTypeAliases
        [HttpPost]
        public async Task<IActionResult> PostProductTypeAlias([FromBody] ProductTypeAlias productTypeAlias) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            productTypeAlias.CreatedAt = DateTime.UtcNow;
            if (productTypeAlias.Alias != null){
                productTypeAlias.Alias = productTypeAlias.Alias.Trim();
            }

            var productType = await _context.ProductTypes.FirstOrDefaultAsync(item => item.Id == productTypeAlias.ProductTypeId);
            if(productType == null){
                return BadRequest("Error - no product found with id " + productTypeAlias.ProductTypeId.ToString());
            }
            
            //check for duplicates
            var matchingProductType = await _context.ProductTypes.FirstOrDefaultAsync(item => item.Value == productTypeAlias.Alias);
            if (matchingProductType != null) {
                return BadRequest(new {
                    ErrorCode = "MATCHING_PRODUCT_FOUND",
                    MatchingProductId = matchingProductType.Id
                });
            }
            var matchingProductTypeAlias = await _context.ProductTypeAliases.FirstOrDefaultAsync(item => item.Alias == productTypeAlias.Alias);
            if (matchingProductTypeAlias != null) {
                return BadRequest(new {
                    ErrorCode = "MATCHING_PRODUCT_ALIAS_FOUND",
                    MatchingProductTypeAliasId = matchingProductTypeAlias.Id,
                    MatchingProductId = matchingProductTypeAlias.ProductTypeId
                });
            }

            _context.ProductTypeAliases.Add(productTypeAlias);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductTypeAlias", new { id = productTypeAlias.Id }, productTypeAlias);
        }


        // PUT: ProductTypeAliases/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutProductTypeAlias([FromRoute] int id, [FromBody] ProductTypeAlias productTypeAlias) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != productTypeAlias.Id) {
                return BadRequest();
            }

            _context.Entry(productTypeAlias).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ProductTypeAliasExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: ProductTypeAliases/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteProductTypeAlias([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productTypeAlias = await _context.ProductTypeAliases.SingleOrDefaultAsync(m => m.Id == id);
            if (productTypeAlias == null) {
                return NotFound();
            }

            _context.ProductTypeAliases.Remove(productTypeAlias);
            await _context.SaveChangesAsync();

            return Ok(productTypeAlias);
        }

        private bool ProductTypeAliasExists(int id) {
            return _context.ProductTypeAliases.Any(e => e.Id == id);
        }
    }
}