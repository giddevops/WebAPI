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
    [Route("RmaReasonOptions")]
    public class RmaReasonOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public RmaReasonOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: RmaReasonOptions
        [HttpGet]
        public IEnumerable<RmaReasonOption> GetRmaReasonOptions()
        {
            return _context.RmaReasonOptions;
        }

        // GET: RmaReasonOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRmaReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaReasonOption = await _context.RmaReasonOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (rmaReasonOption == null)
            {
                return NotFound();
            }

            return Ok(rmaReasonOption);
        }

        // PUT: RmaReasonOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRmaReasonOption([FromRoute] int id, [FromBody] RmaReasonOption rmaReasonOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != rmaReasonOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(rmaReasonOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RmaReasonOptionExists(id))
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

        // POST: RmaReasonOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostRmaReasonOption([FromBody] RmaReasonOption rmaReasonOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.RmaReasonOptions.Add(rmaReasonOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetRmaReasonOption", new { id = rmaReasonOption.Id }, rmaReasonOption);
        }

        // DELETE: RmaReasonOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRmaReasonOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var rmaReasonOption = await _context.RmaReasonOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (rmaReasonOption == null)
            {
                return NotFound();
            }

            _context.RmaReasonOptions.Remove(rmaReasonOption);
            await _context.SaveChangesAsync();

            return Ok(rmaReasonOption);
        }

        private bool RmaReasonOptionExists(int id)
        {
            return _context.RmaReasonOptions.Any(e => e.Id == id);
        }
    }
}