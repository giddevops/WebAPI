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
    [Route("SourceLeadTimeOptions")]
    public class SourceLeadTimeOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public SourceLeadTimeOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: SourceLeadTimeOptions
        [HttpGet]
        public IEnumerable<SourceLeadTimeOption> GetSourceLeadTimeOptions()
        {
            return _context.SourceLeadTimeOptions;
        }

        // GET: SourceLeadTimeOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSourceLeadTimeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sourceLeadTimeOption = await _context.SourceLeadTimeOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (sourceLeadTimeOption == null)
            {
                return NotFound();
            }

            return Ok(sourceLeadTimeOption);
        }

        // PUT: SourceLeadTimeOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutSourceLeadTimeOption([FromRoute] int id, [FromBody] SourceLeadTimeOption sourceLeadTimeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sourceLeadTimeOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(sourceLeadTimeOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SourceLeadTimeOptionExists(id))
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

        // POST: SourceLeadTimeOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostSourceLeadTimeOption([FromBody] SourceLeadTimeOption sourceLeadTimeOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SourceLeadTimeOptions.Add(sourceLeadTimeOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSourceLeadTimeOption", new { id = sourceLeadTimeOption.Id }, sourceLeadTimeOption);
        }

        // DELETE: SourceLeadTimeOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteSourceLeadTimeOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sourceLeadTimeOption = await _context.SourceLeadTimeOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (sourceLeadTimeOption == null)
            {
                return NotFound();
            }

            _context.SourceLeadTimeOptions.Remove(sourceLeadTimeOption);
            await _context.SaveChangesAsync();

            return Ok(sourceLeadTimeOption);
        }

        private bool SourceLeadTimeOptionExists(int id)
        {
            return _context.SourceLeadTimeOptions.Any(e => e.Id == id);
        }
    }
}