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
    [Route("ScannerStations")]
    public class ScannerStationsController : Controller {
        private readonly AppDBContext _context;
        private static readonly HttpClient HttpClient;
        private IConfiguration _configuration;

        static ScannerStationsController() {
            HttpClient = new HttpClient();
        }

        public ScannerStationsController(AppDBContext context, IConfiguration appConfig) {
            _context = context;
            this._configuration = appConfig;
        }

        // GET: ScannerStations
        [HttpGet]
        public ListResult GetScannerStations(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string partNumber = null,
            [FromQuery] string searchString = null,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from scannerStation in _context.ScannerStations select scannerStation;
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
        
        // GET: ScannerStations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetScannerStation([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var scannerStation = await _context.ScannerStations
                .SingleOrDefaultAsync(item => item.Id == id);

            if (scannerStation == null) {
                return NotFound();
            }

            return Ok(scannerStation);
        }

        // PUT: ScannerStations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutScannerStation([FromRoute] int id, [FromBody] ScannerStation scannerStation) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != scannerStation.Id) {
                return BadRequest();
            }

            _context.Entry(scannerStation).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ScannerStationExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: ScannerStations
        [HttpPost]
        public async Task<IActionResult> PostScannerStation([FromBody] ScannerStation scannerStation) {
            scannerStation.CreatedAt = DateTime.UtcNow;
            scannerStation.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);

            ModelState.Clear();
            TryValidateModel(scannerStation);
            this.TryValidateModel(scannerStation);      
      
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            _context.ScannerStations.Add(scannerStation);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetScannerStation", new { id = scannerStation.Id }, scannerStation);
        }

        // DELETE: ScannerStations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteScannerStation([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var scannerStation = await _context.ScannerStations
                .SingleOrDefaultAsync(item => item.Id == id);
            if (scannerStation == null) {
                return NotFound();
            }

            _context.ScannerStations.Remove(scannerStation);
            try {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex) {
                return BadRequest(ex.InnerException != null ? ex.InnerException.Message : ex.Message);
            }

            return Ok(scannerStation);
        }

        private bool ScannerStationExists(int id) {
            return _context.ScannerStations.Any(e => e.Id == id);
        }
    }
}