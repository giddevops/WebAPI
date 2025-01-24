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
    [Route("LeadFilters")]
    public class LeadFiltersController : Controller
    {
        private readonly AppDBContext _context;

        public LeadFiltersController(AppDBContext context)
        {
            _context = context;
        }

        // GET: LeadFilters
        [HttpGet]
        public IEnumerable<LeadFilter> GetLeadFilters()
        {
            return _context.LeadFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
        }

        // GET: LeadFilters/SelectOptions
        // Transform the objects to match other select options with Id, Value pairs
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetLeadFilterSelectOptions()
        {
            return _context.LeadFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)).Select(lf => new{
                Id = lf.Id,
                Value = lf.Name
            });
        }

        // GET: LeadFilters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leadFilter = await _context.LeadFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));

            if (leadFilter == null)
            {
                return NotFound();
            }

            return Ok(leadFilter);
        }

        // PUT: LeadFilters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeadFilter([FromRoute] int id, [FromBody] LeadFilter leadFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != leadFilter.Id)
            {
                return BadRequest();
            }
            
            var dbLeadFilter = await _context.LeadFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));
            if(dbLeadFilter == null){
                return NotFound();
            }


            _context.Entry(leadFilter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeadFilterExists(id))
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

        // POST: LeadFilters
        [HttpPost]
        public async Task<IActionResult> PostLeadFilter([FromBody] LeadFilter leadFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            leadFilter.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            leadFilter.CreatedAt = DateTime.UtcNow;

            _context.LeadFilters.Add(leadFilter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeadFilter", new { id = leadFilter.Id }, leadFilter);
        }

        // DELETE: LeadFilters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeadFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leadFilter = await _context.LeadFilters.SingleOrDefaultAsync(m => m.Id == id && m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            if (leadFilter == null)
            {
                return NotFound();
            }

            _context.LeadFilters.Remove(leadFilter);
            await _context.SaveChangesAsync();

            return Ok(leadFilter);
        }

        private bool LeadFilterExists(int id)
        {
            return _context.LeadFilters.Any(e => e.Id == id);
        }
    }
}