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
    [Route("ViewObjectNameOptions")]
    public class ViewObjectNameOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public ViewObjectNameOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: ViewObjectNameOptions
        [HttpGet]
        public IEnumerable<ViewObjectNameOption> GetViewObjectNameOptions()
        {
            return _context.ViewObjectNameOptions;
        }

        // GET: ViewObjectNameOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetViewObjectNameOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var viewObjectNameOption = await _context.ViewObjectNameOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (viewObjectNameOption == null)
            {
                return NotFound();
            }

            return Ok(viewObjectNameOption);
        }

        // PUT: ViewObjectNameOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutViewObjectNameOption([FromRoute] int id, [FromBody] ViewObjectNameOption viewObjectNameOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != viewObjectNameOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(viewObjectNameOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ViewObjectNameOptionExists(id))
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

        // POST: ViewObjectNameOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostViewObjectNameOption([FromBody] ViewObjectNameOption viewObjectNameOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ViewObjectNameOptions.Add(viewObjectNameOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetViewObjectNameOption", new { id = viewObjectNameOption.Id }, viewObjectNameOption);
        }

        // DELETE: ViewObjectNameOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteViewObjectNameOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var viewObjectNameOption = await _context.ViewObjectNameOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (viewObjectNameOption == null)
            {
                return NotFound();
            }

            _context.ViewObjectNameOptions.Remove(viewObjectNameOption);
            await _context.SaveChangesAsync();

            return Ok(viewObjectNameOption);
        }

        private bool ViewObjectNameOptionExists(int id)
        {
            return _context.ViewObjectNameOptions.Any(e => e.Id == id);
        }
    }
}