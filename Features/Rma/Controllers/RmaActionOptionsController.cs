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
    [Route("RmaActionOptions")]
    public class RmaActionOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public RmaActionOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RmaActionOptions
        [HttpGet]
        public IEnumerable<RmaActionOption> GetRmaActionOptions()
        {
            return _context.RmaActionOptions;
        }

        // GET: RmaActionOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRmaActionOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaActionOption = await _context.RmaActionOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (rmaActionOption == null)
            {
                return NotFound();
            }

            return Ok(rmaActionOption);
        }

        // PUT: RmaActionOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRmaActionOption([FromRoute] int id, [FromBody] RmaActionOption rmaActionOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rmaActionOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(rmaActionOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RmaActionOptionExists(id))
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

        // POST: RmaActionOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostRmaActionOption([FromBody] RmaActionOption rmaActionOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RmaActionOptions.Add(rmaActionOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRmaActionOption", new { id = rmaActionOption.Id }, rmaActionOption);
        }

        // DELETE: RmaActionOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRmaActionOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaActionOption = await _context.RmaActionOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (rmaActionOption == null)
            {
                return NotFound();
            }

            _context.RmaActionOptions.Remove(rmaActionOption);
            await _context.SaveChangesAsync();

            return Ok(rmaActionOption);
        }

        private bool RmaActionOptionExists(int id)
        {
            return _context.RmaActionOptions.Any(e => e.Id == id);
        }
    }
}