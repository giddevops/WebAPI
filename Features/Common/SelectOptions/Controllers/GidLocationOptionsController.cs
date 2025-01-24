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
    [Route("GidLocationOptions")]
    public class GidLocationOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public GidLocationOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: GidLocationOptions
        [HttpGet]
        public IEnumerable<GidLocationOption> GetGidLocationOption([FromQuery] bool includeDefaultShippingAddress)
        {
            if(includeDefaultShippingAddress){
                return _context.GidLocationOptions.Include(item => item.DefaultShippingAddress);
            }
            return _context.GidLocationOptions;
        }

        // GET: GidLocationOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGidLocationOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _context.GidLocationOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // PUT: GidLocationOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutGidLocationOption([FromRoute] int id, [FromBody] GidLocationOption item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != item.Id)
            {
                return BadRequest();
            }

            _context.Entry(item).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GidLocationOptionExists(id))
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

        // POST: GidLocationOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostGidLocationOption([FromBody] GidLocationOption item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            item.CreatedAt = DateTime.UtcNow;
            _context.GidLocationOptions.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGidLocationOption", new { id = item.Id }, item);
        }

        // DELETE: GidLocationOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteGidLocationOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _context.GidLocationOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            _context.GidLocationOptions.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        private bool GidLocationOptionExists(int id)
        {
            return _context.GidLocationOptions.Any(e => e.Id == id);
        }
    }
}