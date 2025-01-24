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
    [Route("SalesOrderFilters")]
    public class SalesOrderFiltersController : Controller
    {
        private readonly AppDBContext _context;

        public SalesOrderFiltersController(AppDBContext context)
        {
            _context = context;
        }

        // GET: SalesOrderFilters
        [HttpGet]
        public IEnumerable<SalesOrderFilter> GetSalesOrderFilters()
        {
            return _context.SalesOrderFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
        }

        // GET: SalesOrderFilters/SelectOptions
        // Transform the objects to match other select options with Id, Value pairs
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetSalesOrderFilterSelectOptions()
        {
            return _context.SalesOrderFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)).Select(lf => new{
                Id = lf.Id,
                Value = lf.Name
            });
        }

        // GET: SalesOrderFilters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSalesOrderFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesOrderFilter = await _context.SalesOrderFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));

            if (salesOrderFilter == null)
            {
                return NotFound();
            }

            return Ok(salesOrderFilter);
        }

        // PUT: SalesOrderFilters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSalesOrderFilter([FromRoute] int id, [FromBody] SalesOrderFilter salesOrderFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != salesOrderFilter.Id)
            {
                return BadRequest();
            }
            
            var dbSalesOrderFilter = await _context.SalesOrderFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));
            if(dbSalesOrderFilter == null){
                return NotFound();
            }


            _context.Entry(salesOrderFilter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SalesOrderFilterExists(id))
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

        // POST: SalesOrderFilters
        [HttpPost]
        public async Task<IActionResult> PostSalesOrderFilter([FromBody] SalesOrderFilter salesOrderFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            salesOrderFilter.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            salesOrderFilter.CreatedAt = DateTime.UtcNow;

            _context.SalesOrderFilters.Add(salesOrderFilter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSalesOrderFilter", new { id = salesOrderFilter.Id }, salesOrderFilter);
        }

        // DELETE: SalesOrderFilters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSalesOrderFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var salesOrderFilter = await _context.SalesOrderFilters.SingleOrDefaultAsync(m => m.Id == id && m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            if (salesOrderFilter == null)
            {
                return NotFound();
            }

            _context.SalesOrderFilters.Remove(salesOrderFilter);
            await _context.SaveChangesAsync();

            return Ok(salesOrderFilter);
        }

        private bool SalesOrderFilterExists(int id)
        {
            return _context.SalesOrderFilters.Any(e => e.Id == id);
        }
    }
}