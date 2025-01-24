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
    [Route("LeadOriginOptions")]
    public class LeadOriginOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public LeadOriginOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: LeadOriginOptions
        [HttpGet]
        public IEnumerable<LeadOriginOption> GetLeadOriginOptions()
        {
            return _context.LeadOriginOptions;
        }

        // GET: LeadOriginOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLeadOriginOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LeadOriginOption = await _context.LeadOriginOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (LeadOriginOption == null)
            {
                return NotFound();
            }

            return Ok(LeadOriginOption);
        }

        // PUT: LeadOriginOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutLeadOriginOption([FromRoute] int id, [FromBody] LeadOriginOption LeadOriginOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != LeadOriginOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(LeadOriginOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeadOriginOptionExists(id))
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

        // POST: LeadOriginOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostLeadOriginOption([FromBody] LeadOriginOption LeadOriginOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LeadOriginOptions.Add(LeadOriginOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeadOriginOption", new { id = LeadOriginOption.Id }, LeadOriginOption);
        }

        // DELETE: LeadOriginOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteLeadOriginOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LeadOriginOption = await _context.LeadOriginOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (LeadOriginOption == null)
            {
                return NotFound();
            }

            _context.LeadOriginOptions.Remove(LeadOriginOption);
            await _context.SaveChangesAsync();

            return Ok(LeadOriginOption);
        }

        private bool LeadOriginOptionExists(int id)
        {
            return _context.LeadOriginOptions.Any(e => e.Id == id);
        }
    }
}