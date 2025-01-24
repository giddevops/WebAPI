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
    [Route("SourceWarrantyOptions")]
    public class SourceWarrantyOptionsController : Controller
    {
        private readonly AppDBContext _context;

        public SourceWarrantyOptionsController(AppDBContext context)
        {
            _context = context;
        }

        // GET: SourceWarrantyOptions
        [HttpGet]
        public IEnumerable<SourceWarrantyOption> GetSourceWarrantyOptions()
        {
            return _context.SourceWarrantyOptions;
        }

        // GET: SourceWarrantyOptions/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetSourceWarrantyOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sourceWarrantyOption = await _context.SourceWarrantyOptions.SingleOrDefaultAsync(m => m.Id == id);

            if (sourceWarrantyOption == null)
            {
                return NotFound();
            }

            return Ok(sourceWarrantyOption);
        }

        // PUT: SourceWarrantyOptions/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutSourceWarrantyOption([FromRoute] int id, [FromBody] SourceWarrantyOption sourceWarrantyOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != sourceWarrantyOption.Id)
            {
                return BadRequest();
            }

            _context.Entry(sourceWarrantyOption).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SourceWarrantyOptionExists(id))
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

        // POST: SourceWarrantyOptions
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostSourceWarrantyOption([FromBody] SourceWarrantyOption sourceWarrantyOption)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.SourceWarrantyOptions.Add(sourceWarrantyOption);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSourceWarrantyOption", new { id = sourceWarrantyOption.Id }, sourceWarrantyOption);
        }

        // DELETE: SourceWarrantyOptions/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteSourceWarrantyOption([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var sourceWarrantyOption = await _context.SourceWarrantyOptions.SingleOrDefaultAsync(m => m.Id == id);
            if (sourceWarrantyOption == null)
            {
                return NotFound();
            }

            _context.SourceWarrantyOptions.Remove(sourceWarrantyOption);
            await _context.SaveChangesAsync();

            return Ok(sourceWarrantyOption);
        }

        private bool SourceWarrantyOptionExists(int id)
        {
            return _context.SourceWarrantyOptions.Any(e => e.Id == id);
        }
    }
}