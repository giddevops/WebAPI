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
    [Route("LeadStatusOptions")]
    public class LeadStatusOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public LeadStatusOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: LeadStatusOptions
        [HttpGet]
        public IEnumerable<LeadStatusOption> GetLeadStatusOptions()
        {
            return _context.LeadStatusOptions;
        }

        // GET: LeadStatusOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leadStatusOption = await _context.LeadStatusOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (leadStatusOption == null)
            {
                return NotFound();
            }

            return Ok(leadStatusOption);
        }

        // PUT: LeadStatusOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutLeadStatusOption([FromRoute] int id, [FromBody] LeadStatusOption leadStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != leadStatusOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(leadStatusOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeadStatusOptionExists(id))
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

        // POST: LeadStatusOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostLeadStatusOption([FromBody] LeadStatusOption leadStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LeadStatusOptions.Add(leadStatusOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeadStatusOption", new { id = leadStatusOption.Id }, leadStatusOption);
        }

        // DELETE: LeadStatusOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteLeadStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var leadStatusOption = await _context.LeadStatusOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (leadStatusOption == null)
            {
                return NotFound();
            }

            _context.LeadStatusOptions.Remove(leadStatusOption);
            await _context.SaveChangesAsync();

            return Ok(leadStatusOption);
        }

        private bool LeadStatusOptionExists(int id)
        {
            return _context.LeadStatusOptions.Any(e => e.Id == id);
        }
    }
}