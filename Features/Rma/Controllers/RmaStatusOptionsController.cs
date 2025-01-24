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
    [Route("RmaStatusOptions")]
    public class RmaStatusOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public RmaStatusOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RmaStatusOptions
        [HttpGet]
        public IEnumerable<RmaStatusOption> GetRmaStatusOptions()
        {
            return _context.RmaStatusOptions;
        }

        // GET: RmaStatusOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRmaStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaStatusOption = await _context.RmaStatusOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (rmaStatusOption == null)
            {
                return NotFound();
            }

            return Ok(rmaStatusOption);
        }

        // PUT: RmaStatusOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRmaStatusOption([FromRoute] int id, [FromBody] RmaStatusOption rmaStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rmaStatusOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(rmaStatusOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RmaStatusOptionExists(id))
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

        // POST: RmaStatusOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostRmaStatusOption([FromBody] RmaStatusOption rmaStatusOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RmaStatusOptions.Add(rmaStatusOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRmaStatusOption", new { id = rmaStatusOption.Id }, rmaStatusOption);
        }

        // DELETE: RmaStatusOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRmaStatusOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaStatusOption = await _context.RmaStatusOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (rmaStatusOption == null)
            {
                return NotFound();
            }

            _context.RmaStatusOptions.Remove(rmaStatusOption);
            await _context.SaveChangesAsync();

            return Ok(rmaStatusOption);
        }

        private bool RmaStatusOptionExists(int id)
        {
            return _context.RmaStatusOptions.Any(e => e.Id == id);
        }
    }
}