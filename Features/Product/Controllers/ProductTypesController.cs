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
    [Route("ProductTypes")]
    public class ProductTypesController : Controller
    {
        private readonly AppDBContext _context;

        public ProductTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ProductTypes
        [HttpGet]
        public IActionResult GetProductTypes(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string value = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true,
            [FromQuery] bool? onlyPartsWithPrefix = null)
        {
            var query = from productType in _context.ProductTypes select productType;

            switch(sortBy){
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                case "Prefix":
                    query = sortAscending ? query.OrderBy(item => item.Prefix) : query.OrderByDescending(item => item.Prefix);
                    break;
                case "IsPiecePart":
                    query = sortAscending ? query.OrderBy(item => item.IsPiecePart) : query.OrderByDescending(item => item.IsPiecePart);
                    break;
                case "IsSerialized":
                    query = sortAscending ? query.OrderBy(item => item.IsSerialized) : query.OrderByDescending(item => item.IsSerialized);
                    break;
                case "Value":
                    query = sortAscending ? query.OrderBy(item => item.Value) : query.OrderByDescending(item => item.Value);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }

            if(!String.IsNullOrWhiteSpace(value)){
                value = value.Trim();
                query = query.Where(item => item.Value.StartsWith(value));
            }

            if(onlyPartsWithPrefix == true){
                query = query.Where(item => item.Prefix != null);
            }
                        
            return Ok(new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            });
        }

        // GET: Products/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<dynamic> Search([FromQuery] string query) {
            int id = 0;
            Int32.TryParse(query, out id);

            IQueryable<ProductType> search = _context.ProductTypes.Where(item =>
                EF.Functions.Like(item.Value, query + '%') ||
                item.Id == id
            );
            return search;
        }

        [HttpGet("{id}/ConvertToAlias")]
        public async Task<IActionResult> ConvertToAlias([FromRoute] int id, [FromQuery] int parentProductTypeId){
            var productType = await _context.ProductTypes.FirstOrDefaultAsync(item => item.Id == id);
            if(productType == null){
                return BadRequest("Error - product type was not found for the original product type");
            }
            var parentProductType = await _context.ProductTypes.FirstOrDefaultAsync(item => item.Id == parentProductTypeId);
            if(parentProductType == null){
                return BadRequest("Error - the parent product type was not found");
            }

            // first convert all products to be a part of the new category
            var products = await _context.Products.Where(item => item.ProductTypeId == id).ToListAsync();
            foreach(var product in products){
                product.ProductTypeId = parentProductTypeId;
            }

            //remove the old one
            _context.ProductTypes.Remove(productType);

            //now create an alias for the product type
            var newAlias = new ProductTypeAlias{
                ProductTypeId = parentProductTypeId,
                Alias = productType.Value
            };
            _context.ProductTypeAliases.Add(newAlias);

            await _context.SaveChangesAsync();

            return Ok();
        }

        // GET: ProductTypes
        [HttpGet("SelectOptions")]
        public IEnumerable<ProductType> GetProductTypeSelectOptions([FromQuery] bool? hasAttributes, [FromQuery] bool? hasPrefix)
        {
            var query = from productType in _context.ProductTypes select productType;

            if(hasAttributes == true){
                query = query.Where(item => item.Attributes.Count > 0);
            }
            if(hasPrefix == true){
                query = query.Where(item => item.Prefix != null);
            }

            query = query.OrderBy(item => item.Value);
            return query;
        }

        // GET: ProductTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productType = await _context.ProductTypes
                .Include(item => item.Attributes)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (productType == null)
            {
                return NotFound();
            }

            return Ok(productType);
        }

        [HttpGet("{id}/Attributes")]
        public async Task<IActionResult> GetProductTypeAttributes([FromRoute] int id)
        {
            var productType = await _context.ProductTypes
                .Include(item => item.Attributes)
                    .ThenInclude(item => item.ValueOptions)
                .SingleOrDefaultAsync(item => item.Id == id);

            return Ok(productType.Attributes);
        }

        // PUT: ProductTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutProductType([FromRoute] int id, [FromBody] ProductType productType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != productType.Id)
            {
                return BadRequest();
            }

            _context.Entry(productType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductTypeExists(id))
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

        // POST: ProductTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostProductType([FromBody] ProductType productType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            productType.CreatedAt = DateTime.UtcNow;

            _context.ProductTypes.Add(productType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProductType", new { id = productType.Id }, productType);
        }

        // DELETE: ProductTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteProductType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var productType = await _context.ProductTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (productType == null)
            {
                return NotFound();
            }

            _context.ProductTypes.Remove(productType);
            await _context.SaveChangesAsync();

            return Ok(productType);
        }

        private bool ProductTypeExists(int id)
        {
            return _context.ProductTypes.Any(e => e.Id == id);
        }
    }
}