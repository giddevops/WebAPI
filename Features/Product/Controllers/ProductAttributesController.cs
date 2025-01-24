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
    [Route("ProductAttributes")]
    public class ProductAttributesController : Controller {
        private readonly AppDBContext _context;

        public ProductAttributesController(AppDBContext context) {
            _context = context;
        }

        // GET: ProductAttributes
        [HttpGet]
        public IActionResult GetProductAttributes([FromQuery] int? productTypeId, [FromQuery] bool? includeOptions) {
            if (productTypeId == null) {
                return BadRequest("productId querystring parameter must be specified");
            }

            var query = from productAttribute in _context.ProductAttributes select productAttribute;
            
            query = query.Where(item => item.ProductTypeId == productTypeId);
            
            if(includeOptions == true){
                query = query.Include(item => item.ValueOptions);
            }

            query = query.OrderBy(item => item.Name);
            
            return Ok(query);
        }

        // GET: ProductAttributes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductAttribute([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productAttribute = await _context.ProductAttributes.SingleOrDefaultAsync(m => m.Id == id);

            if (productAttribute == null) {
                return NotFound();
            }

            return Ok(productAttribute);
        }

        // PUT: ProductAttributes/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProductAttribute([FromRoute] int id, [FromBody] ProductAttribute productAttribute) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != productAttribute.Id) {
                return BadRequest();
            }

            _context.Entry(productAttribute).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ProductAttributeExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ProductAttributes
        [HttpPost]
        public async Task<IActionResult> PostProductAttribute([FromBody] ProductAttribute productAttribute) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            productAttribute.CreatedAt = DateTime.UtcNow;
           
            _context.ProductAttributes.Add(productAttribute);
            await _context.SaveChangesAsync();

            productAttribute = await _context.ProductAttributes
                .Include(item => item.ValueOptions)
                .FirstOrDefaultAsync(item => item.Id == productAttribute.Id);

            return Ok(productAttribute);
        }

        // DELETE: ProductAttributes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProductAttribute([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var productAttribute = await _context.ProductAttributes.SingleOrDefaultAsync(m => m.Id == id);
            if (productAttribute == null) {
                return NotFound();
            }

            _context.ProductAttributes.Remove(productAttribute);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(productAttribute);
        }

        private bool ProductAttributeExists(int id) {
            return _context.ProductAttributes.Any(e => e.Id == id);
        }
    }
}