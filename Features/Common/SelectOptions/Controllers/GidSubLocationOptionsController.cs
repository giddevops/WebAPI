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
    [Route("GidSubLocationOptions")]
    public class GidSubLocationOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public GidSubLocationOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: GidSubLocationOptions
        [HttpGet]
        public IEnumerable<GidSubLocationOption> GetGidSubLocationOption()
        {
            return _context.GidSubLocationOptions;
        }
        
        // GET: GidSubLocationOptions
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetGidSubLocationOptionSelectOptions([FromQuery] int? gidLocationOptionId = null)
        {
            var query = from gidLocationOption in _context.GidSubLocationOptions select gidLocationOption;

            if(gidLocationOptionId != null){
                query = query.Where(item => item.GidLocationOptionId == gidLocationOptionId);
            }

            query = query.OrderByDescending(item => item.Name);


            return query.Select(item => new {
                Id = item.Id,
                Value = item.Name
            });
        }

        // GET: GidSubLocationOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetGidSubLocationOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _context.GidSubLocationOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        // PUT: GidSubLocationOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutGidSubLocationOption([FromRoute] int id, [FromBody] GidSubLocationOption item)
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
                if (!GidSubLocationOptionExists(id))
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

        // POST: GidSubLocationOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostGidSubLocationOption([FromBody] GidSubLocationOption item)
        {
            item.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(User);
            item.CreatedAt = DateTime.UtcNow;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _context.GidSubLocationOptions.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGidSubLocationOption", new { id = item.Id }, item);
        }

        // DELETE: GidSubLocationOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteGidSubLocationOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var item = await _context.GidSubLocationOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (item == null)
            {
                return NotFound();
            }

            _context.GidSubLocationOptions.Remove(item);
            await _context.SaveChangesAsync();

            return Ok(item);
        }

        private bool GidSubLocationOptionExists(int id)
        {
            return _context.GidSubLocationOptions.Any(e => e.Id == id);
        }
    }
}