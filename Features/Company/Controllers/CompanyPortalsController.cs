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
    [Route("CompanyPortals")]
    public class CompanyPortalsController : Controller
    {
        private readonly AppDBContext _context;

        public CompanyPortalsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: CompanyPortals
        [HttpGet]
        public ListResult GetCompanyPortals([FromQuery] int skip = 0, [FromQuery] int perPage = 10)
        {
            var query = from companyPortal in _context.CompanyPortals select companyPortal;

            query = query.OrderByDescending(l => l.CreatedAt);

            return new ListResult
            {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            };
        }



        // GET: CompanyPortals/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCompanyPortal([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyPortal = await _context.CompanyPortals
                .SingleOrDefaultAsync(m => m.Id == id);

            if (companyPortal == null)
            {
                return NotFound();
            }

            return Ok(companyPortal);
        }

        // PUT: CompanyPortals/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCompanyPortal([FromRoute] int id, [FromBody] CompanyPortal companyPortal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != companyPortal.Id)
            {
                return BadRequest();
            }

            _context.Entry(companyPortal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CompanyPortalExists(id))
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

        /// <summary>
        /// POST: CompanyPortals
        /// Create a companyPortal
        /// </summary>
        /// <param name="companyPortal"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PostCompanyPortal([FromBody] CompanyPortal companyPortal)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // //add to event log entry the fact that it was created and who created it
            // _context.CompanyPortalEventLogEntries.Add(new CompanyPortalEventLogEntry
            // {
            //     CompanyPortal = companyPortal,
            //     EventLogEntry = new EventLogEntry
            //     {
            //         Event = "Created",
            //         CreatedAt = DateTime.UtcNow,
            //         OccurredAt = DateTime.UtcNow,
            //         CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User),
            //         UserId = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User)
            //     }
            // });
            companyPortal.CreatedAt = DateTime.UtcNow;
            companyPortal.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            _context.CompanyPortals.Add(companyPortal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCompanyPortal", new { id = companyPortal.Id }, companyPortal);
        }

        // DELETE: CompanyPortals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCompanyPortal([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var companyPortal = await _context.CompanyPortals.SingleOrDefaultAsync(m => m.Id == id);
            if (companyPortal == null)
            {
                return NotFound();
            }

            _context.CompanyPortals.Remove(companyPortal);
            await _context.SaveChangesAsync();

            return Ok(companyPortal);
        }

        private bool CompanyPortalExists(int id)
        {
            return _context.CompanyPortals.Any(e => e.Id == id);
        }
    }
}