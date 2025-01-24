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
    [Route("PurchaseOrderFilters")]
    public class PurchaseOrderFiltersController : Controller
    {
        private readonly AppDBContext _context;

        public PurchaseOrderFiltersController(AppDBContext context)
        {
            _context = context;
        }

        // GET: PurchaseOrderFilters
        [HttpGet]
        public IEnumerable<PurchaseOrderFilter> GetPurchaseOrderFilters()
        {
            return _context.PurchaseOrderFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
        }

        // GET: PurchaseOrderFilters/SelectOptions
        // Transform the objects to match other select options with Id, Value pairs
        [HttpGet("SelectOptions")]
        public IEnumerable<dynamic> GetPurchaseOrderFilterSelectOptions()
        {
            return _context.PurchaseOrderFilters.Where(f => f.Public == true || f.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)).Select(lf => new{
                Id = lf.Id,
                Value = lf.Name
            });
        }

        // GET: PurchaseOrderFilters/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPurchaseOrderFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrderFilter = await _context.PurchaseOrderFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));

            if (purchaseOrderFilter == null)
            {
                return NotFound();
            }

            return Ok(purchaseOrderFilter);
        }

        // PUT: PurchaseOrderFilters/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPurchaseOrderFilter([FromRoute] int id, [FromBody] PurchaseOrderFilter purchaseOrderFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != purchaseOrderFilter.Id)
            {
                return BadRequest();
            }
            
            var dbPurchaseOrderFilter = await _context.PurchaseOrderFilters.SingleOrDefaultAsync(m =>
                m.Id == id &&
                (m.Public == true || m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)));
            if(dbPurchaseOrderFilter == null){
                return NotFound();
            }


            _context.Entry(purchaseOrderFilter).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PurchaseOrderFilterExists(id))
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

        // POST: PurchaseOrderFilters
        [HttpPost]
        public async Task<IActionResult> PostPurchaseOrderFilter([FromBody] PurchaseOrderFilter purchaseOrderFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            purchaseOrderFilter.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            purchaseOrderFilter.CreatedAt = DateTime.UtcNow;

            _context.PurchaseOrderFilters.Add(purchaseOrderFilter);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPurchaseOrderFilter", new { id = purchaseOrderFilter.Id }, purchaseOrderFilter);
        }

        // DELETE: PurchaseOrderFilters/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePurchaseOrderFilter([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var purchaseOrderFilter = await _context.PurchaseOrderFilters.SingleOrDefaultAsync(m => m.Id == id && m.CreatedById == GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User));
            if (purchaseOrderFilter == null)
            {
                return NotFound();
            }

            _context.PurchaseOrderFilters.Remove(purchaseOrderFilter);
            await _context.SaveChangesAsync();

            return Ok(purchaseOrderFilter);
        }

        private bool PurchaseOrderFilterExists(int id)
        {
            return _context.PurchaseOrderFilters.Any(e => e.Id == id);
        }
    }
}