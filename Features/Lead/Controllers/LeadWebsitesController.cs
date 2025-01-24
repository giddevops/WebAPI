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
    [Route("LeadWebsites")]
    public class LeadWebsitesController : Controller
    {
        private readonly AppDBContext _context;

        public LeadWebsitesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: LeadWebsites
        [HttpGet]
        public IEnumerable<LeadWebsite> GetLeadWebsites()
        {
            return _context.LeadWebsites;
        }

        // GET: LeadWebsites/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadWebsite([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LeadWebsite = await _context.LeadWebsites.SingleOrDefaultAsync(m => m.Id == id);

            if (LeadWebsite == null)
            {
                return NotFound();
            }

            return Ok(LeadWebsite);
        }

        // PUT: LeadWebsites/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutLeadWebsite([FromRoute] int id, [FromBody] LeadWebsite LeadWebsite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != LeadWebsite.Id)
            {
                return BadRequest();
            }

            _context.Entry(LeadWebsite).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeadWebsiteExists(id))
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

        // POST: LeadWebsites
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostLeadWebsite([FromBody] LeadWebsite LeadWebsite)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LeadWebsites.Add(LeadWebsite);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeadWebsite", new { id = LeadWebsite.Id }, LeadWebsite);
        }

        // DELETE: LeadWebsites/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteLeadWebsite([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LeadWebsite = await _context.LeadWebsites.SingleOrDefaultAsync(m => m.Id == id);
            if (LeadWebsite == null)
            {
                return NotFound();
            }

            _context.LeadWebsites.Remove(LeadWebsite);
            await _context.SaveChangesAsync();

            return Ok(LeadWebsite);
        }

        private bool LeadWebsiteExists(int id)
        {
            return _context.LeadWebsites.Any(e => e.Id == id);
        }
    }
}