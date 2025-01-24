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
    [Route("ProductAliases")]
    public class ProductAliasesController : Controller {
        private readonly AppDBContext _context;

        public ProductAliasesController(AppDBContext context) {
            _context = context;
        }

        // GET: ProductAliases
        [HttpGet]
        public IActionResult GetProductAliases([FromQuery] int? productId) {
            if (productId == null) {
                return BadRequest("productId querystring parameter must be specified");
            }
            var aliases = _context.ProductAliases
                .Include(item => item.ProductAliasType)
                .Where(item => item.ProductId == productId)
                .OrderBy(item => item.PartNumber);
            
            return Ok(aliases);
        }

        // GET: ProductAliases/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAlias([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productAlias = await _context.ProductAliases.SingleOrDefaultAsync(m => m.Id == id);

            if (productAlias == null) {
                return NotFound();
            }

            return Ok(productAlias);
        }

        // PUT: ProductAliases/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductAlias([FromRoute] int id, [FromBody] ProductAlias productAlias) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != productAlias.Id) {
                return BadRequest();
            }

            _context.Entry(productAlias).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ProductAliasExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ProductAliases
        [HttpPost]
        public async Task<IActionResult> PostProductAlias([FromBody] ProductAlias productAlias) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            productAlias.CreatedAt = DateTime.UtcNow;
            if (productAlias.PartNumber != null)
                productAlias.PartNumber = productAlias.PartNumber.Trim();
            if (productAlias.ManufacturerName != null)
                productAlias.ManufacturerName = productAlias.ManufacturerName.Trim();

            if (!String.IsNullOrEmpty(productAlias.ManufacturerName))
                productAlias.ManufacturerName = productAlias.ManufacturerName.Trim();


            var product = await _context.Products.Include(item => item.Manufacturer).FirstOrDefaultAsync(item => item.Id == productAlias.ProductId);
            if(product == null){
                return BadRequest("Error - no product found with id " + productAlias.ProductId.ToString());
            }
            var manufacturerName = productAlias.ManufacturerName;
            if (String.IsNullOrWhiteSpace(manufacturerName))
                manufacturerName = product.Manufacturer != null ? product.Manufacturer.Name : "";

            //check for duplicates
            var matchingProduct = await _context.Products.FirstOrDefaultAsync(item => item.PartNumber == productAlias.PartNumber && item.Manufacturer.Name == manufacturerName);
            if (matchingProduct != null) {
                return BadRequest(new {
                    ErrorCode = "MATCHING_PRODUCT_FOUND",
                    MatchingProductId = matchingProduct.Id
                });
            }
            var matchingProductAlias = await _context.ProductAliases.FirstOrDefaultAsync(item => item.ManufacturerName == manufacturerName && item.PartNumber == productAlias.PartNumber);
            if (matchingProductAlias != null) {
                return BadRequest(new {
                    ErrorCode = "MATCHING_PRODUCT_ALIAS_FOUND",
                    MatchingProductAliasId = matchingProductAlias.Id,
                    MatchingProductId = matchingProductAlias.ProductId
                });
            }

            _context.ProductAliases.Add(productAlias);
            await _context.SaveChangesAsync();

            productAlias = await _context.ProductAliases.Include(item => item.ProductAliasType).FirstOrDefaultAsync(item => item.Id == productAlias.Id);

            return Ok(productAlias);
        }

        // DELETE: ProductAliases/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAlias([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productAlias = await _context.ProductAliases.SingleOrDefaultAsync(m => m.Id == id);
            if (productAlias == null) {
                return NotFound();
            }

            _context.ProductAliases.Remove(productAlias);
            await _context.SaveChangesAsync();

            return Ok(productAlias);
        }

        private bool ProductAliasExists(int id) {
            return _context.ProductAliases.Any(e => e.Id == id);
        }
    }
}