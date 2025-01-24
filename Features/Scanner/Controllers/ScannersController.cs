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

namespace WebApi.Features.Controllers {
    [Produces("application/json")]
    [Route("Scanners")]
    public class ScannersController : Controller {
        private readonly AppDBContext _context;
        private static readonly HttpClient HttpClient;
        private IConfiguration _configuration;

        static ScannersController() {
            HttpClient = new HttpClient();
        }

        public ScannersController(AppDBContext context, IConfiguration appConfig) {
            _context = context;
            this._configuration = appConfig;
        }

        // GET: Scanners
        [HttpGet]
        public ListResult GetScanners(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string partNumber = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from scanner in _context.Scanners select scanner;
            query = query.OrderByDescending(c => c.CreatedAt);

            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }
            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: Scanners/Search?query=...
        [HttpGet("Search")]
        public IEnumerable<dynamic> Search([FromQuery] string query) {
            int id = 0;
            Int32.TryParse(query, out id);
            IQueryable<Scanner> search = _context.Scanners.Where(scanner =>
                EF.Functions.Like(scanner.SerialNumber, query + '%') ||
                scanner.Id == id
            ).OrderBy(item => item.Id);
            return search;
        }

        // GET: Scanners/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScanner([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var scanner = await _context.Scanners
                .SingleOrDefaultAsync(item => item.Id == id);

            if (scanner == null) {
                return NotFound();
            }

            return Ok(scanner);
        }

        // PUT: Scanners/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScanner([FromRoute] int id, [FromBody] Scanner scanner) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != scanner.Id) {
                return BadRequest();
            }

            _context.Entry(scanner).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ScannerExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Scanners
        [HttpPost]
        public async Task<IActionResult> PostScanner([FromBody] Scanner scanner) {
            scanner.CreatedAt = DateTime.UtcNow;
            scanner.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            ModelState.Clear();
            TryValidateModel(scanner);
            this.TryValidateModel(scanner);      
      
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.Scanners.Add(scanner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScanner", new { id = scanner.Id }, scanner);
        }

        // DELETE: Scanners/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScanner([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var scanner = await _context.Scanners
                .SingleOrDefaultAsync(item => item.Id == id);
            if (scanner == null) {
                return NotFound();
            }

            _context.Scanners.Remove(scanner);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(scanner);
        }

        private bool ScannerExists(int id) {
            return _context.Scanners.Any(e => e.Id == id);
        }
    }
}