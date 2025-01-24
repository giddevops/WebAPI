using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GidIndustrial.Gideon.WebApi.Models;

namespace WebApi.Features.Inventory.Controllers
{
    [Produces("application/json")]
    [Route("WorkLogItemActivityOptions")]
    public class WorkLogItemActivityOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public WorkLogItemActivityOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: WorkLogItemActivityOptions
        [HttpGet]
        public IEnumerable<WorkLogItemActivityOption> GetWorkLogItemActivityOption()
        {
            return _context.WorkLogItemActivityOptions;
        }

        // GET: WorkLogItemActivityOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkLogItemActivityOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workLogItemActivityOption = await _context.WorkLogItemActivityOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (workLogItemActivityOption == null)
            {
                return NotFound();
            }

            return Ok(workLogItemActivityOption);
        }

        // PUT: WorkLogItemActivityOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWorkLogItemActivityOption([FromRoute] int id, [FromBody] WorkLogItemActivityOption workLogItemActivityOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != workLogItemActivityOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(workLogItemActivityOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkLogItemActivityOptionExists(id))
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

        // POST: WorkLogItemActivityOptions
        [RequirePermission("EditDropdownOptions")]
        [HttpPost]
        public async Task<IActionResult> PostWorkLogItemActivityOption([FromBody] WorkLogItemActivityOption workLogItemActivityOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.WorkLogItemActivityOptions.Add(workLogItemActivityOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWorkLogItemActivityOption", new { id = workLogItemActivityOption.Id }, workLogItemActivityOption);
        }

        // DELETE: WorkLogItemActivityOptions/5
        [RequirePermission("EditDropdownOptions")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkLogItemActivityOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workLogItemActivityOption = await _context.WorkLogItemActivityOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (workLogItemActivityOption == null)
            {
                return NotFound();
            }

            _context.WorkLogItemActivityOptions.Remove(workLogItemActivityOption);
            await _context.SaveChangesAsync();

            return Ok(workLogItemActivityOption);
        }

        private bool WorkLogItemActivityOptionExists(int id)
        {
            return _context.WorkLogItemActivityOptions.Any(e => e.Id == id);
        }
    }
}