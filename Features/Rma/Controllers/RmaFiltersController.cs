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
    [Route("RmaFilters")]
    public class RmaFiltersController : Controller
    {
        private readonly AppDBContext _context;

        public RmaFiltersController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RmaFilters
        [HttpGet]
        public IEnumerable<RmaFilter> GetRmaFilters()
        {
            return _context.RmaFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
        }

        // GET: RmaFilters/SelectOptions
        // Transform the objects to match other select options with Id, Value pairs
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetRmaFilterSelectOptions()
        {
            return _context.RmaFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)).Select(lf => new{
                Id = lf.Id,
                Value = lf.Name
            });
        }

        // GET: RmaFilters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRmaFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaFilter = await _context.RmaFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));

            if (rmaFilter == null)
            {
                return NotFound();
            }

            return Ok(rmaFilter);
        }

        // PUT: RmaFilters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRmaFilter([FromRoute] int id, [FromBody] RmaFilter rmaFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rmaFilter.Id)
            {
                return BadRequest();
            }
            
            var dbRmaFilter = await _context.RmaFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));
            if(dbRmaFilter == null){
                return NotFound();
            }


            _context.Entry(rmaFilter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RmaFilterExists(id))
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

        // POST: RmaFilters
        [HttpPost]
        public async Task<IActionResult> PostRmaFilter([FromBody] RmaFilter rmaFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            rmaFilter.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            rmaFilter.CreatedAt = DateTime.UtcNow;

            _context.RmaFilters.Add(rmaFilter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRmaFilter", new { id = rmaFilter.Id }, rmaFilter);
        }

        // DELETE: RmaFilters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRmaFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaFilter = await _context.RmaFilters.SingleOrDefaultAsync(m => m.Id == id && m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            if (rmaFilter == null)
            {
                return NotFound();
            }

            _context.RmaFilters.Remove(rmaFilter);
            await _context.SaveChangesAsync();

            return Ok(rmaFilter);
        }

        private bool RmaFilterExists(int id)
        {
            return _context.RmaFilters.Any(e => e.Id == id);
        }
    }
}