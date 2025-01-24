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
    [Route("Portals")]
    public class PortalsController : Controller {
        private readonly AppDBContext _context;

        public PortalsController(AppDBContext context) {
            _context = context;
        }

        // GET: Portals
        [HttpGet]
        public ListResult GetPortals([FromQuery] int skip = 0, [FromQuery] int perPage = 10) {
            var query = from portal in _context.Portals select portal;
            query = query.OrderByDescending(c => c.CreatedAt);

            return new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }

        // GET: Portals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPortal([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var portal = await _context.Portals.SingleOrDefaultAsync(m => m.Id == id);

            if (portal == null) {
                return NotFound();
            }

            return Ok(portal);
        }

        // GET: Portals/SelectOptions
        [HttpGet("SelectOptions")]
        public async Task<IActionResult> GetPortalSelectOptions([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var portals = await  _context.Portals.Select(item => new{
                Id = item.Id,
                Value = item.Name
            }).ToListAsync();

            return Ok(portals);
        }

        [HttpGet("GetOrCreateByName")]
        public async Task<IActionResult> GetOrCreatePortalByName([FromRoute] string name){
            if(String.IsNullOrEmpty(name)){
                return BadRequest("String must not be empty");
            }
            var portal = await _context.Portals.FirstOrDefaultAsync(item => String.Equals(name, item.Name, StringComparison.OrdinalIgnoreCase));
            if(portal == null){
                portal = new Portal{
                    Name = name
                };
            }
            return Ok(portal);
        }

        // PUT: Portals/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutPortal([FromRoute] int id, [FromBody] Portal portal) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != portal.Id) {
                return BadRequest();
            }

            _context.Entry(portal).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!PortalExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: Portals
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostPortal([FromBody] Portal portal) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            _context.Portals.Add(portal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPortal", new { id = portal.Id }, portal);
        }

        // DELETE: Portals/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeletePortal([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var portal = await _context.Portals.SingleOrDefaultAsync(m => m.Id == id);
            if (portal == null) {
                return NotFound();
            }

            _context.Portals.Remove(portal);
            await _context.SaveChangesAsync();

            return Ok(portal);
        }

        private bool PortalExists(int id) {
            return _context.Portals.Any(e => e.Id == id);
        }
    }
}