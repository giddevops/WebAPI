using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Data;
using Dapper;
using static System.Net.WebRequestMethods;

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Products")]
    public class ProductsController : Controller {
        private readonly AppDBContext _context;
        private static readonly HttpClient HttpClient;
        // private static Dictionary<int,bool> ListingsContainerFetchesInProgress; //I was considering adding something to prevent fetching ListingsContainers for the same product more than one at a time
        private IConfiguration _configuration;

        static ProductsController() {
            HttpClient = new HttpClient();
        }

        public ProductsController(AppDBContext context, IConfiguration appConfig) {
            _context = context;
            this._configuration = appConfig;
        }

        // GET: Products
        [HttpGet]
        public async Task<ListResult> GetProducts(
            [FromQuery] Dictionary<int?, int?> attributeValues,
            [FromQuery] int? productTypeId = null,
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string partNumber = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {

            if (!String.IsNullOrWhiteSpace(partNumber))
            {
                searchString = partNumber;
            }
            if (!String.IsNullOrWhiteSpace(searchString))
                searchString = searchString.Trim();


            var attributeSearchText = "";
            if (attributeValues != null)
            {
                var hasValues = false;
                var values = new List<int?> { };
                var i = 0;
                foreach (var entry in attributeValues)
                {
                    if (entry.Value != null)
                    {
                        values.Add(entry.Value);
                        var table = "PAV_" + i;
                        attributeSearchText += $@"
    JOIN ProductAttributeValue {table} ON ({table}.ProductId = Product.Id
        AND {table}.ProductAttributeValueOptionId = " + entry.Value + @")";
                        ++i;
                    }
                }
    //            if(values.Count > 0)
    //                attributeSearchText = @"
    //JOIN ProductAttributeValue ON ProductAttributeValue.ProductId = Product.Id
    //    AND ProductAttributeValue.ProductAttributeValueOptionId IN (" + String.Join(",", values) + ")";
            }

            var CommandText = $@"";
            var descriptionSearch = "";
            var hasCondition = false;
            if (!String.IsNullOrWhiteSpace(searchString))
            {
                searchString = searchString.Replace("-", "").Replace(" ", "");
                CommandText += @"WHERE Product.Id IN (
    SELECT [KEY] FROM 
    FREETEXTTABLE(Product, (GidPartNumberNoSpecialChars, PartNumberNoSpecialChars, AliasesCache, [Description], ShortDescription), @searchString)
    ORDER BY RANK DESC OFFSET 0 ROWS
)";
                //sortBy = "RANK";
                hasCondition = true;
            } else if (productTypeId != null)
            {
                if (hasCondition)
                    CommandText += " AND ";
                else
                    CommandText += " WHERE ";
                CommandText += "ProductTypeId=@productTypeId ";
            }

            switch (sortBy)
            {
                case "Id":
                    break;
                case "CreatedAt":
                    break;
                case "PartNumber":
                    break;
                case "Type":
                    break;
                case "Alias":
                    break;
                case "GidPartNumber":
                    break;
                case "RANK":
                    break;
                default:
                    //if (descriptionSearch != "")
                    //    sortBy = "Rank";
                    //else
                    sortBy = "CreatedAt";
                    break;
            }


            sortBy = "Product." + sortBy;
            Console.Write(@"IT IS >>> SELECT DISTINCT Product.* FROM Product " + CommandText + @" ORDER BY " + sortBy + " DESC OFFSET @skip ROWS FETCH NEXT @perPage ROWS ONLY;");
            var connection = _context.Database.GetDbConnection();
            var result = await connection.QueryAsync<Product>(@"SELECT DISTINCT Product.* FROM Product " + CommandText + @" ORDER BY " + sortBy + " DESC OFFSET @skip ROWS FETCH NEXT @perPage ROWS ONLY;", new
            {
                partNumber = searchString + "%",
                searchString = searchString,
                skip = skip,
                perPage = perPage,
                productTypeId = productTypeId
            });
            var products = result.ToList();


            var count = -1;
            if (String.IsNullOrWhiteSpace(searchString))
            {
                count = connection.ExecuteScalar<int>(@"SELECT COUNT(*) FROM Product " + CommandText, new
                {
                    partNumber = searchString + "%",
                    searchString = searchString,
                    skip = skip,
                    perPage = perPage,
                    productTypeId = productTypeId
                });
            }

            // Add aliases
            // Create dictionary for maximum effectiveness
            var itemsDict = new Dictionary<int?, Product> { };
            foreach (var item in result)
            {
                item.Aliases = new List<ProductAlias> { };
                itemsDict.Add(item.Id, item);
            }
            if (result.Count() > 0)
            {
                var ids = result.Select(item => item.Id).ToList();
                var aliases = (await connection.QueryAsync<ProductAlias>($@"SELECT * FROM ProductAlias WHERE ProductId IN ({String.Join(',', ids)})")).ToList();
                foreach (var alias in aliases)
                    itemsDict[alias.ProductId].Aliases.Add(alias);
            }


            return new ListResult
            {
                Items = itemsDict.Values.ToList(),
                Count = count
            };
        }


        // GET: Products/Search?query=...
        [HttpGet("GidPartNumber/GetPart")]
        public async Task<IActionResult> SearchPartNumber([FromQuery] string gidPartNumber, [FromQuery] string query, [FromQuery] int? existingId) {
            var dbQuery = from product in _context.Products select product;
            if (!String.IsNullOrWhiteSpace(query))
                dbQuery = dbQuery.Where(item => item.GidPartNumber.StartsWith(query));
            else
                dbQuery = dbQuery.Where(item => item.GidPartNumber == gidPartNumber);
            if (existingId != null) {
                dbQuery = dbQuery.Where(item => item.Id != existingId);
            }
            var prod = await dbQuery.Select(product => new {
                Id = product.Id,
                PartNumber = product.PartNumber,
                GidPartNumber = product.GidPartNumber
            }).FirstOrDefaultAsync();
            return Ok(prod);
        }

        // GET: Products/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<dynamic> Search([FromQuery] string query) {
            IQueryable<Product> search = _context.Products.Where(product =>
            EF.Functions.Like(product.PartNumber, query + '%') ||
            product.Aliases.Any(alias => EF.Functions.Like(alias.PartNumber, query + "%")) ||
            EF.Functions.Like(product.GidPartNumber, query + '%')
        ).Include(p => p.Manufacturer);
            return search.Select(product => new {
                Id = product.Id,
                PartNumber = product.PartNumber,
                GidPartNumber = product.GidPartNumber,
                Name = product.PartNumber,
                Company = product.Manufacturer,
                CompanyName = product.Manufacturer != null ? product.Manufacturer.Name : null
            });
        }

        [HttpGet("{id}/AvailabilityStats")]
        public async Task<IActionResult> GetAvailabilityStats([FromRoute] int id) {
            var product = await _context.Products.AsNoTracking()
                .Where(item => item.Id == id)
                .FirstOrDefaultAsync();

            if (product == null) {
                return NotFound("A product with that Id was not found");
            }

            var numAvailable = await _context.InventoryItems.Where(item => item.ProductId == id && item.InventoryItemStatusOptionId == InventoryItemStatusOption.Available).CountAsync();

            var numCommitted = await _context.InventoryItems.Where(item => item.ProductId == id && item.InventoryItemStatusOptionId == InventoryItemStatusOption.Committed).CountAsync();

            return Ok(new {
                QuantityComitted = numCommitted,
                QuantityAvailable = numAvailable
            });
        }

        // GET: Products/GetPartNumber/{id}=...
        [HttpGet("GetPartNumber/{id}")]
        public IActionResult GetPartNumber([FromRoute] int id) {
            var product = _context.Products
                .Where(item => item.Id == id)
                .Select(item => new Product {
                    Id = item.Id,
                    PartNumber = item.PartNumber
                }).FirstOrDefault();
            if (product == null) {
                return NotFound(new {
                    Error = "A product was not found with that Id " + id.ToString()
                });
            }
            return Ok(product.PartNumber);
        }

        // GET: Products/Details/5
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetProductDetails([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);

            if (product == null) {
                return NotFound();
            }

            await product.GetCounts(_context);

            return Ok(product);
        }

        // GET: Products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var product = await _context.Products
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .Include(item => item.AttributeValues)
                    .ThenInclude(item => item.ProductAttributeValueOption)
                .Include(item => item.ProductType)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (product == null) {
                return NotFound();
            }

            return Ok(product);
        }

        // GET: Products/5/MostRecentListingsContainer
        [HttpGet("{id}/MostRecentListingsContainer")]
        public async Task<IActionResult> GetMostRecentListingsContainer([FromRoute] int id, [FromQuery] bool forceRefresh = false) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);

            if (product == null) {
                return BadRequest(new {
                    Error = "Product was not found"
                });
            }

            var listingsContainer = await _context.ListingsContainers
                .OrderByDescending(item => item.CreatedAt)
                .FirstOrDefaultAsync(item => item.ProductId == id);

            if (listingsContainer == null || forceRefresh) {
                return await this.FetchNewListingsContainerInternal(product);
            }

            return Ok(listingsContainer);
        }


        public async Task<IActionResult> FetchNewListingsContainerInternal(Product product) {
            // if(ProductsController.ListingsContainerFetchesInProgress.ContainsKey(product.Id)){
            //     return BadRequest(new {
            //         Error = "Another process is already fetching. Please try again later"
            //     });
            // }

            try
            {
                var createdById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
                var url = "https://sourcery-api.gidindustrial.com/v1/source?term=" + WebUtility.UrlEncode(product.PartNumber);
                //var url = "http://localhost:8001/v1/source?portals=ebay&term=" + WebUtility.UrlEncode(product.PartNumber);

                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = HttpMethod.Get
                };
                request.Headers.Add("API-Key", _configuration.GetValue<string>("ListingsApiKey"));

                var response = await ProductsController.HttpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if ((int)response.StatusCode != 200)
                {
                    return BadRequest(new
                    {
                        Error = responseBody,
                        StatusCode = response.StatusCode
                    });
                }

                var newListingsContainer = new ListingsContainer
                {
                    ListingsJSON = responseBody,
                    CreatedAt = DateTime.UtcNow,
                    CreatedById = createdById,
                    ProductId = product.Id
                };

                _context.Add(newListingsContainer);
                await _context.SaveChangesAsync();

                return Ok(newListingsContainer);
            }
            catch(Exception ex) {
                string t = ex.Message;
            }

            return null;
        }

        [HttpGet("{id}/FetchNewListingsContainer")]
        public async Task<IActionResult> FetchNewListingsContainer([FromRoute] int id) {
            var product = await _context.Products.FirstOrDefaultAsync(item => item.Id == id);
            if (product == null) {
                return BadRequest(new {
                    Error = "Product not found"
                });
            }
            return await this.FetchNewListingsContainerInternal(product);
        }

        [HttpGet("{id}/ProductKitItems")]
        public async Task<IActionResult> FetchProductKitItems([FromRoute] int id) {
            var items = await _context.ProductKitItems
                .Where(item => item.ParentProductId == id)
                .ToListAsync();
            return Ok(items);
        }

        [HttpPut("{id}/ProductKitItems")]
        public async Task<IActionResult> PutProductKitItems([FromRoute] int id, [FromBody] List<ProductKitItem> productKitItems) {
            var product = await _context.Products.FirstOrDefaultAsync(item => item.Id == id);
            if (product == null) {
                return BadRequest("Error, product not found");
            }
            using (var transaction = _context.Database.BeginTransaction()) {
                _context.ProductKitItems.RemoveRange(_context.ProductKitItems.Where(item => item.ParentProductId == id));
                await _context.SaveChangesAsync();
                foreach (var productKitItem in productKitItems) {
                    if (productKitItem.ParentProductId != product.Id) {
                        return BadRequest("ParentProductId must match product Id");
                    }
                    _context.ProductKitItems.Add(productKitItem);
                }
                await _context.SaveChangesAsync();
                transaction.Commit();
            }
            return Ok();
        }

        [HttpPost("{id}/AttributeValues")]
        public async Task<IActionResult> SetAttributeValues([FromRoute] int id, [FromBody] Dictionary<int, List<int>> productAttributeValues) {

            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id);
            if (product == null) {
                return NotFound("A Product was not found with that Id");
            }

            using (var transaction = _context.Database.BeginTransaction()) {
                _context.ProductAttributeValues.RemoveRange(_context.ProductAttributeValues.Where(item => item.ProductId == product.Id));
                await _context.SaveChangesAsync();
                foreach (var attrValues in productAttributeValues) {
                    foreach (var attrValue in attrValues.Value) {
                        _context.Add(new ProductAttributeValue {
                            CreatedAt = DateTime.UtcNow,
                            ProductAttributeValueOptionId = attrValue,
                            ProductId = product.Id
                        });
                    }
                }
                await _context.SaveChangesAsync();
                transaction.Commit();
            }
            return Ok();
        }

        [HttpGet("GetNextGidPartNumber")]
        public async Task<IActionResult> GetNextGidPartNumber([FromQuery] int productTypeId)
        {
            var productType = await _context.ProductTypes.FirstOrDefaultAsync(item => item.Id == productTypeId);
            var nextGidPartNumber = await productType.GetNextGidPartNumber(_context);
            return Ok(nextGidPartNumber);
        }

        // PUT: Products/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduct([FromRoute] int id, [FromBody] Product product) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != product.Id) {
                return BadRequest();
            }
            var originalProduct = await _context.Products.AsNoTracking().FirstOrDefaultAsync(item => item.Id == product.Id);
            if (originalProduct == null) {
                return NotFound("A Product was not found with that Id");
            }
            product.QuickBooksId = originalProduct.QuickBooksId;
            product.QuickBooksSyncToken = originalProduct.QuickBooksSyncToken;

            _context.Entry(product).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ProductExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }


        // POST: Products
        [HttpPost]
        public async Task<IActionResult> PostProduct([FromBody] Product product, [FromQuery] bool isGidPart = false) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            product.PartNumber = product.PartNumber.Trim();
            if (isGidPart && String.IsNullOrWhiteSpace(product.GidPartNumber)) {
                var productType = await _context.ProductTypes.FirstOrDefaultAsync(item => item.Id == product.ProductTypeId);
                product.GidPartNumber = await productType.GetNextGidPartNumber(_context);
            }

            if(product.ManufacturerId != null)
            {
                var existingProduct = await _context.Products.FirstOrDefaultAsync(item => item.ManufacturerId == product.ManufacturerId && item.PartNumber == product.PartNumber.Trim());
                if(existingProduct != null)
                {
                    return BadRequest(new {
                        Error = "A product with the same manufacturer and part number already exists.",
                        ExistingProductId = existingProduct.Id,
                        ExistingProductPartNumber = existingProduct.PartNumber
                    });
                }
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProduct", new { id = product.Id }, product);
        }

        // DELETE: Products/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var product = await _context.Products
                .Include(item => item.Attachments)
                    .ThenInclude(item => item.Attachment)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null) {
                return NotFound();
            }

            foreach (var itemAttachment in product.Attachments.ToList()) {
                await itemAttachment.Attachment.Delete(_context, _configuration);
            }
            _context.Products.Remove(product);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(product);
        }

        private bool ProductExists(int id) {
            return _context.Products.Any(e => e.Id == id);
        }
    }

    public class ProductAttributeValues {

    }
}

