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
    [Route("LineItemServiceTypes")]
    public class LineItemServiceTypesController : Controller
    {
        private readonly AppDBContext _context;

        public LineItemServiceTypesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: LineItemServiceTypes
        [HttpGet]
        public IEnumerable<LineItemServiceType> GetLineItemServiceTypes()
        {
            return _context.LineItemServiceTypes;
        }

        // GET: LineItemServiceTypes/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetLineItemServiceType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LineItemServiceType = await _context.LineItemServiceTypes.SingleOrDefaultAsync(m => m.Id == id);

            if (LineItemServiceType == null)
            {
                return NotFound();
            }

            return Ok(LineItemServiceType);
        }

        // PUT: LineItemServiceTypes/5
        [HttpPut("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PutLineItemServiceType([FromRoute] int id, [FromBody] LineItemServiceType LineItemServiceType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != LineItemServiceType.Id)
            {
                return BadRequest();
            }

            _context.Entry(LineItemServiceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LineItemServiceTypeExists(id))
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

        // POST: LineItemServiceTypes
        [HttpPost]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> PostLineItemServiceType([FromBody] LineItemServiceType LineItemServiceType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.LineItemServiceTypes.Add(LineItemServiceType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLineItemServiceType", new { id = LineItemServiceType.Id }, LineItemServiceType);
        }

        // DELETE: LineItemServiceTypes/5
        [HttpDelete("{id}")]
        [RequirePermission("EditDropdownOptions")]
        public async Task<IActionResult> DeleteLineItemServiceType([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var LineItemServiceType = await _context.LineItemServiceTypes.SingleOrDefaultAsync(m => m.Id == id);
            if (LineItemServiceType == null)
            {
                return NotFound();
            }

            _context.LineItemServiceTypes.Remove(LineItemServiceType);
            await _context.SaveChangesAsync();

            return Ok(LineItemServiceType);
        }

        private bool LineItemServiceTypeExists(int id)
        {
            return _context.LineItemServiceTypes.Any(e => e.Id == id);
        }
    }
}